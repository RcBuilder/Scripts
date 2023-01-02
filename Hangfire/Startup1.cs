
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
            // TODO ->> trust specific certificates
            // trust all certificates
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            BLL.HangfireManager.Init(app);
            BLL.HangfireManager.Start();
        }
    }
}
