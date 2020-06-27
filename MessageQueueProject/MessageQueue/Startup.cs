using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MessageQueue
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // add web api middleware
            var config = new HttpConfiguration();

            // attribute routing
            config.MapHttpAttributeRoutes();

            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { 
                    controller = "Default",
                    action = "Index",
                    id = RouteParameter.Optional
                }
            );
            */

            app.UseWebApi(config);            
        }
    }
}
