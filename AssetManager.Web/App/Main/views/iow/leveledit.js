(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.level.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iOWLevel',
        function ($scope, $location, $stateParams, levelService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.level = { id: $stateParams.levelId > 0 ? $stateParams.levelId : null };

            abp.ui.setBusy(
                null,
                levelService.getOneIOWLevel({ Id: vm.level.id })
                    .success(function (data) { vm.level = data })
                );

            vm.saveLevel = function () {
                abp.ui.setBusy(
                    null,
                    levelService.updateIOWLevel(vm.level)
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(vm.localize("LevelUpdatedOk"), vm.level.name));
                            $location.path('/iowlevellist');
                        }));
            };
        }
    ]);
})();