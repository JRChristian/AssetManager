(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assettype.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$filter', 'abp.services.app.asset',
        function ($scope, $location, $filter, assetService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                appScopeProvider: vm,
                enableSorting: true,
                enableCellEditOnFocus: true,
                minRowsToShow: 8,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: '10%', minWidth: 40, enableSorting: false, enableFiltering: false, enableColumnMenus: false, enableCellEdit: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><button ng-click="grid.appScope.deleteRow(row)" type="submit" class="btn btn-primary"><i class="fa fa-trash"></i></button></div></div>'
                    },
                    { name: 'name', width: '90%', displayName: vm.localize('Name'), enableCellEdit: true }
                ],
                rowEditWaitInterval: -1
            };

            vm.gridOptions.onRegisterApi = function (gridApi) { vm.gridApi = gridApi; };

            vm.refresh = function () {
                assetService.getAssetTypes({})
                    .success(function (data) {
                        vm.gridOptions.data = $filter('orderBy')(data.assetTypes, "name", false);
                    });
            };

            vm.addNewItem = function () { vm.gridOptions.data.splice(0, 0, { name: vm.localize("AssetPlcNewAssetTypeNamePlaceholder") }); };

            vm.save = function () { abp.ui.setBusy(null, vm.saveAll()) };
            vm.saveAll = function () {
                // Save all rows that changed
                vm.gridDirtyRows = vm.gridApi.rowEdit.getDirtyRows(vm.gridApi.grid);
                var dataRows = vm.gridDirtyRows.map(function (gridRow) { return gridRow.entity; });
                vm.assetTypesToUpdate = [];
                for (var i = 0; i < vm.gridDirtyRows.length; i++) {
                    vm.assetTypesToUpdate[i] = { Name: vm.gridDirtyRows[i].entity.name };
                }
                assetService.updateAssetTypes({ AssetTypes: vm.assetTypesToUpdate })
                    .success(function (data) {
                        abp.notify.success(abp.utils.formatString(vm.localize("AssetMsgAssetTypeSuccessfulChanges"), data.successfulUpdates, vm.gridDirtyRows.length));
                        vm.gridApi.rowEdit.setRowsClean(dataRows);
                    });
                //vm.refresh();
            };

            vm.delete = function (id, name) { abp.ui.setBusy(null, vm.deleteAll(id, name)) };
            vm.deleteRow = function (row) {
                var index = vm.gridOptions.data.indexOf(row.entity);
                vm.assetTypesToUpdate = [{Id: row.entity.id}];
                assetService.deleteAssetTypes({ AssetTypes: vm.assetTypesToUpdate })
                    .success(function (data) {
                        if (data.successfulDeletions > 0)
                            abp.notify.success(abp.utils.formatString(vm.localize("AssetMsgAssetTypeDeletedOk"), row.entity.name));
                        else
                            abp.notify.error(abp.utils.formatString(vm.localize("AssetMsgAssetTypeNotDeleted"), row.entity.name));
                        });
                vm.gridOptions.data.splice(index, 1);
            };

            vm.refresh();
        }
    ]);
})();