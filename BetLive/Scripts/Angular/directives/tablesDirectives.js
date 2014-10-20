'use strict';
bettingApp.directive('fullTime', function () {
    return   {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'fullTime.html',
        link:function(scope,elem,attr){
        
        
        }
    
    };

});
bettingApp.directive('fullTimeUo', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'fullTimeUo.html',
        link: function (scope, elem, attr) {
        }

    };

});
bettingApp.directive('halfTime', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'halfTime.html',
        link: function (scope, elem, attr) {
        }

    };

});
bettingApp.directive('halfTimeUo', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'halfTimeUo.html',
        link: function (scope, elem, attr) {
        }

    };

});
bettingApp.directive('doubleChance', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'doubleChance.html',
        link: function (scope, elem, attr) {
        }

    };

});

bettingApp.directive('handicap', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'handicap.html',
        link: function (scope, elem, attr) {
        }

    };

});

bettingApp.directive('bothTeamsToScore', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'bothTeamsToScore.html',
        link: function (scope, elem, attr) {
        }

    };

});
bettingApp.directive('liveGames', function () {
    return {
        restrict: 'E',
        replace: 'true',
        templateUrl: 'liveGames.html',
        
        link: function (scope, elem, attr) {
        }

    };

});