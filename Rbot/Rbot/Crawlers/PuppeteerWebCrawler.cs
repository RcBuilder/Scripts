using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Rbot
{
    public class PuppeteerWebCrawler : WebCrawler, IDisposable
    {        
        protected Browser Client { get; set; }
        
        public PuppeteerWebCrawler(CrawlConfiguration Config) : base(Config) {        
            this.Client.DefaultWaitForTimeout = this.Config.CrawlTimeoutSeconds * SECOND;            
        }
        
        public override async Task<CrawlSummary> CrawlAsync(Uri uri)
        {            
            if(this.Client == null)
                this.Client = await this.CreateWebBrowser(null);
            return await base.CrawlAsync(uri);
        }

        public override async Task<string> GetContant(Uri uri)
        {
            var page = await this.Client.NewPageAsync();
            await page.GoToAsync(uri.AbsoluteUri);
            ///await page.WaitForSelectorAsync("//div[@class=\"ad_cclk\"]");
            if (this.Config.CrawlDelaySeconds > 0)
                await Task.Delay(Convert.ToInt32(this.Config.CrawlDelaySeconds * SECOND));
            return await page.GetContentAsync();
        }

        private async Task<Browser> CreateWebBrowser(string ChromePath)
        {
            async Task<WebSocket> CreateWebSocketTask(Uri url, IConnectionOptions options, CancellationToken cancellationToken)
            {
                var result = new System.Net.WebSockets.Managed.ClientWebSocket();
                result.Options.KeepAliveInterval = TimeSpan.Zero;
                await result.ConnectAsync(url, cancellationToken).ConfigureAwait(false);
                return result;
            }

            var downloadChrome = string.IsNullOrEmpty(ChromePath) || !File.Exists(ChromePath);
            if (downloadChrome)
            {
                Debug.Write("downloading Chrome...");
                var browserFetcherOptions = new BrowserFetcherOptions
                {
                    Path = $"{AppDomain.CurrentDomain.BaseDirectory}",
                    Platform = Platform.Win64,
                    Product = Product.Chrome
                };
                var browserFetcher = new BrowserFetcher(browserFetcherOptions);
                var revisionInfo = await browserFetcher.DownloadAsync();
                ChromePath = revisionInfo.ExecutablePath;  // set the created chrome path
                Debug.WriteLine("done!");
            }

            var launchOptions = new LaunchOptions
            {
                Headless = false,
                Timeout = (10 * 1000),  // 10 sec
                ExecutablePath = ChromePath, /// $"{AppDomain.CurrentDomain.BaseDirectory}Win64-884014\\chrome-win\\chrome.exe",
                DefaultViewport = null, // max resolution
                IgnoreHTTPSErrors = true,
                Args = new[] { "--no-sandbox", "--disable-gpu" }  // "--window-size=800,1080"
            };

            if (Environment.OSVersion.Version.Major < 10)
                launchOptions.WebSocketFactory = CreateWebSocketTask;  // support for windows-7

            var browser = await Puppeteer.LaunchAsync(launchOptions);
            return browser;
        }

        public void Dispose()
        {
            if (this.Client != null)
                this.Client.Dispose();
        }
    }
}
