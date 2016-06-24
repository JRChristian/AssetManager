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
    //title: extension used to set page title. See http://conceptf1.blogspot.com/2014/11/angularjs-dynamic-page-title.html and http://stackoverflow.com/questions/12506329/how-to-dynamically-change-header-based-on-angularjs-partial-view/12506795#12506795
    app.config([
        '$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/');
            $stateProvider
                .state('home', {
                    url: '/',
                    templateUrl: '/App/Main/views/home/home.cshtml',
                    title: 'Home',
                    menu: 'Home' //Matches to name of 'Home' menu in AssetManagerNavigationProvider
                })
                .state('about', {
                    url: '/about',
                    templateUrl: '/App/Main/views/about/about.cshtml',
                    title: 'About',
                    menu: 'About' //Matches to name of 'About' menu in AssetManagerNavigationProvider
                })
                .state('assetlist', {
                    url: '/assetlist',
                    templateUrl: '/App/Main/views/asset/assetlist.cshtml',
                    title: 'Asset List',
                    menu: 'AssetList' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetlistold', {
                    url: '/assetlistold',
                    templateUrl: '/App/Main/views/asset/list.cshtml',
                    title: 'Asset List',
                    menu: 'AssetListOld' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetedit', {
                    url: '/assetedit?assetId',
                    templateUrl: '/App/Main/views/asset/edit.cshtml',
                    title: 'Asset Edit',
                    menu: 'AssetEdit' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assethierarchy', {
                    url: '/assethierarchy',
                    templateUrl: '/App/Main/views/asset/assethierarchy.cshtml',
                    title: 'Asset Hierarchy',
                    menu: 'AssetHierarchy' //Matches to menu name in AssetManagerNavigationProvider
                })
                .state('assetlevelbarchart', {
                    url: '/assetlevelbarchart',
                    templateUrl: '/App/Main/views/assethealth/assetlevelbarchart.cshtml',
                    title: 'Asset Level Bar Chart',
                    menu: 'AssetLevelBarChart' //Matches to menu name in AssetManagerNavigationProvider
                })
                .state('assetlimitview', {
                    url: '/assetlimitview?assetId',
                    templateUrl: '/App/Main/views/assethealth/assetlimitview.cshtml',
                    title: 'Asset Limit View',
                    menu: 'AssetLimitView' //Matches to menu name in AssetManagerNavigationProvider
                })
                .state('assetnew', {
                    url: '/assetnew',
                    templateUrl: '/App/Main/views/asset/new.cshtml',
                    title: 'New Asset',
                    menu: 'AssetNew' //Matches to name of 'NewAsset' menu in AssetManagerNavigationProvider
                })
                .state('assettypeedit', {
                    url: '/assettypeedit',
                    templateUrl: '/App/Main/views/asset/assettypeedit.cshtml',
                    title: 'Edit Asset Type',
                    menu: 'AssetTypeEdit' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetvariableassignment', {
                    url: '/assetvariableassignment',
                    templateUrl: '/App/Main/views/assethealth/assetvariableassignment.cshtml',
                    title: 'Asset-Variable Assignment',
                    menu: 'AssetVariableAssignment'
                })
                .state('assetvariablelist', {
                    url: '/assetvariablelist',
                    templateUrl: '/App/Main/views/assethealth/assetvariablelist.cshtml',
                    title: 'Asset-Variable Cross-Reference',
                    menu: 'AssetVariableList'
                })
                .state('healthmetriclist', {
                    url: '/healthmetriclist',
                    templateUrl: '/App/Main/views/assethealth/healthmetriclist.cshtml',
                    title: 'Health Metrics',
                    menu: 'healthmetriclist' //Matches to name of menu in AssetManagerNavigationProvider
                })
                .state('assethealthdashboard', {
                    url: '/assethealthdashboard',
                    templateUrl: '/App/Main/views/assethealth/assethealthdashboard.cshtml',
                    title: 'Asset Health Dashboard',
                    menu: 'AssetHealthDashboard'
                })
                .state('imagelist', {
                    url: '/imagelist',
                    templateUrl: '/App/Main/views/images/imagelist.cshtml',
                    title: 'Image List',
                    menu: 'ImageList' //Matches to name of menu in AssetManagerNavigationProvider
                })
                .state('imagelistedit', {
                    url: '/imagelistedit',
                    templateUrl: '/App/Main/views/images/imagelistedit.cshtml',
                    title: 'Edit Image List',
                    menu: 'ImageListEdit' //Matches to name of menu in AssetManagerNavigationProvider
                })
                .state('imageview', {
                    url: '/imageview?Id',
                    templateUrl: '/App/Main/views/images/imageview.cshtml',
                    title: 'Image View',
                    menu: 'ImageView'
                })
                 .state('iowdashboard', {
                     url: '/iowdashboard',
                     templateUrl: '/App/Main/views/iow/dashboard.cshtml',
                     title: 'IOW Dashboard',
                     menu: 'IOWDashboard' //Matches to name of 'IOW' menu in AssetManagerNavigationProvider
                 })
                 .state('iowmain', {
                     url: '/iowmain',
                     templateUrl: '/App/Main/views/iow/main.cshtml',
                     title: 'IOW Main and Playground',
                     menu: 'IOWMain' //Matches to name of 'IOW' menu in AssetManagerNavigationProvider
                })
                .state('iowleveledit', {
                    url: '/iowleveledit?levelId',
                    templateUrl: '/App/Main/views/iow/leveledit.cshtml',
                    title: 'Edit Level Definitions',
                    menu: 'IOWLevelEdit'
                })
                .state('iowlevellist', {
                     url: '/iowlevellist',
                     templateUrl: '/App/Main/views/iow/LevelList.cshtml',
                     title: 'Level List',
                     menu: 'IOWLevelList' //Matches to name of 'IOWLevels' menu in AssetManagerNavigationProvider
                })
                .state('iowvariablechart', {
                    url: '/iowvariablechart?Id',
                    templateUrl: '/App/Main/views/iow/variablechart.cshtml',
                    title: 'Variable Chart',
                    menu: 'IOWVariableChart' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariableedit', {
                    url: '/iowvariableedit?Id',
                    templateUrl: '/App/Main/views/iow/variableedit.cshtml',
                    title: 'Edit Variable',
                    menu: 'IOWVariableEdit' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariablelist', {
                    url: '/iowvariablelist',
                    templateUrl: '/App/Main/views/iow/variablelist.cshtml',
                    title: 'Variable List',
                    menu: 'IOWVariables' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariabledeviations', {
                    url: '/iowvariabledeviations?Id',
                    templateUrl: '/App/Main/views/iow/variabledeviations.cshtml',
                    title: 'Variable Deviations',
                    menu: 'IOWVariableDeviations' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowlimitsummary', {
                    url: '/iowlimitsummary',
                    templateUrl: '/App/Main/views/iow/limitsummary.cshtml',
                    title: 'Limit Summary',
                    menu: 'IOWLimitSummary' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowlimitstatus', {
                    url: '/iowlimitstatus',
                    templateUrl: '/App/Main/views/iow/limitstatus.cshtml',
                    title: 'Limit Status',
                    menu: 'IOWLimitStatus' //Matches to name in AssetManagerNavigationProvider
                })
                .state('iowvariableview', {
                    url: '/iowvariableview?Id',
                    templateUrl: '/App/Main/views/iow/variableview.cshtml',
                    title: 'Variable View',
                    menu: 'IOWVariableView' //Matches to name in AssetManagerNavigationProvider
                })
                .state('tagdata', {
                    url: '/tagdata?tagId',
                    templateUrl: '/App/Main/views/tag/tagdata.cshtml',
                    title: 'Tag Data',
                    menu: 'TagData'
                })
                .state('tagedit', {
                    url: '/tagedit?tagId',
                    templateUrl: '/App/Main/views/tag/tagedit.cshtml',
                    title: 'Edit Tag',
                    menu: 'TagEdit'
                })
                .state('taglist', {
                     url: '/taglist',
                     templateUrl: '/App/Main/views/tag/taglist.cshtml',
                     title: 'Tag List',
                     menu: 'TagList' //Matches name in AssetManagerNavigationProvider
                })
                .state('tagview', {
                    url: '/tagview?tagId',
                    templateUrl: '/App/Main/views/tag/tagview.cshtml',
                    title: 'Tag View',
                    menu: 'TagView'
                })
                .state('userlist', {
                     url: '/userlist',
                     templateUrl: '/App/Main/views/admin/userlist.cshtml',
                     title: 'User List',
                     menu: 'UserList'
                })
                .state('useredit', {
                    url: '/useredit?userId',
                    templateUrl: '/App/Main/views/admin/useredit.cshtml',
                    title: 'Edit User',
                    menu: 'UserEdit' //Matches to name in AssetManagerNavigationProvider
                })
            ;
        }
    ]);

    // Set page title.
    // See http://conceptf1.blogspot.com/2014/11/angularjs-dynamic-page-title.html and http://stackoverflow.com/questions/12506329/how-to-dynamically-change-header-based-on-angularjs-partial-view/12506795#12506795
    app.run(function ($rootScope, $state) {
        $rootScope.$on('$stateChangeSuccess', function () {
            $rootScope.title = $state.current.title;
        });
    });
})();