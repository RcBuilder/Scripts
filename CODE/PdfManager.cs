using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

// Install-Package HtmlRenderer.PdfSharp -Version 1.5.0.6
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

// Install-Package PuppeteerSharp -Version 5.0.0
using PuppeteerSharp;

// Install-Package System.Net.WebSockets.Client.Managed -Version 1.0.22
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Diagnostics;

namespace Helpers
{
    /*
        // TODO ->> More Samples
        USING
        -----
        var sDocument = @"
                    <style>
                        .c1 { color:red; }
                    </style>
                    <h1 class=""c1"">Header<h1>
                    <p>Content</p>
                    <p>תוכן בעברית</p>
                ";

        // From Html-String
        pdfManager.Save(pdfManager.CreateFromHtmlString(sDocument), "D:\\output.pdf");

        // Using Canvas           
        pdfManager.Save(pdfManager.Create(), "D:\\output.pdf");

        // From Web Image            
        pdfManager.Save(await pdfManager.CreateFromImageUrl("https://picsum.photos/id/1011/600/300"), "D:\\output.pdf");
        pdfManager.Save(await pdfManager.CreateFromImageUrl("https://picsum.photos/id/1011/600/300", true), "D:\\output.pdf");

        // From Web Html
        pdfManager.Save(await pdfManager.CreateFromWebHtml("http://example.com/"), "D:\\output.pdf");

        // take screenshot into pdf            
        pdfManager.Save(await pdfManager.CreateFromScreenshot("https://rcb.co.il/Service/WebsiteForYourBusiness"), "D:\\output.pdf");            
        pdfManager.Save(await pdfManager.CreateFromBrowser("https://rcb.co.il/Service/WebsiteForYourBusiness"), "D:\\output.pdf");        
    */

    public enum eImageMode : byte {
        None,
        Stretch,
        Scale
    }

    // TODO ->> Replace XGraphics to a generic object to support ANY provider
    public interface IDrawingElement {
        void Draw(ref XGraphics Gfx);
    }

    public class TextElement : IDrawingElement
    {
        public void Draw(ref XGraphics Gfx) {
            throw new NotImplementedException();
        }
    }

    public class ImageElement : IDrawingElement {
        public void Draw(ref XGraphics Gfx) {
            throw new NotImplementedException();
        }
    }

    public class PdfSettings {
        public string Title { get; set; }
    }

