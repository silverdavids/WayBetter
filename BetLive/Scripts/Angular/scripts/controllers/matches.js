'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:MatchesCtrl
 * @description
 * # MatchesCtrl
 * Controller of the bettingApp
 */

bettingApp.controller('matchesCtrl', ['$scope', 'dataService', function ($scope, dataService) {
    $scope.catFT1X2 = null;
    $scope.catFUO = null;
    $scope.catHT1X2 = null;
    $scope.catUO = null;
    $scope.catDoubleChance = null;
    $scope.cathandicap = null;
    $scope.catBothTeamsToScore = null;
    $scope.catFirstTeamToScore = null;
    $scope.catDrawNoBet = null;
    $scope.matches = null;
    $scope.matchDistinct = function () {
     this.MatchNo=null;
        this.SetNo=null;
        this.Champ=null;
        this.OldDateTime=null;
        this.StartTime=null;
        this.GameStatus=null;
        this.AwayTeamId=null;
        this.HomeTeamId=null;
        this.RegistrationDate=null;
        this.HomeScore=null;
        this.AwayScore=null;
        this.HalfTimeHomeScore=null;
        this.HalfTimeAwayScore=null;
        this.ResultStatus=null;
        this.AwayTeamName=null;
        this.HomeTeamName = null;}
    $scope.matchRow = function () {
        this.matchDistinct = new $scope.matchDistinct();
        this.GameOdds = new Array();
    }
    $scope.matchRows = null;
    $scope.betCategories = null;
    $scope.betCategoryRow = function () {
        this.CategoryName = null;
        this.matchOdds = null;
        this.match = null;
    };
    $scope.betCategoryRows = new Array();
   
    dataService.get("Match/getBetCategories").then(function (results) {
        var betCategories = betCategories || {};
        $scope.betCategories = new Array();
        angular.forEach(results, function (data, i) {
            $scope.betCategories.push(data);

        })

       // $scope.betCategories = betCategories;
        //matches.match=new Array();
        // matches.matches.push()

    }, function (err) {
        $scope.message = err.error_description;
    });
    dataService.get("Match/GetMatches").then(function (results) {
        //var betMatchCategoriesMatches = betMatchCategoriesMatches || {};
       // betMatchCategoriesMatches.Categories = null;
        $scope.matches = new Array();
        angular.forEach(results, function (data, i) {
            var mRow = new $scope.matchRow();
            mRow.matchDistinct.MatchNo = data.MatchNo;
            mRow.matchDistinct.SetNo = data.SetNo;
            mRow.matchDistinct.Champ = data.Champ;
            mRow.matchDistinct.OldDateTime = data.OldDateTime;
            mRow.matchDistinct.StartTime = data.StartTime;
            mRow.matchDistinct.GameStatus = data.GameStatus;
            mRow.matchDistinct.AwayTeamId = data.AwayTeamId;
            mRow.matchDistinct.HomeTeamId = data.HomeTeamId;
            mRow.matchDistinct.RegistrationDate = data.RegistrationDate;
            mRow.matchDistinct.HomeScore = data.HomeScore;
            mRow.matchDistinct.AwayScore = data.AwayScore;
            mRow.matchDistinct.HalfTimeHomeScore = data.HalfTimeHomeScore;
            mRow.matchDistinct.HalfTimeAwayScore = data.HalfTimeAwayScore;
            mRow.matchDistinct.ResultStatus = data.ResultStatus;
            mRow.matchDistinct.AwayTeamName = data.AwayTeamName;
            mRow.matchDistinct.HomeTeamName = data.HomeTeamName;
            mRow.GameOdds = data.GameOdds;
            $scope.matches.push(mRow);
               
           // }

        });
        angular.forEach($scope.betCategories, function (betCat, i) {
            //angular.alert(betCat.Name);
            var betCatRow = new $scope.betCategoryRow();
            betCatRow.CategoryName = betCat.Name;
            angular.forEach($scope.matches, function (match, j) {
                angular.forEach(match.GameOdds, function (GameOdd, k) {

                    if (GameOdd.BetCategory === betCat.Name) {
                        betCatRow.match=match.matchDistinct;
                        betCatRow.matchOdds=GameOdd;
                    }
                })

            });

            $scope.betCategoryRows.push(betCatRow);
        });

    }, function (err) {
        $scope.message = err.error_description;
    });

   

  }]);




function getOddOptionInBetCategory(gameOdds, matchno, requiredOption) {
    var gameodd;
    $.each(gameOdds, function (key, value) {
        if (value.MatchNo == matchno && value.BetOptionId == requiredOption) {
            gameodd = {
                BetCategory: value.BetCategory,
                BetOptionId: value.BetOptionId,
                BetOption: value.BetOption,
                Odd: (typeof value.Odd === 'undefined') ? 0 : value.Odd,
                MatchNo: value.MatchNo,
                HandicapGoals: (typeof value.HandicapGoals === 'undefined') ? 0 : value.HandicapGoals
            };
        }
    });
    return gameodd;
}


function getGameOddsByCategory(category) {
    var gameOdds = [];
    $.each(games, function (index, item) {
        $.each(item.GameOdds, function (key, value) {
            if (value.BetCategory === category) {
                var gameodd =
                {
                    BetCategory: value.BetCategory,
                    BetOptionId: value.BetOptionId,
                    BetOption: value.BetOption,
                    LastUpdateTime: value.LastUpdateTime,
                    Odd: (typeof value.Odd === 'undefined') ? 0 : value.Odd,
                    MatchNo: item.MatchNo,
                    HandicapGoals: (typeof value.HandicapGoals === 'undefined') ? 0 : value.HandicapGoals
                };
                gameOdds.push(gameodd);
            }
        });
    });
    return gameOdds;
}

