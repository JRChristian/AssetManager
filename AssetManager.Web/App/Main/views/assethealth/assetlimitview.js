(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.limitview';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.descriptionText = vm.localize('Description');
            vm.responseGoalText = vm.localize('ResponseGoal');

            vm.asset = { id: $stateParams.assetId > 0 ? $stateParams.assetId : null, name: '', description: '' };
            vm.parentId = -1;
            vm.parentName = "";
            vm.hasParent = false;
            vm.children = [];
            vm.variables = [];
            vm.numberChildren = 0;
            vm.numberLimits = -1;
            vm.variablelimits = [];

            // Set the start day to 30 days ago
            vm.startDay = new Date();
            vm.startDay.setDate(vm.startDay.getDate() - 30);
            vm.endDay = new Date();

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableColumnResizing: true,
                enableFiltering: false,
                enableGridMenu: false,
                minRowsToShow: 6,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: 110, minWidth: 110, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="iowvariablechart({ Id: row.entity.variableId })"><i class="fa fa-line-chart"></i></a> <a class="btn btn-default" ui-sref="iowvariabledeviations({ Id: row.entity.variableId })"><i class="fa fa-exclamation-circle"></i></a> <a class="btn btn-default" ui-sref="iowvariableview({ Id: row.entity.variableId })"><i class="fa fa-binoculars"></i></a> <a class="btn btn-default" ui-sref="iowvariableedit({ Id: row.entity.variableId })"><i class="fa fa-pencil-square-o"></i></a></div></div>'
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
                ]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            abp.ui.setBusy(
                null,
                assetService.getAssetRelatives({ id: vm.asset.id })
                    .success(function (data) {
                        vm.parentId = data.parent != null ? data.parent.id : -1;
                        vm.parentName = data.parent != null ? data.parent.name : "";
                        vm.hasParent = vm.parentId > 0 ? true : false;
                        vm.children = data.children;
                        vm.numberChildren = data.children.length;
                    })
                );

            abp.ui.setBusy(
                null,
                assetHealthService.getAssetVariableList({ assetId: vm.asset.id })
                    .success(function (data) {
                        if (data.assetVariables != null && data.assetVariables.length > 0)
                            vm.variables = data.assetVariables;
                        else
                            vm.variables = [];
                    })
                );

            abp.ui.setBusy( //Set whole page busy until the service completes
                null,
                assetHealthService.getAssetLimitCurrentStatus({ assetId: vm.asset.id })
                    .success(function (data) {
                        vm.gridOptions.data = data.variablelimits;
                    })
                );

            abp.ui.setBusy(
                null,
                assetHealthService.getAssetLimitChartByDay({ assetId: vm.asset.id, startTimestamp: vm.startDay })
                    .success(function (data) {
                        vm.asset.id = data.assetId;
                        vm.asset.name = data.assetName;
                        vm.asset.description = data.assetDescription;
                        vm.startDay = data.startTimestamp;
                        vm.endDay = data.endTimestamp;
                        vm.numberLimits = data.numberLimits;
                        vm.canvasChart = new CanvasJS.Chart("chartContainer", { data: [] });
                        vm.canvasChart.options = data.canvasJS;
                        vm.canvasChart.render();
                    })
                );
        }
    ]);
})();