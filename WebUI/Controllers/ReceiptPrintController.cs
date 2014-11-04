using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DocumentFormat.OpenXml.Presentation;
using WebUI.App_Start;
using WebUI.DataAccessLayer;
using WebUI.Helpers;
using Domain.Models.Concrete;
using System.Globalization;
using System.Drawing.Imaging;
using System.Security.Claims;


namespace WebUI.Controllers
{
    public class ReceiptPrintController : ApiController
    {

        private ApplicationDbContext BetDatabase;//=new ApplicationDbContext();
        private CustomController cc;
        private ApplicationUserManager _userManager;
        //private readonly ApplicationDbContext db = new ApplicationDbContext();
        public ReceiptPrintController()
        {
            cc = new CustomController();
            BetDatabase = cc.BetDatabase;
            // _userManager = this.RequestContext.Principal.Identity//cc.UserManager;

        }
        //public ReceiptPrintController(ICustomController _db) {
        //    BetDatabase = _db.getDbContext();

        //}
        // [Authorize]
        public async Task<IHttpActionResult> ReceiveReceipt([FromBody]Receipt1 receipts)
        {
           
            // IEnumerable<string> headerValues =RequestContext.Principal.Identity request.Headers.GetValues("MyCustomID");
            //var id = headerValues.FirstOrDefault();
           // var identity = (ClaimsIdentity)User.Identity;
           // Claim claims = identity.FindFirst("UserName");
           // var un = RequestContext.Principal.Identity.Name;
            var bcg = new BarCodeGenerator();
            ////var user = this.User.Identity; ;
            ////var account = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == user.Name);
            ////var branchId = Convert.ToInt32(account.AdminE);
            ////var branch = await BetDatabase.Branches.SingleOrDefaultAsync(x => x.BranchId == branchId);
            var receiptid = bcg.GenerateRandomString(16);
          // return Ok(new { message = "Success", receiptFromServer = receipts/*, ReceiptNumber = receiptid*/ });
            var userName = receipts.UserName;
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                userName = User.Identity.Name;
            }

            //var account = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == User.Identity.Name);
            var account = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == userName);
            if (account == null)
            {
                account = new Account { 
                 DateE=DateTime.Now,
                  UserId=userName,
                
                };
                BetDatabase.Accounts.Add(account);
                BetDatabase.SaveChanges();
            }
            var branchId = 1;// Convert.ToInt32(account.AdminE);
            Branch branch = await BetDatabase.Branches.SingleOrDefaultAsync(x => x.BranchId == branchId);
            var betStake = receipts.TotalStake.ToString(CultureInfo.InvariantCulture);
            var ttodd = receipts.TotalOdd;
            const float bettingLimit = 5000000;
            var cost = Convert.ToDouble(betStake);
            var receipt = new Receipt
            {
                UserId  ="TellerTest",//User.Identity.Name,
                BranchId =1 ,//Convert.ToInt16(account.AdminE),
               // ReceiptStatus = 0,
                SetNo = 2014927,
                 TotalOdds = Convert.ToDouble(ttodd),
                 ReceiptStatus = 1,
                 SetSize = receipts.ReceiptSize,
                 Stake = cost,
                 WonSize = 0,
                 SubmitedSize = 0,
                 ReceiptDate = DateTime.Now,
                  Serial = Int2Guid(receiptid),
                // ReceiptId = Convert.ToInt32(receiptid)
            }; //Start New Reciept

           
            string response;
           
           
            account.DateE = DateTime.Now;
            BetDatabase.Receipts.Add(receipt);
            await BetDatabase.SaveChangesAsync();

