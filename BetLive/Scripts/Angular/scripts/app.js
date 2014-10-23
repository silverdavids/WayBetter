'use strict';

/**
 * @ngdoc overview
 * @name bettingApp
 * @description
 * # bettingApp
 *
 * Main module of the application.
 */
var bettingApp=angular.module('bettingApp', ['ngAnimate','ngCookies','ngResource','ngRoute','ngSanitize','ngTouch',
        'LocalStorageModule', 'angular-loading-bar', 'betaDataModule','ui.bootstrap'
  ]);
bettingApp.config(function ($routeProvider) {
    $routeProvider
      .when('/', {
          templateUrl: '/Management/matches',
        controller: 'newMatchCtrl'
      })
      .when('/about', {
        templateUrl: '/Management/about',
        controller: 'AboutCtrl'
      })
        .when('/home', {
            templateUrl: '/Management/home',
            controller: 'homeCtrl'
        })
        .when('/login',{
            templateUrl:'login.html',
            controller:'loginCtrl'
        })
        .when('/signup',{
            templateUrl:'signup.html',
            controller:'signUpCtrl'
        })
        .when('/matches',{
            templateUrl: '/Management/matches',
            controller:'newMatchCtrl'
        })
        .when('/company',{
            templateUrl:'/Management/company',
            controller:'CompanyCtrl'
        })
        .when('/company/create',{
            templateUrl:'/Management/createcompany',
            controller:'CompanyCtrl'
        })
        .when('/company/branches/:companyId',{
            templateUrl:'/Management/branchlist',
            controller:'BranchCtrl'
        })
        .when('/branch/terminals/:branchId',{
            templateUrl:'/Management/terminallist',
            controller:'TerminalCtrl'
        })
        .when('/terminal/shifts/:branchId',{
            templateUrl:'/Management/shiftlist',
            controller:'ShiftCtrl'
        })
        .when('/terminal/shifts/:branchId/:dateFrom/:dateTo',{
            templateUrl:'/Management/shiftlist',
            controller:'ShiftCtrl'
        })
      .otherwise({
        redirectTo: '/'
      });
  });
bettingApp.run(['authService',function(authService){
    authService.fillAuthData();
}]);
$betaData.settings.baseUrl = 'http://localhost:54482/api/';
bettingApp.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

