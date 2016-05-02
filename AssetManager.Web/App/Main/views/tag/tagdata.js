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
                data: []
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

            tagDataService.getTagDataCanvasJS({ id: $stateParams.tagId })
                .success(function (data) {
                    vm.name = data.name;
                    vm.description = data.description;
                    vm.uom = data.uom;
                    vm.precision = data.precision;

                    vm.canvasChart.options = data.canvasJS;
                    vm.canvasChart.render();
                });
        }
    ]);
})();