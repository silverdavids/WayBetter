var matches = null,
    mRow=null


//Class that contains general fields abouteach matvh from each category
var matchDistinct = function () {
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
//Class to Flatten each match with all its odds
var matchRow = function () {
    this.matchDistinct = new matchDistinct();
    this.GameOdds = new Array();
}