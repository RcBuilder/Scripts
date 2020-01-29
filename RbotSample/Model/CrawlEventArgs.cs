using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Crawler.Rbot
{
    public class CrawlEventArgs : EventArgs
    {
        public CrawledPage CrawledPage { get; set; }
    }
}
