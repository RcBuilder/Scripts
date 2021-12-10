using System;

namespace Crawler.Rbot
{
    public class CrawlEventArgs : EventArgs
    {
        public CrawledPage CrawledPage { get; set; }
    }
}
