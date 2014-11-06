/// <reference path="receipt.js" />

function RenderLiveData(receiptApp) {
    var self = this,
        //set reference to the receipt application for continuos integration
         _receiptApp = receiptApp,         
         _betListReff =receiptApp.getBetList(),
        rowTemplate=null,
        ticker = $.connection.liveBetHubAng,
        $gameTable = null,
        $gamesTableBody = null;
    var FT1 = null, FTX = null, FT2 = null, under = null, over = null, RM1 = null, RMX = null, RM2 = null, NG1 = null, NGX = null, NG2 = null;
    var $FT1 = null, $FTX = null, $FT2 = null, $under = null, $over = null, $RM1 = null, $RMX = null, $RM2 = null, $NG1 = null, $NGX = null, $NG2 = null;
    var $row = null,
        gameToCheckOnReceipt = new Bet(0),
        gameToCompareWithFromServer = null;



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
                console.log("Orig Row"+"-"+ ++count);
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
        gameToCompareWithFromServer = game;
        gameToCheckOnReceipt = _betListReff.getBetByMatchIdAndOptionId($.trim(gameToCompareWithFromServer.MatchNo));
        console.log(_betListReff.getBets())
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
         rowTemplate = ' <tr  data-symbol="' + game.MatchNo + '" id="' + game.MatchNo + '"> ' +

       ' <td data-field="shortCode" class="short-code"> ' + game.ShortCode + ' </td> ' +
       ' <td data-field="matchId" class="match-code hidden">' + game.MatchNo + '</td> ' +
       ' <td data-field="startTime" class="start-time">' + game.Minutes + '</td> ' +
       ' <td data-field="startDate" class="start-date hidden"> ' + game.StartDate + '</td> ' +
       ' <td data-field="teamVersus" class="team-versus hidden">' + game.LocalTeam + '  vs  ' + game.AwayTeam + '</td> ' +
       ' <td>' + game.LocalTeam + '</td> ' +
       ' <td data-field="liveScores" class="live-scores">' + game.LocalTeamScore + ' - ' + game.AwayTeamScore + '</td> ' +
       ' <td>' + game.AwayTeam + '</td> ' +
       ' <td style="padding-bottom: 0px;"> ' +
            ' <input type="button" class="btn btn-sm btn-default FT-1 odd" value="' + game.FullTimeOdds.HomeWins + '" data-odd-type="Live" data-option-id="80" data-option-name=1" data-bet-category="FT1X2" /> ' +

   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default FT-X odd" value="' + game.FullTimeOdds.Draw + '" data-odd-type="Live" data-option-id="81" data-option-name="X" data-bet-category="FT1X2" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default FT-2 odd" value="' + game.FullTimeOdds.AwayWins + '" data-odd-type="Live" data-option-id="83" data-option-name="2" data-bet-category="FT1X2" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default under odd" value="' + game.UnderOverOdds.Under + '" data-odd-type="Live" data-option-id="78" data-option-name="U" data-bet-category="U/O" /> ' +
   '  </td> ' +
   '<td id="' + game.MatchNo + '" data-extra-value="' + game.UnderOverOdds.ExtraValue + '"  style="padding-top: 14px;">' +
       '  <span class="extra-value" style="color:#E9860D;">' + game.UnderOverOdds.ExtraValue + '</span>' +
       '</td>' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default over odd" value="' + game.UnderOverOdds.Over + '" data-odd-type="Live" data-option-id="79" data-option-name="O" data-bet-category="U/O" /> ' +
   '  </td> ' +
    ' <td style="padding-bottom: 0px;"> ' +
        ' <input type="button" class="btn btn-sm btn-default DC-1 odd" value="' + game.DoubleChance.HomeWinsOrDraw + '" data-odd-type="Live" data-option-id="74" data-option-name="1X" data-bet-category="Double Chance" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default DC-X odd" value="' + game.DoubleChance.HomeWinsOrAwayWins + '" data-odd-type="Live" data-option-id="71" data-option-name="12" data-bet-category="Double Chance" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default DC-2 odd" value="' + game.DoubleChance.AwayWinsOrDraw + '" data-odd-type="Live" data-option-id="77" data-option-name="X2" data-bet-category="Double Chance" /> ' +
   '  </td> ' +
    ' <td style="padding-bottom: 0px;"> ' +
        ' <input type="button" class="btn btn-sm btn-default NG-1 odd" value="' + game.NextGoal.HomeScores + '" data-odd-type="Live" data-option-id="71" data-option-name="H-GOAL" data-bet-category="FT1X2-NG" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default NG-X odd" value="' + game.NextGoal.Draw + '" data-odd-type="Live" data-option-id="72" data-option-name="NG" data-bet-category="FT1X2-NG" /> ' +
   '  </td> ' +
    ' <td> ' +
        ' <input type="button" class="btn btn-sm btn-default NG-2 odd" value="' + game.NextGoal.AwayScores + '" data-odd-type="Live" data-option-id="73" data-option-name="A-GOAL" data-bet-category="FT1X2-NG" /> ' +
   '  </td> ' +
' </tr> ';
        //return rowTemplate;
    };
    self.checkForChangeInOdd = function (game /*,$gamesTableBody, rowTemplate*/) {
        var MatchNo = game.MatchNo;
        //lets search the DOM once by using this variable for performance reasons
       
        $row = $gamesTableBody.find('tr[id=' + MatchNo + ']');
        //possible reuse of these variables initialisations can boost performance for large datasets
        //var FT1 = null, FTX = null, FT2 = null, under = null, over = null, RM1 = null, RMX = null, RM2 = null, NG1 = null, NGX = null, NG2 = null;
        //var $FT1 = null, $FTX = null, $FT2 = null, $under = null, $over = null, $RM1 = null, $RMX = null, $RM2 = null, $NG1 = null, $NGX = null, $NG2 = null;
        $FT1 = $(" td > input.FT-1", $row);
        $FTX = $(" td > input.FT-X", $row);
        $FT2 = $(" td > input.FT-2", $row);
        $under = $(" td > input.under", $row);
        $over = $(" td > input.over", $row);
        $RM1 = $(" td > input.DC-1", $row);
        $RMX = $(" td > input.DC-X", $row);
        $RM2 = $(" td > input.DC-2", $row);
        $NG1 = $(" td > input.NG-1", $row);
        $NGX = $(" td > input.NG-X", $row);
        $NG2 = $(" td > input.NG-2", $row);

        FT1 = $FT1.attr("value");       
        FTX = $FTX.attr("value");
        FT2 = $FT2.attr("value");
       // console.log(FT1 + " " + FTX + " " + FT2);
        under = $under.attr("value");
        over = $over.attr("value");
        //console.log(under + " " + over );
        RM1 = $RM1.attr("value");
        RMX = $RMX.attr("value");
        RM2 = $RM2.attr("value");
       // console.log(RM1 + " " + RMX + " " + RM2);
        NG1 = $NG1.attr("value");
        NGX = $NGX.attr("value");
        NG2 = $NG2.attr("value");
       // console.log(NG1 + " " + NGX + " " + NG2);

        // full time        
        self.addTemporaryClassOnChange($FT1, FT1, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($FTX, FTX, game.FullTimeOdds.Draw);
        self.addTemporaryClassOnChange($FT2, FT2, game.FullTimeOdds.HomeWins);
        //over/under
        self.addTemporaryClassOnChange($over, over, game.FullTimeOdds.Over);
        self.addTemporaryClassOnChange($under, under, game.FullTimeOdds.Under);
        //DOUBLE CHANCE
        self.addTemporaryClassOnChange($RM1, RM1, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($RMX, RMX, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($RM2, RM2, game.FullTimeOdds.HomeWins);
        //NO GOAL
        self.addTemporaryClassOnChange($NG1, NG1, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($NGX, NGX, game.FullTimeOdds.HomeWins);
        self.addTemporaryClassOnChange($NG2, NG2, game.FullTimeOdds.HomeWins);    

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
    self.addTemporaryClassOnChange = function ($input,oldOddValue,newOddValue) {
        var optionId = $input.data("option-id");
        //console.log(optionName);
        if ($.isNumeric(newOddValue)) {
            if (parseFloat(oldOddValue) > parseFloat(newOddValue)) {
                $input.addTemporaryClass("down", 3000);
                // console.log(FT1 + ' ' + FTX + ' ' + FT2 + ' down');
                self.validateReceipt(true, newOddValue, optionId);
               // console.log("odd dropped");
            }

            if (parseFloat(oldOddValue) < parseFloat(newOddValue)) {
                $input.addTemporaryClass("up", 3000);
                // console.log(FT1 + ' ' + FTX + ' ' + FT2 + ' up');
                self.validateReceipt(true, newOddValue, optionId);
               // console.log("odd increased");
            }
           // console.log(" numeric")
        }
        else {
            self.validateReceipt(false,newOddValue, optionId);
            $input.attr('disabled', "disabled");
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
    self.validateReceipt = function (isAReplacement,newOdd,optionId) {
        //gameToCheckOnReceipt is declared at the top of the function and
        //initilized iun updateGame()
     var   _gameToCheckOnReceipt = gameToCheckOnReceipt;
     var _gameToCompareWithFromServer = gameToCompareWithFromServer;
     if (_gameToCheckOnReceipt)
     {
         //console.log(_gameToCheckOnReceipt);
        // console.log(_gameToCheckOnReceipt.matchId);
               var matchNoOnReceipt = _gameToCheckOnReceipt.matchId;
               var matchNoOnFromServer = gameToCompareWithFromServer.MatchNo;
      
               if (matchNoOnReceipt === matchNoOnFromServer) {
                   if (_gameToCheckOnReceipt.optionId===optionId) {
                    var $betList = $("#betList");
                   // var $bet = $('#' + matchNoOnReceipt, $betList).clone();
                   var $bet = $betList.find('li[id=' + matchNoOnReceipt + ']')
                   //.replaceWith(rowTemplate);
                   // console.log($bet);
                   console.log("found match on receipt");
                   if (isAReplacement) {
                       //ToDo update the bet on the receipt
                       var newBet = _gameToCheckOnReceipt;
                       newBet.odd = newOdd
                       _receiptApp.replaceBetElement(newBet, $bet);
                       console.log(newBet);
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

       // removeBet(bet.betId, $bet);
        //var $oddsTable = $("table.oddstable");
        //$(".odd", $oddsTable).each(function () {

        //    var $me = $(this).parent().siblings("td.match-code");
        //    if ($.trim($me.text()) === $("span.match-code", $bet).text()) {

        //        $(this).removeClass("selected_option");
        //        $(this).attr("disabled", false);
        //        $(this).parent("td").removeClass("selected_option");
        //    }
        //});

    }
};