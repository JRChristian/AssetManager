(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.iow.dashboard';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset', 'abp.services.app.assetType', 'abp.services.app.tagData',
        function ($scope, $location, assetService, assetTypeService, tagDataService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.assets = [];

            vm.refreshAssets = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssets({ AssetTypeId: null }).success(function (data) {
                        vm.assets = data.assets;
                    })
                );
            };
            vm.refreshAssets();
        }
    ]);
})();