using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Crawler.Models
{
    public class CrawlerPDFFilter : CrawlerRegexFilter
    {        
        public CrawlerPDFFilter() : base("\\.pdf") {}
    }
}
