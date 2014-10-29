using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using Domain.Models.Concrete;
using WebGrease.Css.Extensions;
using WebUI.Helpers;
using Domain.Models.ViewModels;
using System.Web;
using System.ComponentModel;


namespace WebUI.Controllers
{ 
    public class FixtureController : CustomController
    {
        private Team _awayTeam, _homeTeam;
        // GET: Fixture
        #region urls
        private readonly string[] _urls =
        {
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/england_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/eurocups_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/qatar_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/malta_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/iran_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/africa_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/argentina_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/asia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/australia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/austria_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/belarus_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/belgium_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/brazil_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/bulgaria_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/chile_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/china_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/colombia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/concacaf_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/costarica_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/croatia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/cyprus_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/czechia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/denmark_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/egypt_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/england_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/equador_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/estonia_shedule?odds=bet365",   
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/finland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/france_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/germany_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/greece_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/holland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/hungary_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/iceland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/ireland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/israel_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/italy_shedule?odds=bet365",     
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/japan_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/korea_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/latvia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/lithuania_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/malta_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/mexico_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/moldova_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/Iran_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/norway_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/paraguay_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/peru_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/poland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/portugal_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/romania_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/russia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/saudi_arabia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/scotland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/serbia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/singapore_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/slovakia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/slovenia_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/southafrica_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/southamerica_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/spain_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/sweden_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/switzerland_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/turkey_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/uae_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/ukraine_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/uruguay_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/usa_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/venezuela_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/wales_shedule?odds=bet365",
            "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/worldcup_shedule?odds=bet365"
        };
        #endregion
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult AddOdds(int ? id=0)
        {
            ViewBag.MatchId = id;
            return View();
        }

        public ActionResult AddMatchOdds()
        {

            return View(new MatchOdd());
        }
    
        public async Task<ActionResult> LoadFixture()
        {
            var count= await UploadGames();
            ViewBag.Message = count + "games loaded";
            return View();
        }

