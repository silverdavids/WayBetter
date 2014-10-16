using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.Models.ViewModels;
using SRN.DAL;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class AdminController : CustomController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SummaryReport()
        {
            return View();
        }
        public ActionResult BranchSummary(int id=0)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TicketSales(string id,string flag)
        {
            string date = id;
            if (!string.IsNullOrEmpty(date))
            {
                if (date.Contains("d"))
                {

                    date =(id.Replace('d', 'j'));                 
                }
            }
            ViewBag.BusinessDay = date;
            ViewBag.Status = flag;
            return View();
        }
        public ActionResult Tickets(string date)
        {
            ViewBag.BusinessDay = date;
            return View();
        } 
        public JsonResult BranchDetails()
        {
            var branches = BetDatabase.Branches.Select(x => new { x.BranchId, x.BranchName, x.Location }).ToList();
            return Json(branches, JsonRequestBehavior.AllowGet); 
        }
    
        public JsonResult SummaryReports()
        {        
            //var query = "select count(betmoney) as TotalBets,sum(betmoney) as Sales,CONVERT(VARCHAR(19),month(betdate))+'/' +CONVERT(VARCHAR(19),day(betdate))+'/'  +CONVERT(VARCHAR(19),Year(betdate)) as businessday from Receipts group by day(betdate) ,month(betdate),year(betdate)";
            const string query = "select count(Stake) as TotalBets,sum(Stake) as Sales,CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate)) as businessday,"+
"[dbo].[ReturnTotalWinAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),"+
"Year(ReceiptDate))) as WinAmount,[dbo].[ReturnTotalOutStandingAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))) as Outstanding ,"+
 "[dbo].[ReturnTotalPaidAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))) as Paid,"+
               " [dbo].[ReturnTotalCanceledTicket](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))) as Canceled,"+
                   "   [dbo].[ReturnTotalCanceledAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))) as TotalCanceled ," +
                      "  [dbo].[ReturnTotalPaidNumber](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))) as PaidNumber," +
                                 " [dbo].[ReturnTicketByStatus](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate)),3) as WinNumber  " +
                 "from Receipts where ReceiptDate is not null  and ReceiptDate > dateadd(day,-8,getdate()) and ReceiptStatus<> -1 group by day(ReceiptDate) ,month(ReceiptDate),year(ReceiptDate) order by month(ReceiptDate) desc,day(ReceiptDate) desc";

            var report = new List<SummaryReportVm>();
            try
            {           
                var bridge = new DBBridge();
                var reader = bridge.ExecuteReaderSQL(query);
                while (reader.Read())
                {
                    var dayreport = new SummaryReportVm
                    {
                        TicketSold = reader.GetInt32(0),
                        Sales = Convert.ToDecimal(reader.GetDouble(1)),
                        BusinessDay = reader.GetString(2),
                        TotalWins = reader.GetInt32(3),
                        PaidOrders = reader.GetInt32(5),
                        Canceled = reader.GetInt32(7),
                        CanceledNumber = reader.GetInt32(6),
                        TicketsPaid = reader.GetInt32(8),
                        WinCount = reader.GetInt32(9),

                       
                    };
                    report.Add(dayreport);
                }         
            }
            catch (Exception er) {       
            }
           var reports = report
           .Select(x => new { x.Sales, x.BusinessDay, x.TotalWins, x.PaidOrders, x.Canceled, Outstanding = x.OutStanding(), x.TicketsPaid, x.CanceledNumber, x.TicketSold, Profit = x.Sales - x.TotalWins,WinCount=x.WinCount,Cash=x.Cash() });
           var counts = reports.Count();
           return Json(reports, JsonRequestBehavior.AllowGet);    
        }
        public JsonResult BranchSummaryReports(int id)
        {
           // var query = "select count(betmoney) as TotalBets,sum(betmoney) as Sales,CONVERT(VARCHAR(19),month(betdate))+'/' +CONVERT(VARCHAR(19),day(betdate))+'/'  +CONVERT(VARCHAR(19),Year(betdate)) as businessday from Receipts where branchid="+id+" group by day(betdate) ,month(betdate),year(betdate)";
    string query = " select count(Stake) as TotalBets,sum(Stake) as Sales,CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate)) as businessday, "+
 " [dbo].[TotalBranchWinAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19), "+
   " Year(ReceiptDate))," + id + ") as WinAmount, " +
  " [dbo].[BranchTotalOutStandingAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))," + id + ") as Outstanding , " +
    " [dbo].[BranchTotalPaidAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))," + id + ") as Paid, +" +
     "  [dbo].[BranchTotalCanceledTicket](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))," + id + ") as Canceled, + " +
      "  [dbo].[BranchTotalCanceledAmount](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))," + id + ") as TotalCanceled ,  " +
        " [dbo].[BranchTotalPaidNumber](CONVERT(VARCHAR(19),month(ReceiptDate))+'/' +CONVERT(VARCHAR(19),day(ReceiptDate))+'/'  +CONVERT(VARCHAR(19),Year(ReceiptDate))," + id + ") as PaidNumber  " +
            " from Receipts where ReceiptDate is not null and ReceiptStatus<> -1  and Receipts.BranchId="+id+" group by day(ReceiptDate) ,month(ReceiptDate),year(ReceiptDate) order by businessday asc ;";

            var report = new List<SummaryReportVm>();
            try
            {
                var bridge = new DBBridge();
                var reader = bridge.ExecuteReaderSQL(query);
                while (reader.Read())
                {
                    var dayreport = new SummaryReportVm
                    {
                        TicketSold = reader.GetInt32(0),
                        Sales = Convert.ToDecimal(reader.GetDouble(1)),
                        BusinessDay = reader.GetString(2),
                        TotalWins = reader.GetInt32(3),
                        PaidOrders = reader.GetInt32(5),
                        Canceled = reader.GetInt32(7),
                        CanceledNumber = reader.GetInt32(6),
                        TicketsPaid = reader.GetInt32(8),
                    };
                    report.Add(dayreport);
                }
            }
            catch (Exception er)
            {
            }
            var reports = report
            .Select(x => new { x.Sales, x.BusinessDay, x.TotalWins, x.PaidOrders, x.Canceled, Outstanding = x.OutStanding(), x.TicketsPaid, x.CanceledNumber, x.TicketSold, Profit = x.Sales- x.TotalWins });
            var counts = reports.Count();
            return Json(reports, JsonRequestBehavior.AllowGet);  
          
        }
    }
}