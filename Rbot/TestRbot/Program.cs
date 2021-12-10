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
            var crawler = new RbotProvider();
            ///var crawler = new RbotProvider(new PuppeteerWebCrawler(RbotProvider.DefaultConfig));

            var pages = await crawler.CollectLinks("https://www.telegram-group.com/en/adults-18/oooh-la-la/");
            ///var pages = await crawler.CollectLinks("https://rcb.co.il/");

            Console.WriteLine($"{pages.Count} Pages:");
            foreach (var page in pages)
                Console.WriteLine($"{page.URI.AbsoluteUri} (Depth {page.Depth})");                     

            Console.ReadKey();
        }
    }
}
