(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$filter', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, $filter, variableService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.assets = [];
            vm.alreadyFetchedAssets = false;

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                tagName: '',
                uom: '',
                limits: []
            };

            abp.ui.setBusy(
                null,
                variableService.getVariableLimits({ Id: vm.variable.id, IncludeUnusedLimits: false })
                    .success(function (data) {
                        vm.variable = data;
                    })
                );

            vm.assetList = function () {
                // Only fetch the asset list once
                if (!vm.alreadyFetchedAssets) {
                    abp.ui.setBusy(
                        null,
                        assetHealthService.getAssetVariableList({ VariableId: vm.variable.id })
                            .success(function (data) {
                                vm.assets = data.assetVariables;
                                vm.alreadyFetchedAssets = true;
                            })
                        );
                };
            };
        }
    ]);
})();
