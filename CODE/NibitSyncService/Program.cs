using System;
using System.Threading.Tasks;

namespace NibitSyncService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new Processor().Run();

            if (Config.IsTestMode)
            {
                Console.WriteLine("DONE! (press any key to exit)");
                Console.ReadKey();
            }
        }
    }
}
