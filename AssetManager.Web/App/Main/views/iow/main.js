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

            // Flot example
            vm.chartData = [[[0, 1], [1, 5], [2, 2]]];
            vm.chartOptions = {};

            // CanvasJS example
            vm.canvasChart = new CanvasJS.Chart("chartContainer", {
                data: [{
                    type: "splineArea", //change it to column, spline, line, pie, etc
                    dataPoints: [
                        { x: 10, y: 10 },
                        { x: 20, y: 14 },
                        { x: 30, y: 18 },
                        { x: 40, y: 22 },
                        { x: 50, y: 18 },
                        { x: 60, y: 28 }
                    ]
                }]
            });
            vm.canvasChart.render();
        }
    ]);
})();