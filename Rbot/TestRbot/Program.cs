using System;
using System.Linq;
using System.Threading.Tasks;
using Crawler;
using Crawler.Rbot;

/*
    Install on Rbot to merge dlls as package for dist.
    > Install-Package ilmerge
    > Install-Package ILMerge.MSBuild.Task 
*/


namespace TestRbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ///await Test1();
            await Test2();

            Console.ReadKey();
        }

        static async Task Test1() {
            /// var crawler = new RbotProvider();
            /// var crawler = new RbotProvider(new PuppeteerWebCrawler(RbotProvider.DefaultConfig));
            var crawler = new RbotProvider(new HttpClientWebCrawler(new CrawlConfiguration
            {
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
        }

        static async Task Test2()
        {
            var crawler = new PuppeteerWebCrawler(new CrawlConfiguration());
            var responseBody = await crawler.GetContant(new Uri("https://www.facebook.com/ads/library/?active_status=active&ad_type=all&country=US&is_targeted_country=false&media_type=all&q=mywebanswers.com&search_type=keyword_unordered"));            
            Console.WriteLine(responseBody);
        }
    }
}