using System.Text;
using Domain.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Xml;
using WebUI.Helpers;
using System.Data.Entity;

namespace WebUI.Controllers
{
    public class ResultController : CustomController
    {
        // GET: Result
        private int count;
        private int countFail;
        private readonly string[] _resultUrls =
        {
     "http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/england",
   " http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/africa",
"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/argentina",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/asia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/australia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/austria",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/belarus",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/belgium",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/brazil",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/bulgaria",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/chile",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/china",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/colombia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/concacaf",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/costarica",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/croatia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/cyprus",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/czechia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/denmark",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/egypt",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/england",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/equador",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/estonia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/eurocups",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/finland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/france ",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/germany",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/greece",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/holland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/hungary",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/iceland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/ireland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/israel",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/italy",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/japan",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/korea",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/latvia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/lithuania",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/mexico",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/norway",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/paraguay",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/peru",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/poland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/portugal",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/romania",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/russia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/saudi_arabia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/scotland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/serbia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/singapore",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/slovakia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/slovenia",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/southafrica",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/southamerica",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/spain",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/sweden",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/switzerland",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/turkey",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/uae",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/ukraine",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/uruguay",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/usa",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/venezuela",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/wales",

"http://www.goalserve.com/getfeed/d1aa4f5599064db8b343090338221a49/soccernew/worldcup",
  
        };
        private static bool _alreadyRunningUpdate;

        public static bool AlreadyRunningUpdate
        {
            get
            {
                lock (RunLock)
                {
                    return _alreadyRunningUpdate;
                }
            }
            set
            {
                lock (RunLock)
                {
                    _alreadyRunningUpdate = value;
                }
            }
        }
        public static object RunLock = new object();
        public ActionResult UpdateScore()
        {
            int AddedScores=score();
            ViewBag.Scores = AddedScores;;
            return View("Index");
        }
        public ActionResult ViewScore()
        {
           score();
            return View("Index");
        }


