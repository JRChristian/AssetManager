(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.image.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.image',
        function ($scope, $location, imageService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableFiltering: true,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: 60, minWidth: 60, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="imageview({ Id: row.entity.id })"><i class="fa fa-picture-o"></i></a></div></div>'
                    },
                    { name: 'name', displayName: vm.localize('Name'), width: '30%', minWidth: 60 },
                    { name: 'description', displayName: vm.localize('Description'), width: '35%' },
                    { name: 'url', displayName: vm.localize('URL'), width: '35%' }
                ]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until the function completes
                    null,
                    imageService.getImageList({}).success(function (data) {
                        vm.gridOptions.data = data.images;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();