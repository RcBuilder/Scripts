using GoogleAdwordsAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AccountChangesGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\tAccount Changes");
                Console.WriteLine("\t===============");

                Console.Write("\tfromDate (yyyy-MM-dd): ");
                var fromDate = DateTime.Parse(Console.ReadLine());               

                Console.Write("\ttoDate (yyyy-MM-dd): ");
                var toDate = DateTime.Parse(Console.ReadLine());                

                Console.Write("\tworking......\n");

                var timer = Stopwatch.StartNew();
                var proxy = new GoogleAdwordsProxy();
                var changes = proxy.GetALLAccountsChanges(fromDate, toDate);
                timer.Stop();

                var sb = new StringBuilder();
                sb.Append("Account, HasChanged\n"); // header

                foreach (var change in changes)
                {
                    Console.WriteLine("\t#{0} {1}", change.AccountId, change.HasChanged);
                    sb.AppendFormat("{0}, {1}\n", change.AccountId, change.HasChanged);
                }

                // e.g: %program%\923577031_201709251400_201709301400.csv
                var fileName = string.Concat(fromDate.ToString("yyyyMMddHHmm"), "_", toDate.ToString("yyyyMMddHHmm"), ".csv");
                var filePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", fileName);

                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                    sw.Write(sb.ToString());

                Console.WriteLine("\t[Export] {0}", fileName);


                Console.WriteLine("\t[Execution Time] {0} sec", (int)timer.Elapsed.TotalSeconds);

            }
            catch (Exception ex) {
                Console.WriteLine("\t[Exception] {0}", ex.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}