using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Itau
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // routes.MapRoute(
            //  name: "Paineis",
            //  url: "",
            //  defaults: new { controller = "Painel", action = "Index"}
            //);

            // routes.MapRoute(
            //   name: "Painel",
            //   url: "{equipe}",
            //   defaults: new { controller = "Painel", action = "Index", equipe = "{equipe}" }
            // );

            // routes.MapRoute(
            //    name: "Configuração",
            //    url: "{equipe}/config",
            //    defaults: new { controller = "Config", action = "EditarSquad", equipe = "{equipe}" }
            //  );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{equipe}",
                defaults: new { controller = "Painel", action = "Index", equipe = UrlParameter.Optional }
            );
        }
    }
}
