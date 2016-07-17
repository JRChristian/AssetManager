(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assetlist';
    app.controller(controllerId, [
        '$scope', '$location', '$state', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, $state, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            // Arguments
            var id = isNaN($stateParams.AssetTypeId) ? null : Number($stateParams.AssetTypeId);
            vm.assetType = { id: id, name: '???' };
            vm.days = isNaN($stateParams.Days) ? 1 : Math.round($stateParams.Days);
            vm.days = Math.max(vm.days, 1);
            vm.days = Math.min(vm.days, 60);

            // Defaults and global variables
            var today = new Date();
            vm.startDate = new Date();
            vm.startDate.setDate(today.getDate() - vm.days);
            vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');

            vm.gotoAssetView = function (assetId) {
                $state.go('assethealthassetview', { AssetId: assetId, Days: vm.days });
            };

            // Get list of asset types
            vm.assetTypes = [];
            abp.ui.setBusy(
                null,
                assetService.getAssetTypes().success(function (data) {
                    vm.assetTypes = data.assetTypes;
                    for (var i = 0; i < vm.assetTypes.length; i++) {
                        if (vm.assetType.id === vm.assetTypes[i].id)
                            vm.assetType = vm.assetTypes[i];
                    }
                }));

            // Toggle between yesterday and 30 days ago
            vm.changeDayRange = function () {
                vm.days = vm.days <= 1 ? 30 : 1;
                $state.go('assethealthassetlist', { AssetTypeId: vm.assetType.id, Days: vm.days });
                //vm.startDate = new Date();
                //vm.startDate.setDate(today.getDate() - vm.days);
                //vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
                //vm.refresh();
            };

            vm.gridOptions = {
                data: [],
                appScopeProvider: vm, // Needed to allow calling functions from within the grid
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableFiltering: true,
                enableGridMenu: true,
                enableSelectAll: true,
                exporterCsvFilename: 'AssetHealth.csv',
                exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
                exporterPdfDefaultStyle: { fontSize: 9 },
                exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
                exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'black' },
                exporterPdfHeader: { text: vm.assetType.name, style: 'headerStyle' },
                exporterPdfFooter: function (currentPage, pageCount) {
                    return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
                },
                exporterPdfCustomFormatter: function (docDefinition) {
                    docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
                    docDefinition.styles.footerStyle = { fontSize: 9, bold: false };
                    return docDefinition;
                },
                exporterPdfOrientation: 'portrait',
                exporterPdfPageSize: 'LETTER',
                exporterPdfMaxGridWidth: 400,
                exporterFieldCallback: function (grid, row, col, value) {
                    if( col.name.substring(0, 6) === 'levels' && Number.isFinite(value) )
                        value = Math.round(value * 100) / 100;
                    return value;
                },
                columnDefs: [
                    {
                        name: 'assetId', displayName: '', width: 60, minWidth: 35, enableSorting: false, enableFiltering: false, enableColumnMenus: false, exporterSuppressExport: true,
                        cellTemplate: '<div class="ui-grid-cell-contents">'
                                    + '<div class="btn-group btn-group-xs">'
                                    + '<a class="btn btn-default" ui-sref="assetedit({ assetId: row.entity.assetId })"><i class="fa fa-pencil-square-o"></i></a> '
                                    + '<button class="btn btn-default" type="submit" ng-click="grid.appScope.gotoAssetView(row.entity.assetId)"><i class="fa fa-bar-chart"></i></a>'
                                    //+ '<a class="btn btn-default" ui-sref="assethealthassetview({ AssetId: row.entity.assetId, Days: 1})"><i class="fa fa-bar-chart"></i></a>'
                                    + '</div>'
                                    + '</div>'
                    },
                    { name: 'assetName', displayName: vm.localize('Name'), width: '15%' },
                    { name: 'assetDescription', displayName: vm.localize('Description'), width: '30%' }
                ]
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.refresh = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetHealthService.getAssetLevelStatsForType({ AssetTypeId: vm.assetType.id, StartTimestamp: vm.startDate })
                        .success(function (data) {

                            vm.gridOptions.data = data.assetStats;
                            vm.gridOptions.exporterPdfHeader.text = data.assetTypeName;
                            var j = vm.gridOptions.columnDefs.length; // Number of columns present
                            // Add columns for each level in the data set
                            for (var i = 0; vm.gridOptions.data !== null && vm.gridOptions.data.length > 0 && i < vm.gridOptions.data[0].levels.length; i++) {
                                // Add a column named "n-Level-Metric" for each level, containing the metric value (a percentage)
                                vm.gridOptions.columnDefs[j++] = {
                                    name: 'levels[' + i.toString() + '].metricValue',
                                    displayName: vm.gridOptions.data[0].levels[i].criticality + '-' + vm.gridOptions.data[0].levels[i].levelName + ' Metric',
                                    cellFilter: 'number: 1',
                                    myErrorLevel: vm.gridOptions.data[0].levels[i].errorLevel,
                                    myWarningLevel: vm.gridOptions.data[0].levels[i].warningLevel,
                                    cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                                        if (grid.getCellValue(row, col) > col.colDef.myErrorLevel)
                                            return 'text-center text-danger';
                                        else if (grid.getCellValue(row, col) > col.colDef.myWarningLevel)
                                            return 'text-center text-warning';
                                        else
                                            return 'text-center';
                                    },
                                    enableColumnMenus: false,
                                    exporterPdfAlign: 'center',
                                    headerCellClass: 'text-center'
                                };
                                // Add a column named "n-Level-Hours" for each level, containing the hours of deviation
                                vm.gridOptions.columnDefs[j++] = {
                                    name: 'levels[' + i.toString() + '].durationHours',
                                    displayName: 'Hours', //vm.gridOptions.data[0].levels[i].criticality + '-' + vm.gridOptions.data[0].levels[i].levelName + ' Hours',
                                    cellFilter: 'number: 1',
                                    cellClass: 'text-center',
                                    enableColumnMenus: false,
                                    exporterPdfAlign: 'center',
                                    headerCellClass: 'text-center'
                                };
                            }
                        })
                );
            };

            vm.refresh();
        }
    ]);
})();