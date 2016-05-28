(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assetvariable.assignment';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset', 'abp.services.app.iowVariable',
        function ($scope, $location, assetService, variableService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: true,
                modifierKeysToMultiSelect: true,
                enableSorting: true,
                enableFiltering: true,
                showGridFooter: true,
                columnDefs: [
                    {
                        name: 'id', displayName: '', width: 35, minWidth: 35, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs"><a class="btn btn-default" ui-sref="iowvariableview({ Id: row.entity.id })"><i class="fa fa-binoculars"></i></a></div></div>'
                    },
                    { name: 'name', displayName: vm.localize('Name'), width: '50%' },
                    {
                        name: 'description', displayName: vm.localize('Description'), width: '*',
                        cellTemplate: '<div class="ui-grid-cell-contents" uib-tooltip="{{row.entity.description}}, {{row.entity.tagName}}, {{row.entity.uom}}" tooltip-append-to-body=true>{{row.entity.description}}</div>'
                    }]
            };

            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.gridOptions2 = {
                data: [],
                onRegisterApi: registerGridApi2,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: true,
                modifierKeysToMultiSelect: true,
                enableExpandAll: true,
                enableFiltering: true,
                enableSorting: true,
                showGridFooter: true,
                showTreeExpandNoChildren: false,
                showTreeRowHeader: true,
                columnDefs: [
                    { name: 'name', displayName: vm.localize('Name'), width: '20%' },
                    { name: 'description', displayName: vm.localize('Description'), width: '40%' },
                    { name: 'assetTypeName', displayName: vm.localize('AssetType'), width: '20%' }]
            };
            function registerGridApi2(gridApi) { vm.gridApi2 = gridApi; }


            vm.refreshData = function () {
                variableService.getAllVariables({})
                    .success(function (data) {
                        vm.gridOptions.data = data.variables;
                    });
                assetService.getAssetHierarchyAsList({})
                    .success(function (data) {
                        for (var i = 0; i < data.assetHierarchy.length; i++)
                            data.assetHierarchy[i].$$treeLevel = data.assetHierarchy[i].level;
                        vm.gridOptions2.data = data.assetHierarchy;
                    });
            };

            vm.selectAllVariables = function () {
                vm.gridApi.selection.selectAllRows();
            };

            vm.clearAllVariables = function () {
                vm.gridApi.selection.clearSelectedRows();
            };

            vm.selectAllAssets = function () {
                vm.gridApi2.selection.selectAllRows();
            };

            vm.clearAllAssets = function () {
                vm.gridApi2.selection.clearSelectedRows();
            };

            vm.expandAllAssets = function () {
                vm.gridApi2.treeBase.expandAllRows();
            };

            vm.collapseAllAssets = function () {
                vm.gridApi2.treeBase.collapseAllRows();
            };

            vm.refreshData();
        }
    ]);
})();