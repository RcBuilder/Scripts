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
    public class GoogleSearchEngine : SEOSearchEngine
    {
        const string baseURL = "https://www.google.com/search?q={0}";
        
        public override IEnumerable<Entities.SearchResult> SearchTopX(string Phrase, int rowcount) {

            ///return Enumerable.Repeat<Entities.SearchResult>(new Entities.SearchResult { Title = "google title" }, rowcount);

            var pageSource = base.GetPageSourceUsingDriver(baseURL, Phrase);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            
            var nodes = doc.DocumentNode.SelectNodes("//div[@id=\"search\"]//h3");
            return nodes.Take(5).Select(x => new Entities.SearchResult {
                Title = HttpUtility.HtmlDecode(x.InnerText),
                SearchEngineType = Entities.eSearchEngine.GOOGLE
            });
        }
    }
}
