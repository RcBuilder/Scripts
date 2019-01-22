using System;
using Microsoft.AspNet.SignalR.Client;

// Install-Package Microsoft.AspNet.SignalR.Client

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var con = new HubConnection("http://localhost:8888/"))
            {
                var hub = con.CreateHubProxy("MyHub");
                hub.On("onMessageAdded", (msg) => {
                    Console.WriteLine(msg);
                });

                con.Start().Wait();
                Console.WriteLine("CONNECTED");

                string message;
                do {
                    Console.WriteLine("write a new message ('exit' to close):");
                    message = Console.ReadLine();
                    hub.Invoke("AddMessage", message);
                }
                while (message != "exit");
            }
        }
    }
}
