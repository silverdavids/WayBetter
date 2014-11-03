'use strict';

/**
 * @ngdoc service
 * @name bettingApp.DataService
 * @description
 * # DataService
 * Service in the bettingApp.
 */




//var $betaData = $betaData || {};
//$betaData.settings = {};
//$betaData.settings.baseUrl = "/";
//$betaData.settings.errorLine = "<br />";

$betaData.module = angular.module('betaDataModule', []);
$betaData.module.factory('dataService', ['$http', '$q', function($http, $q) {
    var service = { onBeforeAjax: null, onAfterAjax: null };
    service.$http = $http;

    service.url = function(link) {
        return $betaData.settings.baseUrl + link;
    };

    service.navigate = function(link) {
        window.location.replace(service.url(link));
    };

    service.getReff = function (id, version) {
        if (!service.isEmpty(id)) {
            return { id: id, version: version };
        }
    };

    service.isEmpty = function (val) {
        return undefined === val || null == val || "" === val;
    };

    service.getDirectPromise = function(data) {
        var deferred = $q.defer();
        deferred.resolve(data);
        return deferred.promise;
    };

    service.post = function(method, input, $scope) {
        service.ajaxStart();
        var deferred = $q.defer();
        service.$http({
            method: 'POST',
            url: service.url(method),
            data: input
        })
            .success(function(data, status, headers, config) {
                service.clearErrors($scope);
                deferred.resolve(data);
                service.ajaxEnd();
            })
            .error(function(data, status, headers, config) {
                deferred.reject();
                service.ajaxEnd();
                service.showErrors($scope, data);
            });
        return deferred.promise;
    };

    service.delete = function(method, input, $scope) {
        service.ajaxStart();
        var deferred = $q.defer();
        service.$http({
            method: 'DELETE',
            url: service.url(method),
            data: input
        })
            .success(function(data, status, headers, config) {
                service.clearErrors($scope);
                deferred.resolve(data);
                service.ajaxEnd();
            })
            .error(function(data, status, headers, config) {
                deferred.reject();
                service.ajaxEnd();
                service.showErrors($scope, data);
            });
        return deferred.promise;
    };

    service.get = function(method, $scope) {
        service.ajaxStart();
        var deferred = $q.defer();
        service.$http({
            method: 'GET',
            url: service.url(method)
        })
            .success(function(data, status, headers, config) {
                service.clearErrors($scope);
                deferred.resolve(data);
                service.ajaxEnd();
            })
            .error(function(data, status, headers, config) {
                deferred.reject();
                service.ajaxEnd();
                service.showErrors($scope, data);
            });
        return deferred.promise;
    };

    service.all = function(promises) {
        return $q.all(promises);
    };

    service.ajaxCallCount = 0;

    service.ajaxStart = function() {
        if(0 == service.ajaxCallCount) {
            if(service.onBeforeAjax) service.onBeforeAjax();
        }

        service.ajaxCallCount++;
    };

    service.ajaxEnd = function() {
        service.ajaxCallCount--;
        if (0 == service.ajaxCallCount) {
            setTimeout(function ()  {
                if (0 == service.ajaxCallCount) {
                    if (service.onAfterAjax) service.onAfterAjax();
                }
            }, 200);
        }
    };

    service.showErrors = function($scope, fail) {
        if(!$scope) return;
        var errors = new Array();
        errors.main = fail.main;
        angular.forEach(fail.failures, function (p, i) {
            var current = service.find(errors, function(m) { return m.key == p.key; });
            if(current) {
                current.text = current.text + $betaData.settings.errorLine + p.text;
            }
            else {
                errors.push(p);
            }
        });
        $scope.$$errors = errors;
    };

    service.clearErrors = function($scope) {
        if(!$scope) return;
        $scope.$$errors = new Array();
        $scope.$$errors.main = "";
    };

    service.find = function (obsArray, isItemFunc) {
        var foundItem = null;
        angular.forEach(obsArray, function (item, i) {
            if (true == isItemFunc(item)) {
                foundItem = item;
                return;
            }
        });

        return foundItem;
    };

    return service;
}]);

$betaData.module.directive("mainerror", function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            scope.$watch('$$errors', function (errors, oldVal) {
                if (!errors) return;
                element.text(errors.main);
            });
        }
    };
});

$betaData.module.directive("errorkey", function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            scope.$watch('$$errors', function (errors, oldVal) {
                element.text("");
                if (!errors) return;
                var key = attrs.errorkey;
                if (errors) {
                    for(var i=0; i<errors.length; i++) {
                        if (key == errors[i].key) {
                            element.html(errors[i].text);
                            break;
                        }
                    }
                }
            });
        }
    };
});