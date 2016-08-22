(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.action';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.descriptionText = vm.localize('Description');
            vm.responseGoalText = vm.localize('ResponseGoal');

            vm.asset = { id: $stateParams.AssetId > 0 ? $stateParams.AssetId : null, name: '', description: '' };
            vm.levelsUsed = [];
            vm.variableLimits = [];

            // Set the start day to 24 hours ago
            vm.startDay = new Date();
            vm.startDay.setDate(vm.startDay.getDate() - 1);
            vm.endDay = new Date();
            vm.hours = Math.abs(vm.endDay - vm.startDay) / 3600000;

            abp.ui.setBusy( //Set whole page busy until the service completes
                null,
                assetHealthService.getAssetLimitCurrentStatus({ assetId: vm.asset.id, maximumHoursSinceLastDeviation: vm.hours })
                    .success(function (data) {
                        vm.asset = data.asset;
                        vm.levelsUsed = data.levelsUsed;
                        vm.variableLimits = data.variableLimits;
                    })
                );
        }
    ]);
})();