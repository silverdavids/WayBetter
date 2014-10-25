'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:MatchesCtrl
 * @description
 * # MatchCtrl
 * Controller of the bettingApp
 */

bettingApp.controller('matchCtrl', ['$scope', 'dataService', function ($scope, dataService) {

    dataService.get("Match/getMatches").then(function (results) {
        var matches = matches || {};
            
        $scope.matches = results;
        //matches.match=new Array();
        // matches.matches.push()

    }, function (err) {
        $scope.message = err.error_description;
    });

}]);
