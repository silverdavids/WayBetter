using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Concurrent;

using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Text;
using System.IO;
using System.Threading;
using WebUI.Helpers;
using BetLive.Controllers.Api;
using System.Web.Http;
using Domain.Models.ViewModels;
using Domain.Models.Concrete;
using WebUI.DataAccessLayer;
using System.Data.Entity.Migrations;

namespace BetLive.Hubs
{
    [HubName("liveBetHubAng")]
    public class LiveGameHub : Hub
    {
        private readonly IList<LiveBetSource> _liveBetUrls;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveScore = new ConcurrentDictionary<string, Game>();
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveOdds = new ConcurrentDictionary<string, Game>();
        private readonly IList<Game> gameToSave;

        private readonly IDictionary<string, SavedMatch> _shortMatchCodes;
       
        //For db connectivity
        private ApplicationDbContext BetDatabase;
        private CustomController cc;

        private Timer _timer;
        private volatile bool _updatingGame = false;
        private readonly object _updateGameLock = new object();
        private readonly Random _updateOrNotRandom = new Random();
        private MyController myController;
        private MatchController myApiController;
        private static int shortMatchCode = 0;
        public static int mockXmlFileExt= 0;
        public int maxShortCodeInDb;

        #region LiveGameHubConstructor
        public LiveGameHub()
        {
           
            gameToSave = new List<Game>();
            //the controller below is for testing please comment it initailisation to save server resources
            myController = new MyController();
            myApiController = new MatchController();
            _shortMatchCodes = new Dictionary<string, SavedMatch>();
            _gamesforLiveScore.Clear();
           
            cc = new CustomController();
            BetDatabase = cc.BetDatabase;
            setInitialStaticCodeFromDb();
            setInitialGamesFromDb();
           
            _liveBetUrls = new List<LiveBetSource>
            {
                new LiveBetSource
                {
                    Name = "Scores",
                    Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/inplay"
                },
                new LiveBetSource
                {
                    Name = "Odds",
                    Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/lines/soccer-inplay"
                }
            };

        }

      

       
        #endregion
        #region getAllNormalGames
        public async Task<List<GameViewModel>> getAllNormalGames()
        {

            var games = await myApiController.GetMatches();
            return games.ToList();

        }
        #endregion
        #region GetAllGames
        public async Task<IEnumerable<Game>> GetAllGames()
        {
            if (mockXmlFileExt < 7)//7
            {
                ++mockXmlFileExt;
            }
            else
            {
                --mockXmlFileExt;
            }
            var scores = await  GetGamesScores();/*await myController.GetGamesScoresFromXml(mockXmlFileExt);*/
            var odds  =await GetGamesOdds();  /* await  myController.GetGamesOddsFromXml(mockXmlFileExt);*/
            var allGames = (from gamescore in scores
                            join gameodds in odds
                            on gamescore.MatchNo equals gameodds.MatchNo
                            select new Game
                            {
                                ShortCode = generateOrGetShortMatchCode(gamescore.MatchNo),
                                MatchNo = gamescore.MatchNo,
                                Minutes = gamescore.Minutes,
                                StartTime=gameodds.StartTime,
                                StartDateToSave=gameodds.StartDate,
                                StartDate = gameodds.StartDate,
                                League=gameodds.League,
                                AwayTeam = gamescore.AwayTeam,
                                LocalTeam = gamescore.LocalTeam,
                                AwayTeamScore = gamescore.AwayTeamScore,
                                LocalTeamScore = gamescore.LocalTeamScore,
                                FullTimeOdds = gameodds.FullTimeOdds,
                                UnderOverOdds = gameodds.UnderOverOdds,
                                RestOfMatch = gameodds.RestOfMatch,
                                NextGoal = gameodds.NextGoal,
                                DoubleChance = gameodds.DoubleChance
                            }).ToList();
                           
            //_timer = new Timer(UpdateGames, null, _updateInterval, _updateInterval);

            foreach (var g in allGames)
            {
                gameToSave.Add(g);
            }
            DoUpdateGames();
            return allGames;
        }

