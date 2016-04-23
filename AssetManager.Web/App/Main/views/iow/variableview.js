(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.variable.view';
    app.controller(controllerId, [
        '$scope', '$location', '$stateParams', 'abp.services.app.iowVariable', 
        function ($scope, $location, $stateParams, variableService) {
            var vm = this;

            vm.localize = abp.localization.getSource('AssetManager');

            vm.variable = {
                id: $stateParams.Id > 0 ? $stateParams.Id : null,
                name: '',
                description: '',
                tagName: '',
                uom: ''
            };

            vm.gridlimits = {
                data: [],
                enableSorting: false,
                enableColumnResizing: true,
                columnDefs: [
                    {
                        name: 'isActive', width: '10%', displayName: vm.localize('IsActive'), 
                        cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD ? "X" : ""}}</div>'
                    },
                    {
                        name: 'name', width: '10%', displayName: vm.localize('Name'),
                        cellTemplate: '<div class="grid-tooltip" tooltip="{{ row.entity.description }}" tooltip-placement="top" tooltip-append-to-body="true">'
                            + '<div class="ui-grid-cell-contents">{{ COL_FIELD }}</div></div>'
                    },
                    { name: 'lowLimit', width: '10%', displayName: vm.localize('LowLimit') },
                    { name: 'highLimit', width: '10%', displayName: vm.localize('HighLimit') },
                    { name: 'cause', width: '20%', displayName: vm.localize('Causes') },
                    { name: 'consequences', width: '20%', displayName: vm.localize('Consequences') },
                    { name: 'action', width: '20%', displayName: vm.localize('Action') }
                    ]
            };

            abp.ui.setBusy(
                null,
                variableService.getOneIowVariable({ Id: vm.variable.id })
                    .success(function (data) { vm.variable = data })
                );

            vm.limits = [];
            abp.ui.setBusy(
                null,
                variableService.getIowLimits({ variableId: vm.variable.id })
                    .success(function (data) { vm.gridlimits.data = data.limits })
                );
        }
    ]);
})();
