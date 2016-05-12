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
                minRowsToShow: 5,
                columnDefs: [
                    {
                        name: 'isActive', width: '10%', displayName: vm.localize('IsActive'), enableCellEdit: true,
                        type: 'boolean' //cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD ? "X" : ""}}</div>'
                    },
                    {
                        name: 'name', width: '10%', displayName: vm.localize('Name'), enableCellEdit: false,
                        cellTemplate: '<div class="grid-tooltip" tooltip="{{ row.entity.description }}" tooltip-placement="top" tooltip-append-to-body="true">'
                            + '<div class="ui-grid-cell-contents">{{ COL_FIELD }}</div></div>'
                    },
                    { name: 'lowLimit', width: '10%', displayName: vm.localize('LowLimit'), enableCellEdit: true },
                    { name: 'highLimit', width: '10%', displayName: vm.localize('HighLimit'), enableCellEdit: true },
                    { name: 'cause', width: '20%', displayName: vm.localize('Causes'), enableCellEdit: true },
                    { name: 'consequences', width: '20%', displayName: vm.localize('Consequences'), enableCellEdit: true },
                    { name: 'action', width: '20%', displayName: vm.localize('Action'), enableCellEdit: true }
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
                        vm.gridOptions.data = $filter('orderBy')(data.limits, "criticality", false);
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
                                    Name: vm.gridDirtyRows[i].entity.name,
                                    IsActive: vm.gridDirtyRows[i].entity.isActive,
                                    IOWLevelId: vm.gridDirtyRows[i].entity.iowLevelId,
                                    Cause: vm.gridDirtyRows[i].entity.cause,
                                    Consequences: vm.gridDirtyRows[i].entity.consequences,
                                    Action: vm.gridDirtyRows[i].entity.action,
                                    LowLimit: vm.gridDirtyRows[i].entity.lowLimit,
                                    HighLimit: vm.gridDirtyRows[i].entity.highLimit
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