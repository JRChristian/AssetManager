(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.chart';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable',
        function ($scope, $location, $stateParams, variableService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null
            };

            vm.canvasChart = new CanvasJS.Chart("chartContainer", {
                title: { text: "Chart Title" },
                axisX: { gridThickness: 1 },
                axisY: { title: "Measurement Units" },
                exportEnabled: true,
                data: []
            });

            variableService.getIowChartCanvasJS({ id: $stateParams.Id })
                .success(function (data) {
                    vm.name = data.name;
                    vm.description = data.description;

                    vm.canvasChart.options = data.canvasJS;
                    vm.canvasChart.render();
                });
        }
    ]);
})();