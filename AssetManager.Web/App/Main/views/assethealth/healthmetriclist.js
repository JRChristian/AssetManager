(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.metriclist';
    app.controller(controllerId, [
        '$scope', '$location', '$uibModal', 'abp.services.app.assetHealth',
        function ($scope, $location, $uibModal, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.metric = { id: 0, name: '' };

            vm.metrics = [];
            vm.refresh = function () {
                abp.ui.setBusy(
                    null,
                    assetHealthService.getHealthMetricList({}).success(function (data) {
                        vm.metrics = data.metrics;
                    }));
            };
            vm.refresh();

            vm.new = function () {
                vm.metric = { id: 0, name: '', assetTypeId: 0, applyToEachAsset: false, levelId: 0, period: 30, metricType: 1, goodDirection: 3, warningLevel: 75, errorLevel: 50, order: 99 };
                vm.open(vm.metric);
            };

            vm.open = function (metric) {
                var modalInstance = $uibModal.open({
                    templateUrl: 'assetHealthMetricEdit.html',
                    controller: 'app.views.assethealth.metric.modal as vm',
                    resolve: { metric: function () { return metric; } }
                });

                modalInstance.result.then(function (metric) {
                    vm.metric = metric;
                    if (vm.metric.action == 'delete') {
                        abp.ui.setBusy(null, assetHealthService.deleteHealthMetric({ Id: vm.metric.id })
                            .success(function () {
                                abp.notify.info(abp.utils.formatString(vm.localize("AssetHealthMsgMetricDeleted"), vm.metric.name));
                                vm.refresh();
                            }));
                    }
                    else {
                        abp.ui.setBusy(null, assetHealthService.updateHealthMetric({ metric: vm.metric })
                            .success(function () {
                                abp.notify.info(abp.utils.formatString(vm.localize("AssetHealthMsgMetricUpdated"), vm.metric.name));
                                vm.refresh();
                            }));
                    }
                });
            };
        }
    ]);

    app.controller('app.views.assethealth.metric.modal', [
    '$scope', '$log', '$uibModalInstance', 'abp.services.app.asset', 'abp.services.app.iowLevel', 'abp.services.app.assetHealth', 'metric',
    function ($scope, $log, $uibModalInstance, assetService, levelService, assetHealthService, metric) {
        var vm = this;
        vm.localize = abp.localization.getSource('AssetManager');
        vm.metric = metric;

        vm.disableDelete = vm.metric.id > 0 ? false : true;

        vm.metricTypes = [];
        abp.ui.setBusy(
            null,
            assetHealthService.getHealthMetricTypes().success(function (data) {
                vm.metricTypes = data.metricTypes;
            }));

        vm.assetTypes = [];
        abp.ui.setBusy(
            null,
            assetService.getAssetTypes().success(function (data) {
                vm.assetTypes = data.assetTypes;
            }));

        vm.levels = [];
        abp.ui.setBusy( //Set whole page busy until getLevels completes
            null,
            levelService.getAllLevels().success(function (data) { vm.levels = data.iowLevels; })
        );

        $scope.ok = function () {
            vm.metric.action = 'save';
            $uibModalInstance.close(vm.metric);
        };

        $scope.delete = function () {
            vm.metric.action = 'delete';
            $uibModalInstance.close(vm.metric);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }]);
})();