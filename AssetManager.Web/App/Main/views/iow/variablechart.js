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
                    for (var i = 0; i < vm.canvasChart.options.data.length; i++ ) {
                        if (vm.canvasChart.options.data[i].type == "rangeArea") {
                            for (var j = 0; j < vm.canvasChart.options.data[i].dataPoints.length; j++) {
                                y = vm.canvasChart.options.data[i].dataPoints[j].y;
                                z = vm.canvasChart.options.data[i].dataPoints[j].z;
                                vm.canvasChart.options.data[i].dataPoints[j].y = [y,z];
                            }
                        }
                    }

                    vm.canvasChart.render();
                });
        }
    ]);
})();