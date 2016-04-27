(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.edit';
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
                precision: ''
            };

            abp.ui.setBusy(
                null,
                tagService.getOneTag({ Id: vm.tag.id })
                    .success(function (data) { vm.tag = data })
                );

            vm.saveTag = function () {
                abp.ui.setBusy(
                    null,
                    tagService.updateTag(vm.tag)
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(localize("TagUpdatedOk"), vm.tag.name));
                            $location.path('/taglist');
                        }));
            };
        }
    ]);
})();