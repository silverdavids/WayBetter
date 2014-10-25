using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Xml;
using ZeusConsole.Models;

namespace ZeusConsole
{
    class Program
    {
        static void Main()
        {
            UpdateGamesTask();
        }

        static Task<int> UpdateGamesTask()
        {
            try
            {
                var files = new[] { "D:/Documents/BWay/rebetxmlfiles/EnglandSchedule.xml" };

                var database = new ZeusDbContext();
                // start of files urls 
                foreach (var file in files)
                {
                    var xmldoc = new XmlDocument();
                    xmldoc.Load(file);

                    var scoresNode = xmldoc.ChildNodes.Item(1);
                    if (scoresNode == null || !scoresNode.HasChildNodes)
                    {
                        Console.WriteLine("There are no leagues in the file");
                        return Task.FromResult(0);
                    }

                    // get the categories or leagues
                    var categoryNodes = scoresNode.ChildNodes;
                    Console.WriteLine("The file has matches for {0} leagues.\nStarting to process leagues.", categoryNodes.Count);

                    foreach (XmlNode categoryNode in categoryNodes)
                    {
                        // check if the category has attributes and child nodes
                        if (categoryNode == null || !categoryNode.HasChildNodes) continue;

                        if (categoryNode.Attributes == null) continue;
                        var league = categoryNode.Attributes["name"].InnerText;
                        var matchesNode = categoryNode.FirstChild;
                        if (matchesNode == null || !matchesNode.HasChildNodes)
                        {
                            Console.WriteLine("The League does not have any games in it....");
                            continue;
                        }

                        var matchNodes = matchesNode.ChildNodes;
                        if(matchNodes.Count <= 0) continue;
                        
                        Console.WriteLine("The League {0} has {1} matches in it.", league, matchNodes.Count);
                        foreach (XmlNode matchNode in matchNodes)
                        {
                            // don't boter saving a match that has no details
                            if(matchNode == null || !matchNode.HasChildNodes || matchNode.Attributes == null ) continue;

                            var betServiceMatchCode = Int32.Parse(matchNode.Attributes["id"].InnerText.Trim());
                            var unformattedStartDate = matchNode.Attributes["formatted_date"].InnerText;
                            var unformattedStartTime = matchNode.Attributes["time"].InnerText;
                            
                            #region Start Date Time Patch

                            var startDateArray = unformattedStartDate.Split(new[] {'.'});
                            var startTimeArray = unformattedStartTime.Split(new[] {':'});

                            var year = Int32.Parse(startDateArray[2]);
                            var month = Int32.Parse(startDateArray[1]);
                            var day = Int32.Parse(startDateArray[0]);
                            var hour = Int32.Parse(startTimeArray[0]);
                            var mins = Int32.Parse(startTimeArray[1]);

                            #endregion

                            var startDateTime = new DateTime(year, month, day, hour, mins, 00);
                            var homeTeamNode = matchNode.ChildNodes[0];
                            var awayTeamNode = matchNode.ChildNodes[1];
                            var oddCategoriesNode = matchNode.ChildNodes[4];

                            // we can not determine the home or away team then retreat
                            if(homeTeamNode.Attributes==null || awayTeamNode.Attributes == null || !oddCategoriesNode.HasChildNodes || oddCategoriesNode.Attributes == null) continue;
                            var liveBettingAttr = oddCategoriesNode.Attributes["live_betting"].InnerText;
                            var liveBetFlag = liveBettingAttr == "True";
                            var homeTeamName = homeTeamNode.Attributes["name"].InnerText;
                            var awayTeamName = awayTeamNode.Attributes["name"].InnerText;

                            Console.WriteLine("{0} => {1} vs {2} at {3:D}. Total Odd Categories: {4}. For Live: {5}",
                                betServiceMatchCode, homeTeamName, awayTeamName, startDateTime, oddCategoriesNode.ChildNodes.Count, liveBetFlag);

                            var match = new Match
                            {
                                AwayTeamName = awayTeamName,
                                BetServiceMatchNo = betServiceMatchCode,
                                GameStatus = "Not Started",
                                HomeTeamName = homeTeamName,
                                League = league,
                                RegistrationDate = DateTime.Now,
                                StartTime = startDateTime
                            };

                            database.Matches.AddOrUpdate(m => m.BetServiceMatchNo, match);
                            database.SaveChanges();
                            var betCategoryNodes = oddCategoriesNode.ChildNodes;

                            // Begin Processing Odds
                            foreach (XmlNode betCategoryNode in betCategoryNodes)
                            {
                                // continue if the node is not well formed
                                if(betCategoryNode.Attributes==null ||! betCategoryNode.HasChildNodes) continue;

                                var betCategoryName = betCategoryNode.Attributes["name"].InnerText;
                                var betOptions = betCategoryNode.ChildNodes;

                                // try getting the oddsNode first and continue if the node is malformed
                                var betCategory = new BetCategory
                                {
                                    BetCategoryName = betCategoryName
                                };

                                database.BetCategories.AddOrUpdate(bc => bc.BetCategoryName, betCategory);
                                database.SaveChanges();
                                switch (betCategoryName)
                                {
                                    #region Full Time Result Odds
                                    case "Full Time Result":
                                        var homeWinNodeName = homeTeamName + " Win";
                                        var awayWinNodeName = awayTeamName + " Win";
                                        foreach (XmlNode betOptionNode in betOptions)
                                        {
                                            if (betOptionNode.Attributes == null) continue;
                                            BetOption betOption;
                                            var oddValue = betOptionNode.Attributes["odd"].InnerText;
                                            var betoption = betOptionNode.Attributes["name"].InnerText;
                                            if (betoption == homeWinNodeName)
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "1"
                                                };

                                            }
                                            else if (betoption == awayWinNodeName)
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "2"
                                                };
                                            }
                                            else
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "X"
                                                };
                                            }
                                            database.BetOptions.AddOrUpdate(bo => new { bo.BetCategoryId, bo.Line, bo.Option }, betOption);
                                            database.SaveChanges();
                                            var matchOdd = new MatchOdd
                                            {
                                                BetOptionId = betOption.BetOptionId,
                                                BetServiceMatchNo = betServiceMatchCode,
                                                Odd = Convert.ToDecimal(oddValue)
                                            };
                                            database.MatchOdds.AddOrUpdate(mo => new { mo.BetServiceMatchNo, mo.BetOptionId }, matchOdd);
                                            database.SaveChanges();
                                        }

                                        break;
                                    #endregion

                                    #region Double Chance Odds
                                    case "Double Chance":
                                        var homeOrDrawNodeName = homeTeamName + " or Draw";
                                        var drawOrAwayNodeName = "Draw or " + awayTeamName;
                                        foreach (XmlNode betOptionNode in betOptions)
                                        {
                                            if (betOptionNode.Attributes == null) continue;
                                            BetOption betOption;
                                            var oddValue = betOptionNode.Attributes["odd"].InnerText;
                                            var betoption = betOptionNode.Attributes["name"].InnerText;
                                            if (betoption == homeOrDrawNodeName)
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "1X"
                                                };

                                            }
                                            else if (betoption == drawOrAwayNodeName)
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "X2"
                                                };
                                            }
                                            else
                                            {
                                                betOption = new BetOption
                                                {
                                                    BetCategoryId = betCategory.BetCategoryId,
                                                    Line = "",
                                                    Option = "12"
                                                };
                                            }
                                            database.BetOptions.AddOrUpdate(bo => new { bo.BetCategoryId, bo.Line, bo.Option }, betOption);
                                            database.SaveChanges();
                                            var matchOdd = new MatchOdd
                                            {
                                                BetOptionId = betOption.BetOptionId,
                                                BetServiceMatchNo = betServiceMatchCode,
                                                Odd = Convert.ToDecimal(oddValue)
                                            };
                                            database.MatchOdds.AddOrUpdate(mo => new { mo.BetServiceMatchNo, mo.BetOptionId }, matchOdd);
                                            database.SaveChanges();
                                        }
                                        break;
                                    #endregion

                                    
                                }
                            }
                        }

                    }
                    continue;
                    //var oddServiceNode = xmldoc.ChildNodes[1];
                    //var resultsNode = oddServiceNode.ChildNodes[1];

                    //foreach (XmlNode eventNode in resultsNode)
                    //{
                    //    var eventId = eventNode.ChildNodes[0].InnerText;
                    //    var unformattedStartDate = eventNode.ChildNodes[1].InnerText;
                    //    var startDateTime = DateTime.Parse(unformattedStartDate);

                    //    var sportIdNode = eventNode.ChildNodes[2];
                    //    var sportId = sportIdNode.InnerText;
                    //    var sportName = (sportIdNode.Attributes != null) ? sportIdNode.Attributes["Name"].InnerText : String.Empty;

                    //    var leagueIdNode = eventNode.ChildNodes[3];
                    //    var leagueId = leagueIdNode.InnerText;
                    //    var leagueName = (leagueIdNode.Attributes != null) ? leagueIdNode.Attributes["Name"].InnerText : String.Empty;

                    //    var locationIdNode = eventNode.ChildNodes[4];
                    //    var locationId = locationIdNode.InnerText;
                    //    var locationName = (locationIdNode.Attributes != null) ? locationIdNode.Attributes["Name"].InnerText : String.Empty;

                    //    var lastUpdateTime = eventNode.ChildNodes[6].InnerText;
                    //    var homeTeamNode = eventNode.ChildNodes[8];
                    //    var homeTeam = (homeTeamNode.Attributes != null) ? homeTeamNode.Attributes["Name"].InnerText : String.Empty;

                    //    var awayTeamNode = eventNode.ChildNodes[9];

                    //    var awayTeam = awayTeamNode.Attributes != null
                    //        ? awayTeamNode.Attributes["Name"].InnerText
                    //        : String.Empty;

                    //    var outcomesNode = eventNode.ChildNodes[14];

                    //    Console.WriteLine("{0} -> {1} vs {2}", leagueName, homeTeam, awayTeam);
                    //    matches.Add(new Match
                    //    {
                    //        AwayTeamName = awayTeam,
                    //        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //        GameStatus = "Not Started",
                    //        HomeTeamName = homeTeam,
                    //        League = leagueName,
                    //        RegistrationDate = DateTime.Now,
                    //        ResultStatus = 0,
                    //        StartTime = startDateTime
                    //    });

                    //    if (!outcomesNode.HasChildNodes) continue;

                    //    #region OutComes
                    //    foreach (XmlNode outcomeNode in outcomesNode.ChildNodes)
                    //    {
                    //        if (outcomeNode.Attributes == null) continue;
                    //        var outcome = outcomeNode.Attributes["name"].InnerText;
                    //        //Console.WriteLine("{0} Odds", outcome);
                    //        var bookmakerNode = outcomeNode.ChildNodes[0];
                    //        var oddsNodes = bookmakerNode.ChildNodes;

                    //        // Locate an out ome in the datbase with that particular name
                    //        var betcategory = new BetCategory
                    //        {
                    //            BetCategoryName = outcome
                    //        };

                    //        //database.BetCategories.AddOrUpdate(bc => bc.BetCategoryName, betcategory);
                    //        //database.SaveChanges();
                    //        switch (outcome)
                    //        {
                    //            #region 1x2 Odds
                    //            case "1X2":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region Under/Over Odds
                    //            case "Under/Over":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {

                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };
                    //                    if (line != "0.5" && line != "1.5" && line != "2.5" && line != "3.5" &&
                    //                        line != "4.5" && line != "5.5" && line != "6.5") continue;
                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region Double Chance Odds
                    //            case "Double Chance":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region European Handicap
                    //            case "European Handicap":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line },betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region Both Teams To Score Odds
                    //            case "Both Teams To Score":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    if (odd.Attributes == null) continue;
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region Under/Over 1st Period
                    //            case "Under/Over 1st Period":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion

                    //            #region 1st Period Winner
                    //            case "1st Period Winner":
                    //                foreach (XmlNode odd in oddsNodes)
                    //                {
                    //                    if (odd.Attributes == null) continue;
                    //                    var bet = odd.Attributes["bet"].InnerText;
                    //                    var line = odd.Attributes["line"].InnerText;
                    //                    var unformattedLastUpdateTime = odd.Attributes["LastUpdate"].InnerText;
                    //                    var betoption = new BetOption
                    //                    {
                    //                        BetCategoryId = betcategory.BetCategoryId,
                    //                        Option = bet,
                    //                        Line = line
                    //                    };

                    //                    //database.BetOptions.AddOrUpdate(bo => new { bo.Option, bo.BetCategoryId, bo.Line }, betoption);
                    //                    //database.SaveChanges();
                    //                    var matchodd = new MatchOdd
                    //                    {
                    //                        BetServiceMatchNo = Convert.ToInt32(eventId),
                    //                        BetOptionId = betoption.BetOptionId,
                    //                        LastUpdateTime = DateTime.Parse(unformattedLastUpdateTime),
                    //                        Odd = Convert.ToDecimal(odd.Attributes["currentPrice"].InnerText)
                    //                    };
                    //                    matchodds.Add(matchodd);
                    //                }
                    //                break;
                    //            #endregion
                    //        }
                    //    }
                    //    #endregion
                    //}
                    //// end of for each for file or url

                    //try
                    //{
                    //    const int chunkSize = 100;
                    //    var totalMatches = matches.Count;
                    //    var chunksToSave = (totalMatches % chunkSize) > 0 ? (totalMatches / chunkSize) + 1 : totalMatches / chunkSize;
                    //    for (var i = 0; i < chunksToSave; i++)
                    //    {
                    //        var chunk = matches.Skip(i * chunkSize).Take(chunkSize);
                    //        var startTime = DateTime.Now;
                    //        //database.Matches.AddOrUpdate(m => m.BetServiceMatchNo, chunk.ToArray());
                    //        //database.SaveChanges();
                    //        var endTime = DateTime.Now;
                    //        var timeSpan = endTime - startTime;
                    //        Console.WriteLine("Matches in Chunk {0} Updated Successfully in {1} Seconds", i + 1, timeSpan.Seconds);
                    //    }

                    //    Console.WriteLine("Matches Updated Successfully");
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine("Error updating games: {0}.", ex.Message);
                    //}
                    //try
                    //{
                    //    const int chunkSize = 100;
                    //    var totalMatchOdds = matchodds.Count;
                    //    var chunksToSave = (totalMatchOdds % chunkSize) > 0
                    //        ? (totalMatchOdds / chunkSize) + 1
                    //        : totalMatchOdds / chunkSize;

                    //    for (var i = 0; i < chunksToSave; i++)
                    //    {
                    //        var chunk = matchodds.Skip(i * chunkSize).Take(chunkSize);
                    //        var startTime = new DateTime();
                    //        //database.MatchOdds.AddOrUpdate(key => new { key.BetServiceMatchNo, key.BetOptionId }, chunk.ToArray());
                    //        //database.SaveChanges();
                    //        var endTime = new DateTime();
                    //        var timeSpan = endTime - startTime;
                    //        Console.WriteLine("MatchOdds in Chunk {0} Updated Successfully in {1} Seconds", i + 1, timeSpan.Seconds);
                    //    }
                    //    Console.WriteLine("All MatchOdds Updated Successfully");
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine("Error updating odds: {0}.", ex.Message);
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            Console.ReadLine();
            return Task.FromResult(1);
        }
    }
}
