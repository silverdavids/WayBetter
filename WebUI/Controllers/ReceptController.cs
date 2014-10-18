using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Models.Concrete;
using Domain.Models.ViewModels;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class ReceptController : CustomController
    {
        //
        // GET: /Recept/
        public ActionResult Index()
        {
            // var receipts = BetDatabase.Receipts.Include(r => r.Branch).OrderByDescending(x => x.betdate).Where(h=>h.status !=5);
            var singleOrDefault = BetDatabase.Accounts.SingleOrDefault(t => t.UserId == User.Identity.Name);
            if (singleOrDefault != null)
                ViewBag.Balance = singleOrDefault.AmountE;
            return View();
        }

        //

        // GET: /Recept/
        public JsonResult GetReciepts()
        {
            var user = BetDatabase.Accounts.SingleOrDefault(t => t.UserId == User.Identity.Name);
            var Branchid =Convert.ToInt32( user.AdminE);
            var date1 = DateTime.Today;
            var date2 = DateTime.Today.AddDays(1);
            var receipts = BetDatabase.Receipts.Include(r => r.Branch).OrderByDescending(x => x.ReceiptDate).Where(h => h.ReceiptStatus != -1 && h.ReceiptDate > date1 && h.ReceiptDate<date2 &&h.BranchId==Branchid).Select(r => new
            {
                ReceiptId = r.ReceiptId,
                r.ReceiptStatus,
                r.Branch.BranchName,
               ReceiptDate=r.ReceiptDate.ToString(),
                r.Stake,
                r.TotalOdds,
                r.UserId,
                
            }).ToList();
            return Json(receipts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecieptsByStatus(int id,string flag)
        {
            var date1 = DateTime.Today;
            var date2 = DateTime.Today.AddDays(1);
            if (!string.IsNullOrEmpty(flag))
            {
                if (flag.Contains("j"))
                {
                    
                    date1 = Convert.ToDateTime(flag.Replace('j', '/'));
                    date2 = date1.AddDays(1);
                }
           
                
            }
            var receipts = BetDatabase.Receipts.Include(r => r.Branch).OrderByDescending(x => x.ReceiptDate).Where(h => h.ReceiptStatus == id && h.ReceiptDate > date1 && h.ReceiptDate < date2).Select(r => new
            {
                ReceiptId = r.ReceiptId,
                r.ReceiptStatus,
                r.Branch.BranchName,
                ReceiptDate = r.ReceiptDate.ToString(),
                r.Stake,
                r.TotalOdds,
                r.UserId,

            }).ToList();
            return Json(receipts, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Recept/Details/5
        public async Task<JsonResult> GetDailyReciepts(string date)
        {
            var date1 = Convert.ToDateTime(date);
            var date2 =  Convert.ToDateTime(date).AddDays(1);
            var receipts = await BetDatabase.Receipts
                .Include(r => r.Branch)
                .OrderByDescending(r => r.ReceiptDate)
                .Where(r => r.ReceiptStatus != -1 && r.ReceiptDate > date1 && r.ReceiptDate < date2).Select(receipt => new
                {
                    RecieptId = receipt.ReceiptId,
                    receipt.Stake,
                    receipt.ReceiptDate,
                    receipt.ReceiptStatus,
                    receipt.Branch.BranchName,
                    receipt.TotalOdds,
                    receipt.UserId
                })
                .ToListAsync();
            return Json(receipts, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Statement()
        {
            var user = await BetDatabase.Accounts
                .OrderByDescending(a => a.DateE)
                .Select(u => new
                {
                    u.UserId,
                    u.AmountE
                })
                .SingleOrDefaultAsync(u => u.UserId == User.Identity.Name);

            ViewBag.Balance = user.AmountE;
            var statements = await BetDatabase.Statements.ToListAsync();
            return View(statements);
        }

        public ActionResult Betmatch()
        {
            return View(new Bet());
        }
        public async Task<ActionResult> Details(int id = 0)
        {
            ViewBag.RecieptId = id;
            var receipt = await BetDatabase.Receipts.Include(r => r.GameBets).SingleOrDefaultAsync(r => r.ReceiptId == id);
            if (Request.IsAjaxRequest()) return Json(receipt, JsonRequestBehavior.AllowGet);
            return View(receipt);       
        }
        //public JsonResult RecieptDetails(int id = 0)
        //{  
        //   // var receiptsdetails = BetDatabase.BettedMatches.Where(x => x.ReceiptId== id);
        //    List<BettedMatch> receiptsmatches = BetDatabase.BettedMatches.Where(x => x.ReceiptId == id).ToList();
        //    var receipt = receiptsmatches.Select(x => new {id=x.MatchID ,choice=x.BetOption.OptionName,x.odd,type=x.BetOption.BetCategory.Bet_type}).ToList();
        //    return Json(receipt, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult RecieptData(string  id = "")
        {
            int serial = Convert.ToInt32(id);
            var constring = ConfigurationManager.ConnectionStrings["BetConnection"].ConnectionString;
            var con = new SqlConnection(constring);               
            var query = "SELECT  BetServiceMatchNo,[dbo].[ReturnTeamName](HomeTeamId) as HomeTeam,[dbo].[ReturnTeamName](AwayTeamId) as AwayTeam,"+
    "[dbo].[ReturnOptionName](BetOptionId) as choice, [dbo].[ReturnCategoryName](BetOptionId) as category,HomeScore,AwayScore,stake,userid,m.starttime,r.TotalOdds , rs.statusName,r.receiptdate, convert(varchar(5) ,HomeScore)+':'+convert(varchar(5) ,AwayScore) as FT,convert(varchar(5) ,HalfTimeHomeScore)+':'+convert(varchar(5) ,HalfTimeAwayScore) as HT " +
 " FROM [dbo].[Bets] bm inner join [dbo].[Matches] m on bm.MatchID=m.BetServiceMatchNo inner join Receipts r on r.ReceiptID=bm.RecieptId  inner join ReceiptStatus rs on r.ReceiptStatus=rs.StatusId    where bm.RecieptId="+id;
            var model = new List<RecieptDetailsVm>();
            var dt = new DataTable();
            con.Open();
            var da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();
       
            for (var i = 0; i <dt.Rows.Count; i++)
                {
                    model.Add(new RecieptDetailsVm
                    {
                        MatchNo = Convert.ToInt32(dt.Rows[i]["BetServiceMatchNo"]),
                        HomeTeam = dt.Rows[i]["HomeTeam"].ToString(),
                        AwayTeam = dt.Rows[i]["AwayTeam"].ToString(),
                        CategoryName = dt.Rows[i]["category"].ToString(),
                        OptionName = dt.Rows[i]["choice"].ToString(),
                        UserId = dt.Rows[i]["userid"].ToString(),
                        SetOdd=Convert.ToDouble(dt.Rows[i]["totalodds"]),
                        BetMoney = Convert.ToDouble(dt.Rows[i]["stake"]),
                        Stime = Convert.ToDateTime(dt.Rows[i]["starttime"]),
                        Status = dt.Rows[i]["StatusName"].ToString(),
                        BetDate = Convert.ToDateTime(dt.Rows[i]["receiptdate"]),
                        FT = dt.Rows[i]["FT"].ToString(),
                        HT = (dt.Rows[i]["HT"]).ToString(),
                        WinAmount = Convert.ToDouble(dt.Rows[i]["totalodds"]) * Convert.ToDouble(dt.Rows[i]["stake"]),
                    });
                }
           var recieptmatches=model.Select(x=>new {id=x.MatchNo,choice=x.OptionName,type=x.CategoryName,x.AwayTeam,x.HomeTeam,x.BetMoney,x.SetOdd,Teller=x.UserId,team=(x.HomeTeam+" vs "+x.AwayTeam),status=x.Status,Scores="(FT"+x.FT+" HT"+x.HT+")",time=x.Stime.ToString(CultureInfo.InvariantCulture),Cancel=CancelStatus(x.BetDate),WonAmount=x.WinAmount}).ToList();
           return Json(recieptmatches, JsonRequestBehavior.AllowGet);    
        
        }
        //
        // GET: /Recept/Create
        public ActionResult Create()
        {          
            ViewBag.BranchId = new SelectList(BetDatabase.Branches, "BranchId", "BranchName");
            return View();
        }
        public bool CancelStatus(DateTime dt) {
            var rt = DateTime.Now.Subtract(dt);
            var totalmin = Convert.ToInt32(rt.TotalMinutes);
            return totalmin < 10;
        }
        //
        public ActionResult Payments()
        {
            var account = BetDatabase.Accounts.SingleOrDefault(t => t.UserId == User.Identity.Name);
            if (account != null)
                ViewBag.Balance = account.AmountE;
            return View();
        }

        // POST: /Recept/Create
        [HttpPost]
        public ActionResult Create(Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Receipts.Add(receipt);
                BetDatabase.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(BetDatabase.Branches, "BranchId", "BranchName", receipt.BranchId);
            return View(receipt);
        }
        //
        // GET: /Recept/Edit/5
        public ActionResult Edit(int id = 0)
        {
            var receipt = BetDatabase.Receipts.Find(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(BetDatabase.Branches, "BranchId", "BranchName", receipt.BranchId);
            return View(receipt);
        }
        //
        // POST: /Recept/Edit/5
        [HttpPost]
        public ActionResult Edit(Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Entry(receipt).State = EntityState.Modified;
                BetDatabase.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(BetDatabase.Branches, "BranchId", "BranchName", receipt.BranchId);
            return View(receipt);
        }
        //
        // GET: /Recept/Delete/5
        public async Task<ActionResult> Delete(int id = 0)
        {
            var receipt = await BetDatabase.Receipts.FindAsync(id);
            receipt.ReceiptStatus = -1;
            BetDatabase.Receipts.AddOrUpdate();
            BetDatabase.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<JsonResult> CancelReciept(int id = 0)
        {
            var receipt = BetDatabase.Receipts.Find(id);
            var tellerAccount = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == receipt.UserId);
            
            if (receipt == null)
            {
                return Json("Receipt Not Found", JsonRequestBehavior.AllowGet);
            }
            tellerAccount.AmountE = tellerAccount.AmountE - receipt.Stake;
            receipt.ReceiptStatus = -1;
            var tellerStatement = new Statement
            {
                BalBefore = tellerAccount.AmountE,
                BalAfter = tellerAccount.AmountE,
                Comment = "Ticket " + receipt.ReceiptId + " Canceling",
                Transcation = "Reciept Canceling",
                Account = tellerAccount.UserId,
                StatetmentDate = DateTime.Now,
                Controller = User.Identity.Name,
            };
            BetDatabase.Statements.Add(tellerStatement);
            BetDatabase.Receipts.AddOrUpdate();
            BetDatabase.Accounts.AddOrUpdate();
            BetDatabase.SaveChanges();
            return Json("Receipt Was Canceled", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PayReciept(int id = 0)
        {
            var msg = "";
            var receipt = await  BetDatabase.Receipts.FindAsync(id);
            if (receipt == null)
            {
                return Json("Reciept Not Found", JsonRequestBehavior.AllowGet);
            }
            if (receipt.ReceiptStatus != 3) return Json(msg, JsonRequestBehavior.AllowGet);
            receipt.ReceiptStatus = 4;
            var cashierAccount = await BetDatabase.Accounts.SingleOrDefaultAsync(t => t.UserId == User.Identity.Name);
            var balance = cashierAccount.AmountE;
            var winAmount = receipt.TotalOdds * receipt.Stake;
            if (balance > winAmount)//Make Payments
            {
                cashierAccount.DateE = DateTime.Now;
                cashierAccount.AmountE = balance - winAmount;
                var statement = new Statement
                {
                    Account = cashierAccount.UserId,
                    BalBefore = cashierAccount.AmountE,
                    Amount = winAmount,
                    BalAfter = cashierAccount.AmountE,
                    Comment = "Payment of Win Ticket Number" + receipt.ReceiptId,
                    Controller = cashierAccount.UserId,
                    Error = false,
                    Transcation = "Ticket Payment",
                    Serial = Convert.ToString(receipt.ReceiptId)
                };
                BetDatabase.Receipts.AddOrUpdate(receipt);
                BetDatabase.Statements.AddOrUpdate(statement);
                BetDatabase.Accounts.AddOrUpdate(cashierAccount);
                BetDatabase.SaveChanges();
                return Json("Reciept Sucessfull", JsonRequestBehavior.AllowGet);
            }
            msg = "You have less money.please contact admin .";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
   
         
      
        //
        // POST: /Recept/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var receipt = BetDatabase.Receipts.Find(id);
            BetDatabase.Receipts.Remove(receipt);
            BetDatabase.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            BetDatabase.Dispose();
            base.Dispose(disposing);
          }
    }
}