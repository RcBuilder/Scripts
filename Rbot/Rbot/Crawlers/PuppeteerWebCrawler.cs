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

        public PuppeteerWebCrawler(CrawlConfiguration Config) : base(Config) {}
        
        public override async Task<CrawlSummary> CrawlAsync(Uri uri)
        {
            if (this.Client == null)
                this.Client = await this.CreateWebBrowser();

            if (this.Client == null)
                this.Client = await this.CreateWebBrowser(null);
            return await base.CrawlAsync(uri);
        }

        public override async Task<string> GetContant(Uri uri)
        {
            if (this.Client == null)
                this.Client = await this.CreateWebBrowser();

            var page = await this.Client.NewPageAsync();
            await page.GoToAsync(uri.AbsoluteUri);
            ///await page.WaitForSelectorAsync("//div[@class=\"ad_cclk\"]");
            if (this.Config.CrawlDelaySeconds > 0)
                await Task.Delay(Convert.ToInt32(this.Config.CrawlDelaySeconds * SECOND));
            return await page.GetContentAsync();
        }

        private async Task<Browser> CreateWebBrowser(string ChromePath = null)
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

/*
    -- PdfManager --

    private Browser browser { get; set; } = null;
    private int delayInMS { get; set; } = 2000;

    public virtual async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
    {
        if(this.browser == null)
            this.browser = await this.CreateWebBrowser(ChromePath);
            
        var webPage = await this.browser.NewPageAsync();
        await webPage.GoToAsync(URL);

        await Task.Delay(this.delayInMS);

        var screenshotOptions = new ScreenshotOptions
        {
            FullPage = true,
            Quality = 100,
            Type = ScreenshotType.Jpeg 
        };

        return await webPage.ScreenshotStreamAsync(screenshotOptions);            
    }

    public virtual async Task<Stream> CreateFromBrowser(string URL, string ChromePath = null)
    {
        if (this.browser == null)
            this.browser = await this.CreateWebBrowser(ChromePath);
            
        var webPage = await this.browser.NewPageAsync();
        await webPage.GoToAsync(URL);
            
        await Task.Delay(this.delayInMS);

        /// return await webPage.PdfDataAsync(); // as bytes
        return await webPage.PdfStreamAsync();   // as stream           
    }

    public virtual async Task SaveFromScreenshot(string URL, string FilePath, string ChromePath = null)
    {
        if (this.browser == null)
            this.browser = await this.CreateWebBrowser(ChromePath);

        var webPage = await this.browser.NewPageAsync();
        await webPage.GoToAsync(URL);

        await Task.Delay(this.delayInMS);

        var screenshotOptions = new ScreenshotOptions
        {
            FullPage = true,
            Quality = 100,
            Type = ScreenshotType.Jpeg
        };

        using (var screenshotStream = await webPage.ScreenshotStreamAsync(screenshotOptions))
        using (var image = Image.FromStream(screenshotStream))
            image.Save(FilePath);            
    }

    public virtual async Task SaveFromBrowser(string URL, string FilePath, string ChromePath = null)
    {
        if (this.browser == null)
            this.browser = await this.CreateWebBrowser(ChromePath);

        var webPage = await this.browser.NewPageAsync();
        await webPage.GoToAsync(URL);

        await Task.Delay(this.delayInMS);

        var pdfOptions = new PdfOptions { };
        await webPage.PdfAsync(FilePath, pdfOptions);            
    }

    // --- 

    private async Task<Browser> CreateWebBrowser(string ChromePath)
    {
        async Task<WebSocket> CreateWebSocketTask(Uri url, IConnectionOptions options, CancellationToken cancellationToken)
        {
            var result = new System.Net.WebSockets.Managed.ClientWebSocket();
            result.Options.KeepAliveInterval = TimeSpan.Zero;
            await result.ConnectAsync(url, cancellationToken).ConfigureAwait(false);
            return result;
        }

        var downloadChrome = string.IsNullOrEmpty(ChromePath);
        if (downloadChrome)
        {
            var browserFetcherOptions = new BrowserFetcherOptions
            {
                Path = $"{AppDomain.CurrentDomain.BaseDirectory}",
                Platform = Platform.Win64,
                Product = Product.Chrome
            };
            var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            var revisionInfo = await browserFetcher.DownloadAsync();
            ChromePath = revisionInfo.ExecutablePath;  // set the created chrome path
        }

        var launchOptions = new LaunchOptions
        {
            Headless = true,
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
        if (this.browser != null)
            this.browser.Dispose();
    }     
*/
