using System.Web.Mvc;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class OddsServiceController : CustomController
    {
        // GET: OddsService
        public ActionResult Index()
        {
            return View();
        }
    }
}