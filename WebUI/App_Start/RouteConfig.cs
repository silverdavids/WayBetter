using System.Web.Mvc;
using System.Web.Routing;

namespace WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                //routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Matches", action = "Index", id = UrlParameter.Optional }
                //routes.MapPageRoute("Tickets", "Reports/Tickets", "~/gbl/Uploadxml.aspx?process=1");
                routes.MapRoute("Default", "{controller}/{action}/{id}/{flag}", new { controller = "Account", action = "Login", id = UrlParameter.Optional, flag = UrlParameter.Optional }
                //routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Fixture", action = "UploadGamesFromDani", id = UrlParameter.Optional }
            );
        }
    }
}
