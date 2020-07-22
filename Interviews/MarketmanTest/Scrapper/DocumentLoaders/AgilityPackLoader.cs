using System.Threading.Tasks;
using Entities;
using HtmlAgilityPack;

namespace Scrapper
{
    public class AgilityPackLoader : IAsyncDocumentLoader
    {
        private const double TimeOutSec = 30;

        public HtmlDocument Load(string URL)
        {
            return new HtmlWeb().Load(URL);
        }

        public Task<HtmlDocument> LoadAsync(string URL)
        {
            return new HtmlWeb().LoadFromWebAsync(URL);
        }

        public HtmlDocument LoadHtml(string Input)
        {
            var document = new HtmlDocument();
            document.LoadHtml(Input);
            return document;
        }
    }
}
