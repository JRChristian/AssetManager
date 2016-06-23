(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.dashboard';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            console.log('hi');
            vm.localize = abp.localization.getSource('AssetManager');
            vm.asset = { id: $stateParams.assetId > 0 ? $stateParams.assetId : null, name: '', description: '' };
            vm.assetLevelStats = [];
            vm.metrics = [];


            // Get overall summary information for the specified asset (and children)

            abp.ui.setBusy(
                null,
                assetHealthService.getAssetHealthMetricValues()
                    .success(function (data) {
                        vm.metrics = data.metrics;
                    })
                );

            vm.refreshAssets = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssets({ AssetTypeId: null }).success(function (data) {
                        vm.assets = data.assets;
                    })
                );
            };
            //vm.refreshAssets();
        }
    ]);
})();