        private DateTime getFormattedStartDate(string gameNode,string startTime)
        {
            char[] del = { '.' };
            var stdate = gameNode.Split(del);
            var stDateTime = stdate[1] + "-" + stdate[0] + "-" + stdate[2]
                             + " " + startTime + ":00";
           return Convert.ToDateTime(stDateTime).ToLocalTime();
        }

        private int generateOrGetShortMatchCode(string matchCode)
        {
            if (_shortMatchCodes.ContainsKey(matchCode))
            {
               
                return _shortMatchCodes[matchCode].ShortMatchCode;
            }
            var _shortMatchCode = ++shortMatchCode;
            var _savedMatch = new SavedMatch{
             ShortMatchCode=_shortMatchCode,
              IsAlreadysaved=false
            };

            _shortMatchCodes.Add(matchCode, _savedMatch);
            
            return shortMatchCode;
        }
        private SavedMatch isMatchAlreadySavedToDb(string matchCode)
        {
            
               SavedMatch savedMatchInList= _shortMatchCodes[matchCode];
              
                   return savedMatchInList;
             
        }
        #endregion
        #region UpdateGames
        private async void UpdateGames(object state)
        {

            try
            {
                //var updatedGames = gameToSave;
                var updatedGames = await GetAllGames();
                lock (_updateGameLock)
                {
                    if (!_updatingGame)
                    {
                        _updatingGame = true;
                        foreach (var game in updatedGames)
                        {
                            if (TryUpdateGame(game))
                            {
                                //broadcast on the UI
                                BroadcastLiveGame(game);
                            }
                        }
                        _updatingGame = false;
                    }
                }
            }
            catch (Exception e)
            {


            }


        }


        private void DoUpdateGames()
        {

            try
            {
                var updatedGames = gameToSave;
                //var updatedGames = await GetAllGames();
                lock (_updateGameLock)
                {
                    if (!_updatingGame)
                    {
                        _updatingGame = true;
                        foreach (var game in updatedGames)
                        {
                            if (TryUpdateGame(game))
                            {
                                //broadcast on the UI
                                 BroadcastLiveGame(game);
                            }
                        }
                        _updatingGame = false;
                    }
                }
            }
            catch (Exception e)
            {


            }


        }
        #endregion
        #region GetGamesScores
        public async Task<IEnumerable<Game>> GetGamesScores()
        {
            var scoresUrl = new LiveBetSource
            {
                Name = "Scores",
                Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/inplay"
            };

            _gamesforLiveScore.Clear();
            var req = (HttpWebRequest)WebRequest.Create(scoresUrl.Url);
            var res = (HttpWebResponse)req.GetResponse();
            var stream = res.GetResponseStream();
            var xmldoc = new XmlDocument();
            if (stream == null) return _gamesforLiveScore.Values; // if the stream is null return the games the way they are

            // Get a UTF-32 encoding by name.
            Encoding encoding = Encoding.GetEncoding("utf-8");
            using (var str = new StreamReader(stream, encoding))
            {
                try
                {
                    xmldoc.Load(str);
                }
                catch (Exception)
                {

                    // continue;
                }
            }

            // check the url that has been loaded
            var categoryList = xmldoc.SelectNodes("/scores/match");
            if (categoryList != null)
                foreach (XmlNode category in categoryList)
                {
                    var match = category;
                    var teams = match.ChildNodes;
                    var testGame = new Game();

                    if (match.Attributes != null)
                    {
                        testGame.MatchNo = match.Attributes["id"].InnerText;
                        testGame.Minutes = match.Attributes["minute"].InnerText;
                       // testGame.StartDate = DateTime.Now.Date.ToString();
                    }

                    foreach (XmlNode team in teams)
                    {
                        var local = team.Name;
                        if (team.Name == "localteam")
                        {

                            var localTeamAttributes = team.Attributes;
                            if (localTeamAttributes != null)
                            {
                                testGame.LocalTeam = localTeamAttributes["name"].InnerText;
                                testGame.LocalTeamScore = localTeamAttributes["score"].InnerText == "" ? "?" : localTeamAttributes["score"].InnerText;
                            }
                        }
                        if (team.Name == "awayteam")
                        {
                            var awayTeamattributes = team.Attributes;
                            if (awayTeamattributes != null)
                            {
                                testGame.AwayTeam = awayTeamattributes["name"].InnerText;
                                testGame.AwayTeamScore = awayTeamattributes["score"].InnerText == "" ? "?" : awayTeamattributes["score"].InnerText;
                            }
                        }

                        _gamesforLiveScore.TryAdd(testGame.MatchNo, testGame);
                    }


                }
            return _gamesforLiveScore.Values;
        }
        #endregion

