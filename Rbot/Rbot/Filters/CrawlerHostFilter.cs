using System;

namespace Crawler.Models
{
    public class CrawlerHostFilter : CrawlerWildcardFilter
    {
        public CrawlerHostFilter(string URL) : this(new Uri(URL)) { }
        public CrawlerHostFilter(Uri URI) : base(URI.Scheme + "://" + URI.Host + "/*") { }
    }
}
