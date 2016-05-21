(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.status';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.iowVariable',
        function ($scope, $location, variableService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.descriptionText = vm.localize('Description');
            vm.responseGoalText = vm.localize('ResponseGoal');
            vm.variablelimits = [];

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
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="iowvariablechart({ Id: row.entity.variableId })"><i class="fa fa-line-chart"></i></a> <a class="btn btn-default" ui-sref="iowvariabledeviations({ Id: row.entity.id })"><i class="fa fa-exclamation-circle"></i></a> <a class="btn btn-default" ui-sref="iowvariableview({ Id: row.entity.variableId })"><i class="fa fa-binoculars"></i></a> <a class="btn btn-default" ui-sref="iowvariableedit({ Id: row.entity.variableId })"><i class="fa fa-wrench"></i></a></div></div>'
                    },
                    {
                        name: 'variableName', displayName: vm.localize('Variable'), width: '30%', minWidth: 50,
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.variableDescription}}" tooltip-append-to-body=true>{{COL_FIELD}}</div>'
                    },
                    {
                        name: 'limitName', displayName: vm.localize('Level'), width: '15%',
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{grid.appScope.vm.descriptionText}}: {{row.entity.levelDescription}} {{grid.appScope.vm.responseGoalText}}: {{row.entity.responseGoal}}" tooltip-append-to-body=true>{{row.entity.limitName}}</div>'
                        //cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{grid.appScope.vm.descriptionText}}: {{row.entity.levelDescription}} {{grid.appScope.vm.responseGoalText}}: {{row.entity.responseGoal}}" tooltip-append-to-body=true>{{row.entity.criticality}}-{{row.entity.levelName}}-{{row.entity.direction | direction}}</div>'
                    },
                    { name: 'limitValue', displayName: vm.localize('Limit'), width: '8%' },
                    {
                        name: 'lastValue', displayName: vm.localize('Value'), width: '8%', 
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.tagName}}: {{row.entity.lastTimestamp | momentLocale}}" tooltip-append-to-body=true>{{COL_FIELD}}</div>'
                    },
                    { name: 'uom', displayName: vm.localize('UOM'), width: '8%' },
                    {
                        name: 'severityMessage2', displayName: vm.localize('Status'), width: '20%', enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.lastDeviationEndTimestamp | momentFromNowBlankNull}}" tooltip-append-to-body=true><span class="{{row.entity.severityClass}}">{{row.entity.severityMessage1}}</span> {{row.entity.severityMessage2}}</div>'
                    }
                ]};

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }
            
            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until the service completes
                    null,
                    variableService.getVariableLimitStatus({}).success(function (data) {
                        vm.gridOptions.data = data.variablelimits;
                        //vm.variablelimits = data.variablelimits;
                    })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();