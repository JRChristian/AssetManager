(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.admin.userlist';
    app.controller(controllerId, [
        '$scope', '$location', 'abp.services.app.user',
        function ($scope, $location, userService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.users = [];

           abp.ui.setBusy(
                null,
                userService.getAllUsers().success(function (data) {
                    vm.users = data.users;
                })
            );
        }
    ]);
})();