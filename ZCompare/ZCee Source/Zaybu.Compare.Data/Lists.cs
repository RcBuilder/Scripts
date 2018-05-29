using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaybu.Compare.Data
{
    /// <summary>
    /// Approximate (lazy) calculations :)
    /// </summary>
    public class StatisticList : List<int>
    {
        public float Average
        {
            get
            {
                long total = 0;
                ForEach(i => total += i);
                return total / Count;
            }
        }

        public int Median { get { Sort(); return base[(Count - 1) / 2]; } }

        public int Min { get { Sort(); return base[0]; } }

        public int Max { get { Sort(); return base[Count - 1]; } }
    }
}
