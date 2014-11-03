'use strict';
//var $liveBetsSrvcData = $liveBetsSrvcData || {};
//$liveBetsSrvcData.settings = {};
//$liveBetsSrvcData.settings.baseUrl = "/";
//$liveBetsSrvcData.settings.Name = null;

bettingApp.factory('liveBetsSrvc', [ '$rootScope', function ( $rootScope) {
    //var ticker = $.connection.liveBetHub;
    // this works as our proxy
    var ticker = null,
        connection = null, connIsStarted = false,
        hubName = $liveBetsSrvcData.settings.Name,
        hubBaseUrl = $liveBetsSrvcData.settings.baseUrl;

    function init() {
        console.log("Connecting to the hub");
        connection = $.hubConnection(hubBaseUrl);
         ticker = connection.createHubProxy(hubName);
        //var that = ticker;
        //listen to game updates from the server
       ticker.on('updateGame', function (game) {
            $rootScope.$emit('updateGame', game);
            //console.log(game);
        });
        //listen to games response updates from the server
        ticker.on('getAllGames', function (games) {
            $rootScope.$emit('getAllGames', games);
        });
        //start the connection and call for all games
        connection.start().done(function () {
            ticker.invoke('TestString').done(function (string) {
                connIsStarted = true;
                console.log(string);
            });
            ticker.invoke('getAllNormalGames').done(function (games) {
                //console.log(games);
                $rootScope.$emit('getAllNormalGames', games);
            });           

          
        });
    };
    function getLiveGames(){
        // get all livescores
        ticker.invoke('getAllGames').done(function (games) {
            console.log(games);
            $rootScope.$emit('getAllGames', games);
        });
    }

    function reconnect() {
        connection = connection || $.hubConnection();      
        ticker = ticker || connection.createHubProxy('liveBetHubAng');
        if (connIsStarted!==true) {

            connection.start().done(function () {
                ticker.invoke('getAllGames').done(function (games) {
                    console.log("Reconnecting to the service");
                    $rootScope.$emit('getAllGames', games);
                }, function (message) {
                    console.log("failed to connect to the service");

                });
            });
           
        } else {
            ticker.invoke('getAllGames').done(function (games) {
                console.log("Reconnecting to the service");
                $rootScope.$emit('getAllGames', games);
            }, function (message) {
                console.log("failed to connect to the service");

            });
        }
        
    }
    return {

        init: init,
        reconnect: reconnect,
        getLiveGames:getLiveGames
    }
}]);