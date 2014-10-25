'use strict';
bettingApp.controller('liveBetsCtrl', ['$scope', '$rootScope', 'liveBet', function ($scope, $rootScope, liveBet) {

    //$scope.gameRow = function () {
    //    this.MatchNo = null;      
    //    this.Minutes=null;
    //    this.LocalTeam = null;
    //    this.AwayTeam = null;
    //    this.LocalTeamScore = null;
    //    this.AwayTeamScore = null;
    //    this.FullTimeOdds = {
    //        AwayWins: "N/A",
    //        Draw: "N/A",
    //        HomeWins: "N/A"
    //    },
    //    this.UnderOverOdds = {
    //        Under: "N/A",
    //        Over: "N/A",
    //        ExtraValue: "N/A"
    //    }
    //}
    //$scope.gameRows =null;
    //$scope.updatedGameRow = null;
    //liveBetsSrvc.init();
    //$rootScope.$on('getAllGames',function(e,games){
    //    $scope.$apply(function(){
    //        loopOverAllGames(games); 
        
    //    });
    //});

    //loopOverAllGames = function (games) {
    //    console.log(games);
    //    $scope.gameRows = new Array();
    //    angular.forEach(games,function(game,i){
    //        var gameRow = new $scope.gameRow();           
    //        gameRow.MatchNo = game.MatchNo
    //        gameRow.Minutes = game.Minutes
    //        gameRow.LocalTeam = game.LocalTeam
    //        gameRow.AwayTeam = game.AwayTeam
    //        gameRow.LocalTeamScore = game.LocalTeamScore
    //        gameRow.AwayTeamScore = game.AwayTeamScore
    //        if (game.FullTimeOdds == null) {
    //            gameRow.FullTimeOdds = gameRow.FullTimeOdds;
    //        }else{
    //            gameRow.FullTimeOdds.AwayWins = game.FullTimeOdds.AwayWins;
    //            gameRow.FullTimeOdds.Draw = game.FullTimeOdds.Draw;
    //            gameRow.FullTimeOdds.HomeWins = game.FullTimeOdds.HomeWins;           
            
    //        }

    //        if (game.UnderOverOdds == null) {
    //            gameRow.UnderOverOdds = gameRow.UnderOverOdds;
    //        }else{
    //            gameRow.UnderOverOdds.Under = game.UnderOverOdds.Under;
    //            gameRow.UnderOverOdds.Over = game.UnderOverOdds.Over;
    //            gameRow.UnderOverOdds.ExtraValue = game.UnderOverOdds.ExtraValue;  
    //        }
    //        $scope.gameRows.push(gameRow);
    //    });
    //}
   
    //$rootScope.$on('updateGame', function (e, game) {
    //    $scope.$apply(function () {
    //        mergeGame(game);


    //    });
    //});

    //mergeGame = function (game) {
    //    var gameRow = new $scope.gameRow();
    //    gameRow.MatchNo = game.MatchNo;
    //    gameRow.Minutes = game.Minutes;
    //    gameRow.LocalTeam = game.LocalTeam;
    //    gameRow.AwayTeam = game.AwayTeam;
    //    gameRow.LocalTeamScore = game.LocalTeamScore;
    //    gameRow.AwayTeamScore = game.AwayTeamScore;
    //    if (game.FullTimeOdds == null) {
    //        gameRow.FullTimeOdds = gameRow.FullTimeOdds;
    //    } else {
           
    //        gameRow.FullTimeOdds.AwayWins = game.FullTimeOdds.AwayWins;
    //        gameRow.FullTimeOdds.Draw = game.FullTimeOdds.Draw;
    //        gameRow.FullTimeOdds.HomeWins = game.FullTimeOdds.HomeWins;

    //    }

    //    if (game.UnderOverOdds == null) {
    //        gameRow.UnderOverOdds = gameRow.UnderOverOdds;
    //    } else {

    //        gameRow.UnderOverOdds.Under = game.UnderOverOdds.Under;
    //        gameRow.UnderOverOdds.Over = game.UnderOverOdds.Over;
    //        gameRow.UnderOverOdds.ExtraValue = game.UnderOverOdds.ExtraValue;
    //    }
    //    $scope.updatedGameRow = gameRow;

    //};

}]);
