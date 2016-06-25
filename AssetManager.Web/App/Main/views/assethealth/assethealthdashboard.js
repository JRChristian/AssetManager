(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.dashboard';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, variableService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.startDate = Date.now();

            vm.metrics = [];
            abp.ui.setBusy(
                null,
                assetHealthService.getAssetHealthMetricValues()
                    .success(function (data) {
                        vm.metrics = data.metrics;
                        var lastName = '';
                        for (var i = 0; i < vm.metrics.length; i++) {
                            // Add a member that will be useful in filtering the table
                            vm.metrics[i].hasLimits = vm.metrics[i].numberLimits > 0 ? true : false;
                            vm.metrics[i].name = vm.metrics[i].applyToEachAsset ? vm.metrics[i].assetName : vm.metrics[i].assetTypeName;
                            vm.metrics[i].repeated = (i > 0 && vm.metrics[i].hasLimits && vm.metrics[i].name == lastName);
                            lastName = vm.metrics[i].hasLimits ? vm.metrics[i].name : lastName;

                            // Build the bullet chart information
                            vm.metrics[i].value = Math.round(100 * vm.metrics[i].value)/100.0;
                            vm.metrics[i].recentValue = Math.round(100 * vm.metrics[i].recentValue) / 100.0;
                            var maxValue = Math.max(vm.metrics[i].error, vm.metrics[i].value, vm.metrics[i].recentValue) * 1.2;
                            vm.metrics[i].chartData = {
                                title: vm.metrics[i].criticality + '-' + vm.metrics[i].levelName,
                                subtitle: '% deviation ' + vm.metrics[i].period + ' days',
                                ranges: [vm.metrics[i].warning, vm.metrics[i].error, maxValue],
                                measures: [vm.metrics[i].value],
                                markers: [vm.metrics[i].recentValue]
                            };
                        }
                        if (vm.metrics.length > 0)
                            vm.startDate = vm.metrics[0].startTimestamp;
                    })
                );

            vm.variableLimits = [];
            abp.ui.setBusy(
                null,
                variableService.getRecentlyDeviatingLimits({ maxCriticality: 2, hoursBack: 24 })
                    .success(function (data) {
                        vm.variableLimits = data.variablelimits;
                        //console.log('Limits: '+ data.variablelimits.length);
                    })
                );

            vm.chartOptions = {
                chart: {
                    type: 'bulletChart',
                    width: 400,
                    height: 48,
                    margins: { top: 0, bottom: 0, left: 0, right: 0 },
                    duration: 500
                }
            };
        }
    ]);
})();