            //////var ttodd = receipts.TotalOdd;
            //////const float bettingLimit = 8000000;
            //////var cost = Convert.ToDouble(betStake);
            if ((cost >= 1000) && (cost <= bettingLimit))//betting limit
            {

                foreach (var betData in receipts.BetData)
                {
                    try
                    {
                        //var tempMatchId = Convert.ToInt32(betData.MatchId);
                        var matchid = 0 ;
                        //Check if the match is a live game
                        if (betData.LiveScores != null)
                        {
                            var tempMatchIdLive = betData.MatchId;
                            matchid = (int)BetDatabase.LiveMatches.Single(x => x.LiveMatchNo.Equals(tempMatchIdLive)).BetServiceMatchNo;
                        }
                        else {
                            var tempMatchIdNormals = Convert.ToInt32(betData.MatchId);
                            matchid = BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchIdNormals).MatchNo;                        
                        }
                        Match match = BetDatabase.Matches.Single(h => h.BetServiceMatchNo == matchid);
                        DateTime _matchTime = match.StartTime;
                        DateTime timenow = DateTime.Now;
                        //if (_matchTime < timenow)
                        //{
                        //    response = ("The Match " + tempMatchId + " Has Started");
                        //    var message = response;
                        //    return Ok(message);
                        //}
                        var bm = new Bet
                        {
                            BetOptionId = Int32.Parse(betData.OptionId),
                            RecieptId = receipt.ReceiptId,
                            MatchId =Convert.ToInt32(matchid),// MatchId = BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchId).MatchNo,
                            BetOdd = Convert.ToDecimal(betData.Odd),
                           // ExtraValue = GetExtraValue(betData.ExtraValue),
                        };
                        BetDatabase.Bets.Add(bm);
                        BetDatabase.SaveChanges();
                    }
                    catch (Exception er)
                    {
                        var message = response = (" An error  has occured:" + er.Message);
                        return Ok(message);
                        var msg = er.Message;
                    }
                }
                // Requires Quick Attention

                //////receipt.TotalOdds = Convert.ToDouble(ttodd);
                //////receipt.ReceiptStatus = 1;
                //////receipt.SetSize = receipts.ReceiptSize;
                //////receipt.Stake = cost;
                //////receipt.WonSize = 0;
                //////receipt.SubmitedSize = 0;
                //////receipt.ReceiptDate = DateTime.Now;
                //////receipt.Serial = Int2Guid(receiptid);
                //////account.DateE = DateTime.Now;
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
                await BetDatabase.SaveChangesAsync();
                var barcodeImage = bcg.CreateBarCode(receiptid);
                var tempPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/BarCodes/" + receiptid.Trim() + ".png");
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

            return Ok(new { message = response, ReceiptNumber = receipt.ReceiptId, ReceiptTime = String.Format("{0:dd/MM/yyyy}", DateTime.Now) + " - " + toJavaScriptDate(DateTime.Now), TellerName = account.UserId, BranchName = branch.BranchName, Balance = account.AmountE, Serial = receiptid, FormatedSerial = GetSerialNumber(receiptid) });
        }


        public string toJavaScriptDate(DateTime value)
        {
            var time = value.ToLongTimeString();
            var ReceiptTime = (value.ToString("HH:mm"));
            return ReceiptTime;
        }

        public string BarcodeFormart(string barcode)
        {
            string[] arr = null;
            string code = "";
            for (int i = 0; i < 12; i++)
            {
                if (i % 3 == 0)
                {
                    arr[i] = barcode.Substring(i + 4);
                }
            }
            for (int x = 0; x < arr.Length; x++)
            {
                code += "-";

            }
            return code;
        }



        public static Guid Int2Guid(string serial)
        {
            long value = Convert.ToInt64(serial);
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
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

        public Decimal GetExtraValue(string ev)
        {
            decimal extraValue = 0;
            if (decimal.TryParse(ev, out extraValue))
            {
                extraValue=Convert.ToDecimal(ev);
            }
            return extraValue;
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
    public class Receipt1
    {
        private List<BetData> _betData;
        public int ReceiptSize { get; set; }
        public double TotalOdd { get; set; }
        public int TotalStake { get; set; }
        public List<BetData> BetData
        {
            get { return _betData ?? (new List<BetData>()); }
            set { _betData = value; }
        }
        public long MultipleBetAmount { get; set; }
        public string UserName { get; set; }


    }
    public class BetData
    {
        public string MatchId { get; set; }
        public string BetCategory { get; set; }
        public string OptionId { get; set; }
        public double Odd { get; set; }
        public long BetAmount { get; set; }
        public string LiveScores { get; set; }
        public string StartTime { get; set; }
        public string ExtraValue { get; set; }
        public string ShortCode { get; set; }
        //public string LiveScores{get;set;}
        //public string ExtraValue { get; set; }
    }
}