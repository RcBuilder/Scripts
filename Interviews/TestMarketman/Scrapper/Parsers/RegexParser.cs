using Entities;
using System.Text.RegularExpressions;

namespace Scrapper
{
    public class RegexParser : IParser<string, string>, IExpression
    {
        public string Expression { get; set; }

        public RegexParser(string Expression) {
            this.Expression = Expression;
        }

        public string Parse(string Input)
        {
            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(this.Expression))
                return null;

            var match = Regex.Match(Input, this.Expression, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            if (match == null || !match.Success) return null;
            return match.Value.Trim();
        }
    }
}
