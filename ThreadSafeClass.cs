using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    /*
        // less then 2000
        var tsC1 = new ThreadSafeClass();
        Parallel.For(0, 1000, _ => { tsC1.Count++; });
        Parallel.For(0, 1000, _ => { tsC1.Count++; });
        Console.WriteLine("count: {0}", tsC1.Count);

        // exactly 2000
        var tsC2 = new ThreadSafeClass();
        Parallel.For(0, 1000, _ => { tsC2.CountSafe(); });
        Parallel.For(0, 1000, _ => { tsC2.CountSafe(); });
        Console.WriteLine("count: {0}", tsC2.Count); 
    */

    public class ThreadSafeClass
    {
        private object syncLock = new object();

        public int Count { get; set; }

        public void CountSafe()
        {
            lock (syncLock)
                Count++;
        }
    }
}
