using System.Text.RegularExpressions;

namespace Crawler.Models
{
    public class CrawlerRegexFilter : ICrawlerFilter
    {        
        public string Expression { get; private set; }
        
        public CrawlerRegexFilter(string Expression) {
            this.Expression = Expression;
        }

        public virtual bool Execute(string Input) {
            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(this.Expression))
                return false;

            return Regex.IsMatch(Input, this.Expression, /*RegexOptions.IgnorePatternWhitespace | */ RegexOptions.IgnoreCase);            
        }
    }
}
