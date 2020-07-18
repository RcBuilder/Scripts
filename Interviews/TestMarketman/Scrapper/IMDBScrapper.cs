using Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapper
{
    public class IMDBScrapper : Scrapper<List<CelebrityCard>>
    {
        protected const string DATA_URL = "https://www.imdb.com/list/ls052283250/";
        public IMDBScrapper(IDocumentLoader DocumentLoader) : base(DocumentLoader) { }
        
        public override HtmlDocument Load()
        {
            return this.DocumentLoader.Load(IMDBScrapper.DATA_URL);                     
        }

        public override async Task<HtmlDocument> LoadAsync() {
            if (this.DocumentLoader is IAsyncDocumentLoader)            
                return await (this.DocumentLoader as IAsyncDocumentLoader).LoadAsync(IMDBScrapper.DATA_URL);
            return this.Load();
        }

        public override void Parse(HtmlDocument Document) {
            this.Value = new List<CelebrityCard>();
            var cardNodes = new XPathNodesParser("//div[@class=\"lister-item mode-detail\"]").Parse(Document);
            foreach (var cardNode in cardNodes)
            {
                var card = new CelebrityCard
                {
                    Name = new XPathParser(".//h3/a").Parse(cardNode),
                    KnownArtwork = new XPathParser(".//p[1]/a").Parse(cardNode),
                    Profile = new XPathAttributeParser(".//div[@class=\"lister-item-image\"]/a/img", "src").Parse(cardNode),
                    Desc = new XPathParser(".//p[2]").Parse(cardNode),
                    Type = new XPathParser(".//h3/following-sibling::p/text()").Parse(cardNode),
                };

                var idHref = new XPathAttributeParser(".//h3/a", "href").Parse(cardNode);
                card.Id = new RegexParser("(?<=/name/)[A-Za-z0-9]+(?=\\?)").Parse(idHref);

                var sBirthDate = new DateParser().Parse(card.Desc);
                if(!string.IsNullOrEmpty(sBirthDate))
                    card.BirthDate = DateTime.Parse(sBirthDate);

                this.Value.Add(card);
            }
        }        
    }
}
