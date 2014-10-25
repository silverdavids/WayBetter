using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetLive.Controllers.UI
{
    public class MyMatchesController : Controller
    {
        // GET: MyMatches
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult matches()
        {
            return this.View();
        }
    }
}