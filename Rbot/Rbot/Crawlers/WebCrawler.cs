using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler.Rbot
{
    public abstract class WebCrawler : IWebCrawler
    {
        protected const int SECOND = 1000;

        protected CrawlConfiguration Config { get; set; }

        public event EventHandler<CrawlEventArgs> PageCrawlCompleted;

        public WebCrawler(CrawlConfiguration Config)
        {
            this.Config = Config;
        }

        protected void OnPageCrawlCompleted(CrawlEventArgs args)
        {
            PageCrawlCompleted?.Invoke(null, args);
        }

        public virtual async Task<CrawlSummary> CrawlAsync(Uri RootUri)
        {
            var summary = new CrawlSummary();
            var queue = new Queue<CrawledPage>();

            var rootPage = new CrawledPage(RootUri, 1);
            await this.CrawlPageAsync(rootPage);
            queue.Enqueue(rootPage);
            var Domain = RootUri.Host;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                try
                {
                    if (current.Depth > this.Config.MaxDepth) continue;
                    if (this.Config.MaxPagesToCrawl > 0 && summary.PagesCount >= this.Config.MaxPagesToCrawl) continue;
                    if (summary.PageExists(current)) continue;
                    if (string.IsNullOrEmpty(current.URI.AbsoluteUri)) continue;

                    summary.AddPages(current);
                    OnPageCrawlCompleted(new CrawlEventArgs
                    {
                        CrawledPage = current
                    });

                    if (current.URI.Host != Domain) continue;

                    await this.CrawlPageAsync(current);                                        
                    foreach (var link in current.Links)
                    {
                        try
                        {                            
                            var uri = new Uri(link);
                            queue.Enqueue(new CrawledPage(uri, current.Depth + 1));                            
                        }
                        catch (Exception Ex)
                        {
                            Console.WriteLine($"[ERROR] Link: '{link}', EX: {Ex.Message}");
                        }
                    }
                }
                catch (Exception Ex) {
                    Console.WriteLine($"[ERROR] Page: '{current.URI}', EX: {Ex.Message}");
                }
            }

            return summary;
        }

        private async Task CrawlPageAsync(CrawledPage page)
        {            
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                var content = await this.GetContant(page.URI);
                var document = new HtmlDocument();
                document.LoadHtml(content);

                var rootNode = string.IsNullOrEmpty(this.Config.RootNodeSelector) ? document.DocumentNode : document.DocumentNode.SelectSingleNode(this.Config.RootNodeSelector);

                if (this.Config.IgnoreFooter)
                {
                    rootNode.SelectSingleNode("//footer")?.RemoveAll();
                    rootNode.SelectSingleNode("//*[contains(@class, 'footer-')]")?.RemoveAll();
                }
                if (this.Config.IgnoreHeader)
                {
                    rootNode.SelectSingleNode("//header")?.RemoveAll();
                    rootNode.SelectSingleNode("//*[contains(@class, 'header-')]")?.RemoveAll();
                }

                if (this.Config.LinksSelector?.StartsWith("//") ?? false)
                    this.Config.LinksSelector = string.Concat(".", this.Config.LinksSelector); // relative

                var nodes = rootNode.SelectNodes(string.IsNullOrEmpty(this.Config.LinksSelector) ? ".//a" : this.Config.LinksSelector);                
                var links = nodes
                    ?.Where(x => x.Name == "a")
                    ?.Select(x => x.Attributes["href"]?.Value?.Trim())
                    .Where(x => !string.IsNullOrEmpty(x)).ToHashSet();

                foreach (var temp in links)
                {
                    var link = temp;
                    if (!link.StartsWith("http"))
                        link = page.URI.Scheme + "://" + page.URI.Host + "/" + (link.StartsWith("/") ? link.Remove(0, 1) : link);
                    page.Links.Add(link);
                };
            }
            catch (Exception ex)
            {
                page.Exception = ex;
            }
            finally
            {
                timer.Stop();
                page.Elapsed = timer.Elapsed;                
            }
        }

        public abstract Task<string> GetContant(Uri uri);
    }
}