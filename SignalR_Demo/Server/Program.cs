using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

// Install-Package Microsoft.AspNet.SignalR
// Install-Package Microsoft.Owin.Hosting
// Install-Package Microsoft.Owin.Cors
// Install-Package Microsoft.Owin.Host.HttpListener

namespace Server
{
    class Program
    {
        static void Main(string[] args) {
            string url = "http://localhost:8888";
            using (WebApp.Start<Startup>(url)) {
                Console.WriteLine("Server is Running");
                Console.ReadKey();
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app) {
            app.UseCors(CorsOptions.AllowAll); // cross-domain support
            app.MapSignalR();
        }
    }

    public class MyHub : Hub {
        public void AddMessage(string message) {
            Clients.All.onMessageAdded(message);
        }
    }
}
