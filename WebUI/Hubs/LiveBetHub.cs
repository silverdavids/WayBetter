//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Data.Entity.Migrations;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml;
//using Domain.Models.Concrete;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using WebUI.DataAccessLayer;


//namespace WebUI.Hubs
//{
//    [HubName("liveBetHub")]
//    public class LiveGameHub : Hub
//    {
//        private readonly IList<LiveBetSource> _liveBetUrls;
//        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
//        private readonly ConcurrentDictionary<string, Game> _gamesforLiveScore = new ConcurrentDictionary<string, Game>();
//        private readonly ConcurrentDictionary<string, Game> _gamesforLiveOdds = new ConcurrentDictionary<string, Game>();
//        private Timer _timer;
//        private volatile bool _updatingGame = false;
//        private readonly object _updateGameLock = new object();
//        private readonly Random _updateOrNotRandom = new Random();
//        private IList<ShortMatchCode> _shortMatchCodes;
//        private ApplicationDbContext _dbContext;
//        private Team _awayTeam, _homeTeam;

//        // ToDo: Add short code
//        public LiveGameHub()
//        {
//            _gamesforLiveScore.Clear();
//            //_shortMatchCodes = BetDatabase.ShortMatchCodes.ToList();

//        }

//        public async Task<IEnumerable<Game>> GetAllGames()
//        {
//            var scores = await GetGamesScores();
//            var odds = await GetGamesOdds();
//            var allGames = (from gamescore in scores
//                            join gameodds in odds
//                            on gamescore.MatchNo equals gameodds.MatchNo
//                            //&& Convert.ToInt32(gamescore.MatchNo) == shortCode.MatchNo
//                            select new Game
//                            {
//                                MatchNo = gamescore.MatchNo,
//                                //ShortCode = gamescore.ShortCode,
//                                Minutes = gamescore.Minutes,
//                                AwayTeam = gamescore.AwayTeam,
//                                LocalTeam = gamescore.LocalTeam,
//                                AwayTeamScore = gamescore.AwayTeamScore,
//                                LocalTeamScore = gamescore.LocalTeamScore,
//                                FullTimeOdds = gameodds.FullTimeOdds,
//                                UnderOverOdds = gameodds.UnderOverOdds,
//                                DoubleChance = gameodds.DoubleChance,
//                                NextGoal = gameodds.NextGoal
//                            }).ToList();

//            IList<Game> liveGames = new List<Game>();

//            foreach (var game in allGames)
//            {
//                if (_shortMatchCodes.All(x => x.MatchNo != Convert.ToInt32(game.MatchNo))) continue;
//                var shortMatchCode = _shortMatchCodes.FirstOrDefault(g => g.MatchNo == Convert.ToInt32(game.MatchNo));
//                if (shortMatchCode == null) continue;
//                game.ShortCode = shortMatchCode.ShortCode;
//                liveGames.Add(game);
//            }
//            _timer = new Timer(UpdateGames, null, _updateInterval, _updateInterval);
//            //return liveGames;
//            return allGames;
//        }

//        private async void UpdateGames(object state)
//        {

//            try
//            {
//                var updatedGames = await GetAllGames();
//                lock (_updateGameLock)
//                {
//                    if (!_updatingGame)
//                    {
//                        var updates = updatedGames as Game[] ?? updatedGames.ToArray();
//                        BroadcastNewGames(updates);
//                        _updatingGame = true;
//                        foreach (var game in updates)
//                        {
//                            if (TryUpdateGame(game))
//                            {
//                                //broadcast on the UI
//                                BroadcastLiveGame(game);
//                            }
//                        }
//                        _updatingGame = false;
//                    }
//                }
//            }
//            catch (Exception e)
//            {


//            }


//        }

//        public async Task<IEnumerable<Game>> GetGamesScores()
//        {
//            var scoresUrl = new LiveBetSource
//            {
//                Name = "Scores",
//                Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/inplay"
//            };

//            _gamesforLiveScore.Clear();
//            var req = (HttpWebRequest)WebRequest.Create(scoresUrl.Url);
//            var res = (HttpWebResponse)req.GetResponse();
//            var stream = res.GetResponseStream();
//            var xmldoc = new XmlDocument();
//            if (stream == null) return _gamesforLiveScore.Values; // if the stream is null return the games the way they are

//            // Get a UTF-32 encoding by name.
//            Encoding encoding = Encoding.GetEncoding("utf-8");
//            using (var str = new StreamReader(stream, encoding))
//            {
//                try
//                {
//                    xmldoc.Load(str);
//                }
//                catch (Exception)
//                {

//                    // continue;
//                }
//            }

//            // check the url that has been loaded
//            var categoryList = xmldoc.SelectNodes("/scores/match");
//            if (categoryList != null)
//                foreach (XmlNode category in categoryList)
//                {
//                    var match = category;
//                    var teams = match.ChildNodes;
//                    var testGame = new Game();

//                    if (match.Attributes != null)
//                    {
//                        testGame.MatchNo = match.Attributes["id"].InnerText;
//                        testGame.Minutes = match.Attributes["minute"].InnerText.Substring(0, 2) + "'";
//                    }

