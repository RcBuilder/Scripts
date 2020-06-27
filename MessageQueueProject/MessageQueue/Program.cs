using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue
{
    class Program
    {
        static string server = "http://localhost:1166";

        static void Main(string[] _args)
        {
            WebApp.Start<Startup>(server);
            Console.WriteLine("Server is Running on port 1166 .... ");            
            Console.ReadKey(); 
        }
    }
}
