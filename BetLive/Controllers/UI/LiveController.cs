using System.Web.Mvc;

namespace BetLive.Controllers.UI
{
    public class LiveController : Controller
    {
        //
        // GET: /LiveGames/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Games()
        {
            return View();
        }
	}
}