(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.dashboard2';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', '$sce', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, $sce, variableService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            vm.localize = abp.localization.getSource('AssetManager');
            //vm.days = 30;

            var today = new Date();
            vm.startDate = new Date();
            vm.startDate.setDate(today.getDate() - vm.days);

            vm.assetId = $stateParams.AssetId > 0 ? $stateParams.AssetId : null;
            vm.assetTypeId = $stateParams.AssetTypeId > 0 ? $stateParams.AssetTypeId : null;
            vm.assetParentId = $stateParams.AssetParentId > 0 ? $stateParams.AssetParentId : null;
            vm.includeChildren = $stateParams.IncludeChildren > 0 ? 1 : 0;
            vm.days = $stateParams.Days > 0 ? $stateParams.Days : 1;
            vm.days = vm.days <= 60 ? vm.days : 60;
            vm.assetOverallName = vm.localize('Overall');
            vm.viewButtonLabel = vm.days <= 1 ? 'View last 30 days' : 'View today';
            vm.showOverall = true;
            vm.overallStats = [];
            vm.assetStats = [];

            vm.changeDayRange = function () {
                vm.days = vm.days <= 1 ? 30 : 1;
                vm.viewButtonLabel = vm.days <= 1 ? 'View last 30 days' : 'View today';
                vm.refresh();
            }

            vm.refresh = function () {
                vm.overallStats = [];
                vm.assetStats = [];
                vm.startDate = new Date();
                vm.startDate.setDate(today.getDate() - vm.days);
                abp.ui.setBusy(
                    null,
                    assetHealthService.getCompoundAssetLevelStats({ AssetId: vm.assetId, AssetTypeId: vm.assetTypeId, IncludeChildren: vm.includeChildren, StartTimestamp: vm.startDate })
                        .success(function (data) {
                            vm.startDate = data.startTimestamp;
                            vm.overallStats = data.overallStats;
                            vm.assetStats = data.assetStats;
                            vm.assetId = data.assetId;
                            vm.assetTypeId = data.assetTypeId;
                            vm.assetParentId = data.assetParentId;
                            if (data.assetName != null)
                                vm.assetOverallName = data.assetName;
                            else if (data.assetTypeName != null)
                                vm.assetOverallName = data.assetTypeName;

                            // If there is just one asset then we want to suppress the overall record
                            //if (data.numberAssets == 1 && data.assetId == data.assetStats[0].assetId)
                            //    vm.showOverall = false;
                            vm.showOverall = (data.numberAssets != 1 || data.assetId == data.assetStats[0].assetId);

                            var label = '';
                            for (var i = 0; i < vm.overallStats.length; i++) {
                                // Style the row
                                if (label == '' && vm.overallStats[i].metricType > 0 && vm.overallStats[i].numberLimits > 0) {
                                    label = vm.bootstrapStyleRow(vm.overallStats[i].criticality, vm.overallStats[i].errorLevel, vm.overallStats[i].warningLevel, vm.overallStats[i].metricValue);
                                }
                                // Style the cells
                                if (vm.overallStats[i].metricType > 0 && vm.overallStats[i].numberLimits > 0)
                                    vm.overallStats[i].style = vm.bootstrapStyleCell(vm.overallStats[i].criticality, vm.overallStats[i].errorLevel, vm.overallStats[i].warningLevel, vm.overallStats[i].metricValue);
                                else
                                    vm.overallStats[i].style = '';
                                // Add a tooltip
                                vm.overallStats[i].tooltip = vm.tooltipHtml(vm.overallStats[i]);
                            }
                            vm.label = label;
                            for (var i = 0; i < vm.assetStats.length; i++) {
                                var label = '';
                                if (vm.assetStats[i].levels != null) {
                                    for (var j = 0; j < vm.assetStats[i].levels.length; j++) {
                                        var a = vm.assetStats[i].levels[j];
                                        // Style the row
                                        if (label == '' && a.metricType > 0 && a.numberLimits > 0) {
                                            label = vm.bootstrapStyleRow(a.criticality, a.errorLevel, a.warningLevel, a.metricValue);
                                        }
                                        // Style the cells
                                        if (a.metricType > 0 && a.numberLimits > 0)
                                            vm.assetStats[i].levels[j].style = vm.bootstrapStyleCell(a.criticality, a.errorLevel, a.warningLevel, a.metricValue);
                                        else
                                            vm.assetStats[i].levels[j].style = '';

                                        // Add a tooltip
                                        vm.assetStats[i].levels[j].tooltip = vm.tooltipHtml(a);
                                    }
                                }
                                vm.assetStats[i].label = label;
                            }
                        })
                    );
            };

            vm.bootstrapStyleRow = function (criticality, errorLevel, warningLevel, value) {
                if (criticality <= 1) {
                    if( value >= errorLevel )
                        style = 'label label-danger';
                    else if (value >= warningLevel )
                        style = 'label label-warning';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if( value >= errorLevel )
                        style = 'label label-warning';
                    else if (value >= warningLevel )
                        style = 'label label-default';
                    else
                        style = '';
                }
                else {
                    if (value >= errorLevel)
                        style = 'label label-default';
                    else if (value >= warningLevel)
                        style = '';
                    else
                        style = '';
                }
                return style;
            };

            vm.bootstrapStyleCell = function (criticality, errorLevel, warningLevel, value) {
                if (criticality <= 1) {
                    if (value >= errorLevel)
                        style = 'text-danger';
                    else if (value >= warningLevel)
                        style = 'text-warning';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if (value >= errorLevel)
                        style = 'text-warning';
                    else if (value >= warningLevel)
                        style = 'text-info';
                    else
                        style = '';
                }
                else {
                    if (value >= errorLevel)
                        style = 'text-info';
                    else if (value >= warningLevel)
                        style = '';
                    else
                        style = '';
                }
                return style;
            };

            vm.tooltipHtml = function(stats) {
                if (stats == null)
                    thetooltip = '';
                else
                    thetooltip = stats.criticality.toString() + '-' + stats.levelName + '<br>'
                        + 'Number of limits: ' + stats.numberLimits + '<br>'
                        + 'Number limits with deviations: ' + stats.numberDeviatingLimits + '<br>'
                        + 'Time in deviation: ' + Math.round(stats.metricValue * 10) / 10 + '%<br>'
                        + 'Warning limit: ' + stats.warningLevel + '%<br>'
                        + 'Error limit: ' + stats.errorLevel + '%';
                return $sce.trustAsHtml(thetooltip);
            };

            vm.variableLimits = [];
            abp.ui.setBusy(
                null,
                variableService.getRecentlyDeviatingLimits({ maxCriticality: 2, hoursBack: 24 })
                    .success(function (data) {
                        vm.variableLimits = data.variablelimits;
                        //console.log('Limits: '+ data.variablelimits.length);
                    })
                );

            vm.refresh();
        }
    ]);
})();