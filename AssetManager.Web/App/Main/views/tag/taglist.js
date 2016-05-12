(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.tag',
        function ($scope, $location, tagService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableColumnResizing: true,
                enableFiltering: true,
                enableGridMenu: true,
                columnDefs: [
                    { name: 'id', width: '10%', displayName: vm.localize('Action'), enableSorting: false, enableFilter: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a ui-sref="tagdata({ tagId: row.entity.id })"><i class="fa fa-line-chart"></i></a> <a ui-sref="tagview({ tagId: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a ui-sref="tagedit({ tagId: row.entity.id })"><i class="fa fa-wrench"></i></a></div>' },
                    { name: 'name', width: '20%', minWidth: 50, displayName: vm.localize('Name'), enableHiding: false },
                    { name: 'description', width: '50%', displayName: vm.localize('Description') },
                    { name: 'uom', width: '10%', displayName: vm.localize('UOM') },
                    //{ name: 'precision', width: '10%', displayName: vm.localize('Precision'), visible: true },
                    { name: 'type', width: '10%', displayName: vm.localize('TagType'), cellFilter: 'tagType', visible: true }
                ]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until getTagListAsync completes
                    null,
                    tagService.getTagList({ Name: '' }).success(function (data) {
                        vm.tags = data.tags;
                        vm.gridOptions.data = data.tags;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();