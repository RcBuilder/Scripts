using System;
using System.Linq;
using System.Threading.Tasks;
using Crawler;
using Crawler.Rbot;

namespace TestRbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /// var crawler = new RbotProvider();
            /// var crawler = new RbotProvider(new PuppeteerWebCrawler(RbotProvider.DefaultConfig));
            var crawler = new RbotProvider(new HttpClientWebCrawler(new CrawlConfiguration {
                MaxDepth = 3,
                CrawlDelaySeconds = 0,
                MaxPagesToCrawl = 1000,
                RootNodeSelector = "//div[@id='main-content']",
                LinksSelector = "(.//span[contains(@class, 'butGroup')]/parent::div/h2/a|.//div[@class='entry-content']/a)"
            }));
            
            var pages = await crawler.CollectLinks("https://www.telegram-group.com/en/adults-18/");
            ///var pages = await crawler.CollectLinks("https://rcb.co.il/");

            Console.WriteLine($"{pages.Count} Pages:");
            foreach (var page in pages)
                Console.WriteLine($"{page.URI.AbsoluteUri} (Depth {page.Depth})");                     

            Console.ReadKey();
        }
    }
}
