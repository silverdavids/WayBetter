/**
 * Created by kazibwesempa on 8/20/2014.
 */
"use strict";
function BettingApp() {
    var version = "Version 1.0.0";
    var betList = new BetList();
    var autoNumericInitializer = new AutoNumericInitializer();
    var maximumNumBet = 0;
    var minStake = 0;
    //var tax = 0;
    
    var setMaxPayoutPossible = 0;
    function setStatus(message) {
        $('#app > footer').text(message);

    };
    this.getBetList = function () {
        return betList;
    };
    this.start = function (_maximumNumBet, _minStake, _setMaxPayoutPossible,_tax) {
        $('#app > header').append(version);
        setStatus("Ready...");
       // bet = _tax || 0.15;
       // Bet.localTax = _tax||0.15;
        maximumNumBet = _maximumNumBet || 12;
        minStake = _minStake || 3000;
        Bet.setMaxPayoutPossible = _setMaxPayoutPossible || 50000000;
        updateSummaryFields();
       

        bindControlButtons(betList);
        manageMultipleBets();

    };
    function manageMultipleBets() {
        Bet.multipleBetAmount = 0;

        var $multipleBet = $("#multiple-bet-template");
        $(".multiple-bet input.multipleBetAmount", $multipleBet).on("change", function () {
            var inputValue = parseInt($(this).autoNumeric('get'));
            Bet.multipleBetAmount = inputValue;
            updateSummaryFields();
        }).autoNumeric('init', { mDec: '0' });

        if (betList.getBets().length == 0) {
            autoNumericInitializer.initAutoNumberOnField($(".multiple-bet input.multipleBetAmount", $multipleBet), 0, 0, '');
            var inputValue = parseInt($(".multiple-bet input.multipleBetAmount", $multipleBet).autoNumeric('get'));

            Bet.multipleBetAmount = inputValue;
            updateSummaryFields();
            autoNumericInitializer.initAutoNumberOnField($(".multiple-bet input.multipleBetAmount", $multipleBet), "", 0, '');
        }
    };
    function clearReceiptAfterPrint() {
       // var $controlButtons = $("#controlButtons");


        betList.removeBets();
        manageMultipleBets();
        $("#betList").empty();
        $("#errors").empty();

        var $multipleBet = $("#multiple-bet-template");
        $(".multiple-bet input.multipleBetAmount", $multipleBet).val("");
        updateSummaryFields();



    };

    function manageLogins($controlButtons) {
        if ($.trim($("p#userName").text()) == "" || $.trim($("p#userName").text()) == 'Anonymous') {
            $(".print-receipt", $controlButtons).val("Login In To Bet ")
           // alert("user nane is null");
        } else {
            $(".print-receipt", $controlButtons).val("Print Receipt");
           // alert("user name  is  not null " + $.trim($("p#userName").text()));
        }
    }
    function bindControlButtons(betList) {
        var $controlButtons = $("#controlButtons");
        manageLogins($controlButtons);
        $(".cancel-receipt", $controlButtons).on("click", function () {

            betList.removeBets();
            manageMultipleBets();
            $("#betList").empty();
            $("#errors").empty();

            var $multipleBet = $("#multiple-bet-template");
            $(".multiple-bet input.multipleBetAmount", $multipleBet).val("");
            updateSummaryFields();
            var $oddsTable = $("table.oddstable");
            $(".odd", $oddsTable).each(function () {
                $(this).removeClass("selected_option");
                $(this).attr("disabled", false);
                $(this).parent("td").removeClass("selected_option");
            });
            if (betList.getBets().length > 0)
                logToConsole();
        });
        $(".print-receipt", $controlButtons).on("click", function () {
            // var $receptToPrint = $("#main");
            /* var windowToPrint = window.open("");
             windowToPrint.document.write($receptToPrint.outerHTML);
             windowToPrint.print();
             windowToPrint.close();*/
            //check if the receipt contains atleast a game




           // $("#loginForm").modal();

            var _validateReceipt = validateReceipt(betList, betList.getTotalBettedAmount());
            if (_validateReceipt.isValid) {

                //logToConsole(betList.getBets());
                var sendReceipt = new SendReceipt();

                sendReceipt.postReceipt(betList).done(function(response){
                    //the code for printing recept is in the receipt sender
                    autoNumericInitializer.initAutoNumberOnField($("#tellerBalance"), response.Balance, 0, ' Teller Balance is Ugx');
                   // $("#tellerBalance").text("Teller Balance is Ugx " + response.Balance);
                    clearReceiptAfterPrint();
                    var $oddsTable = $("table.oddstable");
                    $(".odd", $oddsTable).each(function () {
                        $(this).removeClass("selected_option");
                       
                        $(this).attr("disabled", false);
                        $(this).parent("td").removeClass("selected_option");

                    });
                }).fail(function (error) {
                    //$("#tellerBalance").text("Teller Balance is Ugx " + response.Balance);
                    alert(error);
                });
                
                return false;
            } else {
                printErrorDetails(_validateReceipt.errMessageArr);
            }

        });
    };
    function validateReceipt(betList, totalStake) {
        var _isValid = null,
            _errMessage = null,
            validateObject = {};
        validateObject.errMessageArr = validateObject.errMessageArr || [];
        if (betList.getBets().length > 0 && $("#betList li").length > 0) {
            if (totalStake >= minStake) {
                validateObject.isValid = true;

            } else {
                validateObject.isValid = false;
                validateObject.errMessageArr.push("Total stake has to be grater than Ugx " + minStake + "please enter a value greater this in last input ");
            }

        } else {
            validateObject.isValid = false;
            validateObject.errMessageArr.push("Receipt must have atleast one game to print");
        }

        return validateObject;
    };
    function validateBet(betList, bet) {
        var _isValid = null,
            //_errMessage = null,
            validateObject = {};
        validateObject.isValid = false;
        validateObject.validateObject = null;
        validateObject.errMessageArr = validateObject.errMessageArr || [];
        var numOfBetsAlreadyMade = betList.getBets().length;
       

        if ((numOfBetsAlreadyMade >= maximumNumBet) || betList.getBetByMatchId(bet.matchId) == true || bet.odd <= 0.0) {
            validateObject.isValid = false;

        } else {

            validateObject.isValid = true;
        }

        if(numOfBetsAlreadyMade >= maximumNumBet)
        {
          
            validateObject.errMessageArr.push("Reached a maximum number of bets, you can not bet on more than " + maximumNumBet + " games on one receipt");
        }
        // Check for duplicate bets
        if (betList.getBetByMatchId(bet.matchId) ==true) {
            //validateObject.isValid = false;
            validateObject.errMessageArr.push("You already have this match " + bet.matchId + " on the receipt and it has been ignored,first remove it to change.");
        }

        //check for zero odds
        
        if (bet.odd <= 0.0) {
           
            //validateObject.isValid = false;
            validateObject.errMessageArr.push("Odds can not be zero, please choose another match");
        } 
           

        return validateObject;
    };
    this.mockBets = function (bet) {


        var _validateBet = validateBet(betList, bet);
        //alert(_validateBet.isValid  +"in mockbets");
        if (_validateBet.isValid) {

            betList.addBet(bet);
            addBetElement(bet);
        } else {
            printErrorDetails(_validateBet.errMessageArr);
        }
        //console.log(betList.getBets().length);
    };


    function saveBetList() {
        //to do
        //save bet list to temporary storage
    }

    function logToConsole(value) {

        console.log(betList.getBets());
    }

    function removeBet(betId, $bet) {

        if (betId) {
            betList.removeBet(betId);
            var bet = $bet.remove();

            updateSummaryFields();
            manageMultipleBets();

            logToConsole();
            //saveBetList();
        }



    };
   

    function updateSummaryFields() {
       

        var $selectionSummary = $("#selection-summary");
        $(".total-odds", $selectionSummary).text(betList.getTotalBettedOdd() == 1 ? 0 : betList.getTotalBettedOdd());
        $(".bet-amount", $selectionSummary).text(" " + betList.getTotalBettedAmount());
        $(".number-of-selections", $selectionSummary).text(betList.getBets().length + "  Selections");

        var $stakeSummary = $("#stakeSummary");
       autoNumericInitializer.initAutoNumberOnField($(".total-pay", $stakeSummary), betList.getTotalBettedAmount(), 0, '  ');
        $(".stake-text", $stakeSummary).text("Total Stake " + "(" + betList.getBets().length + ")" + " lines");

        var $totalPayout = $("#totalPaySummary");
       autoNumericInitializer.initAutoNumberOnField($(".total-pay", $totalPayout), betList.getTotalPayout(), 0, '  ')
        var $maxPayout = $("#maxPayout");
       autoNumericInitializer.initAutoNumberOnField($(".max-payout-amount", $maxPayout), betList.getMaxPayout(), 0, ' Ugx ');
        //initAutoNumberOnField( $(".tax",$maxPayout),betList.getTax().toFixed(2),2,' Tax Ugx ');
        // $(".tax",$maxPayout).text("Tax Ugx "+betList.getTax().toFixed(2));

        var $taxAmount = $("#taxSummary");
        autoNumericInitializer.initAutoNumberOnField($(".tax", $taxAmount), betList.getTax().toFixed(2), 0, ' - ');
        
        var $bonusAmount = $("#bonusSummary");
        autoNumericInitializer.initAutoNumberOnField($(".bonus", $bonusAmount), betList.getBonus().toFixed(2), 0, ' + ');
        // $(".tax",$taxAmount).text("Tax Ugx "+betList.getTax().toFixed(2));
        var $multipleBetTemplate = $("#multiple-bet-template");
        $(".total-odds", $multipleBetTemplate).text(betList.getTotalBettedOdd() == 1 ? 0 : betList.getTotalBettedOdd());
        $("#multipleBetLeft .multiple-bet-no").text($("#betList li").length + "=");
    };
    function printErrorDetails(errMessageArr) {
        var html = "<p class='event listening'>";
        for (var i in errMessageArr) {
            html += html + errMessageArr[i] + "</p>";
        }
        html=html+"</div>";
        // alert(html);
        $("#errors").empty();
        $("#errors").append(html)
            .show()
            .focus().toggleClass('hidden');
       // $("#myModal div.modal-body").empty()
       //     .append(html)
       //     .show()
       //     .focus(); //.toggleClass('hidden');
        

       //// bootBox.alert("Error");
       // $("#myModal").on("show", function () {    // wire up the OK button to dismiss the modal when shown
       //     $("#myButton").on('click', function (e) {
       //         console.log("button pressed");   // just as an example...
       //         $("button#myModal").modal('hide');     // dismiss the dialog
       //     });
       // });

       // $("#myModal").on("hide", function () {    // remove the event listeners when the dialog is dismissed
       //     $("#myButton").off("click");
       // });

       // $("#myModal").on("hidden", function () {  // remove the actual elements from the DOM when fully hidden
       //     $("#myModal").remove();
       // });

       // $("#myModal").modal({                    // wire up the actual modal functionality and show the dialog
       //     "backdrop": "static",
       //     "keyboard": true,
       //     "show": true                     // ensure the modal is shown immediately
       // });

    };

    function onChangeBetAmount(betId, $betInput) {
        var fieldName = $betInput.data("field");
        var bet = betList.getBet(betId);
        if (bet) {
            bet[fieldName] = parseInt($betInput.val()) ? parseInt($betInput.val()) : 0;

            updateSummaryFields();
            //the following line is for testing purposes i commented its
            //implementation, no need to worry
            logToConsole();
        }
    };

    function addBetElement(bet) {
        var $bet = $("#bet-template .bet").clone();

        $("span.match-code", $bet).text(bet.shortCode);
        alert(bet.shortCode)
        $bet.data("betId", bet.betId);
        $("#betList").append($bet);
        $("button.delete", $bet).on("click", function () {
            removeBet(bet.betId, $bet);
            var $oddsTable = $("table.oddstable");
            $(".odd", $oddsTable).each(function () {             

                    var $me = $(this).parent().siblings("td.match-code");
                    if ($.trim($me.text()) === $("span.match-code", $bet).text()) {

                $(this).removeClass("selected_option");
                $(this).attr("disabled", false);
                $(this).parent("td").removeClass("selected_option");
            }
            });
        });

        $("span", $bet).each(function () {
            var $input = $(this),
                fieldName = $input.data("field");
            if (fieldName.toString() == "teamVersus".toString() && bet.betCategory == "Handicap") {

                $input.text(bet[fieldName] + "-" + bet["handCapGoalString"]);

            } else  if (fieldName.toString() == "matchId".toString()) {
                    alert(bet.shortCode)
                    $input.text(bet["shortCode"]);
            }
            else{

                $input.text(bet[fieldName]);
            }

            //console.log(fieldName);
            //console.log(bet["handCapGoalString"]);

        });
        $("input", $bet).each(function () {
            var $input = $(this),
                fieldName = $input.data("field");
            $input.val(bet[fieldName]);
        });
        $("input", $bet).on("change", function () {
            onChangeBetAmount(bet.betId, $(this));
        }).on("focus", function () {
            if (parseInt($(this).val()) == 0) {
                $(this).val("");
            }
        });//.maskMoney({thousands:'',decimal:'.',precision:0,allowNegatives:false});
        $("input", $bet).autoNumeric('init', { mDec: '0' });
        updateSummaryFields();

    };


};