        #region GetGamesOdds
        public async Task<IEnumerable<Game>> GetGamesOdds()
        {
            var oddSource = new LiveBetSource
            {
                Name = "Odds",
                Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/lines/soccer-inplay"
            };

            _gamesforLiveOdds.Clear();
            var req = (HttpWebRequest)WebRequest.Create(oddSource.Url);
            var res = (HttpWebResponse)req.GetResponse();
            var stream = res.GetResponseStream();
            var xmldoc = new XmlDocument();
            if (stream == null) return _gamesforLiveOdds.Values; // if the stream is null return the games the way they are

            // Get a UTF-32 encoding by name.
            Encoding encoding = Encoding.GetEncoding("utf-8");
            using (var str = new StreamReader(stream, encoding))
            {
                try
                {
                    xmldoc.Load(str);
                }
                catch (Exception)
                {

                    // continue;
                }
            }

            // check the url that has been loaded
            var categoryList = xmldoc.SelectNodes("/scores/category");
            if (categoryList != null)
            {
                //get the odds from the xml
                foreach (XmlNode category in categoryList)
                {
                    var testGame = new Game();
                    var match = category.ChildNodes[0];
                    if (match.Attributes != null)
                    {
                        testGame.MatchNo = match.Attributes["id"].InnerText;
                        testGame.StartDate = match.Attributes["formatted_date"].InnerText;
                        testGame.StartTime = match.Attributes["time"].InnerText;
                        testGame.League = match.Attributes["league"] != null || match.Attributes["league"].InnerText == ""
                                   ? match.Attributes["league"].InnerText
                                   : "FailedLeague";
                       
                        var odds = match.LastChild;
                        foreach (XmlNode odd in odds)
                        {
                            if (odd.Attributes != null)
                                switch (odd.Attributes["name"].InnerText)
                                {
                                    case "Fulltime Result":
                                        testGame.FullTimeOdds = new FullTimeOdds();
                                        foreach (XmlNode FTO in odd.ChildNodes)
                                        {
                                            if (FTO.Attributes != null)
                                            {
                                                if (FTO.Attributes["extravalue"].InnerText == "1")
                                                {
                                                    var homeWin = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.HomeWins = homeWin;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "X")
                                                {
                                                    var draw = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.Draw = draw;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "2")
                                                {
                                                    var awayWin = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.AwayWins = awayWin;
                                                }
                                            }
                                        }
                                        break;
                                    case "Match Goals":
                                        testGame.UnderOverOdds = new UnderOverOdds();
                                        foreach (XmlNode game in odd.ChildNodes)
                                        {
                                            if (game.Attributes != null)
                                            {
                                                if (game.Attributes["name"].InnerText.Contains("Over"))
                                                {
                                                    var over = game.Attributes["odd"].InnerText;
                                                    testGame.UnderOverOdds.Over = over;
                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
                                                }
                                                if (game.Attributes["name"].InnerText.Contains("Under"))
                                                {
                                                    var under = game.Attributes["odd"].InnerText;
                                                    testGame.UnderOverOdds.Under = under;
                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
                                                }
                                            }
                                        }
                                        break;
                                    case "Next Goal":
                                        testGame.NextGoal = new NextGoal();
                                        foreach (XmlNode FTO in odd.ChildNodes)
                                        {
                                            if (FTO.Attributes != null)
                                            {
                                                if (FTO.Attributes["extravalue"].InnerText == "1")
                                                {
                                                    var homeScores = FTO.Attributes["odd"].InnerText;
                                                    testGame.NextGoal.HomeScores = homeScores;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "X")
                                                {
                                                    var draw = FTO.Attributes["odd"].InnerText;
                                                    testGame.NextGoal.Draw = draw;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "2")
                                                {
                                                    var awayScores = FTO.Attributes["odd"].InnerText;
                                                    testGame.NextGoal.AwayScores = awayScores;
                                                }
                                            }
                                        }
                                        break;
                                    case "Double Chance":
                                        testGame.DoubleChance = new DoubleChance();
                                        foreach (XmlNode FTO in odd.ChildNodes)
                                        {
                                            if (FTO.Attributes != null)
                                            {
                                                if (FTO.Attributes["extravalue"].InnerText == "1X")
                                                {
                                                    var homeWinsOrDraw = FTO.Attributes["odd"].InnerText;
                                                    testGame.DoubleChance.HomeWinsOrDraw = homeWinsOrDraw;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "12")
                                                {
                                                    var homeOrAwayWins = FTO.Attributes["odd"].InnerText;
                                                    testGame.DoubleChance.HomeWinsOrAwayWins = homeOrAwayWins;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "X2")
                                                {
                                                    var awayWinsOrDraw = FTO.Attributes["odd"].InnerText;
                                                    testGame.DoubleChance.AwayWinsOrDraw = awayWinsOrDraw;
                                                }
                                            }
                                        }
                                        break;

                                }

                            _gamesforLiveOdds.TryAdd(testGame.MatchNo, testGame);
                        }
                    }
                }

            }
            return _gamesforLiveOdds.Values;
        }
        #endregion

