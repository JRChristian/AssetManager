(function () {
    'use strict';

    var app = angular.module('app', [
        'ngAnimate',
        'ngSanitize',

        'ui.router',
        'ui.bootstrap',
        'ui.jq',
        'ui.grid',      // Angular UI Grid (ui-grid)
        'ui.grid.edit',
        'ui.grid.rowEdit',
        'ui.grid.cellNav',
        'ui.grid.selection',
        'ui.grid.treeView',
        'ui.bootstrap.showErrors',  // Bootstrap form validation -- see http://blog.yodersolutions.com/bootstrap-form-validation-done-right-in-angularjs/

        'angular-flot', // Flot helper library -- see https://docs.omniref.com/js/npm/angular-flot/0.0.6 

        'abp'
    ]);

    // Bootstrap form validation -- see http://blog.yodersolutions.com/bootstrap-form-validation-done-right-in-angularjs/
    app.config(['showErrorsConfigProvider', function (showErrorsConfigProvider) {
        showErrorsConfigProvider.showSuccess(true);
    }]);

    //Configuration for Angular UI routing.
    //menu: entries *must* match names in AssetManagerNavigationProvider
    app.config([
        '$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/');
            $stateProvider
                .state('home', {
                    url: '/',
                    templateUrl: '/App/Main/views/home/home.cshtml',
                    menu: 'Home' //Matches to name of 'Home' menu in AssetManagerNavigationProvider
                })
                .state('about', {
                    url: '/about',
                    templateUrl: '/App/Main/views/about/about.cshtml',
                    menu: 'About' //Matches to name of 'About' menu in AssetManagerNavigationProvider
                })
                .state('assetlist', {
                    url: '/assetlist',
                    templateUrl: '/App/Main/views/asset/assetlist.cshtml',
                    menu: 'AssetList' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetlistold', {
                    url: '/assetlistold',
                    templateUrl: '/App/Main/views/asset/list.cshtml',
                    menu: 'AssetListOld' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetedit', {
                    url: '/assetedit?assetId',
                    templateUrl: '/App/Main/views/asset/edit.cshtml',
                    menu: 'AssetEdit' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assethierarchy', {
                    url: '/assethierarchy',
                    templateUrl: '/App/Main/views/asset/assethierarchy.cshtml',
                    menu: 'AssetHierarchy' //Matches to menu name in AssetManagerNavigationProvider
                })
                .state('assetlevelbarchart', {
                    url: '/assetlevelbarchart',
                    templateUrl: '/App/Main/views/assethealth/assetlevelbarchart.cshtml',
                    menu: 'AssetLevelBarChart' //Matches to menu name in AssetManagerNavigationProvider
                })
                .state('assetnew', {
                    url: '/assetnew',
                    templateUrl: '/App/Main/views/asset/new.cshtml',
                    menu: 'AssetNew' //Matches to name of 'NewAsset' menu in AssetManagerNavigationProvider
                })
                .state('assettypeedit', {
                    url: '/assettypeedit',
                    templateUrl: '/App/Main/views/asset/assettypeedit.cshtml',
                    menu: 'AssetTypeEdit' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetvariableassignment', {
                    url: '/assetvariableassignment',
                    templateUrl: '/App/Main/views/assethealth/assetvariableassignment.cshtml',
                    menu: 'AssetVariableAssignment'
                })
                .state('assetvariablelist', {
                    url: '/assetvariablelist',
                    templateUrl: '/App/Main/views/assethealth/assetvariablelist.cshtml',
                    menu: 'AssetVariableList'
                })
                .state('imagelist', {
                    url: '/imagelist',
                    templateUrl: '/App/Main/views/images/imagelist.cshtml',
                    menu: 'ImageList' //Matches to name of menu in AssetManagerNavigationProvider
                })
                .state('imagelistedit', {
                    url: '/imagelistedit',
                    templateUrl: '/App/Main/views/images/imagelistedit.cshtml',
                    menu: 'ImageListEdit' //Matches to name of menu in AssetManagerNavigationProvider
                })
                .state('imageview', {
                    url: '/imageview?Id',
                    templateUrl: '/App/Main/views/images/imageview.cshtml',
                    menu: 'ImageView'
                })
                 .state('iowdashboard', {
                     url: '/iowdashboard',
                     templateUrl: '/App/Main/views/iow/dashboard.cshtml',
                     menu: 'IOWDashboard' //Matches to name of 'IOW' menu in AssetManagerNavigationProvider
                 })
                 .state('iowmain', {
                     url: '/iowmain',
                     templateUrl: '/App/Main/views/iow/main.cshtml',
                     menu: 'IOWMain' //Matches to name of 'IOW' menu in AssetManagerNavigationProvider
                })
                .state('iowleveledit', {
                    url: '/iowleveledit?levelId',
                    templateUrl: '/App/Main/views/iow/leveledit.cshtml',
                    menu: 'IOWLevelEdit'
                })
                .state('iowlevellist', {
                     url: '/iowlevellist',
                     templateUrl: '/App/Main/views/iow/LevelList.cshtml',
                     menu: 'IOWLevelList' //Matches to name of 'IOWLevels' menu in AssetManagerNavigationProvider
                })
                .state('iowvariablechart', {
                    url: '/iowvariablechart?Id',
                    templateUrl: '/App/Main/views/iow/variablechart.cshtml',
                    menu: 'IOWVariableChart' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariableedit', {
                    url: '/iowvariableedit?Id',
                    templateUrl: '/App/Main/views/iow/variableedit.cshtml',
                    menu: 'IOWVariableEdit' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariablelist', {
                    url: '/iowvariablelist',
                    templateUrl: '/App/Main/views/iow/variablelist.cshtml',
                    menu: 'IOWVariables' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariabledeviations', {
                    url: '/iowvariabledeviations?Id',
                    templateUrl: '/App/Main/views/iow/variabledeviations.cshtml',
                    menu: 'IOWVariableDeviations' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowlimitsummary', {
                    url: '/iowlimitsummary',
                    templateUrl: '/App/Main/views/iow/limitsummary.cshtml',
                    menu: 'IOWLimitSummary' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowlimitstatus', {
                    url: '/iowlimitstatus',
                    templateUrl: '/App/Main/views/iow/limitstatus.cshtml',
                    menu: 'IOWLimitStatus' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariableview', {
                    url: '/iowvariableview?Id',
                    templateUrl: '/App/Main/views/iow/variableview.cshtml',
                    menu: 'IOWVariableView' //Matches to name in AssetManagerNavigationProvider
                })
                .state('tagdata', {
                    url: '/tagdata?tagId',
                    templateUrl: '/App/Main/views/tag/tagdata.cshtml',
                    menu: 'TagData'
                })
                .state('tagedit', {
                    url: '/tagedit?tagId',
                    templateUrl: '/App/Main/views/tag/tagedit.cshtml',
                    menu: 'TagEdit'
                })
                .state('taglist', {
                     url: '/taglist',
                     templateUrl: '/App/Main/views/tag/taglist.cshtml',
                     menu: 'TagList' //Matches name in AssetManagerNavigationProvider
                })
                .state('tagview', {
                    url: '/tagview?tagId',
                    templateUrl: '/App/Main/views/tag/tagview.cshtml',
                    menu: 'TagView'
                })
                .state('userlist', {
                     url: '/userlist',
                     templateUrl: '/App/Main/views/admin/userlist.cshtml',
                     menu: 'UserList'
                })
                .state('useredit', {
                    url: '/useredit?userId',
                    templateUrl: '/App/Main/views/admin/useredit.cshtml',
                    menu: 'UserEdit' //Matches to name in AssetManagerNavigationProvider
                })
            ;
        }
    ]);
})();