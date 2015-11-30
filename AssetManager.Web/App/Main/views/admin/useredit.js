(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.admin.useredit';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.user',
        function ($scope, $location, $stateParams, userService) {
            var vm = this;
            var localize = abp.localization.getSource('AssetManager');

            vm.user = {
                id: $stateParams.userId > 0 ? $stateParams.userId : null,
                userName: '',
                name: '',
                surname: '',
                emailAddress: '',
                tenancyName: 'Default'
            };

            abp.ui.setBusy(
                null,
                userService.getOneUser({ UserId: vm.user.id })
                    .success(function (data) {
                        vm.user = data.user;
                    }));

            vm.saveUser = function () {
                abp.ui.setBusy(
                    null,
                    userService.updateUser(vm.user)
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(localize("UserUpdatedOk"), vm.user.userName));
                            $location.path('/userlist');
                        }));
            };
        }
    ]);
})();