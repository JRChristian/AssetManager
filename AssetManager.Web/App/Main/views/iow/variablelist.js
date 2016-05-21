(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.iowVariable',
        function ($scope, $location, variableService) {
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
                    {
                        name: 'id', displayName: vm.localize('Action'), width: 110, minWidth: 110, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="iowvariablechart({ Id: row.entity.id })"><i class="fa fa-line-chart"></i></a> <a class="btn btn-default" ui-sref="iowvariabledeviations({ Id: row.entity.id })"><i class="fa fa-exclamation-circle"></i></a> <a class="btn btn-default" ui-sref="iowvariableview({ Id: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a class="btn btn-default" ui-sref="iowvariableedit({ Id: row.entity.id })"><i class="fa fa-wrench"></i></a></div></div>'
                    },
                    { name: 'name', displayName: vm.localize('Name'), width: '30%', minWidth: 50 },
                    { name: 'description', displayName: vm.localize('Description'), width: '30%' },
                    { name: 'tagName', displayName: vm.localize('TagName'), width: '20%' },
                    { name: 'uom', displayName: vm.localize('UOM'), width: '10%' }]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until the service completes
                    null,
                    variableService.getAllVariables({ }).success(function (data) {
                        vm.gridOptions.data = data.variables;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();