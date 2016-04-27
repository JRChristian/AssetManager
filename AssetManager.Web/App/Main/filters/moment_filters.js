angular.module('app')
    .filter('momentFromNow', function () {
        return function (input) {
            return moment(input).fromNow();
        };
    })
    .filter('momentFromNow2', function () {
        return function (input) {
            return moment(input).format("MMM DD hh:mm:ss");
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