//                    foreach (XmlNode team in teams)
//                    {
//                        var local = team.Name;
//                        if (team.Name == "localteam")
//                        {

//                            var localTeamAttributes = team.Attributes;
//                            if (localTeamAttributes != null)
//                            {
//                                testGame.LocalTeam = localTeamAttributes["name"].InnerText;
//                                testGame.LocalTeamScore = localTeamAttributes["score"].InnerText == "" ? "?" : localTeamAttributes["score"].InnerText;
//                            }
//                        }
//                        if (team.Name == "awayteam")
//                        {
//                            var awayTeamattributes = team.Attributes;
//                            if (awayTeamattributes != null)
//                            {
//                                testGame.AwayTeam = awayTeamattributes["name"].InnerText;
//                                testGame.AwayTeamScore = awayTeamattributes["score"].InnerText == "" ? "?" : awayTeamattributes["score"].InnerText;
//                            }
//                        }

//                        _gamesforLiveScore.TryAdd(testGame.MatchNo, testGame);
//                    }


//                }
//            return _gamesforLiveScore.Values;
//        }

//        public async Task<IEnumerable<Game>> GetGamesOdds()
//        {
//            var oddSource = new LiveBetSource
//            {
//                Name = "Odds",
//                Url = "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/lines/soccer-inplay"
//            };

//            _gamesforLiveOdds.Clear();
//            var req = (HttpWebRequest)WebRequest.Create(oddSource.Url);
//            var res = (HttpWebResponse)req.GetResponse();
//            var stream = res.GetResponseStream();
//            var xmldoc = new XmlDocument();
//            if (stream == null) return _gamesforLiveOdds.Values; // if the stream is null return the games the way they are

//            // Get a UTF-32 encoding by name.
//            Encoding encoding = Encoding.GetEncoding("utf-8");
//            using (var str = new StreamReader(stream, encoding))
//            {
//                try
//                {
//                    xmldoc.Load(str);
//                }
//                catch (Exception)
//                {

//                    // continue;
//                }
//            }

//            // check the url that has been loaded
//            var categoryList = xmldoc.SelectNodes("/scores/category");
//            if (categoryList != null)
//            {
//                //get the odds from the xml
//                foreach (XmlNode category in categoryList)
//                {
//                    var testGame = new Game();
//                    var match = category.ChildNodes[0];
//                    if (match.Attributes != null)
//                    {
//                        testGame.MatchNo = match.Attributes["id"].InnerText;
//                        var odds = match.LastChild;
//                        foreach (XmlNode odd in odds)
//                        {
//                            if (odd.Attributes != null)
//                                switch (odd.Attributes["name"].InnerText)
//                                {
//                                    case "Fulltime Result":
//                                        testGame.FullTimeOdds = new FullTimeOdds();
//                                        foreach (XmlNode FTO in odd.ChildNodes)
//                                        {
//                                            if (FTO.Attributes != null)
//                                            {
//                                                if (FTO.Attributes["extravalue"].InnerText == "1")
//                                                {
//                                                    var homeWin = FTO.Attributes["odd"].InnerText;
//                                                    testGame.FullTimeOdds.HomeWins = homeWin;
//                                                }
//                                                if (FTO.Attributes["extravalue"].InnerText == "X")
//                                                {
//                                                    var draw = FTO.Attributes["odd"].InnerText;
//                                                    testGame.FullTimeOdds.Draw = draw;
//                                                }
//                                                if (FTO.Attributes["extravalue"].InnerText == "2")
//                                                {
//                                                    var awayWin = FTO.Attributes["odd"].InnerText;
//                                                    testGame.FullTimeOdds.AwayWins = awayWin;
//                                                }
//                                            }
//                                        }
//                                        break;
//                                    case "Match Goals":
//                                        testGame.UnderOverOdds = new UnderOverOdds();
//                                        foreach (XmlNode game in odd.ChildNodes)
//                                        {
//                                            if (game.Attributes != null)
//                                            {
//                                                if (game.Attributes["name"].InnerText.Contains("Over"))
//                                                {
//                                                    var over = game.Attributes["odd"].InnerText;
//                                                    testGame.UnderOverOdds.Over = over;
//                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
//                                                }
//                                                if (game.Attributes["name"].InnerText.Contains("Under"))
//                                                {
//                                                    var under = game.Attributes["odd"].InnerText;
//                                                    testGame.UnderOverOdds.Under = under;
//                                                    testGame.UnderOverOdds.ExtraValue = game.Attributes["extravalue"].InnerText;
//                                                }
//                                            }
//                                        }
//                                        break;
//                                    case "Next Goal":
//                                        testGame.NextGoal = new NextGoal();
//                                        foreach (XmlNode FTO in odd.ChildNodes)
//                                        {
//                                            if (FTO.Attributes != null)
//                                            {
//                                                if (FTO.Attributes["extravalue"].InnerText == "1")
//                                                {
//                                                    var homeScores = FTO.Attributes["odd"].InnerText;
//                                                    testGame.NextGoal.HomeScores = homeScores;
//                                                }
//                                                if (FTO.Attributes["extravalue"].InnerText == "X")
//                                                {
//                                                    var draw = FTO.Attributes["odd"].InnerText;
//                                                    testGame.NextGoal.Draw = draw;
//                                                }
//                                                if (FTO.Attributes["extravalue"].InnerText == "2")
//                                                {
//                                                    var awayScores = FTO.Attributes["odd"].InnerText;
//                                                    testGame.NextGoal.AwayScores = awayScores;
//                                                }
//                                            }
//                                        }
//                                        break;
//                                    case "Double Chance":
//                                        testGame.DoubleChance = new DoubleChance();
//                                        foreach (XmlNode DC in odd.ChildNodes)
//                                        {
//                                            if (DC.Attributes != null)
//                                            {
//                                                if (DC.Attributes["extravalue"].InnerText == "1X")
//                                                {
//                                                    var homeOrDraw = DC.Attributes["odd"].InnerText;
//                                                    testGame.DoubleChance.HomeOrDraw = homeOrDraw;
//                                                }
//                                                if (DC.Attributes["extravalue"].InnerText == "X2")
//                                                {
//                                                    var awayOrDraw = DC.Attributes["odd"].InnerText;
//                                                    testGame.DoubleChance.AwayOrDraw = awayOrDraw;
//                                                }
//                                                if (DC.Attributes["extravalue"].InnerText == "12")
//                                                {
//                                                    var homeOrAway = DC.Attributes["odd"].InnerText;
//                                                    testGame.DoubleChance.HomeOrAway = homeOrAway;
//                                                }
//                                            }
//                                        }
//                                        break;

