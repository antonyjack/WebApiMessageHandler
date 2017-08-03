using Custom.HttpMessageHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Custom.HttpMessageHandler
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var Handler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), new DelegatingHandler[] { new CustomMessageHandler() });

            //config.MessageHandlers.Add(new CustomMessageHandler()); //Global level handler mapping

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }                
            );

            config.Routes.MapHttpRoute(
                name: "HomeApi",
                routeTemplate: "Home/{action}/{id}",
                defaults: new 
                { 
                    Controller = "Home", 
                    id = RouteParameter.Optional 
                },
                constraints: null,
                handler: Handler //Route level message handler
            );
        }
    }
}
