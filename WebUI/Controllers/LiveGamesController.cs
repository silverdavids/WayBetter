using System.Web.Mvc;

namespace WebUI.Controllers
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