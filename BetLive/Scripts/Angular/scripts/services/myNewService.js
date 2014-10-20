'use strict';

/**
 * @ngdoc service
 * @name bettingApp.matchService
 * @description
 * # matchService
 * Service in the bettingApp.
 */



bettingApp.factory('myNewService', [ '$rootScope',  function ($rootScope, signalRServer) {
        function signalRHubProxyFactory(serverUrl, hubName, startOptions) {
            var connection = $.hubConnection(signalRServer);
            var proxy = connection.createHubProxy(hubName);
            connection.start(startOptions).done(function () { });
            
            return {
                on: function (eventName, callback) {
                    proxy.on(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                off: function (eventName, callback) {
                    proxy.off(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                invoke: function (methodName, callback) {
                    proxy.invoke(methodName)
                        .done(function (result) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                },
                connection: connection
            };
        };

        return signalRHubProxyFactory;    
}]);





function ServerTimeController($scope, signalRHubProxy) {
    var clientPushHubProxy = signalRHubProxy(
        signalRHubProxy.defaultServer, 'clientPushHub',
            { logging: true });
    var serverTimeHubProxy = signalRHubProxy(
        signalRHubProxy.defaultServer, 'serverTimeHub');

    clientPushHubProxy.on('serverTime', function (data) {
        $scope.currentServerTime = data;
        var x = clientPushHubProxy.connection.id;
    });

    $scope.getServerTime = function () {
        serverTimeHubProxy.invoke('getServerTime', function (data) {
            $scope.currentServerTimeManually = data;
        });
    };
};