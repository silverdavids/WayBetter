'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:IndexCtrl
 * @description
 * # IndexCtrl
 * Controller of the bettingApp
 */

bettingApp.controller("indexCtrl", ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.logOut = function() {
        authService.logOut();
        $location.path('/home');
    };

    $scope.authentication = authService.authentication;

}]);