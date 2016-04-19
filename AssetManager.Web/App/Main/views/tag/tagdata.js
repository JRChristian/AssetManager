(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.tag.data';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.tagDataRaw',
        function ($scope, $location, $stateParams, tagDataRawService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.qualitystatus = new Array(vm.localize('QualityBad'), vm.localize('QualityUncertain'), vm.localize('QualityGood'));

            tagDataRawService.getTagDataRawList({ id: $stateParams.tagId })
                .success(function (data) {
                    vm.name = data.name;
                    vm.description = data.description;
                    vm.uom = data.uom;
                    vm.data = [];
                    vm.data = data.tagDataRaw;
                });
        }
    ]);
})();