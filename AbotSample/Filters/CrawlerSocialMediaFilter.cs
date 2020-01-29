using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Crawler.Models
{
    public class CrawlerSocialMediaFilter : CrawlerRegexFilter
    {
        protected static List<string> values = new List<string> {
            @"twitter\.com",
            @"facebook\.com",
            @"linkedin\.com"
        };

        public CrawlerSocialMediaFilter() : base($@"\b https?:// (www.)? ({string.Join("|", values)}) \b") {}
    }
}
