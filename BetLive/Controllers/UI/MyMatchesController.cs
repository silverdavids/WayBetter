using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Helpers;

namespace BetLive.Controllers.UI
{
    public class MyMatchesController : CustomController
    {
        // GET: MyMatches
        public async Task<ActionResult> Index()
        {
            if (!Request.IsAjaxRequest())
            {
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                   var account = await BetDatabase.Accounts.Select(a => new
                    {
                        a.UserId,
                        a.AmountE
                    }).SingleOrDefaultAsync(t => t.UserId == User.Identity.Name);
                    ViewBag.Balance = account.AmountE;
                    return View();
                } 
            }

            return View();
        }
        public ActionResult matches()
        {    
            return this.View();
        }
    }
}