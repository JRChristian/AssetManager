(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.image.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', '$window', 'abp.services.app.image',
        function ($scope, $location, $stateParams, $window, imageService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');
            vm.imageArea = document.getElementById('imageArea');
            vm.imageWidth = vm.imageArea.clientWidth;
            //vm.imageWidth = $window.innerWidth - 120;

            vm.image = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null
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
                vm.imageWidth = vm.imageArea.clientWidth;
                //vm.imageWidth = $window.innerWidth - 120;
            };
        }
    ]);
})();