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
        'LocalStorageModule', 'angular-loading-bar', 'betaDataModule','ui.bootstrap','ngDialog'
  ]);
bettingApp.config(function ($routeProvider) {
    $routeProvider
      .when('/', {
          templateUrl: '/MyMatches/matches',
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
            templateUrl: '/MyMatches/matches',
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
//$betaData.settings.baseUrl = 'http://localhost:54482/api/';
//$betaData.settings.baseUrl = 'http://localhost/testlive.betway.ug/api/';
bettingApp.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!CAUTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//The folowing lines of code settings are supposed to be in the data sevice file 
// but is renderered later than this filein which their initialization occurs
//so they are not binded so i decided to put them here as a fix
//_________________________________________________________________________________
var $betaData = $betaData || {};
$betaData.settings = {};
$betaData.settings.baseUrl = "/";
$betaData.settings.errorLine = "<br />";
//__________________________________________________________________________________
var $authServiceData = $authServiceData || {};
$authServiceData.settings = {};
$authServiceData.settings.baseUrl = "/";
//__________________________GLOBALS DECLARATION______________________________________
var $liveBetsSrvcData = $liveBetsSrvcData || {};
$liveBetsSrvcData.settings = {};
$liveBetsSrvcData.settings.baseUrl = "/";
$liveBetsSrvcData.settings.Name = null;
//___________________________DEVELOPMENT SERVER____________________________________

//Initialize the base urls for all services
$betaData.settings.baseUrl = 'http://localhost:54482/api/';
$receiptSenderData.settings.baseUrl = 'http://localhost:49193/api/';
$authServiceData.settings.baseUrl = 'http://localhost:54482/';
$liveBetsSrvcData.settings.baseUrl = 'http://localhost:54482';
$liveBetsSrvcData.settings.Name = 'liveBetHubAng';
//___________________________STAGING SERVER____________________________________

////Initialize the base urls for all services
//$betaData.settings.baseUrl = 'http://localhost/betlive/api/';
//$receiptSenderData.settings.baseUrl = 'http://localhost/betlive/api/';
//$authServiceData.settings.baseUrl = 'http://localhost/betlive/';
//$liveBetsSrvcData.settings.baseUrl = 'http://localhost/betlive';
//$liveBetsSrvcData.settings.Name = 'liveBetHubAng';

////________________________DEPLOYMENT__SERVER_______________________________________
//$betaData.settings.baseUrl = 'http://testlive.betway.ug/api/';
//$receiptSenderData.settings.baseUrl = 'http://testlive.betway.ug/api/';
//$authServiceData.settings.baseUrl = 'http://testlive.betway.ug/';
//$liveBetsSrvcData.settings.baseUrl = 'http://testlive.betway.ug';
//$liveBetsSrvcData.settings.Name = 'liveBetHubAng';

