
namespace Crawler.Rbot
{
    public class CrawlConfiguration
    {
        public string RootNodeSelector { get; set; }
        public string LinksSelector { get; set; }
        public int MaxPagesToCrawl { get; set; } = 0; // Pages quantity to scan (0 = all)
        public int MaxDepth { get; set; } = 3; // how deep to crawl
        public float CrawlDelaySeconds { get; set; } = 0F;  // min delay time between requests (0 = no-delay)          
        public int CrawlTimeoutSeconds { get; set; } = 30;  // timeout in seconds
        public bool IgnoreFooter { get; set; } = true;  // ignore footer
        public bool IgnoreHeader { get; set; } = true; // ignore header            
    }
}