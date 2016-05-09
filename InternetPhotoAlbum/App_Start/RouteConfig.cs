using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InternetPhotoAlbum
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Photos",
                url: "Home/Photos/{id}/{userName}",
                defaults: new
                {
                    controller = "Home",
                    action = "Photos",
                    id = UrlParameter.Optional,
                    userName = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Rating",
                url: "Home/Rate/{id}/{userName}/{rating}",
                defaults: new
                {
                    controller = "Home",
                    action = "Rate",
                    id = UrlParameter.Optional,
                    userName = UrlParameter.Optional,
                    rating = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "RemoveRate",
                url: "Home/RemoveRate/{id}/{userName}",
                defaults: new
                {
                    controller = "Home",
                    action = "RemoveRate",
                    id = UrlParameter.Optional,
                    userName = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}
