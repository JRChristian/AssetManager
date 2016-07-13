(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assetview';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$sce', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, $sce, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            // Arguments
            vm.asset = {
                id: isNaN($stateParams.AssetId) ? null : $stateParams.AssetId
            };
            vm.days = isNaN($stateParams.Days) ? 1 : Math.round($stateParams.Days);
            vm.days = Math.max(vm.days, 1);
            vm.days = Math.min(vm.days,60);

            // Defaults and global variables
            vm.maxCriticality = 3;
            var today = new Date();
            vm.startDate = new Date();
            vm.startDate.setDate(today.getDate() - vm.days);
            vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
            vm.levels = [];
            vm.problemLimits = [];
            vm.allLimits = [];

            // Get basic asset information
            abp.ui.setBusy(
                null,
                assetService.getOneAsset({ Id: vm.asset.id })
                    .success(function (data) {
                        vm.asset = data.asset;
                    }));

            // Toggle between yesterday and 30 days ago
            vm.changeDayRange = function () {
                vm.days = vm.days <= 1 ? 30 : 1;
                vm.startDate = new Date();
                vm.startDate.setDate(today.getDate() - vm.days);
                vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
                vm.refresh();
                vm.chart();
            };

            vm.refresh = function () {
                vm.levels = [];
                vm.problemLimits = [];
                abp.ui.setBusy(
                    null,
                    assetHealthService.getCompoundAssetLevelStats({ AssetId: vm.asset.id, IncludeChildren: 'none', StartTimestamp: vm.startDate, MaxCriticality: vm.maxCriticality })
                        .success(function (data) {
                            vm.startDate = data.startTimestamp;

                            vm.levels = data.overallStats.levels;
                            for (var i = 0; i < vm.levels.length; i++) {
                                vm.levels[i].label = vm.bootstrapLabel(vm.levels[i].criticality, vm.levels[i].errorLevel, vm.levels[i].warningLevel, vm.levels[i].metricValue);
                                vm.levels[i].textClass = vm.bootstrapTextForMetric(vm.levels[i].criticality, vm.levels[i].errorLevel, vm.levels[i].warningLevel, vm.levels[i].metricValue);
                                vm.levels[i].tooltip = vm.tooltipHtml(vm.levels[i]);
                            }

                            vm.problemLimits = data.problemLimits;
                            for (i = 0; i < vm.problemLimits.length; i++) {
                                vm.problemLimits[i].tooltip = $sce.trustAsHtml(vm.problemLimits[i].variableDescription + '<br />'
                                    + 'Tag ' + vm.problemLimits[i].tagName + '<br />'
                                    + 'current value: ' + vm.problemLimits[i].lastValue + '<br />');
                            }

                        })
                    );
                vm.allLimits = [];
                abp.ui.setBusy(
                    null,
                    assetHealthService.getAssetLimitStats({ AssetId: vm.asset.id, StartTimestamp: vm.startDate })
                        .success(function (data) {
                            vm.allLimits = data.assetStats[0].limits;
                            for (i = 0; i < vm.allLimits.length; i++) {
                                // Status: 0=no deviations in time period, 1=closed deviations in time period, 2=open deviations
                                vm.allLimits[i].status = vm.status(vm.allLimits[i].lastDeviationStartTimestamp, vm.allLimits[i].lastDeviationEndTimestamp, vm.days);
                                vm.allLimits[i].textClass = vm.bootstrapTextForMetric(vm.allLimits[i].criticality, vm.allLimits[i].errorLevel, vm.allLimits[i].warningLevel, vm.allLimits[i].metricValue);
                                vm.allLimits[i].textClassCurrent = vm.bootstrapTextForValue(vm.allLimits[i].criticality, vm.allLimits[i].limitValue, vm.allLimits[i].direction, vm.allLimits[i].actualValue);
                            }
                        })
                    );
            };

            vm.status = function (lastStartTimestamp, lastEndTimestamp, daysThreshold) {
                // Status: 0=no deviations in time period, 1=closed deviations in time period, 2=open deviations
                if (lastStartTimestamp === null)
                    result = 0;
                else if (lastEndTimestamp === null)
                    result = 2;
                else {
                    var daysAgo = (moment() - moment(lastEndTimestamp)) / 86400 / 1000;
                    if (daysAgo <= daysThreshold)
                        result = 1;
                    else
                        result = 0;
                }
                return result + 0;
            };

            vm.chart = function () {
                abp.ui.setBusy(
                    null,
                    assetHealthService.getAssetLimitChartByDay({ assetId: vm.asset.id, startTimestamp: vm.startDate })
                        .success(function (data) {
                            vm.asset.id = data.assetId;
                            vm.asset.name = data.assetName;
                            vm.asset.description = data.assetDescription;
                            vm.startDay = data.startTimestamp;
                            vm.endDay = data.endTimestamp;
                            vm.canvasChart = new CanvasJS.Chart("chartContainer", { data: [] });
                            vm.canvasChart.options = data.canvasJS;
                            vm.canvasChart.render();
                        })
                    );
            };

            vm.bootstrapLabel = function (criticality, errorLevel, warningLevel, value) {
                // Bootstrap Labels. Usage: <span class="{{...}}">...</span>
                if (criticality <= 1) {
                    if (value > errorLevel)
                        style = 'label label-danger';
                    else if (value > warningLevel)
                        style = 'label label-warning';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if (value > errorLevel)
                        style = 'label label-warning';
                    else if (value > warningLevel)
                        style = 'label label-default';
                    else
                        style = '';
                }
                else {
                    if (value > errorLevel)
                        style = 'label label-default';
                    else if (value > warningLevel)
                        style = '';
                    else
                        style = '';
                }
                return style;
            };

            vm.bootstrapTextForMetric = function (criticality, errorLevel, warningLevel, value) {
                // Bootstrap textformating. Usage: <p class="{{...}}">...</p>
                if (criticality <= 1) {
                    if (value > errorLevel)
                        style = 'text-danger';
                    else if (value > warningLevel)
                        style = 'text-warning';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if (value > errorLevel)
                        style = 'text-warning';
                    else if (value > warningLevel)
                        style = 'text-info';
                    else
                        style = '';
                }
                else {
                    if (value > errorLevel)
                        style = 'text-info';
                    else if (value > warningLevel)
                        style = '';
                    else
                        style = '';
                }
                return style;
            };

            vm.bootstrapTextForValue = function (criticality, level, direction, value) {
                // Bootstrap textformating. Usage: <p class="{{...}}">...</p>
                if (criticality <= 1) {
                    if ((direction <= 1 && value < level) || (direction >= 3 && value > level))
                        style = 'text-danger';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if ((direction <= 1 && value < level) || (direction >= 3 && value > level))
                        style = 'text-warning';
                    else
                        style = '';
                }
                else {
                    if ((direction <= 1 && value < level) || (direction >= 3 && value > level))
                        style = 'text-info';
                    else
                        style = '';
                }
                return style;
            };

            vm.tooltipHtml = function (stats) {
                if (stats === null)
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
            
            vm.refresh();
        }
    ]);
})();