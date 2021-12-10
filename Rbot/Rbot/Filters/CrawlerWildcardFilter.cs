using System.Text.RegularExpressions;

namespace Crawler.Models
{
    public class CrawlerWildcardFilter : CrawlerRegexFilter
    {        
        public CrawlerWildcardFilter(string Pattern) : base(Regex.Escape(Pattern).Replace("\\*", ".*?")) {}
    }
}
