(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable', 'abp.services.app.tag',
        function ($scope, $location, $stateParams, variableService, tagService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

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
                //set gridApi on scope
                vm.gridApi = gridApi;
            };

            abp.ui.setBusy(
                null,
                variableService.getOneIowVariable({ Id: vm.variable.id })
                    .success(function (data) { vm.variable = data } )
                );

            vm.limits = [];
            abp.ui.setBusy(
                null,
                variableService.getIowLimits({ variableId: vm.variable.id })
                    .success(function (data) { vm.gridOptions.data = data.limits })
                );

            vm.tags = [];
            abp.ui.setBusy(
                null,
                tagService.getTagListAsync({})
                    .success(function (data) { vm.tags = data.tags })
                    );

            vm.saveVariable = function () {
                abp.ui.setBusy(
                    null,
                    // Save the main part of the variable
                    variableService.updateIowVariable(vm.variable)
                        .success(function (data) {
                            // Save any limit rows that changed
                            vm.variable.id = data.id;
                            vm.gridDirtyRows = vm.gridApi.rowEdit.getDirtyRows(vm.gridApi.grid);
                            for (var i = 0; i < vm.gridDirtyRows.length; i++) {
                                variableService.changeIowLimits({
                                    IOWVariableId: vm.variable.id,
                                    Name: vm.gridDirtyRows[i].entity.name,
                                    IsActive: vm.gridDirtyRows[i].entity.isActive,
                                    IOWLevelId: vm.gridDirtyRows[i].entity.iOWLevelId,
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
        }
    ]);
})();