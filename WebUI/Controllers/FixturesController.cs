using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebUI.Helpers;


namespace WebUI.Controllers
{
    public class FixturesController : CustomController
    {
        // GET: Fixtures
        public ActionResult GenerateFixtures()
        {
            //var fullTime = "FULL-TIME";
            //var halfTime = "HALF-TIME";
            //var doubleChance = "DOUBLE CHANCE";
            //var vdrawNoBet = "DRAW NO BET";
            //var bothTeams = "BOTH TEAMS";
            //var handCap = "HANDCAP";
            //var oneExTwo = "1X2";
            //var twoPointFive = "2.5";

            var headings0 = new[]
            {
                new {NO="No",TIME="Time",LEAGUE="League",HOME="Home",AWAY="Away"}
            };

            var headings1=new []
            {
                new
                {
                    ftOneExTwo="1X2",
                    ftzeroPointFive="0.5",
                    ftOnePointfive="1.5", 
                    ftTwoPointFive="2.5",
                    ftThreePointfive="3.5", 
                    ftFourPointFive="4.5",
                    ftFivePointfive="5.5",                    
                    htOneExTwo2="1X2",
                    htZeroPointFive="0.5",
                    htOnePointfive2="1.5",                   
                    doubleChance = "DOUBLE CHANCE",
                    vdrawNoBet = "DRAW NO BET",
                    bothTeams = "BOTH TEAMS",
                    handCap = "HANDCAP"
                }
            };



          

            var workBook=new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("BetWayFixture1");
            //Merges
            workSheet.Range("A1:E3").Merge(); 
            workSheet.Range("A4:E4").Merge();
            workSheet.Range("F1:AA1").Merge();
            workSheet.Range("F2:AA2").Merge().Value = "Contact: 0200 - 692 - 014                                      Email: customercare@betway.ug";
            workSheet.Range("F4:L4").Merge();
            workSheet.Range("M4:S4").Merge();
            workSheet.Range("T5:V5").Merge();
            workSheet.Range("W5:X5").Merge();
            //Merge A5 to D5 for geadings
            workSheet.Range("A5:A6").Merge();
            workSheet.Range("B5:B6").Merge();
            workSheet.Range("C5:C6").Merge();
            workSheet.Range("D5:D6").Merge();
            workSheet.Range("E5:E6").Merge();



            //_______________________OPTIONS________________________
            //FULLTIME
            workSheet.Cell("F6").Value = "1";
            workSheet.Cell("G6").Value = "X";
            workSheet.Cell("H6").Value = "2";
            //0.5
            workSheet.Cell("I6").Value = "Under";
            workSheet.Cell("J6").Value = "Over";
            //1.5
            workSheet.Cell("K6").Value = "Under";
            workSheet.Cell("L6").Value = "Over";
            //2.5
            workSheet.Cell("M6").Value = "Under";
            workSheet.Cell("N6").Value = "Over";
            //3.5
            workSheet.Cell("O6").Value = "Under";
            workSheet.Cell("P6").Value = "Over";
            //4.5
            workSheet.Cell("Q6").Value = "Under";
            workSheet.Cell("R6").Value = "Over";
            //5.5
            workSheet.Cell("S6").Value = "Under";
            workSheet.Cell("T6").Value = "Over";

            workSheet.Cell("U6").Value = "Under";
            workSheet.Cell("V6").Value = "Over";

            //HALF TIME
            workSheet.Cell("W6").Value = "1";
            workSheet.Cell("X6").Value = "X";
            workSheet.Cell("Y6").Value = "2";

            workSheet.Cell("Z6").Value = "Under";
            workSheet.Cell("AA6").Value = "Over";

            workSheet.Cell("AB6").Value = "Under";
            workSheet.Cell("AC6").Value = "Over";

            //DOUBLE CHANCE
            workSheet.Cell("AD6").Value = "1X";
            workSheet.Cell("AE6").Value = "12";
            workSheet.Cell("AF6").Value = "X2";
   
            //BOTH TEAMS TO SCORE
            workSheet.Cell("AG6").Value = "HOME";
            workSheet.Cell("AH6").Value = "AWAY";
            //DRAW NO BET
            workSheet.Cell("AI6").Value = "Yes";
            workSheet.Cell("AJ6").Value = "No";

            //HANDICAP
            workSheet.Cell("AK6").Value = "Arg";
           

            workSheet.Cell("AL6").Value = "1";
            workSheet.Cell("AM6").Value = "X";
            workSheet.Cell("AN6").Value = "2";


            workSheet.Range("B5:B6").Merge();
            workSheet.Range("C5:C6").Merge();
            workSheet.Range("D5:D6").Merge();
            workSheet.Range("E5:E6").Merge();
           

            //FONTS AND DECORATIONS
            workSheet.Range("A5:E6").Style
                .Font.SetBold(true)
                .Font.SetFontName("Bookman Old Style")
                .Font.SetFontSize(12)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
                .Border.SetInsideBorder(XLBorderStyleValues.Thick);
                
              
            workSheet.Columns(1, 22).AdjustToContents();
            workSheet.Range("F4:AN4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            workSheet.Range("F4:AN4").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            workSheet.Range("F5:AN6").Style.Border.InsideBorder = XLBorderStyleValues.Thick;
            workSheet.Range("F5:AN5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            workSheet.Range("F5:AN5").Style.Border.OutsideBorder=XLBorderStyleValues.Thick;
            workSheet.Cell("A5").Value = headings0;
            //FULLTIME OPTIONS  COLUMN HEADINDS
            workSheet.Range("F5:H5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("F5").Value = "1x2";
            workSheet.Range("I5:J5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("I5").Value = "0.5";          
            workSheet.Range("K5:L5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("K5").Value = "1.5";
            workSheet.Range("M5:N5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("M5").Value = "2.5";
            workSheet.Range("O5:P5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("O5").Value = "3.5";
            workSheet.Range("Q5:R5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("Q5").Value = "4.5";
            workSheet.Range("S5:T5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("T5").Value = "5.5";
            
            //HALF TIME OPTIONS  COLUMN HEADINDS
            workSheet.Range("U5:W5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("U5").Value = "1x2";
            workSheet.Range("X5:Y5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("X5").Value = "1.5";
            workSheet.Range("Z5:AA5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("Z5").Value = "0.5";
            workSheet.Range("AD5:AF5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("AD5").Value = "Double Chance";
            workSheet.Range("AI5:AJ5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("AI5").Value = "Draw No Bet";
            workSheet.Range("AG5:AH5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("AG5").Value = "Both Team";
            workSheet.Range("AK5:AN5").Merge().Style.Font.SetBold(true);
            workSheet.Cell("AK5").Value = "Handcap";

            workSheet.Range("A1:E3").Merge().Value = "BETWAY FIXTURE";
            workSheet.Range("A1:E3").Style
                .Font.SetFontSize(24)
                .Font.SetFontColor(XLColor.BrickRed)
                .Font.SetBold(true)
                .Font.SetFontName("Bookman Old Style");

            workSheet.Range("F1:Z1").Merge().Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            workSheet.Range("A1:E3").Style
                .Font.SetFontSize(20)
                .Font.SetFontColor(XLColor.BrickRed)
                .Font.SetBold(true)
                .Font.SetFontName("Bookman Old Style");

           
            workSheet.Range("F6:AC6").Style
               .Font.SetFontName("Bookman Old Style")
               //.Font.SetFontSize(12)
               .Font.SetBold(false);
            //F4 :V4
            workSheet.Cell("F4").Value = "FULL TIME";
           
            //W4:AC4
            workSheet.Cell("W4").Value = "HALF TIME";
        

            workSheet.Range("F7:AC7").Style
                .Font.SetFontName("Bookman Old Style")
                //.Font.SetFontSize(5)
                .Font.SetBold(false);
            workSheet.Cell("F7").Value = headings1;
           // workSheet.Cell("A7").Style.NumberFormat.SetFormat(IXLNumberFormat);
            var games = GetGames();
           
            workSheet.Range("AK5:AN5").Merge();
            //ADJUST COLUMN WIDTH
            workSheet.Columns("D:E").Width = 18.86;
           // workSheet.Columns("A:E").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            var ws = workSheet.Columns("F:AN");
           
            ws.Width = 6.14;
            ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
           // ws.Style.Border.InsideBorder=XLBorderStyleValues.Medium;
           // workSheet.RowsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            var numberOfGames = games.Count()+6;
            var oddsLastCell = "F7:" + "AN" + numberOfGames;
            var teamsLastCell = "A7:" + "E" + numberOfGames;
            workSheet.Range(oddsLastCell).Style.NumberFormat.SetNumberFormatId(3);
            workSheet.Range(oddsLastCell).Style.Border.InsideBorder = XLBorderStyleValues.Hair;
            workSheet.Range(teamsLastCell).Style.Border.InsideBorder = XLBorderStyleValues.Hair;
            var fthtlastCell = "F7:" + "S" + numberOfGames;
            workSheet.Range(fthtlastCell).Style.Border.BottomBorder= XLBorderStyleValues.Thin;
            var ft1X2LastCell = "F7:" + "H" + numberOfGames;
            workSheet.Range(ft1X2LastCell).Style.Border.OutsideBorder= XLBorderStyleValues.Thin;


            var ftUo05LastCell = "I7:" + "J" + numberOfGames;
            workSheet.Range(ftUo05LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var ftUo15LastCell = "K7:" + "L" + numberOfGames;
            workSheet.Range(ftUo15LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var ftUo25LastCell = "M7:" + "N" + numberOfGames;
            workSheet.Range(ftUo25LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var ftUo35LastCell = "O7:" + "P" + numberOfGames;
            workSheet.Range(ftUo35LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var ftUo45LastCell = "Q7:" + "R" + numberOfGames;
            workSheet.Range(ftUo45LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var ftUo55LastCell = "S7:" + "T" + numberOfGames;
            workSheet.Range(ftUo55LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //HALF TIME CELL RANGES FORM LINES
            var ht1X2LastCell = "U7:" + "W" + numberOfGames;
            workSheet.Range(ht1X2LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var htUo05LastCell = "X7:" + "Y" + numberOfGames;
            workSheet.Range(htUo05LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var htUo15LastCell = "Z7:" + "AA" + numberOfGames;
            workSheet.Range(htUo15LastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var doubleChanceLastCell = "AB7:" + "AD" + numberOfGames;
            workSheet.Range(doubleChanceLastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var drawNoBetLastCell = "AE7:" + "AF" + numberOfGames;
            workSheet.Range(drawNoBetLastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var bothTeamLastCell = "AG7:" + "AI" + numberOfGames;
            workSheet.Range(bothTeamLastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var handCupLastCell = "AJ7:" + "AI" + numberOfGames;
            workSheet.Range(handCupLastCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var fixtureCell = "A1:" + "AN" + numberOfGames;
            workSheet.Range(fixtureCell).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            workSheet.Range("F6:AN6").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            workSheet.Range("F4:V4").Style.Border.RightBorder = XLBorderStyleValues.Thick;
            workSheet.Range("W4:AC4").Style.Border.RightBorder = XLBorderStyleValues.Thick;
          
           // workSheet.Range("T4:V4").Style.Border.RightBorder = XLBorderStyleValues.Thick;


            workSheet.Cell("A7").Value = games;
            return new ExcelResult(workBook, "BetWayFixture"+DateTime.Now.Date);
    }

        public string ConvertDateToTime(DateTime dateParam)
        {
            var date = dateParam.TimeOfDay;
            return date.ToString();
        }

        public List<FixtureViewModel> GetGames()
        {
            //var games =  BetDatabase.Matches.Include(g => g.AwayTeam).Include(g => g.AwayTeam.League).Include(g => g.HomeTeam).Include(g => g.MatchOdds.Select(c => c.BetOption)).ToList();

            var constring = ConfigurationManager.ConnectionStrings["BetConnection"].ConnectionString;
            var con = new SqlConnection(constring);
            const string query = "select M.BetServiceMatchNo as BetServiceMatchNo,sm.shortcode as ShortCode,[dbo].[ReturnTeamName](HomeTeamId) as HomeTeam,M.League as League, " +
                                 "[dbo].[ReturnTeamName](AwayTeamId) as AwayTeam," +
                                 "convert(varchar(10),(DATEPART(HH,M.starttime)))+':'+Convert(varchar(10),DATEPART(mi,M.starttime)) as starttime ," +
                                 "[dbo].[GetOdd]('FT 1x2','FT1',M.BetServiceMatchNo) as FT1," +
                                 "[dbo].[GetOdd]('FT 1x2','FTX',M.BetServiceMatchNo) as FTX," +
                                 "[dbo].[GetOdd]('FT 1x2','FT2',M.BetServiceMatchNo) as FT2," +
                                 "[dbo].[GetOdd]('Double Chance','1X',M.BetServiceMatchNo) as [1X]," +
                                 "[dbo].[GetOdd]('Double Chance','12',M.BetServiceMatchNo) as [12]," +
                                 "[dbo].[GetOdd]('Double Chance','X2',M.BetServiceMatchNo) as [X2]," +
                                  "[dbo].[GetOdd]('FT U/O','FTUnder0.5',M.BetServiceMatchNo) as [FTUnder0.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTOver0.5',M.BetServiceMatchNo) as [FTOver0.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTUnder1.5',M.BetServiceMatchNo) as [FTUnder1.5]," +
                                  "[dbo].[GetOdd]('FT U/O','FTOver1.5',M.BetServiceMatchNo) as [FTOver1.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTUnder2.5',M.BetServiceMatchNo) as [FTUnder2.5]," +
                                  "[dbo].[GetOdd]('FT U/O','FTOver2.5',M.BetServiceMatchNo) as [FTOver2.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTUnder3.5',M.BetServiceMatchNo) as [FTUnder3.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTOver3.5',M.BetServiceMatchNo) as [FTOver3.5]," +
                                  "[dbo].[GetOdd]('FT U/O','FTUnder4.5',M.BetServiceMatchNo) as [FTUnder4.5]," +
                                  "[dbo].[GetOdd]('FT U/O','FTOver4.5',M.BetServiceMatchNo) as [FTOver4.5]," +
                                 "[dbo].[GetOdd]('FT U/O','FTUnder5.5',M.BetServiceMatchNo) as [FTUnder5.5]," +
                                  "[dbo].[GetOdd]('FT U/O','FTOver5.5',M.BetServiceMatchNo) as [FTUnder5.5]," +                                 
                                 "[dbo].[GetOdd]('HT 1x2','HT1',M.BetServiceMatchNo) as HT1," +
                                 "[dbo].[GetOdd]('HT 1x2','HTX',M.BetServiceMatchNo) as HTX," +
                                 "[dbo].[GetOdd]('HT 1x2','HT2',M.BetServiceMatchNo) as HT2," +
                                 "[dbo].[GetOdd]('HT U/O','HTOver0.5',M.BetServiceMatchNo) as [HTOver0.5]," +
                                 "[dbo].[GetOdd]('HT U/O','HTUnder0.5',M.BetServiceMatchNo) as [HTUnder0.5]," +
                                 "[dbo].[GetOdd]('HT U/O','HTOver1.5',M.BetServiceMatchNo) as [HTOver1.5]," +
                                 "[dbo].[GetOdd]('HT U/O','HTUnder1.5',M.BetServiceMatchNo) as [HTUnder1.5]," +
                                  "[dbo].[GetOdd]('HT U/O','HTOver2.5',M.BetServiceMatchNo) as [HTOver2.5]," +
                                 "[dbo].[GetOdd]('HT U/O','HTUnder2.5',M.BetServiceMatchNo) as [HTUnder2.5]," +
                                 "[dbo].[GetOdd]('Both Teams To Score','GG',M.BetServiceMatchNo) as BTYes," +
                                 "[dbo].[GetOdd]('Both Teams To Score','NG',M.BetServiceMatchNo) as BTNo," +
                                "[dbo].[GetOdd]('Handicap','HC1',M.BetServiceMatchNo) as Handicap1," +
                                   "[dbo].[GetOdd]('Handicap','HCX',M.BetServiceMatchNo) as HandicapX," +
                                     "[dbo].[GetHandicapGoals] (M.BetServiceMatchNo) as Goals," +
                                   "[dbo].[GetOdd]('Handicap','HC2',M.BetServiceMatchNo) as Handicap2," +
                                 "[dbo].[GetOdd]('Draw No Bet','DNB1',M.BetServiceMatchNo) as DNB1," +
                                 "[dbo].[GetOdd]('Draw No Bet','DNB2',M.BetServiceMatchNo) as DNB2 from Matches M inner join dbo.ShortMatchCodes sm on sm.MatchNo=m.BetServiceMatchNo "
                                 + " where(StartTime>getdate()) and m.BetServiceMatchNo in (select MO.BetServiceMatchNo from MatchOdds MO) order by sm.shortcode asc ";
            var dt = new DataTable();
            con.Open();
            var da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();
            //var rowNo=0;
            var model = new List<FixtureViewModel>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
               
                model.Add(
                    new FixtureViewModel
                    {
                        ShortCode = (int)(dt.Rows[i]["ShortCode"] ?? "-"),//_no++ ,// Convert.ToInt32(dt.Rows[i]["BetServiceMatchNo"]),
                        League = (dt.Rows[i]["League"] ?? "-").ToString(),
                        Time =Convert.ToDateTime(dt.Rows[i]["starttime"]).TimeOfDay.ToString(),
                        Home = (dt.Rows[i]["HomeTeam"] ?? "-").ToString(),
                        Away = (dt.Rows[i]["AwayTeam"] ?? "-").ToString(),
                        //FULL TIME
                        FT1X = (dt.Rows[i]["FT1"] ?? "-").ToString(),
                        FTX = (dt.Rows[i]["FTX"] ?? "-").ToString(),
                        FTX2 = (dt.Rows[i]["FT2"] ?? "-").ToString(),
                        //FULLTIME OVER UNDER 0.5
                        FTU05 = (dt.Rows[i]["FTUnder0.5"] ?? "-").ToString(),
                        FTO05 = (dt.Rows[i]["FTOver0.5"] ?? "-").ToString(),
                        //FULLTIME OVER UNDER 1.5
                        FTU15 = (dt.Rows[i]["FTUnder1.5"] ?? "-").ToString(),
                        FTO15 = (dt.Rows[i]["FTOver1.5"] ?? "-").ToString(),
                        //FULLTIME OVER UNDER 2.5
                        FTU25 = (dt.Rows[i]["FTUnder2.5"] ?? "-").ToString(),
                        FTO25 = (dt.Rows[i]["FTOver2.5"] ?? "-").ToString(),
                      
                        //FULLTIME OVER UNDER 3.5
                        FTU35 = (dt.Rows[i]["FTUnder3.5"] ?? "-").ToString(),
                        FTO35 = (dt.Rows[i]["FTOver3.5"] ?? "-").ToString(),
                        //FULLTIME OVER UNDER 4.5
                        FTU45 = (dt.Rows[i]["FTUnder4.5"] ?? "-").ToString(),
                        FTO45 = (dt.Rows[i]["FTOver4.5"] ?? "-").ToString(),
                        //FULLTIME OVER UNDER 5.5
                        FTU55 = (dt.Rows[i]["FTUnder5.5"] ?? "-").ToString(),
                        FTO55 = (dt.Rows[i]["FTOver5.5"] ?? "-").ToString(),



                        //HALF TIME
                        HT1X = (dt.Rows[i]["HT1"] ?? "-").ToString(),
                        HTX = (dt.Rows[i]["HTX"] ?? "-").ToString(),
                        HTX2 = (dt.Rows[i]["HT2"] ?? "-").ToString(),
                        //HALFTIME OVER UNDER 0.5
                        HTU05 = (dt.Rows[i]["HTOver0.5"] ?? "-").ToString(),
                        HTO05 = (dt.Rows[i]["HTUnder0.5"] ?? "-").ToString(),
                       
                        //HALFTIME OVER UNDER 1.5
                        HTU15 = (dt.Rows[i]["HTUnder1.5"] ?? "-").ToString(),
                        HTO15 = (dt.Rows[i]["HTOver1.5"] ?? "-").ToString(),
                    

                        //DOUBLE CHANCE
                        DCX1 = (dt.Rows[i]["1X"] ?? "-").ToString(),
                        DCX = (dt.Rows[i]["12"] ?? "-").ToString(),
                        DCX2 = (dt.Rows[i]["X2"] ?? "-").ToString(),
                       
                        //DRAW NO BET
                        //DNBAway = (dt.Rows[i]["DNB2"] ?? "-").ToString(),
                        //DNBHome = (dt.Rows[i]["DNB1"] ?? "-").ToString(),
                        //BOTH TEAMS TO SCORE
                        BTYes = (dt.Rows[i]["BTYes"] ?? "-").ToString(),
                        BTNo = (dt.Rows[i]["BTNo"] ?? "-").ToString(),
                        //HANDCAP
                        HDCGoals = (dt.Rows[i]["Goals"] ?? "-").ToString(),
                        HDC1X = (dt.Rows[i]["Handicap1"] ?? "-").ToString(),
                        HDCX = (dt.Rows[i]["HandicapX"] ?? "-").ToString(),
                        HDCX2 = (dt.Rows[i]["Handicap2"] ?? "-").ToString(),
                       
                       
                      

                    });
            }
            var gv = new GridView { DataSource = dt };
            gv.DataBind();
            //Session["Doc"] = gv; 

            return model;
            //BetServiceMatchNo = g.BetServiceMatchNo.ToString(),

            //var _no = 1;
            //var filteredgames = games.Select(g => new FixtureViewModel
            //{
            //    No = _no++,
            //    Time=g.StartTime.TimeOfDay.ToString(),
            //    League ="",
            //    Home = g.HomeTeam.TeamName,
            //    Away = g.AwayTeam.TeamName,
            //    FT1X =  2.05  ,
            //    FX =2.05,
            //    FTX2 =   2.05                       ,
            //    HT1X =   2.05                       ,
            //    HX =      2.05                    ,
            //    HTX2 =      2.05                    ,
            //     DC1X =      2.05                    ,
            //    DCX =       2.05                   ,
            //    DCX2 =      2.05                    ,
            //    DNBAway =   2.05                       ,
            //    DNBHome =   2.05                       ,
            //    BTYes =      2.05                    ,
            //    BTNo =           2.05               ,
            //     HDCArgs =           2.05               ,
            //    HDCOne =  2.05         ,
            //    HDCX =   2.05   ,
            //    HDC2 =                          2.05,



            //});

            //return filteredgames.ToList();
        }

        public void GenearateExcelFixture(string value, string cellRange, int fontSize, string fontColor, string fontStyle, List<string> headings)
        {
            throw new NotImplementedException();
        }

        // GET: Fixtures/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Fixtures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Fixtures/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Fixtures/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Fixtures/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Fixtures/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Fixtures/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
