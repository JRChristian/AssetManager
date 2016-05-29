(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.assethealth.assetvariablelist';
    var modalControllerId = 'app.views.assethealth.assetvariablelist.modal';
    app.controller(controllerId, [
        '$scope', '$location', '$uibModal', 'abp.services.app.asset', 'abp.services.app.iowVariable', 'abp.services.app.assetHealth',
        function ($scope, $location, $uibModal, assetService, variableService, assetHealthService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.selectedVariable = {name: '?'};

            vm.gridOptions = {
                data: [],
                onRegisterApi: registerGridApi,
                enableSorting: true,
                enableFiltering: true,
                showGridFooter: true,
                columnDefs: [
                    { name: 'assetName', displayName: vm.localize('AssetName'), width: '50%' },
                    { name: 'variableName', displayName: vm.localize('IowVariable'), width: '50%' }]
            };
            function registerGridApi(gridApi) { vm.gridApi = gridApi; }

            vm.getData = function () {
                vm.variables = [];
                variableService.getAllVariables({})
                    .success(function (data) {
                        vm.variables = data.variables;
                    });
                vm.assetHierarchy = [];
                assetService.getAssetHierarchyAsList({})
                    .success(function (data) {
                        for (var i = 0; i < data.assetHierarchy.length; i++)
                            data.assetHierarchy[i].$$treeLevel = data.assetHierarchy[i].level;
                        vm.assetHierarchy = data.assetHierarchy;
                    });
                vm.assetVariables = [];
                assetHealthService.getAssetVariableList({})
                    .success(function (data) {
                        vm.assetVariables = data.assetVariables;
                    });
            };

            vm.open = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: 'myModalContent.html',
                    controller: modalControllerId,
                    resolve: { variables: function () { return vm.variables; } }
                });

                modalInstance.result.then(function (selectedVariable) {
                    vm.selectedVariable = selectedVariable;
                });
            }

            vm.getData();
        }
    ]);

    app.controller(modalControllerId, [
        '$scope', '$uibModalInstance', 'variables',
        function ($scope, $uibModalInstance, variables) {
            $scope.variables = variables;
            $scope.selected = {
                variable: $scope.variables[0]
            };

            $scope.ok = function () {
                $uibModalInstance.close($scope.selected.variable);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
    }]);
})();