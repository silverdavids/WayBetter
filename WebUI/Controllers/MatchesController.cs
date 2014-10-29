using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Domain.Models.Concrete;
using Domain.Models.ViewModels;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class MatchesController : CustomController
    {
        // GET: Matches
        //public async Task<ActionResult> Index()
        //{
        //    var matches = BetDatabase.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.ShortMatchCode);
        //    return View(await matches.ToListAsync());
        //}

        // GET: Match
        public async Task<ActionResult> Index()
        {

            if (!Request.IsAjaxRequest())
            {
                //var account = await BetDatabase.Accounts.Select(a => new
                //{
                //    a.UserId,
                //    a.AmountE
                //}).SingleOrDefaultAsync(t => t.UserId == User.Identity.Name);
                //ViewBag.Balance = account.AmountE;
                return View();
            }
            var games = await BetDatabase.Matches.Include(m => m.HomeTeam).Include(m => m.AwayTeam).ToListAsync();
            //var games = await BetDatabase.ShortMatchCodes.Include(s => s.Match).OrderBy(x => x.ShortCode).ToListAsync();
            //var startTime = DateTime.Now;

            var filteredgames = games.Select(g => new GameViewModel
            {
                AwayScore = g.AwayScore,
                AwayTeamId = g.AwayTeamId,
                AwayTeamName = g.AwayTeam.TeamName,
                Champ = g.League,
                MatchOdds = g.MatchOdds.Select(go => new GameOddViewModel
                {
                    BetCategory = go.BetOption.BetCategory.Name,
                    BetOptionId = go.BetOptionId,
                    BetOption = go.BetOption.Option,
                    LastUpdateTime = go.LastUpdateTime,
                    Odd = go.Odd,
                    Line = go.BetOption.Line
                }).ToList(),
                GameStatus = g.GameStatus,
                HalfTimeAwayScore = g.HalfTimeAwayScore,
                HalfTimeHomeScore = g.HalfTimeHomeScore,
                HomeScore = g.HomeScore,
                HomeTeamName = g.HomeTeam.TeamName,
                HomeTeamId = g.HomeTeamId,
                MatchNo = games.IndexOf(g) + 1,
                RegistrationDate = g.RegistrationDate,
                ResultStatus = g.ResultStatus,
                SetNo = 1234,
                OldDateTime = g.StartTime,
                StartTime = String.Format("{0:dd/M/yyyy}", g.StartTime)
            }).OrderBy(s => s.StartTime);
            // .Where(x => x.OldDateTime>startTime)
            return Json(filteredgames, JsonRequestBehavior.AllowGet);
        }

        // GET: Matches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName");
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName");
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BetServiceMatchNo,League,StartTime,GameStatus,AwayTeamId,HomeTeamId,RegistrationDate,HomeScore,AwayScore,HalfTimeHomeScore,HalfTimeAwayScore,ResultStatus")] Match match)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Matches.Add(match);
                await BetDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BetServiceMatchNo,League,StartTime,GameStatus,AwayTeamId,HomeTeamId,RegistrationDate,HomeScore,AwayScore,HalfTimeHomeScore,HalfTimeAwayScore,ResultStatus")] Match match)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Entry(match).State = EntityState.Modified;
                await BetDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var match = await BetDatabase.Matches.FindAsync(id);
            BetDatabase.Matches.Remove(match);
            await BetDatabase.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<JsonResult> ReceiveReceipt(Receipt1 receipts)
        {
            var bcg = new BarCodeGenerator();
            var account = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == User.Identity.Name);
            var branchId = Convert.ToInt32(account.AdminE);
            var branch = await BetDatabase.Branches.SingleOrDefaultAsync(x => x.BranchId == branchId);
            var receiptid = bcg.GenerateRandomString(16);
            var receipt = new Receipt
            {
                UserId = User.Identity.Name,
                BranchId = Convert.ToInt16(account.AdminE),
                ReceiptStatus = 0,
                SetNo = 2014927,
                // ReceiptId = Convert.ToInt32(receiptid)
            }; //Start New Reciept

            var betStake = receipts.TotalStake.ToString(CultureInfo.InvariantCulture);
            string response;
            BetDatabase.Receipts.Add(receipt);
            await BetDatabase.SaveChangesAsync();

            var ttodd = receipts.TotalOdd;
            const float bettingLimit = 8000000;
            var cost = Convert.ToDouble(betStake);
            if ((cost >= 1000) && (cost <= bettingLimit))//betting limit
            {

                foreach (var betData in receipts.BetData)
                {
                    try
                    {
                        var tempMatchId = Convert.ToInt32(betData.MatchId);
                        var matchid = BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchId).MatchNo;
                        Match match = BetDatabase.Matches.Single(h => h.BetServiceMatchNo == matchid);
                        DateTime _matchTime = match.StartTime;
                        DateTime timenow = DateTime.Now;
                        if (_matchTime < timenow)
                        {
                            response = ("The Match " + tempMatchId + " Has Started");
                            return new JsonResult { Data = new { message = response } };
                        }
                        var bm = new Bet
                        {
                            BetOptionId = Int32.Parse(betData.OptionId),
                            RecieptId = receipt.ReceiptId,
                            MatchId = BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchId).MatchNo,
                            BetOdd = Convert.ToDecimal(betData.Odd),
                        };
                        BetDatabase.Bets.Add(bm);
                        BetDatabase.SaveChanges();
                    }
                    catch (Exception er)
                    {
                        response = (" An error  has occured:" + er.Message);
                        return new JsonResult { Data = new { message = response } };
                        var msg = er.Message;
                    }
                }
                // Requires Quick Attention

                receipt.TotalOdds = Convert.ToDouble(ttodd);
                receipt.ReceiptStatus = 1;
                receipt.SetSize = receipts.ReceiptSize;
                receipt.Stake = cost;
                receipt.WonSize = 0;
                receipt.SubmitedSize = 0;
                receipt.ReceiptDate = DateTime.Now;
                receipt.Serial = Int2Guid(receiptid);
                account.DateE = DateTime.Now;
                // receipt.RecieptID = 34;                    
                BetDatabase.Entry(receipt).State = EntityState.Modified;
                var statement = new Statement
                {
                    Account = receipt.UserId,
                    Amount = receipt.Stake,
                    Controller = receipt.UserId,
                    StatetmentDate = DateTime.Now,
                    BalBefore = account.AmountE,
                    BalAfter = account.AmountE + receipt.Stake,
                    Comment = "Bet Transaction for Ticket No" + receiptid
                };
                account.AmountE = account.AmountE + receipt.Stake;
                statement.Transcation = "Teller Bet";
                statement.Method = "Online";
                statement.Serial = receiptid;

                branch.Balance = branch.Balance + Convert.ToDecimal(receipt.Stake);
                BetDatabase.Entry(branch).State = EntityState.Modified;
                BetDatabase.Accounts.AddOrUpdate(account);
                BetDatabase.Statements.Add(statement);
                BetDatabase.SaveChanges();
                var barcodeImage = bcg.CreateBarCode(receiptid);
                var tempPath = Server.MapPath("~/Content/BarCodes/" + receiptid.Trim() + ".png");
                try
                {
                    barcodeImage.Save(tempPath, ImageFormat.Png);
                }
                catch (Exception)
                {
                }
                response = ("Success");
            }
            else if (cost < 1000)
            {
                receipt.ReceiptId = 0;
                response = ("Minimum betting stake is UGX 1000. Please enter amount more than UGX 1000.");
            }
            else
            {
                receipt.ReceiptId = 0;
                response = ("Maximum stake is UGX " + bettingLimit + ". Please enter amount less than UGX " + bettingLimit + ".");
            }

            return new JsonResult { Data = new { message = response, ReceiptNumber = receipt.ReceiptId, ReceiptTime = String.Format("{0:dd/MM/yyyy}", DateTime.Now) + " - " + toJavaScriptDate(DateTime.Now), TellerName = account.UserId, BranchName = branch.BranchName, Balance = account.AmountE, Serial = receiptid, FormatedSerial = GetSerialNumber(receiptid) } };
        }
        public string toJavaScriptDate(DateTime value)
        {
            var time = value.ToLongTimeString();
            var ReceiptTime = (value.ToString("HH:mm"));
            return ReceiptTime;
        }
        public string GetSerialNumber(string serialGuid)
        {

            var serialArray = serialGuid.ToCharArray();
            var finalSerialNumber = "";
            var j = 0;
            for (var i = 0; i < 16; i++)
            {
                for (j = i; j < 4 + i; j++)
                {
                    finalSerialNumber += serialArray[j];
                }
                if (j == 16)
                {
                    break;
                }
                else
                {
                    i = (j) - 1;
                    finalSerialNumber += "-";
                }
            }

            return finalSerialNumber;
        }
        public Guid GetSerialGuid()
        {
            var serialGuid = Guid.NewGuid();
            return serialGuid;
        }
        public static Guid Int2Guid(string serial)
        {
            long value = Convert.ToInt64(serial);
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BetDatabase.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
