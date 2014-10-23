'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:AboutCtrl
 * @description
 * # AboutCtrl
 * Controller of the bettingApp
 */
angular.module('bettingApp')
  .controller('AboutCtrl', function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
