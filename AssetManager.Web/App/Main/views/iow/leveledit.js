(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.level.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowLevel',
        function ($scope, $location, $stateParams, levelService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.level = { id: $stateParams.levelId > 0 ? $stateParams.levelId : null };

            abp.ui.setBusy(
                null,
                levelService.getLevel({ Id: vm.level.id })
                    .success(function (data) { vm.level = data })
                );

            vm.saveLevel = function () {
                abp.ui.setBusy(
                    null,
                    levelService.updateLevel(vm.level)
                        .success(function (data) {
                            abp.notify.info(abp.utils.formatString(vm.localize("LevelUpdatedOk"), data.name));
                            $location.path('/iowlevellist');
                        }));
            };

            vm.deleteLevel = function () {
                abp.ui.setBusy(
                    null,
                    levelService.deleteLevel({ Id: vm.level.id, Name: vm.level.name })
                        .success(function (data) {
                            if( data.success == true )
                                abp.notify.info(abp.utils.formatString(vm.localize("LevelDeletedOk"), data.name));
                            else
                                abp.notify.info(abp.utils.formatString(vm.localize("LevelNotDeleted"), data.name));
                            $location.path('/iowlevellist');
                        }));
            };
        }
    ]);
})();