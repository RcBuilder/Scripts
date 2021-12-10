using System.Collections.Generic;

namespace Crawler.Models
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
