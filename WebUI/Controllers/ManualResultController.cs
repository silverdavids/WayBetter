using Domain.Models.Concrete;
using Domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class ManualResultController : CustomController
    {
        // GET: ManualResult
        public ActionResult Index()
        {
           
                 return View();
        }
      //  [HttpPost]
        public JsonResult MatchList()
        {
            try
            {
                DateTime startTime = DateTime.Now.AddHours(-24);
                DateTime endTime = DateTime.Now.AddHours(-2);
                var MatchList =
                    BetDatabase.Matches.Where(x=>x.ResultStatus==1&&x.StartTime>startTime&&x.StartTime<endTime&&x.BetServiceMatchNo<10000).OrderByDescending(x=>x.StartTime).Take(1000).ToList();
                int count = MatchList.Count;
                List<MatchScores> score=new List<MatchScores>();
                foreach (Match match in MatchList)
                {
                   MatchScores ms=new MatchScores();
                    ms.MatchNo = match.BetServiceMatchNo;
                    ms.HomeTeam = match.HomeTeam.TeamName;
                    ms.AwayTeam = match.AwayTeam.TeamName;
                    ms.League = match.League;
                    ms.HomeScore = 0;
                    ms.AwayScore = 0;
                    ms.HalfTimeHomeScore = 0;
                    ms.HalfTimeAwayScore = 0;
                    ms.StartTime = match.StartTime.ToString();
                    score.Add(ms);
                }
                return Json(new { Result = "OK", Records = score }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult UpdateMatch(MatchScores _match)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        Result = "ERROR",
                        Message = "Form is not valid! " +
                          "Please correct it and try again."
                    });
                }

                AddScores( _match);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public int AddScores(MatchScores _match)
        {
            int AddedScores = 0;
            int matchNo =_match.MatchNo;
            int homeGoals = _match.HomeScore;
            int awayGoals =_match.AwayScore;
            int homeGoalsHT = _match.HalfTimeHomeScore;
            int awayGoalsHT = _match.HalfTimeAwayScore;
            try
            {
                        
                                 DateTime start = DateTime.Now;
                                  string goalServeMatchID = matchNo.ToString();
                                        try
                                        {
                                          
                                           int mtcno = Convert.ToInt32(goalServeMatchID);
                                            var mtc = BetDatabase.Matches.Include(x => x.AwayTeam).Include(x => x.HomeTeam).Where(c=>c.ResultStatus==1).Single(m => m.BetServiceMatchNo == mtcno);
                                            if (mtc == null)
                                            {                                           
                                            }
                                            // Match mtc = BetDatabase.Matches.Where(mt => mt.BetServiceMatchNo == mtcno).SingleOrDefault();

                                            try
                                            {
                                                mtc.HomeScore = homeGoals;
                                                mtc.AwayScore = awayGoals;
                                                mtc.HalfTimeAwayScore = awayGoalsHT;
                                                mtc.HalfTimeHomeScore = homeGoalsHT;
                                                mtc.ResultStatus = 2;
                                                // BetDatabase.Matches.Attach(mtc);
                                                BetDatabase.Entry(mtc).State = EntityState.Modified;
                                                BetDatabase.SaveChanges();
                                                AddedScores++;
                                            }
                                            catch (DbEntityValidationException ex)
                                            {
                                                StringBuilder sb = new StringBuilder();
                                                foreach (var failure in ex.EntityValidationErrors)
                                                {
                                                    sb.AppendFormat("{0} failed validation\n",
                                                        failure.Entry.Entity.GetType());
                                                    foreach (var error in failure.ValidationErrors)
                                                    {
                                                        sb.AppendFormat("- {0} : {1}", error.PropertyName,
                                                            error.ErrorMessage);
                                                        sb.AppendLine();
                                                    }
                                                }
                                                string msg = sb.ToString();
                                            }

                                            List<Result> resultList = new List<Result>();
                                            /* Match Results*/
                                            if (mtc.HomeScore > mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 1).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 1).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                             

                                            }
                                            else if (mtc.HomeScore < mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 1).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 3).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            
                                            }
                                            else if (mtc.HomeScore == mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 1).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 2).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            /*End Match Result */

                                            /* HaltTime Results*/
                                            if (mtc.HalfTimeHomeScore > mtc.HalfTimeAwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 3).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 12).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                          
                                            }
                                            else if (mtc.HalfTimeHomeScore < mtc.HalfTimeAwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 3).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 14).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                           
                                            }

                                            else if (mtc.HalfTimeHomeScore == mtc.HalfTimeAwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 3).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 13).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            /*End HalfTime Result */

                                            ///*Start Draw No Bet*/
                                            //if (mtc.HomeScore > mtc.AwayScore)
                                            //{
                                            //    Result result = new Result();
                                            //    result.MatchId = mtc.BetServiceMatchNo;
                                            //    result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 9).FirstOrDefault().CategoryId;
                                            //    result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 13).FirstOrDefault().BetOptionId;
                                            //    BetDatabase.Results.Add(result);
                                            //    resultList.Add(result);
                                            //    BetDatabase.SaveChanges();
                                            //}
                                            //else if (mtc.HomeScore < mtc.AwayScore)
                                            //{
                                            //    Result result = new Result();
                                            //    result.MatchId = mtc.BetServiceMatchNo;
                                            //    result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 9).FirstOrDefault().CategoryId;
                                            //    result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.Option == "DNB2").FirstOrDefault().BetOptionId;
                                            //    BetDatabase.Results.Add(result);
                                            //    resultList.Add(result);
                                            //    BetDatabase.SaveChanges();
                                            //}
                                            //else if (mtc.HomeScore == mtc.AwayScore)
                                            //{
                                            //    Result result = new Result();
                                            //    result.MatchId = mtc.BetServiceMatchNo;
                                            //    result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 9).FirstOrDefault().CategoryId;
                                            //    result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.Option == "DNB2").FirstOrDefault().BetOptionId;

                                            //    BetDatabase.Results.Add(result);
                                            //    resultList.Add(result);
                                            //    BetDatabase.SaveChanges();
                                            //}
                                            ///*End Match Result */

                                            /* Start Double Chance*/
                                            if (mtc.HomeScore > mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 9).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 21).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 22).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);

                                            }
                                            else if (mtc.HomeScore < mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 23).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 22).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);

                                            }

                                            else if (mtc.HomeScore == mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 23).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 21).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);

                                            }
                                            /*End Double Chance */

                                            /* Start Unders Over*/
                                            /* Full Time Total Goals or Wire or underover*/
                                            int FullTimeTotalGoals = (int)(mtc.HomeScore + mtc.AwayScore);
                                            if (FullTimeTotalGoals > 0.5)//over 0.5
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 33).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 0.5)  //under0.5
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 32).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                            
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 1.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 5).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 1.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 4).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                         
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 7).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 6).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                          
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 3.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 9).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 3.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 8).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 4.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 11).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 4.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 12).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 5.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 35).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 5.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 34).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            /*End FullTime U/O/
                                             
                                             * 
                                             /*Start HalfTime Unders Over */
                                            /* 0.5  */
                                            int HalfTimeTotalGoals = (int)(mtc.HalfTimeHomeScore + mtc.HalfTimeAwayScore);
                                            if (HalfTimeTotalGoals > 0.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 16).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (HalfTimeTotalGoals < 0.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 15).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            /* 0.5*/
                                            /*1.5*/
                                            if (HalfTimeTotalGoals > 1.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 18).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }

                                            else if (HalfTimeTotalGoals < 1.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 17).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                BetDatabase.SaveChanges();
                                                resultList.Add(result);
                                            }
                                            /*1.5*/
                                            /*2.5 */
                                            if (HalfTimeTotalGoals > 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 20).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (HalfTimeTotalGoals < 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 4).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 19).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            /* HT2.5*/


                                            /* End 2.5*/

                                            /*End Half Time Unders/Overs
                                            /* Both Teams To Score*/

                                            if ((homeGoals > 0) && (awayGoals > 0))
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 7).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 26).Where(l => l.BetCategoryId == result.CategoryId).FirstOrDefault().BetCategoryId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if ((homeGoals == 0) && (awayGoals == 0))
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 7).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 27).Where(l => l.BetCategoryId == result.CategoryId).FirstOrDefault().BetCategoryId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }

                                            /*End unders Both Teams To Score*/

                                            foreach (Result score in resultList)
                                            {
                                                List<Bet> betted = new List<Bet>();
                                                betted = BetDatabase.Bets.Include(f => f.Receipt).Where(s => s.MatchId == score.MatchId).Where(j => j.BetOption.BetCategoryId == score.CategoryId).Where(r => r.GameBetStatus == 0).ToList();
                                                foreach (Bet bm in betted)
                                                {
                                                    Receipt rec = BetDatabase.Receipts.Where(rc => rc.ReceiptId == bm.RecieptId).Where(rc => rc.ReceiptStatus == 1).SingleOrDefault();
                                                    if (rec == null)
                                                    {
                                                        continue;
                                                    }
                                                    rec.SubmitedSize = rec.SubmitedSize + 1;
                                                
                                                    
                                                    if ((bm.BetOptionId == score.OptionId) && (bm.BetOption.BetCategoryId == score.CategoryId))
                                                    {
                                                        rec.WonSize = rec.WonSize + 1;
                                                        bm.GameBetStatus = 2;
                                                    }
                                                    else  if( (score.CategoryId == 2)||(score.CategoryId == 4))
                                                    {
                                                        int subCategory = getSubCategory(score.OptionId);
                                                        int baseCategory = getSubCategory(bm.BetOptionId);
                                                        if (UnderOverCat(subCategory, baseCategory))
                                                        {
                                                            rec.ReceiptStatus = 2;
                                                            bm.GameBetStatus = 1;
                                                        }
                                                        else
                                                        {
                                                            continue;                                                         
                                                        }
                                                    }

                                                    else
                                                    {
                                                        rec.ReceiptStatus = 2;
                                                        bm.GameBetStatus = 1;
                                                    }
                                                    BetDatabase.Entry(bm).State = EntityState.Modified;
                                                    //  BetDatabase.Entry(bm).Entity = EntityState.Modified;

                                                }


                                            }
                                            BetDatabase.SaveChanges();
                                            List<Receipt> WonReciepts = new List<Receipt>();
                                            WonReciepts = BetDatabase.Receipts.Where(w => w.SetSize == w.WonSize).Where(s => s.ReceiptStatus == 1).ToList();
                                            if (WonReciepts.Count > 0)
                                            {
                                                foreach (Receipt wonrec in WonReciepts)
                                                {
                                                    wonrec.ReceiptStatus = 3;

                                                }
                                                BetDatabase.SaveChanges();
                                            }


                                        }
                                        catch (Exception e)
                                        {
                                            string msg = e.Message;
                                          
                                        }
                                                       
                   
               
            }
            catch (Exception exception) { }
       
            return AddedScores;
        }

        public Boolean UnderOverCat(int baseValue,int compValue)
        {
           Boolean CatUnderOver = false;
            if (baseValue==compValue)
            {
                CatUnderOver = true;
            }
            return CatUnderOver;
        }

        public int getSubCategory(int Option )
        {
            if ((Option == 33) || (Option == 32))//Under Over 0.5
            {
                return 1;
            }
            else if ((Option == 4) || (Option == 5))//Under Over 1.5
            {
                return 2;
            }
            else if ((Option == 6) || (Option == 7))//Under Over 2.5
            {
                return 3;
            }
            else if ((Option == 8) || (Option == 9))//Under Over 3.5
            {
                return 4;
            }
            else if ((Option == 10) || (Option == 11))//Under Over 4.5
            {
                return 5;
            }
            else if ((Option == 15) || (Option == 16))//HtUnder Over 0.5
            {
                return 6;
            }
            else if ((Option == 17) || (Option == 18))//HTUnder Over 1.5
            {
                return 7;
            }
            else if ((Option == 19) || (Option == 20))//HT Under Over 2.5
            {
                return 8;
            }
           
            return 0; 

        }
    }
}