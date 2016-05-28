(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assetvariablelist';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableFiltering: true,
                showGridFooter: true,
                columnDefs: [
                    { name: 'assetName', displayName: vm.localize('AssetName'), width: '50%' },
                    { name: 'variableName', displayName: vm.localize('IowVariable'), width: '50%' }]
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refresh = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetHealthService.getAssetVariableList({})
                        .success(function (data) {
                            vm.gridOptions.data = data.assetVariables;
                        })
                );
            };
            vm.refresh();
        }
    ]);
})();