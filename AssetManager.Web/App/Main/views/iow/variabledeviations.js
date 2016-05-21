(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.deviations';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$filter', 'abp.services.app.iowDeviation',
        function ($scope, $location, $stateParams, $filter, deviationService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                tagName: '',
                uom: '',
                limits: []
            };

            abp.ui.setBusy(
                null,
                deviationService.getVariableDeviations({ Id: vm.variable.id })
                    .success(function (data) {
                        vm.variable = data;
                    })
                );
        }
    ]);
})();