        public async Task<int> UploadGamesFromDani()
        {
            var matches = new List<Match>();
            var matchodds = new List<MatchOdd>();
            var xmldoc = new XmlDocument();
            xmldoc.Load(Server.MapPath("~/gbl/GetSportEventsItalyEngland.xml"));
            var oddServiceNode = xmldoc.ChildNodes[1];
            var resultsNode = oddServiceNode.ChildNodes[1];

            foreach (XmlNode eventNode in resultsNode)
            {
                var eventId = eventNode.ChildNodes[0].InnerText;
                var unformattedStartDate = eventNode.ChildNodes[1].InnerText;
                var startDateTime = DateTime.Parse(unformattedStartDate);

                //var sportIdNode = eventNode.ChildNodes[2];
                //var sportId = sportIdNode.InnerText;
                //var sportName = (sportIdNode.Attributes != null) ? sportIdNode.Attributes["Name"].InnerText : String.Empty;

                var locationIdNode = eventNode.ChildNodes[4];
                //var locationId = locationIdNode.InnerText;
                var locationName = (locationIdNode.Attributes != null) ? locationIdNode.Attributes["Name"].InnerText : String.Empty;

                var country = new Country
                {
                    CountryName = locationName
                };

                BetDatabase.Countries.AddOrUpdate(c => c.CountryName, country);
                await BetDatabase.SaveChangesAsync();

                var leagueIdNode = eventNode.ChildNodes[3];
                //var leagueId = leagueIdNode.InnerText;
                var leagueName = (leagueIdNode.Attributes != null) ? leagueIdNode.Attributes["Name"].InnerText : String.Empty;

                var league = new League
                {
                    LeagueName = leagueName,
                    CountryId = country.CountryId
                };

                BetDatabase.Leagues.AddOrUpdate(key => new { key.LeagueName }, league);
                await BetDatabase.SaveChangesAsync();
                

                //var lastUpdateTime = eventNode.ChildNodes[6].InnerText;
                var homeTeamNode = eventNode.ChildNodes[8];
                var homeTeam = (homeTeamNode.Attributes != null) ? homeTeamNode.Attributes["Name"].InnerText : String.Empty;
                var homeTeamObj = new Team
                {
                    TeamName = homeTeam,
                    CountryId =  country.CountryId
                };
                var awayTeamNode = eventNode.ChildNodes[9];
                var awayTeam = awayTeamNode.Attributes != null ? awayTeamNode.Attributes["Name"].InnerText : String.Empty;
                var awayTeamObj = new Team
                {
                    TeamName = awayTeam,
                    CountryId = country.CountryId
                };

                BetDatabase.Teams.AddOrUpdate(t => t.TeamName, new []{ homeTeamObj, awayTeamObj });
                await BetDatabase.SaveChangesAsync();
                var outcomesNode = eventNode.ChildNodes[14];

                matches.Add(new Match
                {
                    AwayTeamId = awayTeamObj.TeamId,
                    BetServiceMatchNo = Convert.ToInt32(eventId),
                    GameStatus = "Not Started",
                    HomeTeamId = homeTeamObj.TeamId,
                    League = leagueName,
                    RegistrationDate = DateTime.Now,
                    ResultStatus = 0,
                    StartTime = startDateTime
                });

                if (!outcomesNode.HasChildNodes) continue;
                #region OutComes
                foreach (XmlNode outcomeNode in outcomesNode.ChildNodes)
                {
                    if (outcomeNode.Attributes == null) continue;
                    var outcome = outcomeNode.Attributes["name"].InnerText;
                    //Console.WriteLine("{0} Odds", outcome);
                    var bookmakerNode = outcomeNode.ChildNodes[0];
                    var oddsNodes = bookmakerNode.ChildNodes;

                    // Locate an out ome in the datbase with that particular name
                    var betcategory = new BetCategory
                    {
                        Name = outcome
                    };

                    BetDatabase.BetCategories.AddOrUpdate(bc => bc.Name, betcategory);
                    BetDatabase.SaveChanges();
                    switch (outcome)
                    {
                        #region 1x2 Odds
                        case "1X2":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region Under/Over Odds
                        case "Under/Over":
                            foreach (XmlNode odd in oddsNodes)
                            {

                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                if (line != "0.5" && line != "1.5" && line != "2.5" && line != "3.5" &&
                                    line != "4.5" && line != "5.5" && line != "6.5") continue;
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region Double Chance Odds
                        case "Double Chance":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region European Handicap
                        case "European Handicap":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line },
                                    betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region Both Teams To Score Odds
                        case "Both Teams To Score":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region Under/Over 1st Period
                        case "Under/Over 1st Period":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(
                                    bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                        #region 1st Period Winner
                        case "1st Period Winner":
                            foreach (XmlNode odd in oddsNodes)
                            {
                                if (odd.Attributes == null) continue;
                                var bet = odd.Attributes["bet"].InnerText;
                                var line = odd.Attributes["line"].InnerText;
                                var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                                var betoption = new BetOption
                                {
                                    BetCategoryId = betcategory.CategoryId,
                                    Option = bet,
                                    Line = line
                                };

                                BetDatabase.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                await BetDatabase.SaveChangesAsync();
                                var matchodd = new MatchOdd
                                {
                                    BetServiceMatchNo = Convert.ToInt32(eventId),
                                    BetOptionId = betoption.BetOptionId,
                                    LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                                    Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                                };
                                matchodds.Add(matchodd);
                            }
                            break;
                        #endregion

                    }
                }
                #endregion

                
            }
            try
            {
                const int chunkSize = 100;
                var totalMatches = matches.Count;
                var chunksToSave = (totalMatches % chunkSize) > 0 ? (totalMatches / chunkSize) + 1 : totalMatches / chunkSize;
                for (var i = 0; i < chunksToSave; i++)
                {
                    var chunk = matches.Skip(i * chunkSize).Take(chunkSize);
                    var startTime = DateTime.Now;
                    BetDatabase.Matches.AddOrUpdate(m => m.BetServiceMatchNo, chunk.ToArray());
                    BetDatabase.SaveChanges();
                    var endTime = DateTime.Now;
                    var timeSpan = endTime - startTime;
                    //Console.WriteLine("Matches in Chunk {0} Updated Successfully in {1} Seconds", i + 1, timeSpan.Seconds);
                }

                //Console.WriteLine("Matches Updated Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating games: {0}.", ex.Message);
            }
            try
            {
                const int chunkSize = 100;
                var totalMatchOdds = matchodds.Count;
                var chunksToSave = (totalMatchOdds % chunkSize) > 0
                    ? (totalMatchOdds / chunkSize) + 1
                    : totalMatchOdds / chunkSize;

                for (var i = 0; i < chunksToSave; i++)
                {
                    var chunk = matchodds.Skip(i * chunkSize).Take(chunkSize);
                    var startTime = new DateTime();
                    BetDatabase.MatchOdds.AddOrUpdate(key => new { key.BetServiceMatchNo, key.BetOptionId }, chunk.ToArray());
                    BetDatabase.SaveChanges();
                    var endTime = new DateTime();
                    var timeSpan = endTime - startTime;
                    //Console.WriteLine("MatchOdds in Chunk {0} Updated Successfully in {1} Seconds", i + 1, timeSpan.Seconds);
                }
                //Console.WriteLine("All MatchOdds Updated Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating odds: {0}.", ex.Message);
            }
            return 1;
        }


        private async Task<int> UploadGames()
        {
            foreach (var url in _urls)
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                var res = (HttpWebResponse)req.GetResponse();
                var stream = res.GetResponseStream();
                var xmldoc = new XmlDocument();
                {
                    xmldoc.Load(stream);
                    var categoryList = xmldoc.SelectNodes("/scores/category");

                    if (categoryList == null) return 0;
                    foreach (XmlNode node in categoryList)
                    {
                        var matches = node.ChildNodes;
                        foreach (XmlNode matchesNode in matches)
                        {
                            var games = matchesNode.ChildNodes;
                            foreach (XmlNode gameNode in games)
                            {
                                var home = gameNode.ChildNodes[0];
                                var away = gameNode.ChildNodes[1];
                                var odd = gameNode.ChildNodes[4];
                                var gameOdds = odd.ChildNodes;
                                var league = node.Attributes["name"] != null
                                    ? node.Attributes["name"].InnerText
                                    : "FailedLeague";
                                var countryName = node.Attributes["file_group"].InnerText;
                                char[] del = { '.' };
                                var stdate = gameNode.Attributes["formatted_date"].InnerText.Split(del);
                                var stDateTime = stdate[1] + "-" + stdate[0] + "-" + stdate[2]
                                                 + " " + gameNode.Attributes["time"].InnerText + ":00";
                               
                                if (Convert.ToDateTime(stDateTime) > DateTime.Now.AddDays(3))
                                {
                                    continue;
                                }
                                var country = BetDatabase.Countries.SingleOrDefault(c => c.CountryName == countryName);
                                if (country == null)
                                {
                                    country = new Country
                                    {
                                        CountryName = countryName
                                    };
                                    BetDatabase.Countries.AddOrUpdate(c => c.CountryName, country);
                                    await BetDatabase.SaveChangesAsync();
                                }

                                // save the league
                                BetDatabase.Leagues.AddOrUpdate(l => l.LeagueName, new League
                                {
                                    LeagueName = league,
                                    Country = country

                                });
                                await BetDatabase.SaveChangesAsync();

                                var game = new Match();
                                var gameodds = new List<MatchOdd>();
                                game.League = league;
                                game.StartTime = Convert.ToDateTime(stDateTime).ToLocalTime();
                               
                                if (home.Name == "localteam")
                                {
                                    _homeTeam = new Team
                                    {
                                        TeamName = home.Attributes["name"].InnerText,
                                        CountryId = country.CountryId
                                    };
                                    BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _homeTeam);
                                    await BetDatabase.SaveChangesAsync();
                                    game.HomeTeamId = _homeTeam.TeamId;
                                }
                                if (away.Name == "visitorteam")
                                {
                                    _awayTeam = new Team
                                    {
                                        TeamName = away.Attributes["name"].InnerText,
                                        CountryId = country.CountryId
                                    };
                                    BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _awayTeam);
                                    await BetDatabase.SaveChangesAsync();
                                    game.AwayTeamId = _awayTeam.TeamId;

                                }

                              
                              
                                var goalServeMatchId = gameNode.Attributes["id"].InnerText;
                                #region odtypes
                                foreach (XmlNode oddType in gameOdds) 
                                {
                                    var bettype = oddType.Attributes["name"].InnerText;
                                    switch (bettype)
                                    {
                                        case "Full Time Result":
                                            var normalOdds = oddType.ChildNodes;
                                            foreach (XmlNode normalodd in normalOdds)
                                            {
                                                var choice = normalodd.Attributes.GetNamedItem("name").Value;
                                                if (choice == _homeTeam.TeamName + " Win")
                                                {
                                                    gameodds.Add(new MatchOdd
                                                    {
                                                        //1
                                                        BetOptionId = 1,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                normalodd.Attributes.GetNamedItem("odd").Value)
                                                    });
                                                }
                                                else if (choice == "Draw")
                                                {
                                                    gameodds.Add(new MatchOdd
                                                    {
                                                        // X
                                                        BetOptionId = 2,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                normalodd.Attributes.GetNamedItem("odd").Value)
                                                    });
                                                }
                                                else if (choice == _awayTeam.TeamName + " Win")
                                                {
                                                    gameodds.Add(new MatchOdd
                                                    {
                                                        // 2
                                                        BetOptionId = 3,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                normalodd.Attributes.GetNamedItem("odd").Value)
                                                    });
                                                }
                                            }
                                            break;
                                        case "Double Chance":
                                            gameodds.AddRange(new[]
                                            {
                                                new MatchOdd
                                                {
                                                    //1X
                                                    BetOptionId = 21,
                                                    Odd =
                                                        oddType.ChildNodes[2].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[2].Attributes["odd"].Value)
                                                            : 0
                                                },
                                                new MatchOdd
                                                {
                                                    //12
                                                    BetOptionId = 22,
                                                    Odd =
                                                        oddType.ChildNodes[1].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                            : 0
                                                },
                                                new MatchOdd
                                                {
                                                    //X2
                                                    BetOptionId = 23,
                                                    Odd =
                                                        oddType.ChildNodes[0].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                            : 0
                                                }
                                            });
                                            break;
                                        case "Half-Time":
                                            gameodds.AddRange(new[]
                                            {
                                                new MatchOdd
                                                {
                                                    //1X
                                                    BetOptionId = 12,
                                                    Odd =
                                                        oddType.ChildNodes[0].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                            : 0
                                                },
                                                new MatchOdd
                                                {
                                                    //12
                                                    BetOptionId = 13,
                                                    Odd =
                                                        oddType.ChildNodes[1].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                            : 0
                                                },
                                                new MatchOdd
                                                {
                                                    //X2
                                                    BetOptionId = 14,
                                                    Odd =
                                                        oddType.ChildNodes[2].Attributes != null
                                                            ? Convert.ToDecimal(
                                                                oddType.ChildNodes[2].Attributes["odd"].Value)
                                                            : 0
                                                }
                                            });
                                            break;
                                        case "Handicap Result":
                                            //var hhome = Convert.ToInt16(oddType.ChildNodes[0].Attributes["extravalue"].Value);
                                            //var haway = Convert.ToInt16(oddType.ChildNodes[2].Attributes["extravalue"].Value);
                                            gameodds.AddRange(new[]
                                            {
                                                new MatchOdd
                                                {
                                                    // Handicap Home HC1
                                                    BetOptionId = 24,
                                                    Odd =
                                                        Convert.ToDecimal(
                                                            oddType.ChildNodes[0].Attributes["odd"].Value),
                                                    //Line = hhome < 0 ? hhome - hhome : hhome
                                                },
                                                new MatchOdd
                                                {
                                                    // HAndicap Away HC2
                                                    BetOptionId = 31,
                                                    Odd =
                                                        Convert.ToDecimal(
                                                            oddType.ChildNodes[1].Attributes["odd"].Value),
                                                    //Line = haway < 0 ? (haway - haway) : haway
                                                },
                                                new MatchOdd
                                                {
                                                    // HAndicap Away HC2
                                                    BetOptionId = 25,
                                                    Odd =
                                                        Convert.ToDecimal(
                                                            oddType.ChildNodes[2].Attributes["odd"].Value),
                                                    //Line = haway < 0 ? (haway - haway) : haway
                                                }

                                            });
                                            break;

                                        case "Draw No Bet":
                                            gameodds.AddRange(new[]
                                            {
                                                new MatchOdd
                                                {
                                                    //DNB1
                                                    BetOptionId = 28,
                                                    Odd =
                                                        Convert.ToDecimal(
                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                                                },
                                                new MatchOdd
                                                {
                                                    //DNB2
                                                    BetOptionId = 29,
                                                    Odd =
                                                        Convert.ToDecimal(
                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                                                }
                                            });
                                            break;

                                        case "Both Teams to Score":
                                                gameodds.AddRange(new[]
                                                {
                                                    new MatchOdd
                                                    {
                                                        // GG
                                                        BetOptionId = 26,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // NG
                                                        BetOptionId = 27,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                    }
                                                });
                                            break;

                                        case "Total Goals":
                                                gameodds.AddRange(new[]
                                                {
                                                    new MatchOdd
                                                    {
                                                        // FTUnder2.5
                                                        BetOptionId = 6,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTOver2.5
                                                        BetOptionId = 7,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                    }
                                                });
                                            break;

                                        case "Alternative Total Goals":
                                                gameodds.AddRange(new[]
                                                {
                                                    new MatchOdd
                                                    {
                                                        // FTUnder0.5
                                                        BetOptionId = 32,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTOver0.5
                                                        BetOptionId = 33,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTUnder1.5
                                                        BetOptionId = 4,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[3].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTOver1.5
                                                        BetOptionId = 5,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[2].Attributes["odd"].Value)
                                                    },
                                                
                                                    new MatchOdd
                                                    {
                                                        // FTOver3.5
                                                        BetOptionId = 9,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[4].Attributes["odd"].Value)
                                                    },
                                                        new MatchOdd
                                                    {
                                                        // FTUnder3.5
                                                        BetOptionId = 8,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[5].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTUnder4.5
                                                        BetOptionId = 10,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[7].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTOver4.5
                                                        BetOptionId = 11,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[6].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTUnder5.5
                                                        BetOptionId = 34,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[7].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        // FTOver5.5
                                                        BetOptionId = 35,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[6].Attributes["odd"].Value)
                                                    }
                                                });
                                            break;
                                        case "First Half Goals":
                                                gameodds.AddRange(new[]
                                                {
                                                    new MatchOdd
                                                    {
                                                        //HTUnder0.5
                                                        BetOptionId = 15,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[1].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        //HTOver0.5
                                                        BetOptionId = 16,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[0].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        //HTUnder1.5
                                                        BetOptionId = 17,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[3].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        //HTOver1.5
                                                        BetOptionId = 18,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[2].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        //HTUnder2.5
                                                        BetOptionId = 19,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[5].Attributes["odd"].Value)
                                                    },
                                                    new MatchOdd
                                                    {
                                                        //HTOver2.5
                                                        BetOptionId = 20,
                                                        Odd =
                                                            Convert.ToDecimal(
                                                                oddType.ChildNodes[4].Attributes["odd"].Value)
                                                    }
                                                });
                                            break;
                                    }
                                } 
