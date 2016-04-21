(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable',
        function ($scope, $location, $stateParams, variableService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                uom: ''
            };

            variableService.getOneIowVariable({ Id: vm.variable.id })
                .success(function (data) { vm.variable.name = data.name; vm.variable.description = data.description; vm.variable.uom = data.uom });

            vm.saveVariable = function () {
                abp.ui.setBusy(
                    null,
                    variableService.updateIowVariable(vm.variable)
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(localize("VariableUpdatedOk"), vm.variable.name));
                            $location.path('/iowvariablelist');
                        }));
            };
        }
    ]);
})();