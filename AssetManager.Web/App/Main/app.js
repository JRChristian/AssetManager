﻿(function () {
    'use strict';

    var app = angular.module('app', [
        'ngAnimate',
        'ngSanitize',

        'ui.router',
        'ui.bootstrap',
        'ui.jq',

        'abp'
    ]);

    //Configuration for Angular UI routing.
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
                });
        }
    ]);
})();