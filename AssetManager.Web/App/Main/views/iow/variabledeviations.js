(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.deviations';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$filter', 'abp.services.app.iowDeviation',
        function ($scope, $location, $stateParams, $filter, deviationService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.limitClass = ['label label-default', 'label label-danger', 'label label-warning', 'label label-default', '']; // Used for Bootstrap class labels

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                tagName: '',
                uom: '',
                limits: []
            };

            vm.canvasChart = null;

            abp.ui.setBusy(
                null,
                deviationService.getVariableDeviations({ Id: vm.variable.id })
                    .success(function (data) {
                        vm.variable = data;
                    })
                );

            // Set the start day to 30 days ago
            vm.startDay = new Date();
            vm.startDay.setDate(vm.startDay.getDate() - 30);
            vm.endDay = new Date();

            abp.ui.setBusy(
                null,
                deviationService.getLimitStatsChartByDay({ VariableId: vm.variable.id, StartTimestamp: vm.startDay })
                    .success(function (data) {
                        vm.startDay = data.startTimestamp;
                        vm.endDay = data.endTimestamp;
                        vm.canvasChart = new CanvasJS.Chart("chartContainer", { data: [] });
                        vm.canvasChart.options = data.canvasJS;
                        vm.canvasChart.render();
                    })
                );
        }
    ]);
})();
