(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.iow.dashboard';
    app.controller(controllerId, [
        '$scope', '$log', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $log, $location, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.$log = $log;
            console.log('hi');
            vm.localize = abp.localization.getSource('AssetManager');
            vm.asset = { id: $stateParams.assetId > 0 ? $stateParams.assetId : null, name: '', description: '' };
            vm.assetLevelStats = [];

            // Set the start day to 30 days ago
            vm.startDay = new Date();
            vm.startDay.setDate(vm.startDay.getDate() - 30);
            vm.endDay = new Date();
            vm.durationHours = -1;

            // Get overall summary information for the specified asset (and children)
            abp.ui.setBusy( //Set whole page busy until the service completes
                null,
                assetHealthService.getAssetLevelStats({ assetId: vm.asset.id, startTimestamp: vm.startDay })
                    .success(function (data) {
                        vm.durationHours = data.durationHours;
                        vm.assetLevelStats = data.assetLevelStats;
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