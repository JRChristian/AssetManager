angular.module('app')
    .filter('momentFromNow', function () {
        return function (input) {
            return moment(input).fromNow();
        };
    });


/* Possible new filter
angular.module('myApp', [])
  .filter('moment', [
    function () {
        return function (date, method) {
            var momented = moment(date);
            return momented[method].apply(momented, Array.prototype.slice.call(arguments, 2));
        };
    }
  ]);
*/