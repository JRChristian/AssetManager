(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.limit.summary';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.iowDeviation',
        function ($scope, $location, deviationService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');
            vm.variablelimits = [];
            vm.hoursBack = 720;
            vm.hoursBackMsg = vm.localize('IowTxtLimitSummarySubtitle', vm.hoursBack);

            /*
            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableColumnResizing: true,
                enableFiltering: false,
                enableGridMenu: true,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: 60, minWidth: 60, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a ui-sref="iowvariablechart({ Id: row.entity.variableId })"><i class="fa fa-line-chart"></i></a> <a ui-sref="iowvariableview({ Id: row.entity.variableId })"><i class="fa fa-binoculars"></i></a> <a ui-sref="iowvariableedit({ Id: row.entity.variableId })"><i class="fa fa-pencil-square-o"></i></a></div>'
                    },
                    { name: 'variableName', displayName: vm.localize('Variable'), width: '20%', minWidth: 50 },
                    { name: 'levelName', displayName: vm.localize('Level'), width: '*', cellTooltip: '{row.entity.levelDescription}' },
                    { name: 'direction', displayName: vm.localize('Direction'), width: '*', cellFilter: 'direction' },
                    { name: 'limitValue', displayName: vm.localize('Limit'), width: '*' },
                    { name: 'lastStatus', displayName: vm.localize('Status'), width: '*', cellFilter: 'deviation' },
                    { name: 'LastValue', displayName: vm.localize('Value'), width: '*', cellTooltip: '{row.entity.uom}' },
                    { name: 'uom', displayName: vm.localize('UOM'), width: '*' }]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }
            */
            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until the service completes
                    null,
                    deviationService.getVariableLimitCurrent({ includeAllVariables: true, maxCriticality: 0, hoursBack: vm.hoursBack }).success(function (data) {
                        //vm.gridOptions.data = data.variablelimits;
                        vm.variablelimits = data.limitstatus;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();