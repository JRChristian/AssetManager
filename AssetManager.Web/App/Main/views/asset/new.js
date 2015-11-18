﻿(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.asset.new';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset', 'abp.services.app.assetType',
        function ($scope, $location, assetService, assetTypeService) {
            var vm = this;

            var localize = abp.localization.getSource('AssetManager');

            vm.asset = {
                name: '',
                description: '',
                assetTypeId: null
            };

            vm.assettypes = [];
            abp.ui.setBusy( //Set whole page busy until getAssetTypes completes
                null,
                assetTypeService.getAllAssetTypes().success(function (data) {
                    vm.assettypes = data.assetTypes;
                }));

            vm.saveAsset = function () {
                abp.ui.setBusy(
                    null,
                    assetService.createAsset(vm.asset).success(function () {
                        abp.notify.info(abp.utils.formatString(localize("AssetCreatedOk"), vm.asset.name));
                        $state.go('^');//$location.path('/');
                    })
                );
            };
        }
    ]);
})();