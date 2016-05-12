(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.level.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowLevel',
        function ($scope, $location, $stateParams, levelService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.deleteEnabled = false;
            vm.level = { id: $stateParams.levelId > 0 ? $stateParams.levelId : null };

            abp.ui.setBusy(
                null,
                levelService.getLevel({ Id: vm.level.id })
                    .success(function (data) {
                        vm.level = data.level;
                        if (data.levelUseCount == 0 && data.level != null && data.level.id > 0)
                            vm.deleteEnabled = true;
                    }));

            vm.saveLevel = function () {
                $scope.$broadcast('show-errors-check-validity');
                if ($scope.editLevelForm.$valid)
                {
                    abp.ui.setBusy(null, levelService.updateLevel(vm.level)
                        .success(function (data) {
                            abp.notify.success(abp.utils.formatString(vm.localize("LevelUpdatedOk"), data.name));
                            $location.path('/iowlevellist');
                        }));
                }
                else
                {
                    abp.notify.error(abp.utils.formatString(vm.localize("LevelNotUpdated"), vm.level.name));
                }
            };

            vm.deleteLevel = function () {
                abp.ui.setBusy(
                    null,
                    levelService.deleteLevel({ Id: vm.level.id, Name: vm.level.name })
                        .success(function (data) {
                            if( data.success == true )
                                abp.notify.success(abp.utils.formatString(vm.localize("LevelDeletedOk"), data.name));
                            else
                                abp.notify.error(abp.utils.formatString(vm.localize("LevelNotDeleted"), data.name));
                            $location.path('/iowlevellist');
                        }));
            };
        }
    ]);
})();