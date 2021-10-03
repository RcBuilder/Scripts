
// Install-Package Microsoft.Owin -Version 4.2.0
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BrokerServicesService.Startup1))]

namespace BrokerServicesService
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {            
            BLL.HangfireManager.Init(app);
            BLL.HangfireManager.Start();
        }
    }
}
