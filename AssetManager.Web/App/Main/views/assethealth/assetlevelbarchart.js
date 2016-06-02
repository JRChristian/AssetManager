(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.barchart';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.canvasChart = new CanvasJS.Chart("chartContainer", {
                title: { text: "Chart Title" },
                data: []
            });

            assetHealthService.getAssetLevelChartCanvasJS({ AggregationHours: 0 })
                .success(function (data) {
                    vm.canvasChart.options = data.canvasJS;
                    vm.canvasChart.render();
                });
        }
    ]);
})();