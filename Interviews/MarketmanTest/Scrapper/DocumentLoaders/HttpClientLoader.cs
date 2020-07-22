using Entities;
using HtmlAgilityPack;
using System;
using System.Net.Http;

namespace Scrapper
{
    public class HttpClientLoader : IDocumentLoader
    {
        private const double TimeOutSec = 30;
        private HttpClientHandler settings { get; } = new HttpClientHandler {
            UseProxy = false  // to improve velocity (skipping the proxy seek)
        };

        public HtmlDocument Load(string URL)
        {
            var result = string.Empty;
            using (var client = new HttpClient(this.settings, false))
            {
                client.Timeout = TimeSpan.FromSeconds(TimeOutSec);
                using (var resonse = client.GetAsync(URL).Result)
                    result = resonse.Content.ReadAsStringAsync().Result;
            }

            return LoadHtml(result);
        }

        public HtmlDocument LoadHtml(string Input)
        {
            var document = new HtmlDocument();
            document.LoadHtml(Input);
            return document;
        }
    }
}