        /// <summary>
        /// Updates the Db
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private  bool  TryUpdateGame(Game game)
        {
            
           // check  whether To save this game the db or not
            var matchInList =isMatchAlreadySavedToDb(game.MatchNo);
            if (matchInList.IsAlreadysaved)
            {
             return true;            
            }

            //Todo: update the records in the DB
  #region trySaveMatchesToDb
            LiveMatch liveMatchToSave;
           // List<LiveMatches> liveMatchesList=new List<LiveMatches>();
            Match match;
           
           // List<Match> matches=new List<Match>();
           // foreach (var game in allGames)
           // {
            try {

                Country country;
                int _HomeTeamId = 0;
                int _AwayTeamId = 0;
                int _countryId = 0;
                var league = BetDatabase.Leagues.SingleOrDefault(c => c.LeagueName == game.League);
                if (league != null)
                {
                    country = BetDatabase.Countries.SingleOrDefault(c => c.CountryId == league.CountryId);
                    
                    if (country == null)
                    {
                        country = new Country
                        {
                            CountryName = "Unknown"
                        };
                        BetDatabase.Countries.AddOrUpdate(c => c.CountryName, country);
                        BetDatabase.SaveChangesAsync();
                        
                    }
                    _countryId = country.CountryId;
                }
                else {
                    country = BetDatabase.Countries.SingleOrDefault(c => c.CountryName == "Unknown");
                 
                    if (country == null)
                    {
                        country = new Country
                        {
                            CountryName = "Unknown"
                        };
                        BetDatabase.Countries.AddOrUpdate(c => c.CountryName, country);
                        BetDatabase.SaveChanges();                       
                    }
                    _countryId = country.CountryId;
                    BetDatabase.Leagues.AddOrUpdate(l => l.LeagueName, new League
                    {
                        LeagueName = game.League,
                        CountryId = _countryId,
                    });
                    BetDatabase.SaveChanges();           
                
                
                }
               
                Team _homeTeam;
                if (game.LocalTeam != null)
                {
                    _homeTeam = new Team
                    {
                        TeamName = game.LocalTeam,
                        CountryId = _countryId
                    };
                    BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _homeTeam);
                     BetDatabase.SaveChanges();
                    _HomeTeamId = _homeTeam.TeamId;
                }

                Team _awayTeam;
                if (game.AwayTeam != null)
                {
                    _awayTeam = new Team
                    {
                        TeamName = game.AwayTeam,
                       CountryId = _countryId
                    };
                    BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _awayTeam);
                     BetDatabase.SaveChanges();
                    _AwayTeamId = _awayTeam.TeamId;
                }
                match = new Match
                 {
                     StartTime = getFormattedStartDate(game.StartDateToSave, game.StartTime),
                     AwayTeamId = _AwayTeamId,
                     HomeTeamId = _HomeTeamId,
                     BetServiceMatchNo = game.ShortCode,
                     //ShortMatchCode = new ShortMatchCode(),// Convert.ToInt32(game.ShortCode),
                     League = game.League,
                     RegistrationDate = DateTime.Now,
                     ResultStatus=0,
                     GameStatus = double.Parse(formatMinutes(game.Minutes)) > 0 ? "Started" : "Not Started",

                 };

