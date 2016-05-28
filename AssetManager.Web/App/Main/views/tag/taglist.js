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
                enableColumnResizing: false,
                enableFiltering: true,
                enableGridMenu: true,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: '10%', minWidth: 85, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="tagdata({ tagId: row.entity.id })"><i class="fa fa-line-chart"></i></a> <a class="btn btn-default" ui-sref="tagview({ tagId: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a class="btn btn-default" ui-sref="tagedit({ tagId: row.entity.id })"><i class="fa fa-pencil-square-o"></i></a></div></div>'
                    },
                    { name: 'name', displayName: vm.localize('Name'), width: '15%', minWidth: 60, enableHiding: false },
                    { name: 'description', displayName: vm.localize('Description'), width: '45%' },
                    { name: 'uom', displayName: vm.localize('UOM'), width: '10%' },
                    //{ name: 'precision', width: '10%', displayName: vm.localize('Precision'), visible: true },
                    { name: 'type', displayName: vm.localize('TagType'), width: '10%', cellFilter: 'tagType', visible: true },
                    { 
                        name: 'lastValue', displayName: vm.localize('Value'), width: '10%', visible: true, enableFiltering: false,
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.lastTimestamp | momentLocale}}: {{row.entity.lastTimestamp | momentFromNow}}" tooltip-append-to-body=true>{{COL_FIELD | number:row.entity.precision}}</div>'
                    }
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