(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.tag',
        function ($scope, $location, $stateParams, tagService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.tag = {
                id: $stateParams.tagId > 0 ? $stateParams.tagId : null,
                name: '',
                description: '',
                uom: '',
                precison: '',
                type: 0
            };

            abp.ui.setBusy(
                null,
                tagService.getOneTag({ Id: vm.tag.id })
                    .success(function (data) { vm.tag = data })
                );
        }
    ]);
})();