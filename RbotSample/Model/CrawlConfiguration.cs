using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Crawler.Rbot
{
    public class CrawlConfiguration
    {
        public int MaxPagesToCrawl { get; set; }  // Pages quantity to scan 
        public int MinCrawlDelayPerDomainMilliSeconds { get; set; }  // min delay time between requests 
        public int MaxConcurrentThreads { get; set; }  // max threads quantity 
        public int CrawlTimeoutSeconds { get; set; }  // timeout in seconds
        public bool IgnoreFooter { get; set; }  // ignore footer
        public bool IgnoreHeader { get; set; }  // ignore header
    }
}
