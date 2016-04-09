(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.iow.main';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset', 'abp.services.app.assetType',
        function ($scope, $location, assetService, assetTypeService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.assets = [];
            vm.assettypes = [];

            $scope.selectedAssetType = 0;
            $scope.$watch('selectedAssetType', function (value) { vm.refreshAssets(); });

            abp.ui.setBusy( //Set whole page busy until getAssetTypes completes
                null,
                assetTypeService.getAllAssetTypes().success(function (data) {
                    vm.assettypes = data.assetTypes;
                }));

            vm.refreshAssets = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssets({
                        AssetTypeId: $scope.selectedAssetType > 0 ? $scope.selectedAssetType : null
                    }).success(function (data) {
                        vm.assets = data.assets;
                    })
                );
            };

            vm.getAssetCountText = function () {
                return abp.utils.formatString(vm.localize('AssetCount'), vm.assets.length);
            };

            function randomIntFromInternal(min, max) {
                return Math.floor(Math.random() * (max - min + 1) + min);
            };
            
            vm.myStatus = function () {
                var status = "Normal";
                switch (randomIntFromInternal(1, 10)) {
                    case 1:
                        status = "Critical";
                        break;
                    case 2:
                        status = "Error";
                        break;
                    case 3:
                        status = "Warning";
                        break;
                    default:
                        status = "Normal";
                }
                return status;
            };
        }
    ]);
})();