$(function () {

    //setTimeout(function () {
        console.log("initialising  receipt scripts");
    //please avoid using the "this" as this in other languages like C# and Java means
    // the object you are inside of but here in javascripts it can mean  different things as follows
    //1. it can mean the object your inside of
    //2. for event handlers , it cann the object that called the function which for this case its the window
    // object.
    // there the code below can rewriten like this
    /*
     * window.app=new BettingApp();
     * window.app.start();
     * this is because this event handler ( or call it "function") has been called by the JQuery's $(handler) or $(document).ready(handler)
     * or javascript's window.document.ready(handler)
     *
     * */

    var maximumNumBets = 50;//set this value to the maximum number of bets allowed for this branch
    var minStake = 2000;//set this value to the minimum stake for the branch
    var setMaxPayoutPossible = 50000000;
    var tax = 0;

    var thisApp = this.app = new BettingApp();
    //this.renderTable = new RenderTables(this.app.getBetList());
        //this.renderTable.startRenderingTables();
    this.renderLiveData = new RenderLiveData(this.app.getBetList());
   
    this.app.start(maximumNumBets, minStake, setMaxPayoutPossible, tax);
    //var thisApp = this.app;

    // var $match = $('table');
    //var $match = $("#match .match");
    // var betList = new Array();
    $('table.oddstable').delegate('.odd', 'mouseover', function () {
        //console.log("hovered");
        // alert("fired click");
        var $that = $(this);

        var chosenOdd = $that.val(),
            optionId = $that.data("option-id"),
            handCapGoalString = $that.data("hc-homegoal") + ":" + $that.data("hc-awaygoal"),
            $matchCode = $that.parent().siblings("td.match-code"),
            matchCode = $.trim($matchCode.text()),
            optionName = $that.data("option-name"),
            bet = new Bet(matchCode);
        //attach tooltips
        makeToolTip($that, { matchCode: matchCode, optionName: optionName });

    });

    var $categories2 = $("#categories2");
    $("#maindiv").scroll(function () {

        var mainDivScrollPosition = $("#maindiv").scrollTop();

        // console.log(/*categories2.bottom + "-" +*/ oddstable);

        if (mainDivScrollPosition > 29) {
            //console.log("scrolled");
            $("table", $categories2).removeClass("hidden");
        } else if (mainDivScrollPosition < 29) {
            //console.log("scrolled");
            $("table", $categories2).addClass("hidden");
        }
    });

    $('table.oddstable').delegate('.odd', 'click', function () {

       // alert("fired click");

        var $that = $(this);
        //check if the match has already been selected, if yes remove it from the list and receipt

        if (checkForAlreadySelectedMatch($that) == true) {

            var chosenOdd = $that.val(),
                optionId = $that.data("option-id"),
                handCapGoalString = $that.data("hc-homegoal") + ":" + $that.data("hc-awaygoal"),
                $matchCode = $that.parent().siblings("td.match-code"),
                matchCode = $.trim($matchCode.text()),
                 $shortCode = $that.parent().siblings("td.short-code"),
                 shortCode = $.trim($shortCode.text()),
                optionName = $that.data("option-name"),
                liveScores = $that.parent().siblings("td.live-scores").text(),
                extraValue = $that.parent().siblings("td#" + matchCode).data("extra-value"),
                bet = new Bet(matchCode);
            alert(extraValue);
            // $that.tooltip({ placement: 'top', title:''+ matchCode + " " + optionName +''});
            // console.log(handCapGoalString);
            bet.shortCode = shortCode;
            bet.handCapGoalString = handCapGoalString;
            bet.odd = chosenOdd;
            bet.optionId = optionId;
            bet["optionName"] = optionName;
            bet["betCategory"] = $that.data("bet-category");
            bet["optionName"] = $that.data("option-name");
            bet["liveScores"] = liveScores;
            bet["extraValue"] = extraValue;
            $matchCode.siblings("td").each(function () {
                var fieldName = $(this).data("field");
                bet[fieldName] = $(this).text();
            });
            var $parentTr = $matchCode.parent();
            $("td>input.odd", $parentTr).each(function () {
                var fieldName = $(this).data("field");
                if (fieldName) {
                    //bet[fieldName] = $(this).val();
                    // bet["betCategory"] = $(this).data("bet-category");
                    // bet["optionName"] = $(this).data("option-name");
                }
                //alert("fired click");
            });

           
            // betList.push(bet);
            makeBet(bet);
            $that.parent("td").addClass("selected_option");
            scrollReceiptToBottom();


            console.log(bet);
            //disable all input elements  for this match
            var $oddsTable = $("table.oddstable");
            $(".odd", $oddsTable).each(function () {
                var $me = $(this).parent().siblings("td.match-code");
                if ($.trim($me.text()) === matchCode)
                {
                 $(this).attr("disabled","disabled");

                }
               // $(this).removeCla ss("selected_option");

            });
           
        } else {
            //$that.removeClass("selected_option");
            alert("Match already exits on the receipt remove it first to select again");
        }

    });
        //listening to the change event of the table odds
    $('table.oddstable').delegate('.oddinput', 'change', function () {

         alert("fired change");

        var $that = $(this);
        //check if the match has already been selected on the receipt, if yes update it on the receipt
        $matchCode1 = $that.parent().siblings("td.match-code"),
               matchCode1 = $.trim($matchCode.text());
        var     optionId1 = $that.data("option-id");
        if (checkRemoveBetByMatchId(matchCode1, optionId1) == matchCode1) {

            var chosenOdd = $that.val(),
                optionId = $that.data("option-id"),
                handCapGoalString = $that.data("hc-homegoal") + ":" + $that.data("hc-awaygoal"),
                $matchCode = $that.parent().siblings("td.match-code"),
                matchCode = $.trim($matchCode.text()),
                optionName = $that.data("option-name"),
                liveScores = $that.parent().siblings("td.live-scores").text(),
                extraValue = $that.parent().siblings("td> span.extra-value").text(),
                bet = new Bet(matchCode);
            // $that.tooltip({ placement: 'top', title:''+ matchCode + " " + optionName +''});
            // console.log(handCapGoalString);
            bet.handCapGoalString = handCapGoalString;
            bet.odd = chosenOdd;
            bet.optionId = optionId;
            bet["optionName"] = optionName;
            bet["betCategory"] = $that.data("bet-category");
            bet["optionName"] = $that.data("option-name");
            bet["liveScores"] = liveScores;
            bet["extraValue"] = extraValue;
            $matchCode.siblings("td").each(function () {
                var fieldName = $(this).data("field");
                bet[fieldName] = $(this).text();
            });
            var $parentTr = $matchCode.parent();
            $("td>input.odd", $parentTr).each(function () {
                var fieldName = $(this).data("field");
                if (fieldName) {
                    //bet[fieldName] = $(this).val();
                    // bet["betCategory"] = $(this).data("bet-category");
                    // bet["optionName"] = $(this).data("option-name");
                }
                //alert("fired click");
            });


            // betList.push(bet);
            if ($.isNumeric(bet.odd)) {
            makeBet(bet);
        }
            $that.parent("td").addClass("selected_option");
            scrollReceiptToBottom();


            console.log(bet);
            //disable all input elements  for this match
            var $oddsTable = $("table.oddstable");
            $(".odd", $oddsTable).each(function () {
                var $me = $(this).parent().siblings("td.match-code");
                if ($.trim($me.text()) === matchCode) {
                    $(this).attr("disabled", "disabled");

                }
                // $(this).removeCla ss("selected_option");

            });

        } else {
            //$that.removeClass("selected_option");
            console.log("Match not found continuing");
        }

    });

    var scrollReceiptToBottom = function () {
        var mainAppScrollPosition = $("#app").scrollTop();
        $("#app").scrollTop(500);
        console.log(mainAppScrollPosition);
    };
    function checkForAlreadySelectedMatch($input) {
        if ($input.hasClass("selected_option")) {
            //$input.removeClass("selected_option");
            return false;
        } else {

             
            $input.toggleClass("selected_option");
            return true;
        }
    }

    function makeToolTip($input, objArgs) {
        $input.tooltip({ placement: 'top', title: '' + objArgs.matchCode + "-" + objArgs.optionName + '' });
    }
    function makeBet(bet) {

        thisApp.mockBets(bet);
    }
   // $("#app").offcanvas('show')
   // }, 12000);
});