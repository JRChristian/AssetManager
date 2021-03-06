﻿(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.level.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.iowLevel',
        function ($scope, $location, levelService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.levels = [];

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until getLevels completes
                    null,
                    levelService.getAllLevels().success(function (data) { vm.levels = data.iowLevels; })
                );
            };

            vm.createDefaults = function () {
                levelService.createDefaultLevels();
                vm.refreshLevels();
            };

            vm.refreshLevels();
        }
    ]);
})();