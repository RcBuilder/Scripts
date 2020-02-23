using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Crawler.Models
{
    public class CrawlerHostFilter : CrawlerWildcardFilter
    {
        public CrawlerHostFilter(string URL) : this(new Uri(URL)) { }
        public CrawlerHostFilter(Uri URI) : base(URI.Scheme + "://" + URI.Host + "/*") { }
    }
}
