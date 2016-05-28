(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.asset.assethierarchy';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset',
        function ($scope, $location, assetService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableExpandAll: true,
                enableFiltering: true,
                enableSorting: true,
                enableGridMenu: true,
                showTreeExpandNoChildren: false,
                showTreeRowHeader: true,
                columnDefs: [
                    { name: 'name', displayName: vm.localize('Name'), width: '20%' },
                    { name: 'description', displayName: vm.localize('Description'), width: '40%' },
                    { name: 'assetTypeName', displayName: vm.localize('AssetType'), width: '20%' }]
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refresh = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssetHierarchyAsList({})
                        .success(function (data) {
                            for (var i = 0; i < data.assetHierarchy.length; i++)
                                data.assetHierarchy[i].$$treeLevel = data.assetHierarchy[i].level;
                            vm.gridOptions.data = data.assetHierarchy;
                        })
                );
            };

            vm.expandAll = function () {
                vm.gridApi.treeBase.expandAllRows();
            };

            vm.collapseAll = function () {
                vm.gridApi.treeBase.collapseAllRows();
            };

            vm.refresh();
        }
    ]);
})();