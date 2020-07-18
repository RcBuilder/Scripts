using Entities;
using HtmlAgilityPack;

namespace Scrapper
{
    public class XPathAttributeParser : INodeParser<string>, IExpression
    {
        public string Expression { get; set; }
        public string AttributeName { get; set; }

        public XPathAttributeParser(string Expression, string AttributeName) {
            this.Expression = Expression;
            this.AttributeName = AttributeName;
        }

        public string Parse(HtmlNode Input)
        {
            if (Input == null || string.IsNullOrEmpty(this.Expression))
                return null;
            return Input.SelectSingleNode(this.Expression)?.Attributes[this.AttributeName]?.Value?.Trim();
        }
    }
}
