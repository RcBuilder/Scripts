using Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProxyServer
{        
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // trust all certificates
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            // message handler registration
            /// config.MessageHandlers.Add(new SampleProxyMessageHandler("https://jsonplaceholder.typicode.com"));

            var jwtKey = ConfigurationManager.AppSettings["JWTSecretKey"]?.Trim();
            var logTraffic = (ConfigurationManager.AppSettings["LogTraffic"]?.Trim() ?? "1") == "1";
            var logger = new SQLDBLogger(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString?.Trim());            
            config.MessageHandlers.Add(new CreativeProxyMessageHandler(jwtKey, logTraffic ? logger : null));
            
            // any request
            config.Routes.MapHttpRoute("default", "{*path}");
        }
    }
}
