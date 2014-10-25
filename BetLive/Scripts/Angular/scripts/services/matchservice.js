'use strict';

/**
 * @ngdoc service
 * @name bettingApp.matchService
 * @description
 * # matchService
 * Service in the bettingApp.
 */



bettingApp.factory('matchService', ['$http', function ($http) {

    var serviceBase = 'http://localhost:1035/';
    var matchesServiceFactory = {};

    var _getMatches = function () {

        return $http.get(serviceBase + 'api/Orders').then(function (results) {
            return results;
        });
    };

    var _getItems = function (url) {

        return $http.get(serviceBase + 'api/'+url).then(function (results) {
            return results;
        });
    };

    var _getItem = function (url,id) {

        return $http.get(serviceBase + 'api/'+url+"/"+id).then(function (results) {
            return results;
        });
    };

    matchesServiceFactory.getMatches = _getMatches;
    matchesServiceFactory.getItems = _getItems;
    matchesServiceFactory.getItem = _getItem;

    return matchesServiceFactory;

}]);