//                                }

//                            _gamesforLiveOdds.TryAdd(testGame.MatchNo, testGame);
//                        }
//                    }
//                }

//            }
//            return _gamesforLiveOdds.Values;
//        }

//        /// <summary>
//        /// Updates the Db
//        /// </summary>
//        /// <param name="game"></param>
//        /// <returns></returns>
//        private bool TryUpdateGame(Game game)
//        {
//            // Randomly choose whether to update this game or not
//            var r = _updateOrNotRandom.NextDouble();
//            if (r > .1)
//            {
//                return false;
//            }
//            //DateTime matchTime = DateTime.Now.AddHours(-2);
//            //_homeTeam = BetDatabase.Teams.Single(t => t.TeamName==game.LocalTeam);        
//            //_awayTeam =BetDatabase.Teams.Single(t => t.TeamName==game.LocalTeam);
//            //Match playingMatch =
//            //   BetDatabase.Matches.Single(m => m.StartTime> matchTime && m.HomeTeamId == _homeTeam.TeamId && m.AwayTeamId == _awayTeam.TeamId);
//            //if (playingMatch == null)
//            //{
//            //    return false;
//            //}
//            //else
//            //{
//            //    game.ShortCode = playingMatch.ShortMatchCode.ShortCode;
//            //}
//            //Todo: update the records in the DB

//            ////_games.Clear();
//            //_games.TryAdd(game.BetServiceMatchNo, game);
        
//            return true;
//        }

//        private void BroadcastLiveGame(Game game)
//        {
//            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveGameHub>();
//            if (hubContext != null)
//            {

//                hubContext.Clients.All.updateGame(game);
//            }
//        }

//        // ToDo: re-implement this block
//        private void BroadcastNewGames(IEnumerable<Game> games)
//        {
//            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveGameHub>();
//            if (hubContext != null)
//            {

//                hubContext.Clients.All.getUpdatedGames(games);
//            }
//        }
//        public string TestString()
//        {
//            return "Connected to the hub.....";
//        }

//        private ApplicationDbContext BetDatabase
//        {
//            get { return _dbContext ?? (_dbContext = new ApplicationDbContext()); }
//            set
//            {
//                _dbContext = value;
//            }
//        }


//    }


//    public class LiveBetSource
//    {
//        public string Name { get; set; }
//        public string Url { get; set; }
//    }

//    public class Game
//    {
//        public string MatchNo { get; set; }
//        public int ShortCode { get; set; }
//        public string Minutes { get; set; }
//        public string LocalTeam { get; set; }
//        public string AwayTeam { get; set; }
//        public string LocalTeamScore { get; set; }
//        public string AwayTeamScore { get; set; }
//        public FullTimeOdds FullTimeOdds { get; set; }
//        public UnderOverOdds UnderOverOdds { get; set; }
//        public DoubleChance DoubleChance { get; set; }
//        public NextGoal NextGoal { get; set; }
//    }

//    public class FullTimeOdds
//    {
//        public string HomeWins { get; set; }
//        public string Draw { get; set; }
//        public string AwayWins { get; set; }
//    }

//    public class UnderOverOdds
//    {
//        public string Under { get; set; }
//        public string Over { get; set; }
//        public string ExtraValue { get; set; }
//    }
//    public class DoubleChance
//    {
//        public string HomeOrDraw { get; set; }
//        public string AwayOrDraw { get; set; }
//        public string HomeOrAway { get; set; }
//    }
//    public class NextGoal
//    {
//        public string HomeScores { get; set; }
//        public string Draw { get; set; }
//        public string AwayScores { get; set; }
//    }


//}
