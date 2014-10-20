'use strict';

bettingApp.factory('liveBetsSrvc', [ '$rootScope', function ( $rootScope) {
    //var ticker = $.connection.liveBetHub;
    // this works as our proxy
   // var ticker = null;
    function init() {
       var  connection = $.hubConnection(),
         ticker = connection.createHubProxy('liveBetHubAng');
        //var that = ticker;
        //listen to game updates from the server
       ticker.on('updateGame', function (game) {
            $rootScope.$emit('updateGame', game);
            //console.log(game);
        });
        //listen to games response updates from the server
        ticker.on('GetAllGames', function (games) {
            $rootScope.$emit('getAllGames', games);
        });
        //start the connection and call for all games
        connection.start().done(function () {
            ticker.invoke('TestString').done(function (string) {
                console.log(string);
            });

            // get all livescores
            ticker.invoke('GetAllGames').done(function (games) {
                //console.log(games);
                $rootScope.$emit('getAllGames', games);
            });
        });
    };
    return {

        init: init
    }
}]);