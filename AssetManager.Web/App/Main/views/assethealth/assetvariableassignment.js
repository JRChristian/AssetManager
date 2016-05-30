(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assignment';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$uibModal', 'abp.services.app.asset', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $uibModal, assetService, variableService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.selectedAsset = '';
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
                    { name: 'assetTypeName', displayName: vm.localize('AssetType'), width: '20%' }]
            };
            function registerGridApi(gridApi) {
                vm.gridApi = gridApi;
                vm.selectedRows = vm.gridApi.selection.getSelectedRows();
                gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                    if (vm.gridApi.selection.getSelectedRows().length == 0)
                        vm.noAssetIsSelected = true;
                    else
                        vm.noAssetIsSelected = false;
                    vm.selectedAsset = row.entity.name;
                });
            }

            vm.getSelectedRows = function () {
                var currentSelection = vm.gridApi.selection.getSelectedRows();
            };

            vm.getData = function () {
                vm.variables = [];
                variableService.getAllVariables({})
                    .success(function (data) {
                        vm.variables = data.variables;
                    });
                vm.assetHierarchy = [];
                assetService.getAssetHierarchyAsList({})
                    .success(function (data) {
                        for (var i = 0; i < data.assetHierarchy.length; i++)
                            data.assetHierarchy[i].$$treeLevel = data.assetHierarchy[i].level;
                        vm.assetHierarchy = data.assetHierarchy;
                        vm.gridOptions.data = data.assetHierarchy;
                    });
                vm.assetVariables = [];
                assetHealthService.getAssetVariableList({})
                    .success(function (data) {
                        vm.assetVariables = data.assetVariables;
                    });
            };

            vm.selectAllAssets = function () {
                vm.gridApi.selection.selectAllRows();
            };

            vm.clearAllAssets = function () {
                vm.gridApi.selection.clearSelectedRows();
            };

            vm.expandAllAssets = function () {
                vm.gridApi.treeBase.expandAllRows();
            };

            vm.collapseAllAssets = function () {
                vm.gridApi.treeBase.collapseAllRows();
            };

            vm.open = function () {
                $log.log('Selection ' + vm.selectedAsset);
                var x = vm.gridApi.selection.getSelectedRows();
                var modalInstance = $uibModal.open({
                    templateUrl: 'assetHealthVariableAssignment.html',
                    controller: 'app.views.assetvariablelist.modal as vm',
                    resolve: { variables: function () { return vm.variables; } }
                });

                modalInstance.result.then(function (selectedVariable) {
                    vm.selectedVariable = selectedVariable;
                });
            }

            vm.getData();
        }
    ]);

    app.controller('app.views.assetvariablelist.modal', [
        '$scope', '$log', '$uibModalInstance', 'variables',
        function ($scope, $log, $uibModalInstance, variables) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.variables = variables;

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
                    { name: 'name', width: '50%' },
                    { name: 'description', width: '*'}]
                    /*{
                        name: 'description', width: '*',
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.description}}, {{row.entity.tagName}}, {{row.entity.uom}}" tooltip-append-to-body=true>{{row.entity.description}}</div>'
                    }]*/
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            $scope.selectAllVariables = function () {
                vm.gridApi.selection.selectAllRows();
            };

            $scope.clearAllVariables = function () {
                vm.gridApi.selection.clearSelectedRows();
            };

            $scope.selected = {
                variable: vm.variables[0]
            };

            $scope.ok = function () {
                $uibModalInstance.close(vm.selectedVariable);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]);
})();