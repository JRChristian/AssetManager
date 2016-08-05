(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.deviations';
    app.controller(controllerId, [
        '$scope', '$location', '$state', '$stateParams', '$filter', 'abp.services.app.iowDeviation',
        function ($scope, $location, $state, $stateParams, $filter, deviationService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.limitClass = ['label label-default', 'label label-danger', 'label label-warning', 'label label-default', '']; // Used for Bootstrap class labels

            // Arguments
            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                limits: []
            };
            vm.days = $stateParams.Days > 0 ? $stateParams.Days : 30;
            vm.days = vm.days <= 60 ? vm.days : 60;

            // Defaults and global variables
            vm.canvasChart = null;
            var today = new Date();
            vm.startDate = new Date();
            vm.startDate.setDate(today.getDate() - vm.days);
            vm.endDate = new Date();
            vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');

            vm.changeDayRange = function () {
                vm.days = vm.days <= 1 ? 30 : 1;
                $state.go('iowvariabledeviations', { Id: vm.variable.id, Days: vm.days });
                //vm.startDate = new Date();
                //vm.startDate.setDate(today.getDate() - vm.days);
                //vm.viewButtonLabel = vm.days <= 1 ? vm.localize('AssetHealthBtnViewLast30Days') : vm.localize('AssetHealthBtnViewToday');
                //vm.refresh();
            };

            abp.ui.setBusy(
                null,
                deviationService.getVariableDeviations({ Id: vm.variable.id, StartTimestamp: vm.startDate })
                    .success(function (data) {
                        vm.variable = data;
                    })
                );

            abp.ui.setBusy(
                null,
                deviationService.getLimitStatsChartByDay({ VariableId: vm.variable.id, StartTimestamp: vm.startDate })
                    .success(function (data) {
                        vm.startDate = data.startTimestamp;
                        vm.endDate = data.endTimestamp;
                        vm.canvasChart = new CanvasJS.Chart("chartContainer", { data: [] });
                        vm.canvasChart.options = data.canvasJS;
                        vm.canvasChart.render();
                    })
                );
        }
    ]);
})();
