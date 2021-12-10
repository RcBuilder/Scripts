using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler.Rbot
{
    public class HttpClientWebCrawler : WebCrawler, IDisposable
    {        
        protected HttpClient Client { get; set; }
        
        public HttpClientWebCrawler(CrawlConfiguration Config) : base(Config)
        {            
            this.Client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            }, false);

            this.Client.Timeout = TimeSpan.FromSeconds(this.Config.CrawlTimeoutSeconds);
            this.Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.116 Safari/537.36");
        }

        public void Dispose()
        {
            if (this.Client != null)
                this.Client.Dispose();
        }

        public override async Task<string> GetContant(Uri uri)
        {
            var resonse = await Client.GetAsync(uri.ToString()).ConfigureAwait(false);
            if(this.Config.CrawlDelaySeconds > 0)
                await Task.Delay(Convert.ToInt32(this.Config.CrawlDelaySeconds * SECOND));
            return await resonse.Content.ReadAsStringAsync();
        }
    }
}
