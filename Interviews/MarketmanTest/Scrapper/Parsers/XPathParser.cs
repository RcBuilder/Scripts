using Entities;
using HtmlAgilityPack;

namespace Scrapper
{
    public class XPathParser : INodeParser<string>, IExpression
    {
        public string Expression { get; set; }

        public XPathParser(string Expression) {
            this.Expression = Expression;
        }

        public string Parse(HtmlNode Input)
        {
            if (Input == null || string.IsNullOrEmpty(this.Expression))
                return null;
            return Input.SelectSingleNode(this.Expression)?.InnerText.Trim();
        }
    }
}
