using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class BingSearchEngine : SEOSearchEngine
    {
        const string baseURL = "https://www.bing.com/search?q={0}";

        public override IEnumerable<Entities.SearchResult> SearchTopX(string Phrase, int rowcount)
        {
            ///return Enumerable.Repeat<Entities.SearchResult>(new Entities.SearchResult { Title = "bing title" }, rowcount);

            var pageSource = base.GetPageSource(baseURL, Phrase);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);            
            var nodes = doc.DocumentNode.SelectNodes("//div[@id=\"b_content\"]//h2/a"); // //div[@class=\"b_title\"]/h2
            return nodes.Take(5).Select(x => new Entities.SearchResult
            {
                Title = HttpUtility.HtmlDecode(x.InnerText),
                SearchEngineType = Entities.eSearchEngine.BING
            });
        }
    }
}
