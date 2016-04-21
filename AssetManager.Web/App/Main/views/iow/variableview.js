(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable',
        function ($scope, $location, $stateParams, variableService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                tagName: '',
                uom: ''
            };

            abp.ui.setBusy(
                null,
                variableService.getOneIowVariable({ Id: vm.variable.id })
                    .success(function (data) { vm.variable = data })
                );
        }
    ]);
})();