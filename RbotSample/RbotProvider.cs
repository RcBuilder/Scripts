using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CliClap.Crawler.Models;
using HtmlAgilityPack;
using CliClap.Crawler.Rbot;

namespace CliClap.Crawler
{    
    public class RbotProvider
    {
        protected static CrawlConfiguration DefaultConfig = new CrawlConfiguration
        {
            MaxPagesToCrawl = 100,  // Pages quantity to scan 
            MinCrawlDelayPerDomainMilliSeconds = 100,  // min delay time between requests 
            MaxConcurrentThreads = 10,  // max threads quantity 
            CrawlTimeoutSeconds = 30  // timeout in seconds
        };

        public async Task<List<CrawlerPage>> CollectLinks(string URL, IEnumerable<ICrawlerFilter> Filters = null)
        {
            var result = new HashSet<CrawlerPage>(new CrawlerPageComparer());
            var crawler = new WebCrawler(RbotProvider.DefaultConfig);

            var useFilters = Filters != null && Filters.Count() > 0;

            crawler.PageCrawlCompleted += (s, e) => {
                var currentURL = e.CrawledPage.Uri.ToString();
                Debug.WriteLine($"Page {e.CrawledPage.Uri}");

                var match = Filters?.FirstOrDefault(f => f.Execute(currentURL));
                if (!useFilters || (useFilters && match != null))
                {
                    result.Add(new CrawlerPage
                    {
                        URL = currentURL,
                        ContentType = (match is CrawlerVideoFilter) ? eCrawlerContentType.VIDEO : eCrawlerContentType.CONTENT
                    });
                }
            };

            var crawlSummary = await crawler.CrawlAsync(new Uri(URL));
            Debug.WriteLine($"[Completed] {crawlSummary.CrawlContext.CrawledCount}");

            return result.ToList();
        }
    }    
}