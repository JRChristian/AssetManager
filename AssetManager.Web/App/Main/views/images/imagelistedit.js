(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.image.listedit';
    app.controller(controllerId, [
        '$scope', '$location', '$filter', 'abp.services.app.image',
        function ($scope, $location, $filter, imageService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableCellEditOnFocus: true,
                enableSorting: true,
                enableFiltering: true,
                columnDefs: [
                    { name: 'name', displayName: vm.localize('Name'), width: '15%', minWidth: 60, enableCellEdit: true },
                    { name: 'description', displayName: vm.localize('Description'), width: '45%', enableCellEdit: true },
                    { name: 'url', displayName: vm.localize('URL'), width: '40%', enableCellEdit: true }
                ],
                rowEditWaitInterval: -1
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refreshImages = function () {
                abp.ui.setBusy( //Set whole page busy until the function completes
                    null,
                    imageService.getImageList({}).success(function (data) {
                        vm.gridOptions.data = $filter('orderBy')(data.images, "name", false);
                    })
                );
            };

            vm.refreshImages();

            vm.saveAll = function ()
            {
                vm.gridDirtyRows = vm.gridApi.rowEdit.getDirtyRows(vm.gridApi.grid);
                var dataRows = vm.gridDirtyRows.map(function (gridRow) { return gridRow.entity; });
                vm.changedImages = [];
                for (var i = 0; i < vm.gridDirtyRows.length; i++)
                {
                    vm.changedImages.push({ Name: vm.gridDirtyRows[i].entity.name, Description: vm.gridDirtyRows[i].entity.description, Url: vm.gridDirtyRows[i].entity.url });
                }
                imageService.updateImageList({ Images: vm.changedImages })
                        .success(function (data) {
                            if (data.updateSucceeded.length > 0) {
                                abp.notify.info(abp.utils.formatString(vm.localize("ImgMsgImagedUpdateSucceeded"), data.updateSucceeded.length, vm.gridDirtyRows.length));
                                vm.gridApi.rowEdit.setRowsClean(dataRows);
                                //vm.refreshImages();
                                $location.path('/imagelist');
                            }
                            else {
                                abp.notify.info(abp.utils.formatString(vm.localize("ImgMsgImagedUpdateFailed"), vm.gridDirtyRows.length));
                            }
                        })};

            vm.addItem = function () {
                vm.gridOptions.data.push({name: 'New images', description: '', URL: ''});
            };
        }
    ]);
})();