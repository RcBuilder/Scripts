using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CliClap.Crawler.Rbot
{
    public class CrawlContext
    {
        private int _CrawledCount = 0;
        public int CrawledCount {
            get {
                return _CrawledCount;
            }
            set {
                Interlocked.Add(ref _CrawledCount, 1);
            }
        }
    }
}
