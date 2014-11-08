/// <reference path="receipt.js" />

function RenderLivedispalyData() {
    var self = this,      
        rowTemplate = null,
        ticker = $.connection.liveBetHubAng,
        $gameTable = null,
        $gamesTableBody = null;
    var FT1 = null, FTX = null, FT2 = null, under = null, over = null, RM1 = null, RMX = null, RM2 = null, NG1 = null, NGX = null, NG2 = null;
    var $FT1 = null, $FTX = null, $FT2 = null, $under = null, $over = null, $RM1 = null, $RMX = null, $RM2 = null, $NG1 = null, $NGX = null, $NG2 = null;
    var $row = null,
        gameToCheckOnReceipt = new Bet(0);



    self.init = function () {
        ticker.server.testString().done(function (string) {
            console.log(string);
        });

        // get all livescores
        ticker.server.getAllGames().done(function (games) {
            $gameTable = $('#MatchTable'),
            $gamesTableBody = $gameTable.find('#tbody');
            $gamesTableBody.empty();
            console.log(games);
            var count = 0;
            $.each(games, function () {

                var game = this;
                game = self.validateNullGames(game);
                rowTemplate = self.contructRowTemplate(game);
                $gamesTableBody.append(rowTemplate);
                console.log("Orig Row" + "-" + ++count);
            });
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updateGame = function (game) {
        // console.clear();
        // console.log(game);
        $gameTable = $('#MatchTable'),
       $gamesTableBody = $gameTable.find('#tbody');
        var game = self.validateNullGames(game);
        // save the game temporarily to validate it if already on the receipt
       // gameToCompareWithFromServer = game;
        //gameToCheckOnReceipt = _betListReff.getBetByMatchIdAndOptionId($.trim(gameToCompareWithFromServer.MatchNo));
        //console.log(_betListReff.getBets())
        //console.log(gameToCheckOnReceipt);
        self.contructRowTemplate(game);
        self.checkForChangeInOdd(game/*, $gamesTableBody, rowTemplate*/);

    };
    // Start the connection
    $.connection.hub.start().done(self.init);
    //_______________________Suport methods________________________
    self.validateNullGames = function (game) {
        if (game.FullTimeOdds == null) {
            game.FullTimeOdds = {
                AwayWins: "N/A",
                Draw: "N/A",
                HomeWins: "N/A"
            };
        }
        if (game.DoubleChance == null) {
            game.DoubleChance = {
                AwayWinsOrDraw: "N/A",
                HomeWinsOrAwayWins: "N/A",
                HomeWinsOrDraw: "N/A"
            };
        }
        if (game.NextGoal == null) {
            game.NextGoal = {
                AwayScores: "N/A",
                Draw: "N/A",
                HomeScores: "N/A"
            };
        }

        if (game.UnderOverOdds == null) {
            game.UnderOverOdds = {
                Under: "N/A",
                Over: "N/A",
                ExtraValue: "N/A"
            };
        }
        return game;
    };
    self.contructRowTemplate = function (game) {
        rowTemplate = '<tr id="' + game.MatchNo + '">' +
                        '<td class="text-center">' + game.ShortCode + '</td>' +
                        '<td style="color:#F1C40F;">' + game.Minutes + '</td>' +
                        '<td>' + game.LocalTeam + '</td>' +
                        '<td class="text-center" style="color:#ff0000;">' + game.LocalTeamScore + ' - ' + game.AwayTeamScore + '</td>' +
                        '<td>' + game.AwayTeam + '</td>' +
                        '<td class="FT-1 td-border" style="padding-bottom: 0px;">' + game.FullTimeOdds.HomeWins + '</td>' +
                        '<td class="FT-X">' + game.FullTimeOdds.Draw + '</td>' +
                        '<td class="FT-2">' + game.FullTimeOdds.AwayWins + '</td>' +
                        '<td class="under  td-border">' + game.UnderOverOdds.Under + '</td>' +
                        '<td class="text-center">' +
                        '  <span style="color:#F1C40F;">' + game.UnderOverOdds.ExtraValue + '</span>' +
                        '</td>' +
                        '<td class="over">' + game.UnderOverOdds.Over + '</td>' +
                        '<td class="DC-1X  td-border" style="padding-bottom: 0px;">' + game.DoubleChance.HomeWinsOrDraw + '</td>' +
                        '<td class="DC-12">' + game.DoubleChance.HomeWinsOrAwayWins + '</td>' +
                        '<td class="DC-X2">' + game.DoubleChance.AwayWinsOrDraw + '</td>' +
                        
                        '<td class="NG-1  td-border" style="padding-bottom: 0px;">' + game.NextGoal.HomeScores + '</td>' +
                        '<td class="NG-X">' + game.NextGoal.Draw + '</td>' +
                        '<td class="NG-2">' + game.NextGoal.AwayScores + '</td>' +
                        '</tr>';
        //return rowTemplate;
    };
    self.checkForChangeInOdd = function (game /*,$gamesTableBody, rowTemplate*/) {
        var MatchNo = game.MatchNo;
        //lets search the DOM once by using this variable for performance reasons

        $row = $gamesTableBody.find('tr[id=' + MatchNo + ']');
        //possible reuse of these variables initialisations can boost performance for large datasets
        //var FT1 = null, FTX = null, FT2 = null, under = null, over = null, RM1 = null, RMX = null, RM2 = null, NG1 = null, NGX = null, NG2 = null;
        //var $FT1 = null, $FTX = null, $FT2 = null, $under = null, $over = null, $RM1 = null, $RMX = null, $RM2 = null, $NG1 = null, $NGX = null, $NG2 = null;
        $FT1 = $(" td > td.FT-1", $row);
        $FTX = $(" td > td.FT-X", $row);
        $FT2 = $(" td > td.FT-2", $row);
        $under = $(" td > td.under", $row);
        $over = $(" td > td.over", $row);
        $RM1 = $(" td > td.DC-1", $row);
        $RMX = $(" td > td.DC-X", $row);
        $RM2 = $(" td > td.DC-2", $row);
        $NG1 = $(" td > td.NG-1", $row);
        $NGX = $(" td > td.NG-X", $row);
        $NG2 = $(" td > td.NG-2", $row);

        FT1 = $FT1.html();
        FTX = $FTX.html();
        FT2 = $FT2.html();
        // console.log(FT1 + " " + FTX + " " + FT2);
        under = $under.html();
        over = $over.html();
        //console.log(under + " " + over );
        RM1 = $RM1.html();
        RMX = $RMX.html();
        RM2 = $RM2.html();
        // console.log(RM1 + " " + RMX + " " + RM2);
        NG1 = $NG1.html();
        NGX = $NGX.html();
        NG2 = $NG2.html();
        // console.log(NG1 + " " + NGX + " " + NG2);

        // full time        
        self.addTemporaryClassOnChange($FT1, FT1, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($FTX, FTX, game.FullTimeOdds.Draw);
        self.addTemporaryClassOnChange($FT2, FT2, game.FullTimeOdds.HomeWins);
        //over/under
        self.addTemporaryClassOnChange($over, over, game.UnderOverOdds.Over);
        self.addTemporaryClassOnChange($under, under, game.UnderOverOdds.Under);
        //DOUBLE CHANCE
        self.addTemporaryClassOnChange($RM1, RM1, game.DoubleChance.HomeWinsOrDraw);
        self.addTemporaryClassOnChange($RMX, RMX, game.DoubleChance.HomeWinsOrAwayWins);
        self.addTemporaryClassOnChange($RM2, RM2, game.DoubleChance.AwayWinsOrDraw);
        //NO GOAL
        self.addTemporaryClassOnChange($NG1, NG1, game.NextGoal.HomeScores);
        self.addTemporaryClassOnChange($NGX, NGX, game.NextGoal.Draw);
        self.addTemporaryClassOnChange($NG2, NG2, game.NextGoal.AwayScores);

        self.appendOrAddTableRows($row/*, rowTemplate*/);
    };
    self.appendOrAddTableRows = function ($row/*, rowTemplate*/) {
        //only execute  if $row is not  null else length of undefined err 
        if ($row) {
            if ($row.length > 0) {
                $row.replaceWith(rowTemplate);
                // console.log("Replaced Row");
            } else {
                $gamesTableBody.append(rowTemplate);
                // console.log("Added New Row");
            }
        }
    };
    self.addTemporaryClassOnChange = function ($input, oldOddValue, newOddValue) {
        var optionId = $input.data("option-id");
        //console.log(optionName);
        if ($.isNumeric(newOddValue)) {
            if (parseFloat(oldOddValue) > parseFloat(newOddValue)) {
                $input.addTemporaryClass("dropped", 3000);
                // console.log(FT1 + ' ' + FTX + ' ' + FT2 + ' down');
               // self.validateReceipt(true, newOddValue, optionId);
                // console.log("odd dropped");
            }

            if (parseFloat(oldOddValue) < parseFloat(newOddValue)) {
                $input.addTemporaryClass("raised", 3000);
                // console.log(FT1 + ' ' + FTX + ' ' + FT2 + ' up');
               // self.validateReceipt(true, newOddValue, optionId);
                // console.log("odd increased");
            }
            // console.log(" numeric")
        }
        else {
           // self.validateReceipt(false, newOddValue, optionId);
           // $input.attr('disabled', "disabled");
            // console.log("not numeric")
        }

    };
    self.getMatchClassIfMatchIsSelected = function (matchCode, optionName) {
        //console.log(_betListReff.getBets());


        var classString = null;
        var res = _betListReff.getBetByOptionNameAndMatchId($.trim(matchCode), optionName);
        console.log(res);
        if (res == true) {

            classString = "btn btn-default btn-xs odd selected_option";
        } else {

            classString = "btn btn-default btn-xs odd not_selected_option";
        }
        return classString.toString();
    };
    self.validateReceipt = function (isAReplacement, newOdd, optionId) {
        //gameToCheckOnReceipt is declared at the top of the function and
        //initilized iun updateGame()
        var _gameToCheckOnReceipt = gameToCheckOnReceipt;
        var _gameToCompareWithFromServer = gameToCompareWithFromServer;
        if (_gameToCheckOnReceipt) {
            //console.log(_gameToCheckOnReceipt);
            // console.log(_gameToCheckOnReceipt.matchId);
            var matchNoOnReceipt = _gameToCheckOnReceipt.matchId;
            var matchNoOnFromServer = _gameToCompareWithFromServer.MatchNo;

            if (matchNoOnReceipt === matchNoOnFromServer) {
                if (parseInt(_gameToCheckOnReceipt.optionId) == parseInt(optionId)) {
                    var $betList = $("#betList");
                    var $bet = $betList.find('li[id=' + matchNoOnReceipt + ']');
                    //console.log("found match on receipt");
                    if (isAReplacement) {
                        //ToDo update the bet on the receipt
                        var newBet = _gameToCheckOnReceipt;
                        newBet.odd = newOdd
                        _receiptApp.replaceBetElement(newBet, $bet);
                        //console.log(newBet);
                        console.log("replaced bet");

                    } else {
                        // remove the Bet from the receipt
                        _receiptApp.removeBet(_gameToCheckOnReceipt.betId, $bet);
                        console.log("removed bet");
                    }

                    console.log(_betListReff.getBets());

                }

            }

        }

       

    }
};