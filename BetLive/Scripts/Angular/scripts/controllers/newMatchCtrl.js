bettingApp.controller('newMatchCtrl', ['$scope', 'dataService', function ($scope, dataService) {



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


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Matches
    $scope.matches = null;
    $scope.matchDistinct = function () {
        this.MatchNo = null;
        this.SetNo = null;
        this.Champ = null;
        this.OldDateTime = null;
        this.StartTime = null;
        this.GameStatus = null;
        this.AwayTeamId = null;
        this.HomeTeamId = null;
        this.RegistrationDate = null;
        this.HomeScore = null;
        this.AwayScore = null;
        this.HalfTimeHomeScore = null;
        this.HalfTimeAwayScore = null;
        this.ResultStatus = null;
        this.AwayTeamName = null;
        this.HomeTeamName = null;
    }
       
    $scope.gameOdd = function (value,item) {
        this.BetCategory= value.BetCategory,
        this.BetOptionId= value.BetOptionId,
        this.BetOption= value.BetOption,
        this.Odd= (typeof value.Odd === 'undefined') ? 0 : value.Odd,
        this.MatchNo= value.MatchNo,
        this.HandicapGoals= (typeof value.HandicapGoals === 'undefined') ? 0 : value.HandicapGoals
    };
        $scope.gameWithOddForEachCatRow = function () {
            this.matchDistinct = new $scope.matchDistinct();
            this.homeOdd = $scope.gameOdd();
            this.drawOdd = new $scope.gameOdd();
            this.awayOdd = $scope.gameOdd();
        }
        $scope.gameWithOddForEachCatRows = null;
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





    dataService.get("Match/GetMatches").then(function (results) {
        $scope.oddCategoryRow = function () {
            this.match = new $scope.matchDistinct();
            this.oddHome = null;
            this.oddDraw = null;
            this.oddAway = null;
        };
        $scope.oddCategoryRows = new Array();
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
            if(data.GameOdds.length!==0){
            $scope.matches.push(mRow);
        }

        });
        $scope.ft1X2Odds = getGameOddsByCategory("FT 1x2");

        angular.forEach($scope.matches, function (item, key) {
            var oddCategoryRow = new $scope.oddCategoryRow()
            oddCategoryRow.match = item.matchDistinct;
            oddCategoryRow.oddHome = getOddOptionInBetCategory($scope.ft1X2Odds, item.matchDistinct.MatchNo, 1);
            oddCategoryRow.oddDraw = getOddOptionInBetCategory($scope.ft1X2Odds, item.matchDistinct.MatchNo, 2);
            oddCategoryRow.oddAway = getOddOptionInBetCategory($scope.ft1X2Odds, item.matchDistinct.MatchNo, 3);
            $scope.oddCategoryRows.push(oddCategoryRow);
        });
       

    }, function (err) {
        $scope.message = err.error_description;
    });

   
    //angular.forEach(getGameOddsByCategory("FT 1x2"), function (item, key) {
    //    $scope.ft1X2Odds.push(item);

    //});

    function getGameOddsByCategory(category) {
        var gameOdds = [];
        angular.forEach($scope.matches, function (item,index) {
            angular.forEach(item.GameOdds, function (value,key) {
                if (value.BetCategory === category) {
                    var gameodd =
                    {
                        BetCategory: value.BetCategory,
                        BetOptionId: value.BetOptionId,
                        BetOption: value.BetOption,
                        LastUpdateTime: value.LastUpdateTime,
                        Odd: (typeof value.Odd === 'undefined') ? 0 : value.Odd,
                        MatchNo: item.matchDistinct.MatchNo,
                        HandicapGoals: (typeof value.HandicapGoals === 'undefined') ? 0 : value.HandicapGoals
                    };
                    gameOdds.push(gameodd);
                }
            });
        });
        return gameOdds;
    }
    function getOddOptionInBetCategory(gameOdds, matchno, requiredOption) {
        var gameodd;
        angular.forEach(gameOdds, function (value,key) {
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
}]);