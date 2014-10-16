using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Xml;
using ZeusConsole.Models;

namespace ZeusConsole
{
    class Program
    {
        static void Main()
        {
            try
            {
                var database = new ZeusDbContext();
                var matches = new List<Match>();
                var matchodds = new List<MatchOdd>();
                var xmldoc = new XmlDocument();
                xmldoc.Load("D:/Documents/BWay/rebetxmlfiles/GetSportEventsItalyEngland.xml");
                var oddServiceNode = xmldoc.ChildNodes[1];
                var resultsNode = oddServiceNode.ChildNodes[1];

                foreach (XmlNode eventNode in resultsNode)
                {
                    var eventId = eventNode.ChildNodes[0].InnerText;
                    var unformattedStartDate = eventNode.ChildNodes[1].InnerText;
                    var startDateTime = DateTime.Parse(unformattedStartDate);

                    var sportIdNode = eventNode.ChildNodes[2];
                    var sportId = sportIdNode.InnerText;
                    var sportName = (sportIdNode.Attributes != null) ? sportIdNode.Attributes["Name"].InnerText : String.Empty;

                    var leagueIdNode = eventNode.ChildNodes[3];
                    var leagueId = leagueIdNode.InnerText;
                    var leagueName = (leagueIdNode.Attributes != null) ? leagueIdNode.Attributes["Name"].InnerText : String.Empty;

                    var locationIdNode = eventNode.ChildNodes[4];
                    var locationId = locationIdNode.InnerText;
                    var locationName = (locationIdNode.Attributes != null) ? locationIdNode.Attributes["Name"].InnerText : String.Empty;

                    var lastUpdateTime = eventNode.ChildNodes[6].InnerText;
                    var homeTeamNode = eventNode.ChildNodes[8];
                    var homeTeam = (homeTeamNode.Attributes != null) ? homeTeamNode.Attributes["Name"].InnerText : String.Empty;

                    var awayTeamNode = eventNode.ChildNodes[9];

                    var awayTeam = awayTeamNode.Attributes != null ? awayTeamNode.Attributes["Name"].InnerText : String.Empty;

                    var outcomesNode = eventNode.ChildNodes[14];

                    matches.Add(new Match
                    {
                        AwayTeamName = awayTeam,
                        BetServiceMatchNo = Convert.ToInt32(eventId),
                        GameStatus = "Not Started",
                        HomeTeamName = homeTeam,
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
                            BetCategoryName = outcome
                        };

                        database.BetCategories.AddOrUpdate(bc => bc.BetCategoryName, betcategory);
                        database.SaveChanges();
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
                                        BetCategoryId = betcategory.BetCategoryId,
                                        Option = bet,
                                        Line = line
                                    };

                                    database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                    database.SaveChanges();
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
                                        BetCategoryId = betcategory.BetCategoryId,
                                        Option = bet,
                                        Line = line
                                    };

                                    database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                    database.SaveChanges();
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
                                        BetCategoryId = betcategory.BetCategoryId,
                                        Option = bet,
                                        Line = line
                                    };

                                    database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                    database.SaveChanges();
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
                                        BetCategoryId = betcategory.BetCategoryId,
                                        Option = bet,
                                        Line = line
                                    };

                                    database.BetOptions.AddOrUpdate(bo => new {bo.Option, bo.BetCategoryId, bo.Line},
                                        betoption);
                                    database.SaveChanges();
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
                                        BetCategoryId = betcategory.BetCategoryId,
                                        Option = bet,
                                        Line = line
                                    };

                                    database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                                    database.SaveChanges();
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
                    database.Matches.AddOrUpdate(m => m.BetServiceMatchNo, matches.ToArray());
                    database.SaveChanges();
                    Console.WriteLine("Matches Updated Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating games: {0}.", ex.Message);
                }
                try{
                    database.MatchOdds.AddOrUpdate(key => new { key.BetServiceMatchNo , key.BetOptionId }, matchodds.ToArray());
                    database.SaveChanges();
                    Console.WriteLine("MatchOdds Updated Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating odds: {0}.", ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            Console.ReadLine();
        }
        
    }
}
