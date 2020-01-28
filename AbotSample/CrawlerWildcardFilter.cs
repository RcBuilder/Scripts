using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Web.Serivces.API.Models
{
    public class CrawlerWildcardFilter : CrawlerRegexFilter
    {        
        public CrawlerWildcardFilter(string Pattern) : base(Regex.Escape(Pattern).Replace("\\*", ".*?")) {}
    }
}
