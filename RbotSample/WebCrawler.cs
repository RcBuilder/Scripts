using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Crawler.Rbot
{
    public class WebCrawler
    {
        protected HttpClient client { get; set; }
        protected CrawlConfiguration Config { get; set; }

        public event EventHandler<CrawlEventArgs> PageCrawlCompleted;

        public WebCrawler(CrawlConfiguration Config)
        {
            this.Config = Config;
            this.client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            }, false);
	    this.client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.116 Safari/537.36");
        }

        protected void OnPageCrawlCompleted(CrawlEventArgs args)
        {
            PageCrawlCompleted?.Invoke(null, args);
        }

        public async Task<CrawlResult> CrawlAsync(Uri uri)
        {

            var result = new CrawlResult();

            var timer = new Stopwatch();
            timer.Start();

            try
            {
                var response = string.Empty;

                client.Timeout = TimeSpan.FromSeconds(this.Config.CrawlTimeoutSeconds);
                using (var resonse = await client.GetAsync(uri.ToString()).ConfigureAwait(false))
                    response = await resonse.Content.ReadAsStringAsync();

                var document = new HtmlDocument();
                document.LoadHtml(response);

                if (this.Config.IgnoreFooter) {
                    document.DocumentNode.SelectSingleNode("//footer")?.RemoveAll();
                    document.DocumentNode.SelectSingleNode("//*[contains(@class, 'footer-')]")?.RemoveAll();
                }
                if (this.Config.IgnoreHeader) {
                    document.DocumentNode.SelectSingleNode("//header")?.RemoveAll();
                    document.DocumentNode.SelectSingleNode("//*[contains(@class, 'header-')]")?.RemoveAll();
                }

                var links = document.DocumentNode.SelectNodes("//a")
                    ?.Select(x => x.Attributes["href"]?.Value)
                    .Where(x => !string.IsNullOrEmpty(x));

                links.AsParallel().ForAll(link => {

                    if (!link.StartsWith("http"))
                        link = uri.Scheme + "://" + uri.Host + "/" + (link.StartsWith("/") ? link.Remove(0, 1) : link);

                    result.CrawlContext.CrawledCount++;
                    OnPageCrawlCompleted(new CrawlEventArgs
                    {
                        CrawledPage = new CrawledPage
                        {
                            Uri = new Uri(link)
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
            }

            timer.Stop();

            result.Elapsed = timer.Elapsed;
            return result;
        }
    }
}