#endregion
                                game.MatchOdds = gameodds;
                                game.BetServiceMatchNo = Convert.ToInt32(goalServeMatchId);
                                game.StartTime = Convert.ToDateTime(stDateTime).ToLocalTime();
                                game.ResultStatus = 1;
                               // game.MatchOdds.ForEach(g => g.BetServiceMatchNo = game.BetServiceMatchNo);
                                BetDatabase.Matches.AddOrUpdate(game);
                                await BetDatabase.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            return 1;
        }

        public void AddOdd(MatchOddsVm odds)
        {
            var match = BetDatabase.Matches.Find(odds.Mat);
            var matno = Convert.ToInt32(odds.Mat);
            if (match == null) return;
            MatchOdd matchOdd;
            if (odds.OddFt1 != 0)
            {

                matchOdd = new MatchOdd {BetOptionId = 1, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddFt1)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }
            if (odds.OddFTX != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 2, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddFTX)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }
            if (odds.OddFT2 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 3, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddFT2)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                    
                BetDatabase.SaveChanges();
            }

            if (odds.oddFtUnder15 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 4,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder15)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtOver15 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 5,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver15)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtUnder25 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 6,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder25)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtOver25 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 7,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver25)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtUnder35 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 8,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder35)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }

            if (odds.oddFtOver35 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 9,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver35)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                
            }
            if (odds.oddFtUnder45 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 10,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder45)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                      
            }

            if (odds.oddFtOver45 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 11,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver45)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.OddHt1 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 12, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddHt1)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.OddHtx != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 13, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddHtx)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.OddHt2 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 14, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.OddHt2)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();

            }
            if (odds.oddHtUnder05 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 15,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtUnder05)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddHtOver05 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 16,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtOver05)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);

            }
            if (odds.oddHtUnder15 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 17,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtUnder15)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddHtOver15 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 18,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtOver15)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }
            if (odds.oddHtUnder25 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 19,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtUnder25)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddHtOver25 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 20,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddHtOver25)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.odd1X != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 21, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.odd1X)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            /*double chance*/
            if (odds.odd12 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 22, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.odd12)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddX2 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 23, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.oddX2)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }

            if (odds.oddHC1 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 24,
                    BetServiceMatchNo = matno,
                    //Line = odds.HomeGoal,
                    Odd = Convert.ToDecimal(odds.oddHC1)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }
            if (odds.oddHC2!= 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 25,
                    BetServiceMatchNo = matno,
                    //Line = odds.AwayGoal,
                    Odd = Convert.ToDecimal(odds.oddHC2)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.OddFT2 != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 26, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.oddGG)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddGG != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 27, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.oddNG)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);

            }
            if (odds.oddHCX != 0)
            {
                matchOdd = new MatchOdd {BetOptionId = 31, BetServiceMatchNo = matno, Odd = Convert.ToDecimal(odds.oddHCX)};
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtUnder05 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 32,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder05)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
                BetDatabase.SaveChanges();
            }
            if (odds.oddFtOver05 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 33,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver05)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtUnder55 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 34,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtUnder55)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            if (odds.oddFtOver55 != 0)
            {
                matchOdd = new MatchOdd
                {
                    BetOptionId = 35,
                    BetServiceMatchNo = matno,
                    Odd = Convert.ToDecimal(odds.oddFtOver55)
                };
                BetDatabase.MatchOdds.AddOrUpdate(matchOdd);
            }
            BetDatabase.SaveChanges();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var ds = new DataSet();
            var httpPostedFileBase = Request.Files["file"];
            if (httpPostedFileBase == null || httpPostedFileBase.ContentLength <= 0) return View();
            var postedFileBase = Request.Files["file"];
            if (postedFileBase != null)
            {
                var fileExtension = Path.GetExtension(postedFileBase.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    var fileLocation = Server.MapPath("~/Content/Files/") + postedFileBase.FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {

                        System.IO.File.Delete(fileLocation);
                    }
                    postedFileBase.SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                                   fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    switch (fileExtension)
                    {
                        case ".xls":
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                                    fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            break;
                        case ".xlsx":
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            break;
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    var excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();

                    var dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    var excelSheets = new String[dt.Rows.Count];
                    var t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    var excelConnection1 = new OleDbConnection(excelConnectionString);


                    var query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (var dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
                if (fileExtension != null && fileExtension.ToLower().Equals(".xml"))
                {
                    var fileBase = Request.Files["FileUpload"];
                    if (fileBase != null)
                    {
                        var fileLocation = Server.MapPath("~/Content/Files/") + fileBase.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        fileBase.SaveAs(fileLocation);
                        var xmlreader = new XmlTextReader(fileLocation);
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }
                }
            }
            var excelmatches = new List<Match>();

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
            var countryName = (ds.Tables[0].Rows[i][3]).ToString();
            var country = BetDatabase.Countries.SingleOrDefault(c => c.CountryName == countryName);
            if (country == null)
            {
                country = new Country
                {
                    CountryName = countryName
                };
                BetDatabase.Countries.AddOrUpdate(c => c.CountryName, country);
                BetDatabase.SaveChanges();
            }

            // save the league
            BetDatabase.Leagues.AddOrUpdate(l => l.LeagueName, new League
            {
                LeagueName = countryName,
                Country = country

            });

            var st = ds.Tables[0].Rows[i][0].ToString();
                _homeTeam = new Team
                {
                    TeamName = ds.Tables[0].Rows[i][4].ToString(),
                    CountryId = country.CountryId
                };
                BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _homeTeam);
                BetDatabase.SaveChanges();
                _awayTeam = new Team
                {
                    TeamName = ds.Tables[0].Rows[i][5].ToString(),
                    CountryId = country.CountryId
                };
                BetDatabase.Teams.AddOrUpdate(t => t.TeamName, _awayTeam);
                BetDatabase.SaveChanges();


                if (st == "") continue;
                var matno = Convert.ToInt32(ds.Tables[0].Rows[i][0] ?? 0);
                              
                var index = (ds.Tables[0].Rows[i][1]).ToString().IndexOf(" ", StringComparison.Ordinal);
                var stDate = (ds.Tables[0].Rows[i][1]).ToString().Substring(0,index);
                var stTime =  (ds.Tables[0].Rows[i][2]) + ":00";
                var matchTime = Convert.ToDateTime(stDate+" "+stTime);
                var match = new Match
                {
                    BetServiceMatchNo = matno,
                    StartTime = matchTime,
                    League = (ds.Tables[0].Rows[i][3]).ToString(),
                    MatchOdds = new List<MatchOdd>
                    {
                        new MatchOdd// Full Time Results
                        {
                            BetOptionId = 1,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][6] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 2,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][7] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 3,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][8] ?? 0),
                        },


                        new MatchOdd  //Under 2.5
                        {
                            BetOptionId = 6,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][9] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 7,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][10] ?? 0),
                        },

                        new MatchOdd  // Full Time1.5
                        {
                            BetOptionId = 4,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][11] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 5,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][12] ?? 0),
                        },


                                    
                        new MatchOdd//HalfTime
                        {
                            BetOptionId = 12,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][13] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 13,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][14] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 14,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][15] ?? 0),
                        },

                        new MatchOdd   //HalfTime Under
                        {//1.5
                            BetOptionId = 17,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][16] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 18,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][17] ?? 0),
                        },

                        new MatchOdd //Ht 0.5
                        {
                            BetOptionId = 15,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][18] ?? 0),
                        },
                        new MatchOdd
                        {
                            BetOptionId = 16,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][19] ?? 0),
                        },
                        new MatchOdd  //double chance 1x
                        {
                            BetOptionId = 21,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][20] ?? 0),
                        },
                        //Double Chance
                        new MatchOdd //x2
                        {
                            BetOptionId = 23,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][21] ?? 0),
                        },
                        new MatchOdd  //Double Chance x2
                        {
                            BetOptionId = 22,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][22] ?? 0),
                        },

                        //HandCap home
                        new MatchOdd // hc1
                        {
                            BetOptionId = 24,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,                 
                            //Line = Convert.ToInt16(ds.Tables[0].Rows[i][26] ?? 0),
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][23] ?? 0),

                        },



                        //HandCap X
                        new MatchOdd
                        {
                            BetOptionId = 31,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][24] ?? 0),
                        },
                                       
                        //HandCap Away
                        new MatchOdd
                        {
                            BetOptionId = 25,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            //Line = Convert.ToInt16(ds.Tables[0].Rows[i][27] ?? 0),
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][25] ?? 0),
                        },



                        // Both Teams To Score  Yes
                        new MatchOdd
                        {
                            BetOptionId = 26,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][28] ?? 0),
                        },
                                        
                        // Both Teams To Score  No
                        new MatchOdd
                        {
                            BetOptionId = 27,
                            BetServiceMatchNo = matno,
                            LastUpdateTime = DateTime.Now,
                            Odd = Convert.ToDecimal(ds.Tables[0].Rows[i][29] ?? 0),
                        },                                

                    },
                    AwayTeamId = _awayTeam.TeamId,
                    HomeTeamId = _homeTeam.TeamId,
                    RegistrationDate = DateTime.Now,
                    ResultStatus = 1,
                };
                excelmatches.Add(match);
                match.MatchOdds.ForEach(g => g.BetServiceMatchNo = match.BetServiceMatchNo);
                BetDatabase.Matches.AddOrUpdate(match);
                BetDatabase.SaveChanges();
            }
            return View();
        }

        public decimal GetOdd(string num)
        {
            double odd;
            odd = Double.TryParse(num, out odd) ? Convert.ToDouble(num) : 1;
            return Convert.ToDecimal(odd);
        }

        public static DataTable ConvertToDatatable<T>(IList<T> data)
        {
            var props =
                TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        //public JsonResult GetMatch(string id)
        //{
        //    var matNo = Convert.ToInt32(id);
        //    var game = BetDatabase.Matches.SingleOrDefault(x => x.BetServiceMatchNo == matNo);
        //    var odds = BetDatabase.MatchOdds.Where(g => g.BetServiceMatchNo == game.BetServiceMatchNo).ToList();
        //    if (odds.Count > 0)
        //    {
        //        if (game != null)
        //            return Json(new
        //            {
        //                mat = game.HomeTeam.TeamName + " " + game.AwayTeam.TeamName,
        //                oddFT1 = odds.SingleOrDefault(x => x.BetOptionId == 1) != null? odds.SingleOrDefault(x => x.BetOptionId == 1).Odd: 0,
        //                oddFTX = odds.SingleOrDefault(x => x.BetOptionId == 2).Odd,
        //                oddFT2 = odds.SingleOrDefault(x => x.BetOptionId == 3).Odd,

        //                oddHT1 = odds.SingleOrDefault(x => x.BetOptionId == 12).Odd,
        //                oddHTX = odds.SingleOrDefault(x => x.BetOptionId == 13).Odd,
        //                oddHT2 = odds.SingleOrDefault(x => x.BetOptionId == 14).Odd,		            
        //                //odd1X =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddX2=odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //odd12 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHC1 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHCX =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHC2 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //HomeGoal =odd.SingleOrDefault(x=>x.BetOptionId==1).Line,
        //                //AwayGoal=odd.SingleOrDefault(x=>x.BetOptionId==1).Line,
        //                //oddHtUnder05 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,   
        //                //oddHtOver05 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtOver15  =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtUnder15  =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtOver25 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtUnder25 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtOver35 = odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddHtUnder35 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,           
        //                //oddFtUnder05 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver05 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtUnder15 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver15 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtUnder25 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver25 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver35 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtUnder35 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtUnder45 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver45 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtOver55 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddFtUnder55 =odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddGG=odd.SingleOrDefault(x=>x.BetOptionId==1).Odd,
        //                //oddNG = odd.SingleOrDefault(x => x.BetOptionId == 1).Odd,         

        //            }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new {game = game.HomeTeam.TeamName + game.AwayTeam.TeamName},JsonRequestBehavior.AllowGet);
        //    }

        //}
        
    }
}