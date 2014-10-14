//using System.Timers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace WebUI.Hubs
{
    [HubName("liveBetHub")]
    public class LiveGameHub : Hub
    {
        private readonly IList<LiveBetSource> _liveBetUrls;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveScore = new ConcurrentDictionary<string, Game>();
        private readonly ConcurrentDictionary<string, Game> _gamesforLiveOdds = new ConcurrentDictionary<string, Game>();
        private Timer _timer;
        private volatile bool _updatingGame = false;
        private readonly object _updateGameLock = new object();
        private readonly Random _updateOrNotRandom = new Random();

        public LiveGameHub()
        {
            _gamesforLiveScore.Clear();
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

        public async Task<IEnumerable<Game>> GetAllGames()
        {
            var scores = await GetGamesScores();
            var odds = await GetGamesOdds();
            var allGames = (from gamescore in scores
                            from gameodds in odds
                            where gamescore.MatchNo == gameodds.MatchNo
                            select new Game
                            {
                                MatchNo = gamescore.MatchNo,
                                Minutes = gamescore.Minutes,
                                AwayTeam = gamescore.AwayTeam,
                                LocalTeam = gamescore.LocalTeam,
                                AwayTeamScore = gamescore.AwayTeamScore,
                                LocalTeamScore = gamescore.LocalTeamScore,
                                FullTimeOdds = gameodds.FullTimeOdds,
                                UnderOverOdds = gameodds.UnderOverOdds
                            }).ToList();
            _timer = new Timer(UpdateGames, null, _updateInterval, _updateInterval);
            return allGames;
        }

        private async void UpdateGames(object state)
        {

            try
            {
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
                    }

                    foreach (XmlNode team in teams)
                    {
                        var local = team.Name;
                        if (team.Name=="localteam")
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
                                            if (FTO.Attributes!=null)
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

        /// <summary>
        /// Updates the Db
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private bool TryUpdateGame(Game game)
        {
            // Randomly choose whether to update this game or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }

            //Todo: update the records in the DB

            ////_games.Clear();
            //_games.TryAdd(game.MatchNo, game);
            return true;
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


    }


    public class LiveBetSource
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Game
    {
        public string MatchNo { get; set; }
        public string Minutes { get; set; }
        public string LocalTeam { get; set; }
        public string AwayTeam { get; set; }
        public string LocalTeamScore { get; set; }
        public string AwayTeamScore { get; set; }
        public FullTimeOdds FullTimeOdds { get; set; }
        public UnderOverOdds UnderOverOdds { get; set; }
        public RestofMatch RestofMatch { get; set; }
        public NextGoal NextGoal { get; set; }
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
        public string None { get; set; }
        public string AwayScores { get; set; }
    }
}