               // matches.Add(match);
                liveMatchToSave = new LiveMatch
                {
                    LiveMatchNo = int.Parse(game.MatchNo),
                    BetServiceMatchNo = game.ShortCode,
                    SetDate = DateTime.Now
                };
               // liveMatchesList.Add(liveMatchToSave);
                //we could have used AddOrUpdate btu since we always check if match already exits in memory,its useless, unless need be later

  #endregion
               // matches.ForEach(m => BetDatabase.Matches.Add(m));
                try {
                    BetDatabase.Matches.AddOrUpdate(match);
                    BetDatabase.SaveChanges();

                    BetDatabase.LiveMatches.AddOrUpdate(liveMatchToSave);
                      BetDatabase.SaveChanges();
                }
                catch (Exception ex){
                    matchInList.IsAlreadysaved = false;
                    return false;                
                }
                
                //save the live macthes MatchNo int the LiveMatches table
               
               
              
           // }
            ////_games.Clear();
            //_games.TryAdd(game.MatchNo, game);
                //update the matchcode list if save is successfull
                 matchInList.IsAlreadysaved = true;
                 return true;
                }
            catch(Exception ex){
                //update the matchcode list if save is successfull
                matchInList.IsAlreadysaved = false ;
                return false;
            }

            //if youve got this far, dont broadcast the match
            return false;
        }

        private string formatMinutes(string minutes)
        {
            char[] del = { ':' };
            var stdate = minutes.Split(del);
            var stDateTime = stdate[0] + "." + stdate[1];
                    return stDateTime;       
        }


        private void BroadcastLiveGame(Game game)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveGameHub>();
            if (hubContext != null)
            {

                hubContext.Clients.All.updateGame(game);
            }

        }

        public string TestString()
        {
            return "Connected to the hub.....";
        }

        #region Support Methods
        private void setInitialStaticCodeFromDb()
        {
            //maxShortCodeInDb = (int)(from lm in BetDatabase.LiveMatches select lm.BetServiceMatchNo).Max();
            //if (maxShortCodeInDb != 0 || maxShortCodeInDb != null)
            //{
            //    shortMatchCode = maxShortCodeInDb;
            //}
        }



        private void setInitialGamesFromDb()
        {
            //var savedLiveMatches = BetDatabase.LiveMatches.Where(lm => lm.SetDate.Date == DateTime.Now.Date);
            //if (savedLiveMatches != null)
            //{
            //    foreach (var liveMatch in savedLiveMatches)
            //    {
            //        var savedMatch = new SavedMatch
            //        {
            //            ShortMatchCode = (int)liveMatch.BetServiceMatchNo,
            //            IsAlreadysaved = true,
            //        };
            //        _shortMatchCodes.Add(liveMatch.LiveMatchNo.ToString(), savedMatch);
            //    }
            //}
        }
        #endregion
    }

    #region stub controller to load lives from xml files
    public class MyController : CustomController
    {
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveScore = new ConcurrentDictionary<string, Game>();
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveOdds = new ConcurrentDictionary<string, Game>();
        #region GetGamesScoresFromXml
        public async Task<IEnumerable<Game>> GetGamesScoresFromXml(int mockXmlFileExt)
        {
            var xmldoc = new XmlDocument();
            _gamesforLiveScore.Clear();
            // xmldoc.Load(Server.MapPath("~/soccerInPlayScores.xml"));
            try
            {
                //StreamReader strReader = new StreamReader(Server.MapPath("~/Xml/england_shedule.xml"));
                // var stream= await strReader.ReadToEndAsync();

                xmldoc.Load("H:/MyWorks/BettingAppDirectory11/BetLive/Xml/soccerInPlayScores" + mockXmlFileExt.ToString() + ".xml");
            }
            catch (Exception e)
            {
                e.StackTrace.ToString();
            }
            var categoryList = xmldoc.SelectNodes("/scores/match");


            if (categoryList == null) return _gamesforLiveScore.Values; // if the stream is null return the games the way they are


            // check the url that has been loaded

            if (categoryList != null)
                foreach (XmlNode category in categoryList)
                {
                    var match = category;
                    var teams = match.ChildNodes;
                    var testGame = new Game();

                    if (match.Attributes != null)
                    {
                        testGame.MatchNo = match.Attributes["id"].InnerText;
                        testGame.Minutes = match.Attributes["minute"].InnerText;
                       // testGame.StartDate = DateTime.Now.Date.ToString();
                    }

                    foreach (XmlNode team in teams)
                    {
                        var local = team.Name;
                        if (team.Name == "localteam")
                        {

                            var localTeamAttributes = team.Attributes;
                            if (localTeamAttributes != null)
                            {
                                testGame.LocalTeam = localTeamAttributes["name"].InnerText;
                                testGame.LocalTeamScore = localTeamAttributes["score"].InnerText == "" ? "?" : localTeamAttributes["score"].InnerText;
                            }
                        }
                        else
                        {
                            var awayTeamattributes = team.Attributes;
                            if (awayTeamattributes != null)
                            {
                                testGame.AwayTeam = awayTeamattributes["name"].InnerText;
                                testGame.AwayTeamScore = awayTeamattributes["score"].InnerText == "" ? "?" : awayTeamattributes["score"].InnerText;
                            }
                        }

                        _gamesforLiveScore.TryAdd(testGame.MatchNo, testGame);
                    }


                }
            return _gamesforLiveScore.Values;
        }

        #endregion
        #region GetGamesOddsFromXml
        public async Task<IEnumerable<Game>> GetGamesOddsFromXml(int mockXmlFileExt)
        {




            var xmldoc = new XmlDocument();
            xmldoc.Load("H:/MyWorks/BettingAppDirectory11/BetLive/Xml/soccerInPlayLliveOdds" + mockXmlFileExt + ".xml");
            var categoryList = xmldoc.SelectNodes("/scores/category");
            if (categoryList == null) return _gamesforLiveOdds.Values; // if the stream is null return the games the way they are

            // Get a UTF-32 encoding by name.


            // check the url that has been loaded

            if (categoryList != null)
            {
                //get the odds from the xml
                foreach (XmlNode category in categoryList)
                {
                    var testGame = new Game();
                    var match = category.ChildNodes[0];
                    if (match.Attributes != null)
                    {
                        testGame.MatchNo = match.Attributes["id"].InnerText;
                        testGame.StartDate = match.Attributes["formatted_date"].InnerText;
                        testGame.StartTime = match.Attributes["time"].InnerText;
                        testGame.League = match.Attributes["league"] != null || match.Attributes["league"].InnerText==""
                                  ? match.Attributes["league"].InnerText
                                  : "FailedLeague";
                       // testGame.StartDate = match.Attributes["date"].InnerText;
                        var odds = match.LastChild;
                        foreach (XmlNode odd in odds)
                        {
                            if (odd.Attributes != null)
                                switch (odd.Attributes["name"].InnerText)
                                {
                                    case "Fulltime Result":
                                        testGame.FullTimeOdds = new FullTimeOdds();
                                        foreach (XmlNode FTO in odd.ChildNodes)
                                        {
                                            if (FTO.Attributes != null)
                                            {
                                                if (FTO.Attributes["extravalue"].InnerText == "1")
                                                {
                                                    var homeWin = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.HomeWins = homeWin;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "X")
                                                {
                                                    var draw = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.Draw = draw;
                                                }
                                                if (FTO.Attributes["extravalue"].InnerText == "2")
                                                {
                                                    var awayWin = FTO.Attributes["odd"].InnerText;
                                                    testGame.FullTimeOdds.AwayWins = awayWin;
                                                }
                                            }
                                        }
                                        break;
                                    case "Match Goals":
                                        testGame.UnderOverOdds = new UnderOverOdds();
                                        foreach (XmlNode game in odd.ChildNodes)
                                        {
                                            if (game.Attributes != null)
                                            {
                                                if (game.Attributes["name"].InnerText.Contains("Over"))
                                                {
                                                    var over = game.Attributes["odd"].InnerText;
                                                    testGame.UnderOverOdds.Over = over;
                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
                                                }
                                                if (game.Attributes["name"].InnerText.Contains("Under"))
                                                {
                                                    var under = game.Attributes["odd"].InnerText;
                                                    testGame.UnderOverOdds.Under = under;
                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
                                                }
                                            }
                                        }
                                        break;

                                }

                            _gamesforLiveOdds.TryAdd(testGame.MatchNo, testGame);
                        }
                    }
                }

            }
            return _gamesforLiveOdds.Values;
        }
        #endregion
        public List<int?> setInitialStaticCodeFromDb()
        {
          var  maxShortCodeInDb = (from lm in BetDatabase.LiveMatches select lm.BetServiceMatchNo).ToList<int?>();
            //if (maxShortCodeInDb != 0 || maxShortCodeInDb != null)
            //{
            //   var shortMatchCode = maxShortCodeInDb;
            //   return shortMatchCode;
            //}
          return maxShortCodeInDb;
        }



        public IDictionary<string, SavedMatch> setInitialGamesFromDb()
        {
               IDictionary<string, SavedMatch> _shortMatchCodes = new  Dictionary<string, SavedMatch>();
            var savedLiveMatches = BetDatabase.LiveMatches.Where(lm => lm.SetDate.Date == DateTime.Now.Date);
            if (savedLiveMatches != null)
            {
                foreach (var liveMatch in savedLiveMatches)
                {
                    var savedMatch = new SavedMatch
                    {
                        ShortMatchCode = (int)liveMatch.BetServiceMatchNo,
                        IsAlreadysaved = true,
                    };
                    _shortMatchCodes.Add(liveMatch.LiveMatchNo.ToString(), savedMatch);
                }
            }
            return _shortMatchCodes;
        }
    }

    #endregion
    #region Support Classes
    public class LiveBetSource
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Game
    {
        public int ShortCode { get; set; }
        public string MatchNo { get; set; }
        public string League { set; get; }
        public string StartTime { get; set; }
        public string Minutes { get; set; }
        public string StartDate { get; set; }
        public string LocalTeam { get; set; }
        public string AwayTeam { get; set; }
        public string LocalTeamScore { get; set; }
        public string AwayTeamScore { get; set; }
        public string StartDateToSave { get; set; }
        public FullTimeOdds FullTimeOdds { get; set; }
        public UnderOverOdds UnderOverOdds { get; set; }
        public RestofMatch RestOfMatch { get; set; }
        public NextGoal NextGoal { get; set; }
        public DoubleChance DoubleChance { get; set; }
    }

    public class FullTimeOdds
    {
        public string HomeWins { get; set; }
        public string Draw { get; set; }
        public string AwayWins { get; set; }
    }

    public class UnderOverOdds
    {
        public string Under { get; set; }
        public string Over { get; set; }
        public string ExtraValue { get; set; }
    }
    public class RestofMatch
    {
        public string HomeWins { get; set; }
        public string Draw { get; set; }
        public string AwayWins { get; set; }
    }
    public class NextGoal
    {
        public string HomeScores { get; set; }
        public string Draw { get; set; }
        public string AwayScores { get; set; }
    }
    public class DoubleChance
    {
        public string HomeWinsOrDraw { get; set; }
        public string HomeWinsOrAwayWins { get; set; }
        public string AwayWinsOrDraw { get; set; }
    }
    #endregion




}