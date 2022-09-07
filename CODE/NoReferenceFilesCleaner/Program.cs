using App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NoReferenceFilesCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var mode = (Entities.eExecutionMode)Enum.Parse(typeof(Entities.eExecutionMode), ConfigurationManager.AppSettings["MODE"].Trim());
            Console.WriteLine($"[MODE = {mode}]");

            var useBackup = ConfigurationManager.AppSettings["USE_BACKUP"].Trim() == "1";
            Console.WriteLine($"[USE_BACKUP = {useBackup}]");

            Console.WriteLine($"\nRUNNING VIDEO PROCESS...");
            VideoProcess.Run(mode, useBackup);
           
            Console.WriteLine($"\nRUNNING BOOK PROCESS...");
            BookProcess.Run(mode, useBackup);

            Console.WriteLine($"COMPLETED! press any key to exit");
            Console.ReadKey();
        }
    }
}