    public interface IPdfBrowserProvider
    {
        Task<Stream> CreateFromScreenshot(string URL, string ChromePath, eImageMode ImageMode);
        Task<Stream> CreateFromBrowser(string URL, string ChromePath = null);
        Task SaveFromScreenshot(string URL, string FilePath, string ChromePath);
        Task SaveFromBrowser(string URL, string FilePath, string ChromePath);
    }
    public interface IPdfProvider : IPdfBrowserProvider
    {
        Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings);
        Stream CreateFromHtmlString(string HtmlString);
        Task<Stream> CreateFromHtmlFile(string FilePath);
        Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode);
        Stream CreateFromImage(Image Image, eImageMode ImageMode);
        Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode);
        Stream CreateFromImageFile(string FilePath, eImageMode ImageMode);
        Task<Stream> CreateFromWebHtml(string URL);        
        void Save(Stream PdfStream, string FilePath, bool CloseStream);
        ///HttpResponseMessage CreateHttpResponse(string URL);
    }    
    public interface IPdfManager : IPdfProvider { }


    public class PdfSharpProviderBase {
        public void Save(Stream PdfStream, string FilePath, bool CloseStream = true)
        {
            PdfStream.Position = 0;
            using (var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                PdfStream.CopyTo(fs);
            if (CloseStream) PdfStream.Dispose();
        }

        // ---

        private async Task CopyStream(Stream Input, Stream Output)
        {
            /// Input.CopyTo(Output);            
            await Input.CopyToAsync(Output);

            /*
            var buffer = new byte[32768];
            int read;
            while ((read = await Input.ReadAsync(buffer, 0, buffer.Length)) > 0)            
                Output.Write(buffer, 0, read);
            */

            Output.Position = 0;
        }
    }

    public class PdfPuppeteerBrowserProvider : PdfSharpProviderBase, IPdfBrowserProvider
    {
        public virtual async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            var browser = await this.CreateWebBrowser(ChromePath);
            var webPage = await browser.NewPageAsync();
            await webPage.GoToAsync(URL);

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
            var browser = await this.CreateWebBrowser(ChromePath);
            var webPage = await browser.NewPageAsync();
            await webPage.GoToAsync(URL);

            /// return await webPage.PdfDataAsync(); // as bytes
            return await webPage.PdfStreamAsync();   // as stream           
        }

        public virtual async Task SaveFromScreenshot(string URL, string FilePath, string ChromePath = null)
        {
            var browser = await this.CreateWebBrowser(ChromePath);
            var webPage = await browser.NewPageAsync();
            await webPage.GoToAsync(URL);

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
            var browser = await this.CreateWebBrowser(ChromePath);
            var webPage = await browser.NewPageAsync();
            await webPage.GoToAsync(URL);

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
    }

    // TODO ->> Interface for other providers (??) - IText7, wkhtmltopdf etc.
    // TODO ->> No Support for Hebrew in PdfSharp! (??)
    // TODO ->> ALL - Title, Alignment And Position
    // TODO ->> Encoding, External/Internal CSS
    // TODO ->> Image Size & Alignment                
    // TODO ->> IDrawingElement (text, image etc.)
    public class PdfSharpProvider : PdfPuppeteerBrowserProvider, IPdfProvider
    {        
        public Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings = null)
        {
            var pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = Settings?.Title ?? "";

            var page = pdfDoc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            foreach (var e in Elements)
                e.Draw(ref gfx);

            /*
            // var fontOptions = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var font = new XFont("Arial", 20, XFontStyle.Regular);
            gfx1.DrawString("Hello World", font, XBrushes.Black, new XRect(0, 0, page1.Width, page1.Height), XStringFormats.Center);

            var page2 = pdfDoc.AddPage();
            var gfx2 = XGraphics.FromPdfPage(page2);
            var image = XImage.FromFile("D:\\237-200x200.jpg");
            gfx2.DrawImage(image, new XRect(0, 0, page2.Width, page2.Height)); // full page

            var page3 = pdfDoc.AddPage();
            var gfx3 = XGraphics.FromPdfPage(page3);
            gfx3.DrawImage(image, new XRect(0, 0, image.PixelWidth, image.PixelHeight)); // image size
            */

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        public Stream CreateFromHtmlString(string HtmlString)
        {
            var config = new PdfGenerateConfig
            {
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait
            };
            config.SetMargins(16);
            var pdfDoc = PdfGenerator.GeneratePdf(HtmlString, config);

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        public async Task<Stream> CreateFromHtmlFile(string FilePath)
        {
            var HtmlString = string.Empty;

            /// HtmlString = File.ReadAllText(FilePath, Encoding.UTF8);
            using (var fs = File.OpenText(FilePath))
                HtmlString = await fs.ReadToEndAsync();

            return this.CreateFromHtmlString(HtmlString);
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode = eImageMode.None)
        {
            var pdfDoc = new PdfDocument();

            using (var image = Bitmap)
                this.DrawImage(pdfDoc, ImageMode, image);

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        public Stream CreateFromImage(Image Image, eImageMode ImageMode = eImageMode.None)
        {
            return this.CreateFromBitmap(new Bitmap(Image), ImageMode);
        }

        public async Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode = eImageMode.None)
        {
            var pdfDoc = new PdfDocument();

            /* read as Bytes
            byte[] data;
            using (var client = new WebClient())
                data = await client.DownloadDataTaskAsync(new Uri(URL));
            using (var ms = new MemoryStream(data))
            using (var image = Image.FromStream(ms))
                this.DrawImage(pdfDoc, ImageMode, image);
            */

            // read as Stream
            using (var client = new WebClient())
            using (var imageStream = await client.OpenReadTaskAsync(new Uri(URL)))
            using (var image = Image.FromStream(imageStream))
                this.DrawImage(pdfDoc, ImageMode, image);

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        public Stream CreateFromImageFile(string FilePath, eImageMode ImageMode = eImageMode.None)
        {
            return this.CreateFromImage(Image.FromFile(FilePath), ImageMode);
        }

        public async Task<Stream> CreateFromWebHtml(string URL)
        {
            var HtmlString = string.Empty;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                HtmlString = await client.DownloadStringTaskAsync(URL);
            }

            return this.CreateFromHtmlString(HtmlString);
        }

        public override async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            var pdfDoc = new PdfDocument();
            using (var screenshotStream = await base.CreateFromScreenshot(URL, ChromePath, ImageMode))
            using (var image = Image.FromStream(screenshotStream))            
                this.DrawImage(pdfDoc, ImageMode, image);
        
            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        // --- 

        private void DrawImage(PdfDocument pdfDoc, eImageMode ImageMode, Image image)
        {
            var page = pdfDoc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var pdfImage = XImage.FromGdiPlusImage(image);


            switch (ImageMode)
            {
                case eImageMode.Scale:
                    if (pdfImage.PixelWidth <= page.Width && pdfImage.PixelHeight <= page.Height) // no need for scaling - image is smaller than page
                        gfx.DrawImage(pdfImage, new XRect(0, 0, pdfImage.PixelWidth, pdfImage.PixelHeight));
                    else if (pdfImage.PixelWidth > page.Width) // scale by width
                        gfx.DrawImage(pdfImage, new XRect(0, 0, page.Width, (page.Width / pdfImage.PixelWidth) * pdfImage.PixelHeight));
                    else // scale by height                    
                        gfx.DrawImage(pdfImage, new XRect(0, 0, (page.Height / pdfImage.PixelHeight) * pdfImage.PixelWidth, page.Height));
                    break;
                case eImageMode.Stretch:
                    gfx.DrawImage(pdfImage, new XRect(0, 0, page.Width, page.Height), new XRect(0, 0, pdfImage.PixelWidth, pdfImage.PixelHeight), XGraphicsUnit.Point);
                    break;
                default:
                case eImageMode.None:
                    gfx.DrawImage(pdfImage, new XRect(0, 0, pdfImage.PixelWidth, pdfImage.PixelHeight));
                    break;
            }
        }
    }
    
    /*
        about:
        a CLI Tool to generate PDF files and Images from HTML content or URI        
        see E:\Scripts\Utilities\wk-Html-2-Pdf
    */
    public class WkHtml2PdfProvider : PdfPuppeteerBrowserProvider, IPdfProvider
    {
        private readonly string UtilityPath = $"{AppDomain.CurrentDomain.BaseDirectory}wkhtmltopdf.exe";
        private const string GlobalOptions = "--enable-local-file-access --quiet --encoding windows-1255";

        public Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromHtmlString(string HtmlString)
        {
            return ProcessManager.InteractAsStream(this.UtilityPath, $"{GlobalOptions} - -", HtmlString);
        }

        public async Task<Stream> CreateFromHtmlFile(string FilePath)
        {
            var HtmlString = string.Empty;

            /// HtmlString = File.ReadAllText(FilePath, Encoding.UTF8);
            using (var fs = File.OpenText(FilePath))
                HtmlString = await fs.ReadToEndAsync();

            return this.CreateFromHtmlString(HtmlString);
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode)
        {
            // create temporary file
            var temp = $"{AppDomain.CurrentDomain.BaseDirectory}temp_{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            Bitmap.Save(temp);

            var stream = this.CreateFromImageFile(temp, ImageMode);

            File.Delete(temp);
            return stream;
        }

        public Stream CreateFromImage(Image Image, eImageMode ImageMode)
        {
            return this.CreateFromBitmap(new Bitmap(Image), ImageMode);
        }

        public async Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode)
        {            
            return this.CreateFromImageFile(URL, ImageMode);
        }

        public Stream CreateFromImageFile(string FilePath, eImageMode ImageMode)
        {
            var HtmlString = $@"
		        <style>
			        img {{ width: 100%; }}
                </style>
		        <img src='{FilePath}' />
	        ";

            return this.CreateFromHtmlString(HtmlString);
        }

        public async Task<Stream> CreateFromWebHtml(string URL)
        {
            return await this.CreateFromScreenshot(URL);
        }
        
        public override async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {                                    
            return ProcessManager.InteractAsStream(this.UtilityPath, $"{GlobalOptions} {URL} -", "");
        }

        public override async Task<Stream> CreateFromBrowser(string URL, string ChromePath = null)
        {
            return await this.CreateFromScreenshot(URL, ChromePath);
        }

        public override async Task SaveFromScreenshot(string URL, string FilePath, string ChromePath = null)
        {
            ProcessManager.Execute(this.UtilityPath, $"{GlobalOptions} {URL} {FilePath}");
        }

        public override async Task SaveFromBrowser(string URL, string FilePath, string ChromePath = null)
        {
            await this.SaveFromScreenshot(URL, FilePath, ChromePath);
        }

        // ---

        /*            
	        USING
	        -----
	        // TestApp1.exe
	        static void Main(string[] args) {
                Console.WriteLine($"Hello {args[0]} From Test Utility");        
            }
	
	        // TestApp2.exe
	        static void Main(string[] args) {        
                var input = Console.ReadLine();
                Console.WriteLine($"Your Input is '{input}'");
            }
	
	        -
		
	        ProcessManager.Execute($"D:\\TestApp1.exe", "Roby");  // No Output!!
	
	        var processResult1 = ProcessManager.ExecuteAsString($"D:\\TestApp1.exe", "Roby");  
	        Console.WriteLine(processResult1);  // Hello Roby From Test Utility
	
	        var processResult2 = ProcessManager.ExecuteAsStream($"D:\\TestApp1.exe", "Roby");  
	        Console.WriteLine(processResult2);  // Hello Roby From Test Utility
	
	        var processResult3 = ProcessManager.InteractAsString($"D:\\TestApp2.exe", "", "Its Working!");
	        Console.WriteLine(processResult3);  // Your Input is 'Its Working!'	
        */
        class ProcessManager
        {
            public static void Execute(string Command, string Args = null)
            {
                using (var p = new Process())
                {
                    SetCommonProperties(p);
                    p.StartInfo.FileName = Command;
                    p.StartInfo.Arguments = Args ?? string.Empty;
                    p.Start();
                    p.WaitForExit();
                }
            }

            public static string ExecuteAsString(string Command, string Args = null)
            {
                using (var p = new Process())
                {
                    SetCommonProperties(p);
                    p.StartInfo.FileName = Command;
                    p.StartInfo.Arguments = Args ?? string.Empty;
                    p.Start();
                    p.WaitForExit();

                    return p.StandardOutput.ReadToEnd();
                }
            }

            public static Stream ExecuteAsStream(string Command, string Args = null)
            {
                using (var p = new Process())
                {
                    SetCommonProperties(p);
                    p.StartInfo.FileName = Command;
                    p.StartInfo.Arguments = Args ?? string.Empty;
                    p.Start();
                    p.WaitForExit();

                    return p.StandardOutput.BaseStream;
                }
            }

            public static string InteractAsString(string Command, string Args, string Input)
            {
                using (var p = new Process())
                {
                    SetCommonProperties(p);
                    p.StartInfo.FileName = Command;
                    p.StartInfo.Arguments = Args ?? string.Empty;
                    p.Start();

                    try
                    {
                        using (var stdin = p.StandardInput)
                        {
                            stdin.AutoFlush = true;
                            stdin.Write(Input);
                        };

                        var output = p.StandardOutput.ReadToEnd();
                        p.StandardOutput.Close();

                        p.WaitForExit();
                        return output;
                    }
                    catch { return null; }
                }
            }

            public static Stream InteractAsStream(string Command, string Args, string Input)
            {
                using (var p = new Process())
                {
                    SetCommonProperties(p);
                    p.StartInfo.FileName = Command;
                    p.StartInfo.Arguments = Args ?? string.Empty;
                    p.Start();

                    try
                    {
                        using (var stdin = p.StandardInput)
                        {
                            stdin.AutoFlush = true;
                            stdin.Write(Input);
                        };

                        var ms = new MemoryStream();
                        p.StandardOutput.BaseStream.CopyTo(ms);
                        p.StandardOutput.Close();

                        p.WaitForExit();

                        ms.Position = 0;
                        return ms;
                    }
                    catch { return null; }
                }
            }

            // --- 

            private static void SetCommonProperties(Process p)
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
            }
        }
    }

    // TODO ->> QA (all providers!)
    public class PdfManager : IPdfManager
    {
        private IPdfProvider PdfProvider { get; set; } = new PdfSharpProvider();        
        
        public Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings = null)
        {
            return this.PdfProvider.Create(Elements, Settings);
        }

        public Stream CreateFromHtmlString(string HtmlString)
        {
            return this.PdfProvider.CreateFromHtmlString(HtmlString);
        }

        public async Task<Stream> CreateFromHtmlFile(string FilePath)
        {
            return await this.PdfProvider.CreateFromHtmlFile(FilePath);
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode = eImageMode.None)
        {
            return this.PdfProvider.CreateFromBitmap(Bitmap, ImageMode);
        }

        public Stream CreateFromImage(Image Image, eImageMode ImageMode = eImageMode.None)
        {
            return this.PdfProvider.CreateFromImage(Image, ImageMode);
        }

        public async Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode = eImageMode.None)
        {
            return await this.PdfProvider.CreateFromImageUrl(URL, ImageMode);
        }

        public Stream CreateFromImageFile(string FilePath, eImageMode ImageMode = eImageMode.None)
        {
            return this.PdfProvider.CreateFromImageFile(FilePath, ImageMode);
        }

        public async Task<Stream> CreateFromWebHtml(string URL)
        {
            return await this.PdfProvider.CreateFromWebHtml(URL);
        }

        public async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            return await this.PdfProvider.CreateFromScreenshot(URL, ChromePath, ImageMode);
        }

        public async Task<Stream> CreateFromBrowser(string URL, string ChromePath = null)
        {
            return await this.PdfProvider.CreateFromBrowser(URL, ChromePath);
        }

        public async Task SaveFromScreenshot(string URL, string FilePath, string ChromePath = null)
        {
            await this.PdfProvider.SaveFromScreenshot(URL, FilePath, ChromePath);
        }

        public async Task SaveFromBrowser(string URL, string FilePath, string ChromePath = null)
        {
            await this.PdfProvider.SaveFromBrowser(URL, FilePath, ChromePath);
        }

        public void Save(Stream PdfStream, string FilePath, bool CloseStream = true)
        {
            this.PdfProvider.Save(PdfStream, FilePath, CloseStream);
        }
    }
}