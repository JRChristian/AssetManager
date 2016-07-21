(function () {
    var app = angular.module('app');

    var controllerId = 'app.views.asset.assettree';
    app.controller(controllerId, [
        '$scope', 'abp.services.app.asset',
        function ($scope, assetService) {
            var vm = this;
            vm.localize = abp.localization.getSource('AssetManager');
            vm.assetUpdates = [];
            vm.treeData = [];
            vm.treeOptions = {
                accept: function (sourceNodeScope, destNodesScope, destIndex) {
                    return true;
                },
                dropped: function (e) {
                    var asset = e.source.nodeScope.$modelValue;
                    var dest = e.dest.nodesScope.node;
                    if( typeof asset !== "undefined" && typeof asset.title !== "undefined") {
                        if (typeof dest !== "undefined" && typeof dest.title !== "undefined" && asset.parentAssetTreeDtoId !== dest.id)
                            // New parent
                            vm.assetUpdates.push({ Name: asset.title, ParentAssetName: dest.title });
                        else if (typeof dest === "undefined")
                            // No parent -- move to top of hierarchy
                            vm.assetUpdates.push({ Name: asset.title, ParentAssetName: "" });
                    };
                    return true;
                },
            };

            vm.refresh = function () {
                abp.ui.setBusy( //Set whole page busy until getAssets completes
                    null,
                    assetService.getAssetTree({})
                        .success(function (data) {
                            vm.treeData = data.assetTree;
                            vm.assetUpdates = [];
                        })
                );
            };

            vm.remove = function (scope) {
                scope.remove();
            };

            vm.toggle = function (scope) {
                scope.toggle();
            };

            vm.moveLastToTheBeginning = function () {
                var a = vm.treeData.pop();
                vm.treeData.splice(0, 0, a);
            };

            vm.newSubItem = function (scope) {
                var nodeData = scope.$modelValue;
                nodeData.nodes.push({
                    id: nodeData.id * 10 + nodeData.nodes.length,
                    title: nodeData.title + '.' + (nodeData.nodes.length + 1),
                    nodes: []
                });
            };

            vm.visible = function (item) {
                return !($scope.query && $scope.query.length > 0
                && item.title.indexOf($scope.query) === -1);
            };

            vm.findNodes = function () {
            };

            vm.expandAll = function () {
                $scope.$broadcast('angular-ui-tree:expand-all');
            };

            vm.collapseAll = function () {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            };

            vm.save = function () {
                abp.ui.setBusy(
                    null,
                    assetService.updateAssetHierarchy({ Assets: vm.assetUpdates })
                        .success(function () {
                            abp.notify.info(abp.utils.formatString(localize("AssetUpdatedOk"), vm.asset.name));
                            //$location.path('/assetlist');
                            vm.refresh();
                        }));
            };

            vm.cancel = function () {
                vm.refresh();
            };

            vm.refresh();
        }
    ]);
})();