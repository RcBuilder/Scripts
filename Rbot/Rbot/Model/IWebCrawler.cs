
using System;
using System.Threading.Tasks;

namespace Crawler.Rbot
{
    public interface IWebCrawler
    {
        event EventHandler<CrawlEventArgs> PageCrawlCompleted;
        Task<CrawlSummary> CrawlAsync(Uri uri);
    }
}
