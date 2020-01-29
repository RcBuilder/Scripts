﻿using System;
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

        public async Task<List<CrawlerPage>> CollectLinks(string URL, IEnumerable<ICrawlerFilter> IncludeFilters = null, IEnumerable<ICrawlerFilter> ExcludeFilters = null)
        {
            var result = new HashSet<CrawlerPage>(new CrawlerPageComparer());
            var crawler = new WebCrawler(RbotProvider.DefaultConfig);

            var useIncludeFilters = IncludeFilters != null && IncludeFilters.Count() > 0;
            var useExcludeFilters = ExcludeFilters != null && ExcludeFilters.Count() > 0;

            crawler.PageCrawlCompleted += (s, e) => {
                var currentURL = e.CrawledPage.Uri.GetLeftPart(UriPartial.Path); // remove query-params
                Debug.WriteLine($"Page {e.CrawledPage.Uri}");

                var match = IncludeFilters?.FirstOrDefault(f => f.Execute(currentURL));
                if (useIncludeFilters && match == null) {
                    Debug.WriteLine("skipped by Include filter");
                    return;
                }

                match = ExcludeFilters?.FirstOrDefault(f => f.Execute(currentURL));
                if (useExcludeFilters && match != null) {
                    Debug.WriteLine("skipped by Exclude filter");
                    return;
                }

                result.Add(new CrawlerPage
                {
                    URL = currentURL,
                    ContentType = (match is CrawlerVideoFilter) ? eCrawlerContentType.VIDEO : eCrawlerContentType.CONTENT
                });
            };

            var crawlSummary = await crawler.CrawlAsync(new Uri(URL));
            Debug.WriteLine($"[Completed] {crawlSummary.CrawlContext.CrawledCount}");

            return result.ToList();
        }
    }    
}