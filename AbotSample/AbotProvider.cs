using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Abot2.Poco;
using Abot2.Crawler;
using CliClap.Web.Serivces.API.Models;
using System.Threading.Tasks;

/*    
    Abot Crawler
    ------------
    Nuget:
    Install-Package Abot

    Namespaces:
    using Abot2.Poco;
    using Abot2.Crawler;

    Sources:
    https://www.nuget.org/packages/Abot/
    https://github.com/sjdirect/abot

    Process:
    trigger a 'PageCrawlCompleted' Event for each crawled page.
    await the 'CrawlAsync' method to get a summary of the crawling process (optional).   

    Config:
    CrawlTimeoutSeconds
    MaxConcurrentThreads
    MaxPagesToCrawl
    UserAgentString
    ConfigurationExtensions  
        
    Events:
    PageCrawlStarting
    PageCrawlCompleted
    PageCrawlDisallowed
    PageLinksCrawlDisallowed
*/

namespace CliClap.Web.Serivces.API.CrawlerServices
{
    public class AbotProvider
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
            var result = new List<CrawlerPage>();                      
            var crawler = new PoliteWebCrawler(AbotProvider.DefaultConfig);

            crawler.PageCrawlCompleted += (s, e) => {
                var currentURL = e.CrawledPage.Uri.ToString();
                /// Console.WriteLine($"Page {e.CrawledPage.Uri}, Depth: {e.CrawledPage.CrawlDepth} -> {e.CrawledPage.ParsedLinks?.Count()} Links");

                if (Filters == null || Filters.Any(f => f.Execute(currentURL)))
                {                
                    result.Add(new CrawlerPage
                    {
                        URL = currentURL,
                        Depth = e.CrawledPage.CrawlDepth,
                        Links = e.CrawledPage.ParsedLinks?.Select(x => x.HrefValue.ToString()).ToList()
                    });
                }
            };

            ///var crawlSummary = await crawler.CrawlAsync(new Uri(URL));
            ///Console.WriteLine($"[Completed] {crawlSummary.CrawlContext.CrawledCount}");

            await crawler.CrawlAsync(new Uri(URL));
            return result;
        }
    }
}