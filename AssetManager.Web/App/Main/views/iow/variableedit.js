(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$filter', 'abp.services.app.iowVariable', 'abp.services.app.tag',
        function ($scope, $location, $stateParams, $filter, variableService, tagService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.deleteEnabled = true;

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                uom: ''
            };

            vm.gridOptions = {
                data: [],
                enableSorting: false,
                enableColumnResizing: true,
                enableCellEditOnFocus: true,
                minRowsToShow: 8,
                columnDefs: [
                    {
                        name: 'isActive', width: '50', displayName: vm.localize('IsActive'), enableCellEdit: true,
                        type: 'boolean', cellTemplate: '<input type="checkbox" ng-model="row.entity.isActive">'
                    },
                    {
                        name: 'name', width: '100', displayName: vm.localize('Name'), enableCellEdit: false,
                        cellTemplate: '<div class="grid-tooltip" tooltip="{{ row.entity.description }}" tooltip-placement="top" tooltip-append-to-body="true">'
                            + '<div class="ui-grid-cell-contents">{{ COL_FIELD }}</div></div>'
                    },
                    { name: 'direction', width: '50', displayName: vm.localize('Direction'), enableCellEdit: false, cellFilter: 'direction' },
                    { name: 'value', width: '50', displayName: vm.localize('Limit'), enableCellEdit: true, cellTemplate: '<div class="ui-grid-cell-contents"><div align="center">{{ COL_FIELD }}</div></div>' },
                    { name: 'cause', width: '*', displayName: vm.localize('Causes'), enableCellEdit: true },
                    { name: 'consequences', width: '*', displayName: vm.localize('Consequences'), enableCellEdit: true },
                    { name: 'action', width: '*', displayName: vm.localize('Action'), enableCellEdit: true }
                ],
                rowEditWaitInterval: -1
            };

            vm.gridOptions.onRegisterApi = function (gridApi) {
                vm.gridApi = gridApi;
            };

            abp.ui.setBusy(
                null,
                variableService.getVariableLimits({ Id: vm.variable.id, IncludeUnusedLimits: true })
                    .success(function (data) {
                        vm.variable = data;
                        vm.gridOptions.minRowsToShow = data.limits.length;
                        for (i = 0; i < data.limits.length; i++) {
                            if (isNaN(data.limits[i].value))
                                data.limits[i].value = null;
                        }
                        vm.gridOptions.data = $filter('orderBy')(data.limits, "sortOrder", false);
                    })
                );
            /*
            abp.ui.setBusy(
                null,
                variableService.getVariable({ Id: vm.variable.id })
                    .success(function (data) { vm.variable = data.variable } )
                );

            vm.limits = [];
            abp.ui.setBusy(
                null,
                variableService.getIowLimits({ variableId: vm.variable.id })
                    .success(function (data) { vm.gridOptions.data = data.limits })
                );
            */

            vm.tags = [];
            abp.ui.setBusy(
                null,
                tagService.getTagList({})
                    .success(function (data) { vm.tags = data.tags })
                    );

            vm.saveVariable = function () {
                abp.ui.setBusy(
                    null,
                    // Save the main part of the variable
                    variableService.updateVariable(vm.variable)
                        .success(function (data) {
                            // Save any limit rows that changed
                            vm.variable.id = data.id;
                            vm.gridDirtyRows = vm.gridApi.rowEdit.getDirtyRows(vm.gridApi.grid);
                            for (var i = 0; i < vm.gridDirtyRows.length; i++) {
                                variableService.updateLimit({
                                    IowVariableId: vm.variable.id,
                                    LevelName: vm.gridDirtyRows[i].entity.name,
                                    IsActive: vm.gridDirtyRows[i].entity.isActive,
                                    IOWLevelId: vm.gridDirtyRows[i].entity.iowLevelId,
                                    Cause: vm.gridDirtyRows[i].entity.cause,
                                    Consequences: vm.gridDirtyRows[i].entity.consequences,
                                    Action: vm.gridDirtyRows[i].entity.action,
                                    Value: vm.gridDirtyRows[i].entity.value,
                                    Direction: vm.gridDirtyRows[i].entity.direction
                                });
                            }
                            abp.notify.info(abp.utils.formatString(vm.localize("VariableUpdatedOk"), vm.variable.name));
                            $location.path('/iowvariablelist');
                        }));
            };

            vm.deleteVariable = function () {
                abp.ui.setBusy(
                    null,
                    variableService.deleteVariable({ Id: vm.variable.id })
                        .success(function (data) {
                            if (data.success == true)
                                abp.notify.success(abp.utils.formatString(vm.localize("VariableDeletedOk"), data.name));
                            else
                                abp.notify.error(abp.utils.formatString(vm.localize("VariableNotDeleted"), data.name));
                            $location.path('/iowvariablelist');
                        }));
            };

        }
    ]);
})();