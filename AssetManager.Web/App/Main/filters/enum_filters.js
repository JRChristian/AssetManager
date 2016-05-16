angular.module('app')
    .filter('tagQuality', function () {
        return function (input) {
            var localize = abp.localization.getSource('AssetManager');
            all = new Array(localize('QualityBad'), localize('QualityUncertain'), localize('QualityGood'));
            return all[input];
        };
    })
    .filter('tagType', function () {
        return function (input) {
            var localize = abp.localization.getSource('AssetManager');
            all = new Array(localize('TagContinuous'), localize('TagEvent'));
            return all[input];
        };
    })
    .filter('direction', function () {
        return function (input) {
            var localize = abp.localization.getSource('AssetManager');
            all = new Array(localize('DirectionNone'), localize('DirectionLow'), localize('DirectionFocus'), localize('DirectionHigh'));
            return all[input];
        };
    })
    .filter('deviation', function () {
        return function (input) {
            var localize = abp.localization.getSource('AssetManager');
            all = new Array(localize('DeviationNormal'), localize('DeviationClosed'), localize('DeviationOpen'));
            return all[input];
        };
    })
;