        [HttpPost]
        public JsonResult ResultProcess()
        {
          int RespCount=  score();
          return Json(new {Resp = RespCount},JsonRequestBehavior.AllowGet);
        }
        public int  score()
        {
            int AddedScores = 0;
            try
            {
                if (!AlreadyRunningUpdate)
                {
                    //AlreadyRunningUpdate = true;                
                    DateTime start = DateTime.Now;            
                    foreach (var iURL in _resultUrls)
                    {
                        var req = (HttpWebRequest)WebRequest.Create(iURL);
                        var res = (HttpWebResponse)req.GetResponse();
                        var stream = res.GetResponseStream();
                        var xmldoc = new XmlDocument();
                       xmldoc.Load(stream);
                        var categoryList = xmldoc.SelectNodes("/scores/category");
                        int loops = 0;
                        string goalServeMatchID = "";
                        foreach (XmlNode categoryNode in categoryList)
                        {
                            XmlNodeList matches = categoryNode.ChildNodes;
                            foreach (XmlNode matchesNode in matches)
                            {
                                loops += 1;

                                XmlNodeList games = matchesNode.ChildNodes;
                                foreach (XmlNode gameNode in games)
                                {
                                    string status = gameNode.Attributes["status"].InnerText.ToString();
                                    if (status == "FT")//update for only full time games
                                    {
                                        XmlNode home = gameNode.ChildNodes[0];
                                        XmlNode away = gameNode.ChildNodes[1];
                                        XmlNode halftime = gameNode.ChildNodes[3];
                                        string hostTeam = home.Attributes["name"].InnerText;
                                        string awayTeam = away.Attributes["name"].InnerText;
                                        goalServeMatchID = gameNode.Attributes["id"].InnerText;
                                        try
                                        {
                                            int homeGoals = Convert.ToInt16(home.Attributes["goals"].InnerText);
                                            int awayGoals = Convert.ToInt16(away.Attributes["goals"].InnerText);
                                            string half_time = halftime.Attributes["score"].InnerText; //format[3-2]
                                            char[] del2 = { '-' };
                                            string[] hf = half_time.Substring(1, half_time.Length - 2).Split(del2);
                                            int homeGoalsHT = Convert.ToInt16(hf[0]);
                                            int awayGoalsHT = Convert.ToInt16(hf[1]);
                                         
                                            int mtcno = Convert.ToInt32(goalServeMatchID);

                                            var mtc = BetDatabase.Matches.Include(x => x.AwayTeam).Include(x => x.HomeTeam).Where(c => c.ResultStatus == 1).Single(m => m.BetServiceMatchNo == mtcno);
                                            if (mtc == null)
                                            {
                                                countFail++;
                                                continue;
                                            }
                                           // Match mtc = BetDatabase.Matches.Where(mt => mt.BetServiceMatchNo == mtcno).SingleOrDefault();

                                            try
                                            {
                                                mtc.HomeScore = homeGoals;
                                                mtc.AwayScore = awayGoals;
                                                mtc.HalfTimeAwayScore = awayGoalsHT;
                                                mtc.HalfTimeHomeScore = homeGoalsHT;
                                               // BetDatabase.Matches.Attach(mtc);
                                                mtc.ResultStatus = 2;
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
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId== 1).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                                BetDatabase.SaveChanges();

                                            }
                                            else if (mtc.HomeScore < mtc.AwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 1).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId ==3).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                                BetDatabase.SaveChanges();
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
                                                BetDatabase.SaveChanges();
                                            }
                                            else if (mtc.HalfTimeHomeScore < mtc.HalfTimeAwayScore)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 3).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(h => h.BetCategoryId == result.CategoryId).Where(c => c.BetOptionId == 14).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                                BetDatabase.SaveChanges();
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
                                                BetDatabase.SaveChanges();
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
                                             //   BetDatabase.SaveChanges();
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 7).FirstOrDefault().BetOptionId;
                                                //BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 2.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 6).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                               // BetDatabase.SaveChanges(); 
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 3.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 9).FirstOrDefault().BetOptionId;
                                               // BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 3.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 8).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                               // BetDatabase.SaveChanges();
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 4.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 11).FirstOrDefault().BetOptionId;
                                               // BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 4.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 12).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                              //  BetDatabase.SaveChanges();
                                                resultList.Add(result);
                                            }
                                            if (FullTimeTotalGoals > 5.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 35).FirstOrDefault().BetOptionId;
                                               // BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (FullTimeTotalGoals < 5.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 34).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                                //BetDatabase.SaveChanges();
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
                                               // BetDatabase.Results.Add(result);
                                                resultList.Add(result);
                                            }
                                            else if (HalfTimeTotalGoals < 0.5)
                                            {
                                                Result result = new Result();
                                                result.MatchId = mtc.BetServiceMatchNo;
                                                result.CategoryId = BetDatabase.BetCategories.Where(c => c.CategoryId == 2).FirstOrDefault().CategoryId;
                                                result.OptionId = BetDatabase.BetOptions.Where(c => c.BetOptionId == 15).FirstOrDefault().BetOptionId;
                                                BetDatabase.Results.Add(result);
                                               // BetDatabase.SaveChanges();
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
                                               // BetDatabase.SaveChanges();
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
                                                //BetDatabase.SaveChanges();
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
                                                betted = BetDatabase.Bets.Include(f =>f.Receipt).Where(s => s.MatchId == score.MatchId).Where(j => j.BetOption.BetCategoryId == score.CategoryId).Where(r=>r.GameBetStatus==0).ToList();
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
                                                    else if (( score.CategoryId==2)||( score.CategoryId==3))
                                                    {
                                                        rec.ReceiptStatus = 2;
                                                        bm.GameBetStatus = 1;
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
                                            countFail++;
                                            continue;
                                        }
                                    }

                                } //end game
                            } //end matches
                            //   LbResult.Text = loops.ToString();

                        } //end category, xml parse         

                        try
                        {
                            xmldoc.Load(new MemoryStream());
                        }
                        catch (Exception ex)
                        {


                        }
                        Thread.Sleep(5000);
                    }

                    var totalTime = (DateTime.Now - start);
                    //            });
                    respond("Success score updates: " + count + "<br />Pending/Failed: " + countFail);
                }
                else
                {
                    respond("<b><font color=\"Red\">Update is already running in the background</font></b>");
                }
            }
            catch (Exception exception) { }
            AlreadyRunningUpdate = false;
            return AddedScores;
        }

        public ActionResult AddResults()
        {
            return View("AddResults");
        }



        private void respond(string response)
        {
            try
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.AddHeader("Content-Type", "text/plain");
                Response.Write(response);
                Response.Flush();
                Response.End();
            }
            catch (Exception e)
            {
            }
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}