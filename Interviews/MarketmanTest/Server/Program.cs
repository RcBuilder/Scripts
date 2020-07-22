using Microsoft.Owin.Hosting;
using System;

namespace Server
{
    class Program
    {
        static string server = "http://localhost:1166";
        static void Main(string[] args)
        {
            WebApp.Start<Startup>(server);
            Console.WriteLine("Server is Running on port 1166 .... ");
            Console.ReadKey();
        }
    }
}
