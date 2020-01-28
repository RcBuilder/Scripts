using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Crawler.Rbot
{
    public class CrawlResult
    {
        public Uri RootUri { get; set; }
        public TimeSpan Elapsed { get; set; }
        public Exception ErrorException { get; set; } = null;
        public CrawlContext CrawlContext { get; set; } = new CrawlContext();

        public bool ErrorOccurred
        {
            get
            {
                return this.ErrorException != null;
            }
        }
    }
}
