using Entities;
using HtmlAgilityPack;

namespace Scrapper
{
    public class XPathNodeParser : IDocumentParser<HtmlNode>, IExpression
    {
        public string Expression { get; set; }

        public XPathNodeParser(string Expression) {
            this.Expression = Expression;
        }

        public HtmlNode Parse(HtmlDocument Input)
        {
            return new XPathNodesParser(this.Expression).Parse(Input)?[0];
        }
    }
}
