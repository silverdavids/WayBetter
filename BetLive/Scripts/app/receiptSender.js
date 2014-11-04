/**
 * Created by kazibwesempa on 9/12/2014.
 */
var $receiptSenderData = $receiptSenderData || {};
$receiptSenderData.settings = {};
$receiptSenderData.settings.baseUrl = "/";
function Receipt() {

    this.ReceiptSize = 0;
    this.TotalOdd = 0;
    this.TotalStake = 0;
    this.betData = new Array();
    this.MultipleBetAmount = 0;
    this.UserName = null;
}

function BetData() {

    this.MatchId = null;

    this.BetCategory = null;

    this.OptionId = 0;

    this.Odd = 1;
    this.BetAmount = 0;
    this.LiveScores = null;
    this.StartTime = null;
    this.ExtraValue = null;
    this.ShortCode = 0;

};


function SendReceipt() {


    this.postReceipt = function (betList) {
        var diferred = $.Deferred();
        var receipt = new Receipt();
        var bets = betList.getBets();
        receipt.ReceiptSize = bets.length;
        receipt.TotalOdd = betList.getTotalBettedOdd();
        receipt.TotalStake = betList.getTotalBettedAmount();
        receipt.MultipleBetAmount = Bet.multipleBetAmount;
        var authData = {};
        authData = JSON.parse(localStorage.getItem("ls.authorizationData"));
        receipt.UserName = authData.userName;
        alert( receipt.UserName);
        //receipt.UserName = "";
        for (var i in bets) {
            var _betData = new BetData(),
                _bet = bets[i];
            _betData.MatchId = _bet.matchId;
            _betData.BetCategory = _bet.betCategory;
            _betData.OptionId = _bet.optionId;
            _betData.Odd = _bet.odd;
            _betData.BetAmount = _bet.betAmount
            _betData.LiveScores = _bet.liveScores;
            _betData.StartTime = _bet.startTime;
            _betData.ExtraValue = _bet.extraValue;
            _betData.ShortCode = _bet.shortCode;
            receipt.betData.push(_betData);

        }
        if (betList.getBets().length > 0) console.log(receipt);

      //  var url = "http://localhost:49193/api/ReceiptPrint/ReceiveReceipt";
        var url = $receiptSenderData.settings.baseUrl + "ReceiptPrint/ReceiveReceipt";
        $.ajax({
            url: url,
            type: "POST",
            data: receipt,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            dataType: "json",
            beforeSend: function (jqXHR, settings) {
                // config.headers = config.headers || {};
                var authData = {};
                authData = JSON.parse(localStorage.getItem("ls.authorizationData"));
                //console.log("got it " + authData.token);
                if (authData) {
                    //config.headers.Authorization = 'Bearer ' + authData.token;
                    jqXHR.setRequestHeader('Authorization', 'Bearer ' + authData.token);
                    jqXHR.setRequestHeader('UN', authData.userName);
                    console.log(authData.token);
           
                    //$.extend(settings, { headers: { 'Authorization': 'Bearer ' + authData.token } })
                }
            }


        }).done(function (response) {
            var Message = response.message;
            alert(response);
            console.log(response);
            if (Message == "Success") {
                console.log(response.receiptFromServer)
                console.log(response.FormatedSerial);
                $("#rcpTorderId").text(response.ReceiptNumber);
                $("#rcpTbarCode span.caption").text(response.FormatedSerial);
                $("#rcpTtimeOfBet").text(response.ReceiptTime);
                $("#rcpTbranch").text(response.BranchName);
                $("#rcpTteller").text(response.TellerName);
                $("#rcpTbarCode img").attr({
                    src: "http://localhost:49193/Content/Barcodes/" + response.Serial + ".png",
                    alt: response.Serial//.toString()
                });
                // ReceiptNumber

                $("#tellerBalance").text("Teller Balance is Ugx " + response.Balance);
                var $receiptDiv = $("#rcpTreceiptTemplate");
                var receiptGen = new ReceiptGen();
                receiptGen.clearReceipt();
                receiptGen.generateReceipt(betList);
                // $receiptDiv.removeClass("hidden");

                $receiptDiv.print();
                $receiptDiv.addClass("hidden");


                diferred.resolve(response);

            } else {
                diferred.reject("Failed to submit the receipt with error message " + '""' + response.message + '""' + " contact your systems administrator ");

            }


        });

        return diferred.promise();
    };
}