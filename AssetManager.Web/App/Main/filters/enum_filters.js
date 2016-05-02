angular.module('app')
    .filter('tagType', function () {
        return function (input) {
            var localize = abp.localization.getSource('AssetManager');
            allTypes = new Array(localize('TagContinuous'), localize('TagEvent'));
            return allTypes[input];
        };
    });