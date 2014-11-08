﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BetLive
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DefaultWithControllerAndActionMethod",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "MyMatches", action = "Index", id = UrlParameter.Optional }
                defaults: new { controller = "Live", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Management", action = "matches", id = UrlParameter.Optional }
           );
        }
    }
}