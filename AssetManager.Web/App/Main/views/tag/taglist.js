(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.list';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.tag',
        function ($scope, $location, tagService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.levels = [];

            vm.refreshLevels = function () {
                abp.ui.setBusy( //Set whole page busy until getTagListAsync completes
                    null,
                    tagService.getTagListAsync({ Name: '' }).success(function (data) { vm.tags = data.tags; })
                );
            };

            vm.refreshLevels();
        }
    ]);
})();