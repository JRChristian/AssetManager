﻿(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.asset.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetType',
        function ($scope, $location, $stateParams, assetService, assetTypeService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.asset = {
                id: $stateParams.assetId > 0 ? $stateParams.assetId : null,
                name: '',
                description: '',
                assetTypeId: null
            };

            abp.ui.setBusy(
                null,
                assetService.getOneAsset({ Id: vm.asset.id })
                    .success(function (data) {
                        vm.asset = data.asset;
                }));

            vm.assettypes = [];
            abp.ui.setBusy(
                null,
                assetTypeService.getAllAssetTypes().success(function (data) {
                    vm.assettypes = data.assetTypes;
                }));

            vm.saveAsset = function () {
                abp.ui.setBusy(
                    null,
                    assetService.updateAsset( vm.asset )
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(localize("AssetUpdatedOk"), vm.asset.name));
                            $location.path('/assetlist');
                    }));
            };
        }
    ]);
})();