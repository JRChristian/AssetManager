(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.edit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.tag', 
        function ($scope, $location, $stateParams, tagService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.chartEnabled = false;
            vm.tagTypes = [{ id: 0, name: vm.localize('TagContinuous') }, { id: 1, name: vm.localize('TagEvent') }];

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
                    .success(function (data) {
                        vm.tag = data;
                        if (vm.tag.id > 0)
                            vm.chartEnabled = true;
                    })
                );

            vm.saveTag = function () {
                abp.ui.setBusy(
                    null,
                    tagService.updateTag(vm.tag)
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(vm.localize("TagUpdatedOk"), vm.tag.name));
                            $location.path('/taglist');
                        }));
            };
        }
    ]);
})();