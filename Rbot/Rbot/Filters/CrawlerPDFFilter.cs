
namespace Crawler.Models
{
    public class CrawlerPDFFilter : CrawlerRegexFilter
    {        
        public CrawlerPDFFilter() : base("\\.pdf") {}
    }
}
