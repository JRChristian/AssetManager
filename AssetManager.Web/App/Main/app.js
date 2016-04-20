(function () {
    'use strict';

    var app = angular.module('app', [
        'ngAnimate',
        'ngSanitize',

        'ui.router',
        'ui.bootstrap',
        'ui.jq',
        'ui.grid',  // Angular UI Grid

        'abp'
    ]);

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
                    templateUrl: '/App/Main/views/asset/list.cshtml',
                    menu: 'AssetList' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetedit', {
                    url: '/assetedit?assetId',
                    templateUrl: '/App/Main/views/asset/edit.cshtml',
                    menu: 'AssetEdit' //Matches to name of 'Assets' menu in AssetManagerNavigationProvider
                })
                .state('assetnew', {
                    url: '/assetnew',
                    templateUrl: '/App/Main/views/asset/new.cshtml',
                    menu: 'AssetNew' //Matches to name of 'NewAsset' menu in AssetManagerNavigationProvider
                })
                 .state('iow', {
                     url: '/iow',
                     templateUrl: '/App/Main/views/iow/main.cshtml',
                     menu: 'IOW' //Matches to name of 'IOW' menu in AssetManagerNavigationProvider
                })
                .state('iowlevels', {
                     url: '/iowlevellist',
                     templateUrl: '/App/Main/views/iow/LevelList.cshtml',
                     menu: 'IOWLevels' //Matches to name of 'IOWLevels' menu in AssetManagerNavigationProvider
                })
                .state('tagdata', {
                    url: '/tagdata?tagId',
                    templateUrl: '/App/Main/views/tag/tagdata.cshtml',
                    menu: 'TagData'
                }).state('tagedit', {
                    url: '/tagedit?tagId',
                    templateUrl: '/App/Main/views/tag/tagedit.cshtml',
                    menu: 'TagEdit'
                })
                .state('taglist', {
                     url: '/taglist',
                     templateUrl: '/App/Main/views/tag/taglist.cshtml',
                     menu: 'TagList' //Matches name in AssetManagerNavigationProvider
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