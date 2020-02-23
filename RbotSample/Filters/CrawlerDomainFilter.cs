using CliClap.Library.SearchWebsite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Crawler.Models
{
    public class CrawlerDomainFilter : CrawlerWildcardFilter
    {        
        public CrawlerDomainFilter(string URL) : base($"*{ NetDomain.GetDomainFromUrl(URL) }/*") { }        
    }
}
