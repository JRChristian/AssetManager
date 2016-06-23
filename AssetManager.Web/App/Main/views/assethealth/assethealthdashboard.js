(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.dashboard';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, variableService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.metrics = [];
            abp.ui.setBusy(
                null,
                assetHealthService.getAssetHealthMetricValues()
                    .success(function (data) {
                        vm.metrics = data.metrics;
                        for (var i = 0; i < vm.metrics.length; i++)
                            vm.metrics[i].hasLimits = vm.metrics[i].numberLimits > 0 ? true : false;
                    })
                );

            vm.variableLimits = [];
            abp.ui.setBusy(
                null,
                variableService.getRecentlyDeviatingLimits({ maxCriticality: 2, hoursBack: 24 })
                    .success(function (data) {
                        vm.variableLimits = data.variablelimits;
                        console.log('Limits: '+ data.variablelimits.length);
                    })
                );
        }
    ]);
})();