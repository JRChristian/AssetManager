(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.tag',
        function ($scope, $location, tagService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');
/*
            vm.gridOptions = {};
            vm.gridOptions.appScopeProvider = vm;
            vm.tagdata = function (id) {
                tagdata({ tagId: id });
            };
*/
            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableColumnResizing: true,
                enableFiltering: true,
                enableGridMenu: true,
                columnDefs: [
                    { name: 'name', width: '20%', minWidth: 50, displayName: vm.localize('Name'), enableHiding: false },
                    { name: 'description', width: '50%', displayName: vm.localize('Description') },
                    { name: 'uom', width: '10%', displayName: vm.localize('UOM') },
                    { name: 'precision', width: '10%', displayName: vm.localize('Precision'), visible: false },
                    { name: 'id', width: '10%', displayName: vm.localize('Action'), enableSorting: false, enableFilter: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a ui-sref="tagdata({ tagId: row.entity.id })"><i class="fa fa-bar-chart"></i></a> <a ui-sref="tagview({ tagId: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a ui-sref="tagedit({ tagId: row.entity.id })"><i class="fa fa-pencil"></i></a></div>'
                    }]
                    //{ name: 'id', cellTemplate: '<div class="ui-grid-cell-contents"><button class="btn btn-primary" ng-click="grid.appScope.vm.tagdata( row.entity.id )">Click Me</button></div>' }]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until getTagListAsync completes
                    null,
                    tagService.getTagListAsync({ Name: '' }).success(function (data) {
                        vm.tags = data.tags;
                        vm.gridOptions.data = data.tags;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();