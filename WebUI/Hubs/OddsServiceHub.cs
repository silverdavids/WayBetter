using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using DocumentFormat.OpenXml.Bibliography;
using Domain.Models.Concrete;
using Domain.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using WebUI.DataAccessLayer;

namespace WebUI.Hubs
{
    public class OddsServiceHub : Hub
    {
        private Team _awayTeam, _homeTeam;

        public void Hello()
        {
            Clients.Caller.preprogress(GoalServeUpload());
        }


        public async Task<int> GoalServeUpload()
        {
            var context = new ApplicationDbContext();
            Clients.Caller.uploadprogress("Upload Initiated");
            //var matchesSaved = 0;
            //var matchesWithOdds = 0;
            var path = (HttpContext.Current == null)? 
                HostingEnvironment.MapPath("~/gbl/england_shedule.xml") : 
                HttpContext.Current.Server.MapPath("~/gbl/england_shedule.xml");

            if (path == null) return 0;
            var xmldoc = new XmlDocument();
            xmldoc.Load(path);
            Clients.Caller.uploadprogress("Xml File Loaded");

            var categoryNodes = xmldoc.SelectNodes("/scores/category");

            if (categoryNodes == null) return 0;
            foreach (XmlNode categoryNode in categoryNodes)
            {
                // Category or League does not have any matches presented - proceed to the next category
                if (!categoryNode.HasChildNodes || categoryNode.Attributes == null) continue;
                var league = categoryNode.Attributes["name"].InnerText;
                var countryName = categoryNode.Attributes["file_group"].InnerText;
                Clients.Caller.uploadprogress(String.Format("{0} => {1}", countryName, league));
                var matchesNode = categoryNode.ChildNodes[0];

                // No matches presented - proceed to the next category
                if (!matchesNode.HasChildNodes) continue;

                // Checking match by match
                var matches = matchesNode.ChildNodes;
                Clients.Caller.uploadprogress(matches.Count);
                foreach (XmlNode matchNode in matches)
                {
                    if (!matchNode.HasChildNodes || matchNode.Attributes == null) continue;

                    // process node attributes
                    var unformattedStartDate = matchNode.Attributes["formatted_date"].InnerText.Split(new []{'.'});
                    var unformattedStartTime = matchNode.Attributes["time"].InnerText.Split(new[] {':'});

                    var betServiceMatchNo = Int32.Parse(matchNode.Attributes["id"].InnerText);
                    #region DateTime Fix
                    var year = Int32.Parse(unformattedStartDate[2]);
                    var month = Int32.Parse(unformattedStartDate[1]);
                    var date = Int32.Parse(unformattedStartDate[0]);
                    var hour = Int32.Parse(unformattedStartTime[0]);
                    var minutes = Int32.Parse(unformattedStartTime[1]);
                    #endregion
                    var startTime = new DateTime(year, month, date, hour, minutes, 0);

                    // extract subnodes from parent node
                    var homeTeamNode = matchNode.ChildNodes[0];
                    var awayTeamNode = matchNode.ChildNodes[1];
                    var oddsNode = matchNode.ChildNodes[4];

                    // check for a malformed team node 
                    if (awayTeamNode.Attributes == null || homeTeamNode.Attributes == null) continue;
                    
                    // check if we have any odds for the game
                    if (!oddsNode.HasChildNodes || oddsNode.Attributes == null) continue;

                    // get the live betting flag
                    var isForLiveBet = oddsNode.Attributes["live_betting"].InnerText.ToLower().Contains("true");
                    var betCategoryNodes = oddsNode.ChildNodes;

                    var returnString = String.Format("{0} {1} vs {2} at {3:D} with total bet categories => {4} is slated for LiveBetting: {5}",
                        betServiceMatchNo,
                        homeTeamNode.Attributes["name"].InnerText, 
                        awayTeamNode.Attributes["name"].InnerText, 
                        startTime,
                        betCategoryNodes.Count,
                        isForLiveBet);

                    // at this point we are able to save only games that have odds in them and not omuwawa gwe mipiira
                    Clients.Caller.uploadprogress(returnString);
                    
                    // Try Creating a match object
                    // Section 1.0 Away Team Object
                    //var country = context.

                    //var match = new Match()
                    //{

                    //};

                    var matchodds = new List<MatchOdd>();

                    // Extractiing the Bet Category Options
                    foreach (XmlNode betCategoryNode in betCategoryNodes)
                    {
                        // check if it has bet options and attributes
                        if (!betCategoryNode.HasChildNodes || betCategoryNode.Attributes == null) continue;
                        var betCategoryName = betCategoryNode.Attributes["name"].InnerText;
                        var betCategory = new BetCategory
                        {
                            Name = betCategoryName
                        };
                        context.BetCategories.AddOrUpdate(bc => bc.Name, betCategory);
                        await context.SaveChangesAsync();
                        Clients.Caller.uploadprogress(betCategory);

                        switch (betCategoryName)
                        {
                            // 1.0 ::: Goal Goal (GG) - No Goal (NG)
                            #region Both Teams To Score
                            case "Total Goals Odd/Even":
                                Clients.Caller.uploadprogress("Total Goals Odd/Even");
                                foreach (XmlNode oddNode in betCategoryNode.ChildNodes)
                                {
                                    try
                                    {
                                        if (oddNode.Attributes == null) continue;
                                        var option = oddNode.Attributes["name"].InnerText;
                                        const string line = "";
                                        var unformattedLastUpdateTime = oddNode.Attributes["last_updated"].InnerText;
                                        var odd = Convert.ToDecimal(oddNode.Attributes["odd"].InnerText);
                                        var betoption = new BetOption
                                        {
                                            BetCategoryId = betCategory.CategoryId,
                                            Line = line,
                                            Option = option,
                                        };

                                        context.BetOptions.AddOrUpdate(bo => new { bo.BetCategoryId, bo.Option, bo.Line }, betoption);
                                        await context.SaveChangesAsync();
                                        Clients.Caller.uploadprogress(betoption);

                                        #region DateTime Fix
                                        var oddDateString = (unformattedLastUpdateTime.Split(new[] { ' ' })[0]).Split(new[] { '/' });
                                        var oddTimeString = (unformattedLastUpdateTime.Split(new[] { ' ' })[1]).Split(new[] { ':' });
                                        var oddyear = Int32.Parse(oddDateString[2]) + 2000;
                                        var oddmonth = Int32.Parse(oddDateString[1]);
                                        var odddate = Int32.Parse(oddDateString[0]);
                                        var oddhour = Int32.Parse(oddTimeString[0]);
                                        var oddminutes = Int32.Parse(oddTimeString[1]);
                                        #endregion
                                        var lastUpdateTime = new DateTime(oddyear, oddmonth, odddate, oddhour, oddminutes, 0);
                                        matchodds.Add(new MatchOdd
                                        {
                                            BetOptionId = betoption.BetOptionId,
                                            BetServiceMatchNo = betServiceMatchNo,
                                            LastUpdateTime = lastUpdateTime,
                                            Odd = odd
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Clients.Caller.uploadprogress(ex.Message);
                                    }
                                }
                                break;
                            #endregion
                        }

                        Clients.Caller.uploadprogress(matchodds);
                    }
                    #region First Comment This Out

                    //    foreach (XmlNode oddType in gameOdds)
                    //    {
                    //        var bettype = oddType.Attributes["name"].InnerText;
                    //        switch (bettype)
                    //        {
                    //            case "Full Time Result":
                    //                var normalOdds = oddType.ChildNodes;
                    //                foreach (XmlNode normalodd in normalOdds)
                    //                {
                    //                    var choice = normalodd.Attributes.GetNamedItem("name").Value;
                    //                    if (choice == _homeTeam.TeamName + " Win")
                    //                    {
                    //                        gameodds.Add(new MatchOdd
                    //                        {
                    //                            //1
                    //                            BetOptionId = 1,
                    //                            Odd =
                    //                                Convert.ToDecimal(
                    //                                    normalodd.Attributes.GetNamedItem("odd").Value)
                    //                        });
                    //                    }
                    //                    else if (choice == "Draw")
                    //                    {
                    //                        gameodds.Add(new MatchOdd
                    //                        {
                    //                            // X
                    //                            BetOptionId = 2,
                    //                            Odd =
                    //                                Convert.ToDecimal(
                    //                                    normalodd.Attributes.GetNamedItem("odd").Value)
                    //                        });
                    //                    }
                    //                    else if (choice == _awayTeam.TeamName + " Win")
                    //                    {
                    //                        gameodds.Add(new MatchOdd
                    //                        {
                    //                            // 2
                    //                            BetOptionId = 3,
                    //                            Odd =
                    //                                Convert.ToDecimal(
                    //                                    normalodd.Attributes.GetNamedItem("odd").Value)
                    //                        });
                    //                    }
                    //                }
                    //                break;
                    //            case "Double Chance":
                    //                gameodds.AddRange(new[]
                    //                                        {
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //1X
                    //                                                BetOptionId = 21,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[2].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[2].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //12
                    //                                                BetOptionId = 22,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[1].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //X2
                    //                                                BetOptionId = 23,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[0].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            }
                    //                                        });
                    //                break;
                    //            case "Half-Time":
                    //                gameodds.AddRange(new[]
                    //                                        {
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //1X
                    //                                                BetOptionId = 12,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[0].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //12
                    //                                                BetOptionId = 13,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[1].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //X2
                    //                                                BetOptionId = 14,
                    //                                                Odd =
                    //                                                    oddType.ChildNodes[2].Attributes != null
                    //                                                        ? Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[2].Attributes["odd"].Value)
                    //                                                        : 0
                    //                                            }
                    //                                        });
                    //                break;
                    //            case "Handicap Result":
                    //                //var hhome = Convert.ToInt16(oddType.ChildNodes[0].Attributes["extravalue"].Value);
                    //                //var haway = Convert.ToInt16(oddType.ChildNodes[2].Attributes["extravalue"].Value);
                    //                gameodds.AddRange(new[]
                    //                                        {
                    //                                            new MatchOdd
                    //                                            {
                    //                                                // Handicap Home HC1
                    //                                                BetOptionId = 24,
                    //                                                Odd =
                    //                                                    Convert.ToDecimal(
                    //                                                        oddType.ChildNodes[0].Attributes["odd"].Value),
                    //                                                //Line = hhome < 0 ? hhome - hhome : hhome
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                // HAndicap Away HC2
                    //                                                BetOptionId = 31,
                    //                                                Odd =
                    //                                                    Convert.ToDecimal(
                    //                                                        oddType.ChildNodes[1].Attributes["odd"].Value),
                    //                                                //Line = haway < 0 ? (haway - haway) : haway
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                // HAndicap Away HC2
                    //                                                BetOptionId = 25,
                    //                                                Odd =
                    //                                                    Convert.ToDecimal(
                    //                                                        oddType.ChildNodes[2].Attributes["odd"].Value),
                    //                                                //Line = haway < 0 ? (haway - haway) : haway
                    //                                            }

                    //                                        });
                    //                break;

                    //            case "Draw No Bet":
                    //                gameodds.AddRange(new[]
                    //                                        {
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //DNB1
                    //                                                BetOptionId = 28,
                    //                                                Odd =
                    //                                                    Convert.ToDecimal(
                    //                                                        oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                            },
                    //                                            new MatchOdd
                    //                                            {
                    //                                                //DNB2
                    //                                                BetOptionId = 29,
                    //                                                Odd =
                    //                                                    Convert.ToDecimal(
                    //                                                        oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                            }
                    //                                        });
                    //                break;

                    //            

                    //            case "Total Goals":
                    //                gameodds.AddRange(new[]
                    //                                            {
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTUnder2.5
                    //                                                    BetOptionId = 6,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver2.5
                    //                                                    BetOptionId = 7,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                                }
                    //                                            });
                    //                break;

                    //            case "Alternative Total Goals":
                    //                gameodds.AddRange(new[]
                    //                                            {
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTUnder0.5
                    //                                                    BetOptionId = 32,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver0.5
                    //                                                    BetOptionId = 33,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTUnder1.5
                    //                                                    BetOptionId = 4,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[3].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver1.5
                    //                                                    BetOptionId = 5,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[2].Attributes["odd"].Value)
                    //                                                },

                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver3.5
                    //                                                    BetOptionId = 9,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[4].Attributes["odd"].Value)
                    //                                                },
                    //                                                    new MatchOdd
                    //                                                {
                    //                                                    // FTUnder3.5
                    //                                                    BetOptionId = 8,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[5].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTUnder4.5
                    //                                                    BetOptionId = 10,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[7].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver4.5
                    //                                                    BetOptionId = 11,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[6].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTUnder5.5
                    //                                                    BetOptionId = 34,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[7].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    // FTOver5.5
                    //                                                    BetOptionId = 35,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[6].Attributes["odd"].Value)
                    //                                                }
                    //                                            });
                    //                break;
                    //            case "First Half Goals":
                    //                gameodds.AddRange(new[]
                    //                                            {
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTUnder0.5
                    //                                                    BetOptionId = 15,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[1].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTOver0.5
                    //                                                    BetOptionId = 16,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[0].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTUnder1.5
                    //                                                    BetOptionId = 17,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[3].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTOver1.5
                    //                                                    BetOptionId = 18,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[2].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTUnder2.5
                    //                                                    BetOptionId = 19,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[5].Attributes["odd"].Value)
                    //                                                },
                    //                                                new MatchOdd
                    //                                                {
                    //                                                    //HTOver2.5
                    //                                                    BetOptionId = 20,
                    //                                                    Odd =
                    //                                                        Convert.ToDecimal(
                    //                                                            oddType.ChildNodes[4].Attributes["odd"].Value)
                    //                                                }
                    //                                            });
                    //                break;
                    //        }
                    //        //        //game.MatchOdds = gameodds;
                    //        //        //game.BetServiceMatchNo = Convert.ToInt32(goalServeMatchId);
                    //        //        //game.StartTime = Convert.ToDateTime(stDateTime).ToLocalTime();
                    //        //        //game.ResultStatus = 1;
                    //        //        //game.MatchOdds.ForEach(g => g.BetServiceMatchNo = game.BetServiceMatchNo);
                    //        //        //context.Matches.AddOrUpdate(game);
                    //        //        //await context.SaveChangesAsync();
                    //    }
                    //}

                    #endregion
                }
            }
            return await Task.FromResult(0);
        } 
    }
}