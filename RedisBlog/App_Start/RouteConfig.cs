using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RedisBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");            

            routes.MapRoute(
                name: "EditItem",
                url: "EditItem/{ProductID}/{Id}",
                defaults: new { controller = "Home", action = "EditItem", ProductID = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AddItem",
                url: "AddItem/{selectedProduct}",
                defaults: new { controller = "Home", action = "AddItem", selectedProduct = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
