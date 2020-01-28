using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Crawler
{
    public class CrawlerPageComparer : IEqualityComparer<CrawlerPage>
    {
        public bool Equals(CrawlerPage x, CrawlerPage y)
        {
            return String.Equals(x.URL, y.URL, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(CrawlerPage obj)
        {
            return obj.URL.GetHashCode();
        }
    }
}
