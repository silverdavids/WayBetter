using System.Web;
using Domain.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using WebUI.App_Start;
using WebUI.DataAccessLayer;
using WebUI.Helpers;
using System.Data.Entity.Migrations;
using System.Drawing.Imaging;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
namespace WebUI.Controllers
{
    public class ReceiptPrintController : ApiController
    {
          //private ApplicationDbContext BetDatabase;
          private ApplicationDbContext _dbContext;
          private ApplicationUserManager _userManager;
     //   private ICustomController _userManager;
          private readonly ApplicationDbContext BetDatabase = new ApplicationDbContext();
        public ReceiptPrintController(){}
        public ReceiptPrintController(ICustomController _db) {
            BetDatabase = _db.getDbContext();
        
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return null;
                    //  return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }
        public ApplicationDbContext BetDatabases
        {
            get { return _dbContext ?? (_dbContext = new ApplicationDbContext()); }
            set
            {
                _dbContext = value;
            }
        }

         public  async  Task<IHttpActionResult> ReceiveReceipt(Receipt1 receipts)
         {
          
            var bcg = new BarCodeGenerator();
            var userName = "TellerTest";
             if (!string.IsNullOrEmpty(User.Identity.Name))
             {
                 userName = User.Identity.Name;
             }
           
            //var account = await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == User.Identity.Name);
             var account =await BetDatabase.Accounts.SingleOrDefaultAsync(x => x.UserId == userName);
             
            // Account account=new Account();
             var branchId = 1;// Convert.ToInt32(account.AdminE);
             Branch branch =await BetDatabase.Branches.SingleOrDefaultAsync(x => x.BranchId == branchId);

            var receiptid = bcg.GenerateRandomString(16);
           
            var receipt = new Receipt
            {
                UserId ="TellerTest",// User.Identity.Name,
                BranchId =1,//Convert.ToInt16(account.AmountE),
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

                List<BetData> bd = new List<BetData>();
              
                var num = new Random();
                int[] testgames =
                {
                    1934720, 1934721, 1934722, 1934723, 1934725, 1936051, 1936052, 1936692, 1937588,
                    1937589
                };
                var receiptGames = receipts.BetInfo.Split('_');
                for (int i = 0; i < receiptGames.Length-1; i++)
                {
                    if (string.IsNullOrEmpty(receiptGames[i]))
                    {
                        return Ok("An Error Occured");
                    }
                    string[] gameData = receiptGames[i].Split('S');
                    BetData gamedata = new BetData
                    {
                        MatchId = testgames[i].ToString(),//(gameData[0]).ToString(),
                        OptionId = (gameData[1]).ToString(),
                        Odd = Double.Parse(gameData[2]),
                        // ExtraValue = (gameData[3]).ToString(), 
                    };
                    bd.Add(gamedata);

                }
            foreach(var betData in bd)
                {            
                    try
                    {
                        var tempMatchId =Convert.ToInt32(betData.MatchId);
                       // var matchid = BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchId).MatchNo;
                        Match match = await BetDatabase.Matches.SingleOrDefaultAsync(h => h.BetServiceMatchNo == tempMatchId);

                        DateTime  _matchTime = match.StartTime;
                        DateTime timenow = DateTime.Now;
                        if (_matchTime < timenow)
                        {
                            response = ("The Match " + tempMatchId + " Has Started");
                          var   message=response;
                            return  Ok(message);
                        }
                        var bm = new Bet
                        {
                            BetOptionId = Int32.Parse(betData.OptionId),
                            RecieptId = receipt.ReceiptId,
                            MatchId =tempMatchId, //BetDatabase.ShortMatchCodes.Single(x => x.ShortCode == tempMatchId).MatchNo,  
                            BetOdd = Convert.ToDecimal(betData.Odd),
                        };
                        BetDatabase.Bets.Add(bm);
                        BetDatabase.SaveChanges();
                    }
                    catch (Exception er)
                    {
                       var message= response = (" An error  has occured:"+er.Message);
                        return  Ok(message);
                        var msg = er.Message;
                    }
                }
                // Requires Quick Attention
              
                receipt.TotalOdds= Convert.ToDouble(ttodd);
                receipt.ReceiptStatus = 1;
                receipt.SetSize = receipts.ReceiptSize;
                receipt.Stake = cost;
                receipt.WonSize = 0;
                receipt.SubmitedSize = 0;
                receipt.ReceiptDate = DateTime.Now;
                receipt.Serial = Int2Guid(receiptid);    
                account.DateE  = DateTime.Now;
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
                    Comment ="Bet Transaction for Ticket No"+receiptid
                };
                account.AmountE = account.AmountE + receipt.Stake;
                statement.Transcation = "Teller Bet";
                statement.Method = "Online";
                statement.Serial = receiptid;
            
                branch.Balance = branch.Balance +Convert.ToDecimal( receipt.Stake);
                BetDatabase.Entry(branch).State = EntityState.Modified;
                BetDatabase.Accounts.AddOrUpdate(account);
                BetDatabase.Statements.Add(statement);
                await  BetDatabase.SaveChangesAsync();
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

            return Ok( new { message = response, ReceiptNumber = receipt.ReceiptId, ReceiptTime = String.Format("{0:dd/MM/yyyy}", DateTime.Now) + " - " + toJavaScriptDate(DateTime.Now), TellerName = account.UserId, BranchName = branch.BranchName, Balance = account.AmountE, Serial = receiptid, FormatedSerial = GetSerialNumber(receiptid)  });
        }

  
          public  string toJavaScriptDate(DateTime value)
          {
              var time = value.ToLongTimeString();
             var ReceiptTime = (value.ToString("HH:mm"));
             return ReceiptTime;
          }

        public string BarcodeFormart(string barcode)
        {
            string[] arr=null;
            string code = "";
            for (int i = 0; i < 12; i++)
            {
                if (i%3 == 0)
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



        public static Guid Int2Guid(string  serial)
        {
            long value = Convert.ToInt64(serial);
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

          public string GetSerialNumber(  string serialGuid)
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
        public string BetInfo { get; set; }


    }
    public class BetData
    {
        public string MatchId { get; set; }
        public string BetCategory { get; set; }
        public string OptionId { get; set; }
        public double Odd { get; set; }
        public long BetAmount { get; set; }
         public string LiveScores{ get; set; }
        public string StartTime { get; set; }
        public string ExtraValue{ get; set; }
        
         //public string LiveScores{get;set;}
         //public string ExtraValue { get; set; }
    }

    }


