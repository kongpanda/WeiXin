using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WeiXin.Handler;
namespace WeiXin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new MessageHandler1());
            config.MessageHandlers.Add(new MessageHandler2());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "weixinapi",
                defaults: new { id = RouteParameter.Optional }
            );
           
            
        }
    }
}
