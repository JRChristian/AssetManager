(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.iowVariable',
        function ($scope, $location, variableService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridvariables = {
                data: [],
                enableSorting: true,
                enableColumnResizing: true,
                columnDefs: [
                    { name: 'name', width: '30%', minWidth: 50, displayName: vm.localize('Name') },
                    { name: 'description', width: '30%', displayName: vm.localize('Description') },
                    { name: 'tagName', width: '20%', displayName: vm.localize('TagName') },
                    { name: 'uom', width: '10%', displayName: vm.localize('UOM') },
                    {
                        name: 'id', width: '10%', displayName: vm.localize('Action'), enableSorting: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a ui-sref="tagdata({ tagId: row.entity.tagId })"><i class="fa fa-bar-chart"></i></a> <a ui-sref="iowvariableview({ Id: row.entity.id })"><i class="fa fa-binoculars"></i></a> <a ui-sref="iowvariableedit({ Id: row.entity.id })"><i class="fa fa-pencil"></i></a></div>'
                    }]
                //{ name: 'id', cellTemplate: '<div class="ui-grid-cell-contents"><button class="btn btn-primary" ng-click="grid.appScope.vm.tagdata( row.entity.id )">Click Me</button></div>' }]
            };

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until getTagListAsync completes
                    null,
                    variableService.getIowVariablesAsync({ }).success(function (data) {
                        vm.gridvariables.data = data.iowVariables;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();