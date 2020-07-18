using Entities;
using HtmlAgilityPack;

namespace Scrapper
{
    public class XPathNodesParser : IDocumentParser<HtmlNodeCollection>, IExpression
    {
        public string Expression { get; set; }

        public XPathNodesParser(string Expression) {
            this.Expression = Expression;
        }

        public HtmlNodeCollection Parse(HtmlDocument Input)
        {
            if (Input == null || string.IsNullOrEmpty(this.Expression))
                return null;
            return Input.DocumentNode.SelectNodes(this.Expression);            
        }
    }
}
