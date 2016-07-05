(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.dashboardForAssets';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', '$sce', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, $sce, variableService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            vm.localize = abp.localization.getSource('AssetManager');

            // Arguments
            vm.assetId = $stateParams.AssetId > 0 ? $stateParams.AssetId : null;
            vm.days = $stateParams.Days > 0 ? $stateParams.Days : 1;
            vm.days = vm.days <= 60 ? vm.days : 60;

            // Defaults and global variables
            var today = new Date();
            vm.startDate = new Date();
            vm.startDate.setDate(today.getDate() - vm.days);
            vm.assetParentId = null;
            vm.assetName = vm.localize('Overall');
            vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
            vm.showOverall = true;
            vm.overallStats = [];
            vm.childStats = [];
            vm.problemLimits = [];
            vm.levels = [];

            vm.changeDayRange = function () {
                vm.days = vm.days <= 1 ? 30 : 1;
                vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
                vm.refresh();
            };

            vm.refresh = function () {
                vm.overallStats = [];
                vm.childStats = [];
                vm.startDate = new Date();
                vm.startDate.setDate(today.getDate() - vm.days);
                abp.ui.setBusy(
                    null,
                    assetHealthService.getCompoundAssetLevelStats({ AssetId: vm.assetId, IncludeAssetChildren: 1, StartTimestamp: vm.startDate, MaxCriticality: 3 })
                        .success(function (data) {
                            vm.startDate = data.startTimestamp;
                            vm.overallStats = data.overallStats;
                            vm.childStats = data.childStats;
                            vm.problemLimits = data.problemLimits;
                            vm.levels = data.levels;
                            vm.assetId = data.overallStats.assetId;
                            vm.assetParentId = data.assetParentId;
                            vm.assetName = data.overallStats.assetId > 0 && data.overallStats.assetName !== null ? data.overallStats.assetName : vm.localize('Overall');

                            // If there is just one asset then we want to suppress the overall record
                            //if (data.numberAssets == 1 && data.assetId == data.childStats[0].assetId)
                            //    vm.showOverall = false;
                            if (data.numberAssets > 1 && data.overallStats.assetId != data.childStats[0].assetId)
                                vm.showOverall = true;
                            else
                                vm.showOverall = false;

                            var label = '';
                            for (var i = 0; i < vm.overallStats.levels.length; i++) {
                                // Style the row
                                if (label === '' && vm.overallStats.levels[i].metricType > 0 && vm.overallStats.levels[i].numberLimits > 0) {
                                    label = vm.bootstrapStyleRow(vm.overallStats.levels[i].criticality, vm.overallStats.levels[i].errorLevel, vm.overallStats.levels[i].warningLevel, vm.overallStats.levels[i].metricValue);
                                }
                                // Style the cells
                                if (vm.overallStats.levels[i].metricType > 0 && vm.overallStats.levels[i].numberLimits > 0)
                                    vm.overallStats.levels[i].style = vm.bootstrapStyleCell(vm.overallStats.levels[i].criticality, vm.overallStats.levels[i].errorLevel, vm.overallStats.levels[i].warningLevel, vm.overallStats.levels[i].metricValue);
                                else
                                    vm.overallStats.levels[i].style = '';
                                // Add a tooltip
                                vm.overallStats.levels[i].tooltip = vm.tooltipHtml(vm.overallStats.levels[i]);
                            }
                            vm.label = label;
                            for (i = 0; i < vm.childStats.length; i++) {
                                label = '';
                                if (vm.childStats[i].levels !== null) {
                                    for (var j = 0; j < vm.childStats[i].levels.length; j++) {
                                        var a = vm.childStats[i].levels[j];
                                        // Style the row
                                        if (label === '' && a.metricType > 0 && a.numberLimits > 0) {
                                            label = vm.bootstrapStyleRow(a.criticality, a.errorLevel, a.warningLevel, a.metricValue);
                                        }
                                        // Style the cells
                                        if (a.metricType > 0 && a.numberLimits > 0)
                                            vm.childStats[i].levels[j].style = vm.bootstrapStyleCell(a.criticality, a.errorLevel, a.warningLevel, a.metricValue);
                                        else
                                            vm.childStats[i].levels[j].style = '';

                                        // Add a tooltip
                                        vm.childStats[i].levels[j].tooltip = vm.tooltipHtml(a);
                                    }
                                }
                                vm.childStats[i].label = label;
                            }

                            for (i = 0; i < vm.problemLimits.length; i++) {
                                vm.problemLimits[i].tooltip = $sce.trustAsHtml(vm.problemLimits[i].variableDescription + '<br />'
                                    + 'Tag ' + vm.problemLimits[i].tagName + '<br />'
                                    + 'current value: ' + vm.problemLimits[i].lastValue + '<br />');
                            }
                        })
                    );
            };

            vm.bootstrapStyleRow = function (criticality, errorLevel, warningLevel, value) {
                if (criticality <= 1) {
                    if( value > errorLevel )
                        style = 'label label-danger';
                    else if (value > warningLevel )
                        style = 'label label-warning';
                    else
                        style = '';
                }
                else if (criticality <= 2) {
                    if( value > errorLevel )
                        style = 'label label-warning';
                    else if (value > warningLevel )
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

            vm.bootstrapStyleCell = function (criticality, errorLevel, warningLevel, value) {
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

            vm.tooltipHtml = function(stats) {
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