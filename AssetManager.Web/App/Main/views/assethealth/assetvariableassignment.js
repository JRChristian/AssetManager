(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assignment';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$uibModal', 'uiGridConstants', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $uibModal, uiGridConstants, variableService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.assetHierarchy = [];
            vm.variables = [];
            vm.selectedAssetName = '';
            vm.selectedAssetIndex = -1;
            vm.selectedVariables = [];
            vm.noAssetIsSelected = true;

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: false,
                noUnselect: false,
                //multiSelect: true,
                //modifierKeysToMultiSelect: true,
                enableExpandAll: true,
                enableFiltering: true,
                enableSorting: true,
                showGridFooter: true,
                showTreeExpandNoChildren: false,
                showTreeRowHeader: true,
                columnDefs: [
                    { name: 'name', displayName: vm.localize('Name'), width: '20%' },
                    { name: 'description', displayName: vm.localize('Description'), width: '40%' },
                    { name: 'assetTypeName', displayName: vm.localize('AssetType'), width: '20%' },
                    {
                        name: 'variableCount', displayName: vm.localize('AssetHealthTblVariableCount'), width: '10%',
                        cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.variables.length}}</div>'
                    }
                ]
            };
            function registerGridApi(gridApi) {
                vm.gridApi = gridApi;
                vm.selectedRows = vm.gridApi.selection.getSelectedRows();
                gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    vm.changedRow = row.entity.name;
                    if (vm.gridApi.selection.getSelectedRows().length == 0) {
                        vm.noAssetIsSelected = true;
                        vm.selectedAssetName = '';
                        vm.selectedAssetIndex = -1;
                        vm.selectedVariables = [];
                    }
                    else {
                        vm.noAssetIsSelected = false;
                        vm.selectedAssetName = row.entity.name;
                        vm.selectedAssetIndex = row.entity.originalIndex;
                        vm.selectedVariables = row.entity.variables;
                    }
                });
            }

            vm.getVariables = function () {
                vm.variables = [];
                abp.ui.setBusy( //Set whole page busy until service completes
                    null,
                    variableService.getAllVariables({})
                        .success(function (data) {
                            vm.variables = data.variables;
                        }));
            };

            vm.getAssetHierarchy = function () {
                vm.assetHierarchy = [];
                abp.ui.setBusy( //Set whole page busy until service completes
                    null,
                    assetHealthService.getAssetHierarchyWithVariablesAsList({})
                        .success(function (data) {
                            for (var i = 0; i < data.assetHierarchy.length; i++) {
                                data.assetHierarchy[i].$$treeLevel = data.assetHierarchy[i].level;
                                data.assetHierarchy[i].originalIndex = i;
                            }
                            vm.assetHierarchy = data.assetHierarchy;
                            vm.gridOptions.data = data.assetHierarchy;
                        }));
            };

            vm.expandAllAssets = function () {
                vm.gridApi.treeBase.expandAllRows();
            };

            vm.collapseAllAssets = function () {
                vm.gridApi.treeBase.collapseAllRows();
            };

            vm.open = function () {
                // Update the list of all variables with information about which variables have already been selected for this asset
                // Key assumption: lists are already sorted
                // IsAssigned values: 0=not assigned already; 1=assigned already; 2=not assigned & add; 3=assigned & delete
                vm.allVariablesWithSelection = vm.variables;
                j = 0; // Index into vm.variablesForAsset array
                for (var i = 0; i < vm.allVariablesWithSelection.length; i++) {
                    vm.allVariablesWithSelection[i].originalIndex = i;
                    if (j < vm.selectedVariables.length && vm.allVariablesWithSelection[i].name == vm.selectedVariables[j].name) {
                        vm.allVariablesWithSelection[i].isAssigned = 1;
                        j++;
                    }
                    else if (j < vm.selectedVariables.length && vm.allVariablesWithSelection[i].name < vm.selectedVariables[j].name) {
                        vm.allVariablesWithSelection[i].isAssigned = 0;
                    }
                    else {
                        vm.allVariablesWithSelection[i].isAssigned = 0;
                    }
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'assetHealthVariableAssignment.html',
                    controller: 'app.views.assetvariablelist.modal as vm',
                    resolve: { variables: function () { return vm.allVariablesWithSelection; } }
                });

                modalInstance.result.then(function (selectedVariables) {
                    //vm.allVariablesWithSelection = selectedVariables;
                    var updates = [];
                    var deletes = [];
                    for (var i = 0; i < selectedVariables.length; i++)
                    {
                        if (selectedVariables[i].isAssigned == 2)
                            updates.push({ AssetName: vm.selectedAssetName, VariableName: selectedVariables[i].name });
                        else if (selectedVariables[i].isAssigned == 3)
                            deletes.push({ AssetName: vm.selectedAssetName, VariableName: selectedVariables[i].name });
                    }
                    $log.log('Updating asset ' + vm.selectedAssetName + ' to add ' + updates.length + ' variables and to delete ' + deletes.length + ' variables');
                    if( updates.length > 0 )
                        abp.ui.setBusy( null, assetHealthService.updateAssetVariableList({ AssetVariables: updates }) );
                    if (deletes.length > 0)
                        abp.ui.setBusy(null, assetHealthService.deleteAssetVariableList({ AssetVariables: deletes }));
                    vm.getAssetHierarchy();
                    vm.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS)
                });
            }

            vm.getVariables();
            vm.getAssetHierarchy();
        }
    ]);

    app.controller('app.views.assetvariablelist.modal', [
        '$scope', '$log', '$uibModalInstance', 'variables',
        function ($scope, $log, $uibModalInstance, variables) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.variables = variables;

            // IsAssigned values: 0=not assigned already; 1=assigned already; 2=not assigned & add; 3=assigned & delete
            // When selecting a variable, this array defines the new states. 0 => 2; 1 => 1; 2 => 2; 3 => 1
            vm.isAssignedNewStateOnSelect = [2, 1, 2, 1];
            // When de-selecting a variable, this array defines the new states. 0 => 0; 1 => 3; 2 => 0; 3 => 3
            vm.isAssignedNewStateOnDeselect = [0, 3, 0, 3];

            vm.gridOptions = {
                data: variables,
                onRegisterApi: registerGridApi,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: true,
                modifierKeysToMultiSelect: true,
                enableSorting: true,
                enableFiltering: true,
                showGridFooter: true,
                columnDefs: [
                    { name: 'name', displayName: vm.localize('Variable'), width: '30%' },
                    { name: 'description', displayName: vm.localize('Description'), width: '40%' },
                    { name: 'tagName', displayName: vm.localize('TagName'), width: '20%' },
                    { name: 'isAssigned', displayName: vm.localize('AssetHealthTblVariableIsUsed'), width: '*' }]
            };
            function registerGridApi(gridApi) {
                vm.gridApi = gridApi;

                // Select variables marked as already selected in the input list
                vm.gridApi.grid.modifyRows(vm.gridOptions.data);
                for (var i = 0; i < vm.variables.length; i++) {
                    if (vm.variables[i].isAssigned == 1)
                        vm.gridApi.selection.selectRow(vm.gridOptions.data[i]);
                }

                // Handle row selections. First get the row variable, then its index in the original list, then update its selection status in the original list
                vm.gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    i = row.entity.originalIndex;
                    if( row.isSelected )
                        vm.variables[i].isAssigned = vm.isAssignedNewStateOnSelect[vm.variables[i].isAssigned];
                    else
                        vm.variables[i].isAssigned = vm.isAssignedNewStateOnDeselect[vm.variables[i].isAssigned];
                });
            };

            $scope.selectAllVariables = function () {
                vm.gridApi.selection.selectAllRows();
                for (var i = 0; i < vm.variables.length; i++) {
                    vm.variables[i].isAssigned = vm.isAssignedNewStateOnSelect[vm.variables[i].isAssigned];
                }
            };

            $scope.clearAllVariables = function () {
                vm.gridApi.selection.clearSelectedRows();
                for (var i = 0; i < vm.variables.length; i++) {
                    vm.variables[i].isAssigned = vm.isAssignedNewStateOnDeselect[vm.variables[i].isAssigned];
                }
            };

            $scope.ok = function () {
                $uibModalInstance.close(vm.variables);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]);
})();