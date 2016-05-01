(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.data';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.tagData',
        function ($scope, $location, $stateParams, tagDataService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.qualitystatus = new Array(vm.localize('QualityBad'), vm.localize('QualityUncertain'), vm.localize('QualityGood'));

            vm.canvasChart = new CanvasJS.Chart("chartContainer", {
                title: { text: "Chart Title" },
                axisX: { gridThickness: 1 },
                axisY: { title: "Measurement Units" },
                exportEnabled: true,
                data: [
                  {
                      type: "line",
                      markerType: "none",
                      xValueType: "dateTime",
                      color: "rgba(0,75,141,0.7)",
                      dataPoints: []
                  }
                ]
            });

            tagDataService.getTagDataRawList({ id: $stateParams.tagId })
                .success(function (data) {
                    vm.name = data.name;
                    vm.description = data.description;
                    vm.uom = data.uom;
                    vm.precision = data.precision;
                    vm.data = [];
                    vm.data = data.tagDataRaw;
                });

            tagDataService.getTagDataChart({ id: $stateParams.tagId })
                .success(function (data) {
                    vm.name = data.name;
                    vm.description = data.description;
                    vm.uom = data.uom;
                    vm.precision = data.precision;

                    vm.canvasChart.options.title.text = data.name;
                    vm.canvasChart.options.axisX.minimum = new Date(data.minTimestampJS);
                    vm.canvasChart.options.axisY.title = data.uom;
                    vm.canvasChart.options.data[0].dataPoints = data.tagDataChart;
                    vm.canvasChart.render();
                });
        }
    ]);
})();