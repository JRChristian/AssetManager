(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable', 'abp.services.app.tag',
        function ($scope, $location, $stateParams, variableService, tagService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                uom: ''
            };

            abp.ui.setBusy(
                null,
                variableService.getOneIowVariable({ Id: vm.variable.id })
                    .success(function (data) { vm.variable = data } )
                    //if (data != null) { vm.variable.name = data.name; vm.variable.description = data.description; vm.variable.uom = data.uom }
                );

            vm.tags = [];
            abp.ui.setBusy(
                null,
                tagService.getTagListAsync({})
                    .success(function (data) { vm.tags = data.tags })
                    );

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