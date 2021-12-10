using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Crawler.Models;

namespace Crawler.Rbot
{    
    public class RbotProvider
    {
        protected IWebCrawler Crawler { get; set; }

        public RbotProvider() : this(new HttpClientWebCrawler(new CrawlConfiguration())) { }
        public RbotProvider(IWebCrawler Crawler) {
            this.Crawler = Crawler;
        }

        public async Task<List<CrawledPage>> CollectLinks(string URL, IEnumerable<ICrawlerFilter> IncludeFilters = null, IEnumerable<ICrawlerFilter> ExcludeFilters = null)
        {
            var result = new HashSet<CrawledPage>(new CrawledPageComparer());
            
            var useIncludeFilters = IncludeFilters != null && IncludeFilters.Count() > 0;
            var useExcludeFilters = ExcludeFilters != null && ExcludeFilters.Count() > 0;

            this.Crawler.PageCrawlCompleted += (s, e) => {
                var currentURL = e.CrawledPage.URI.GetLeftPart(UriPartial.Path); // remove query-params
                
                var matchInclude = IncludeFilters?.FirstOrDefault(f => f.Execute(currentURL));
                if (useIncludeFilters && matchInclude == null) {
                    Console.WriteLine($"Page {e.CrawledPage.URI} Skipped by Include filter");
                    return;
                }

                var matchExclude = ExcludeFilters?.FirstOrDefault(f => f.Execute(currentURL));
                if (useExcludeFilters && matchExclude != null) {
                    Console.WriteLine($"Page {e.CrawledPage.URI} Skipped by Exclude filter");
                    return;
                }

                if (matchInclude is CrawlerVideoFilter)
                    currentURL = e.CrawledPage.URI.ToString();

                Console.WriteLine($"Page {e.CrawledPage.URI} was added");                
                result.Add(e.CrawledPage);
            };

            var crawlSummary = await this.Crawler.CrawlAsync(new Uri(URL));
            Console.WriteLine($"[Rbot] Completed: {crawlSummary.PagesCount}");

            if (this.Crawler is IDisposable)
                (this.Crawler as IDisposable).Dispose();

            return result.ToList();
        }
    }    
}