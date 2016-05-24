(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.image.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$window', 'abp.services.app.image',
        function ($scope, $location, $stateParams, $window, imageService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');
            vm.imageWidth = $window.innerWidth - 120;

            vm.image = {
                id: $stateParams.id > 0 ? $stateParams.id : null
            };

            abp.ui.setBusy(
                null,
                imageService.getImage({ Id: vm.image.id })
                    .success(function (data) { vm.image = data.image })
                );

            vm.fullSize = function () {
                vm.imageWidth = '';
            };

            vm.windowSize = function() {
                vm.imageWidth = $window.innerWidth - 120;
            };
        }
    ]);
})();