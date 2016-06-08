(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.limitview';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.asset', 'abp.services.app.assetHealth',
        function ($scope, $location, $stateParams, assetService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.asset = { id: $stateParams.assetId > 0 ? $stateParams.assetId : null, name: '', description: '' };
            vm.parentId = -1;
            vm.parentName = "";
            vm.hasParent = false;
            vm.children = [];
            vm.variables = [];
            vm.numberChildren = 0;
            vm.numberLimits = -1;

            // Set the start day to 30 days ago
            vm.startDay = new Date();
            vm.startDay.setDate(vm.startDay.getDate() - 30);
            vm.endDay = new Date();

            abp.ui.setBusy(
                null,
                assetService.getAssetRelatives({ id: vm.asset.id })
                    .success(function (data) {
                        vm.parentId = data.parent != null ? data.parent.id : -1;
                        vm.parentName = data.parent != null ? data.parent.name : "";
                        vm.hasParent = vm.parentId > 0 ? true : false;
                        vm.children = data.children;
                        vm.numberChildren = data.children.length;
                    })
                );

            abp.ui.setBusy(
                null,
                assetHealthService.getAssetVariableList({ assetId: vm.asset.id })
                    .success(function (data) {
                        if (data.assetVariables != null && data.assetVariables.length > 0)
                            vm.variables = data.assetVariables;
                        else
                            vm.variables = [];
                    })
                );

            abp.ui.setBusy(
                null,
                assetHealthService.getAssetLimitChartByDay({ assetId: vm.asset.id, startTimestamp: vm.startDay })
                    .success(function (data) {
                        vm.asset.id = data.assetId;
                        vm.asset.name = data.assetName;
                        vm.startDay = data.startTimestamp;
                        vm.endDay = data.endTimestamp;
                        vm.numberLimits = data.numberLimits;
                        vm.canvasChart = new CanvasJS.Chart("chartContainer", { data: [] });
                        vm.canvasChart.options = data.canvasJS;
                        vm.canvasChart.render();
                    })
                );
        }
    ]);
})();