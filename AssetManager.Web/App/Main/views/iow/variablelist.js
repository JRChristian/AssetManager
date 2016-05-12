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
                enableSorting: true,
                enableColumnResizing: true,
                rowHeight: 33,
                columnDefs: [
                    {
                        name: 'id', width: '10%', displayName: vm.localize('Action'), enableSorting: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a ui-sref="iowvariablechart({ Id: row.entity.id })"><i class="fa fa-line-chart"></i></a> <a ui-sref="iowvariableview({ Id: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a ui-sref="iowvariableedit({ Id: row.entity.id })"><i class="fa fa-wrench"></i></a></div>'
                    },
                    { name: 'name', width: '30%', minWidth: 50, displayName: vm.localize('Name') },
                    { name: 'description', width: '30%', displayName: vm.localize('Description') },
                    { name: 'tagName', width: '20%', displayName: vm.localize('TagName') },
                    { name: 'uom', width: '10%', displayName: vm.localize('UOM') }]
            };

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