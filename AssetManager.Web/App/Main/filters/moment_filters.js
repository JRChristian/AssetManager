angular.module('app')
    .filter('momentFromNow', function () {
        return function (input) {
            return moment(input).fromNow();
        };
    })
    .filter('momentFromNowBlank', function () {
        return function (input, alternate) {
            if (input !== null)
                return moment(input).fromNow();
            else
                return alternate;
        };
    })
    .filter('momentFromNow2', function () {
        return function (input) {
            return moment(input).format("MMM DD HH:mm:ss");
        };
    })
    .filter('momentFromNowBlankNull', function () {
        return function (input) {
            if (input !== null)
                return moment(input).format("MMM DD HH:mm:ss");
            else
                return '';
        };
    })
    .filter('momentFromNowBlankOngoing', function () {
        return function (input) {
            if (input !== null)
                return moment(input).format("MMM DD HH:mm:ss");
            else
                return 'ongoing';
        };
    })
    .filter('momentLocale', function () {
        return function (input) {
            return moment(input).format("LLL");
        };
    })
    .filter('moment', [
        // See http://stackoverflow.com/questions/14774486/use-jquery-timeago-or-momentjs-and-angularjs-together
        function () {
            return function (date, method) {
                var momented = moment(date);
                return momented[method].apply(momented, Array.prototype.slice.call(arguments, 2));
            };
        }
    ]);