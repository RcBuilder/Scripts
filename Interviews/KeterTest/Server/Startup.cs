using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;


namespace Server
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();  // attribute routing     
            app.UseCors(CorsOptions.AllowAll);  // Install-Package Microsoft.Owin.Cors
            app.UseWebApi(config);  // add web api middleware          
        }
    }
}
