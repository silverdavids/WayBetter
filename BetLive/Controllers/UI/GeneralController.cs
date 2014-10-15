using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetLive.Controllers.UI
{
    public class GeneralController : Controller
    {
        // GET: General
        public ActionResult Index()
        {
            return View();
        }

        // GET: General/Details/5
        public ActionResult main()
        {
            return View();
        }

        // GET: General/Create
        public ActionResult about()
        {
            return this.View();
        }
        public ActionResult home()
        {
            return View();
        }

        // GET: General/Create
        public ActionResult login()
        {
            return View();
        }

        public ActionResult signup()
        {
            return View();
        }

        // GET: General/Create
        public ActionResult matches()
        {
            return View();
        }

        public ActionResult company()
        {
            return View();
        }

        // GET: General/Create
        public ActionResult createcompany()
        {
            return View();
        }
        public ActionResult branchlist()
        {
            return View();
        }

        // GET: General/Create
        public ActionResult terminallist()
        {
            return View();
        }
        public ActionResult shiftlist()
        {
            return View();
        }
        // POST: General/Create
       
    

      
    }
}
