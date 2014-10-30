using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class Live1Controller : Controller
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