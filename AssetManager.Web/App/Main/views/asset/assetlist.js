﻿        (function () {
    var app = angular.module('app');

    var controllerId = 'app.views.asset.assetlist';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.asset',
        function ($scope, $location, assetService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableFiltering: true,
                enableGridMenu: true,
                columnDefs: [
                    {
                        name: 'id', displayName: vm.localize('Action'), width: 85, minWidth: 50, enableSorting: false, enableFiltering: false, enableColumnMenus: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="btn-group btn-group-xs">'
                                    + '<a class="btn btn-default" ui-sref="assetedit({ assetId: row.entity.id })"><i class="fa fa-pencil-square-o"></i></a> '
                                    + '<a class="btn btn-default" ui-sref="assetlimitview({ assetId: row.entity.id })"><i class="fa fa-bar-chart"></i></a>'
                                    + '</div></div>'
                    },
                    { name: 'name', displayName: vm.localize('Name'), width: '20%' },
                    { name: 'description', displayName: vm.localize('Description'), width: '35%' },
                    { name: 'assetTypeName', displayName: vm.localize('AssetType'), width: '10%' },
                    { name: 'materials', displayName: vm.localize('Materials') }]
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refresh = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssets({})
                        .success(function (data) {
                            vm.gridOptions.data = data.assets;
                        })
                );
            };
            vm.refresh();
        }
    ]);
})();