using System;
using System.Collections.Generic;

namespace Crawler.Rbot
{
    public class CrawledPage
    {
        public Uri URI { get; set; }
        public int Depth { get; set; }        
        public TimeSpan Elapsed { get; set; }
        public Exception Exception { get; set; }
        public HashSet<string> Links { get; set; } = new HashSet<string>();

        public CrawledPage(Uri URI, int Depth) {
            this.URI = URI;
            this.Depth = Depth;
        }
    }
}
