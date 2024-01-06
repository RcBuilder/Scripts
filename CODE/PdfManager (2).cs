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
using nsPdfSharp = PdfSharp.Pdf;

// Install-Package PuppeteerSharp -Version 5.0.0
using PuppeteerSharp;

// Install-Package iTextSharp -Version 5.5.13.1 [Obsolete]
///using iTextSharp.text.pdf;
///using iTextSharp.text.pdf;
///using iTextSharp.text.pdf.parser;
///using nsTextSharp5Pdf = iTextSharp.text.pdf;

// Install-Package itext7 -Version 7.2.4
// Install-Package itext7.pdfhtml -Version 4.0.4 
// https://itextpdf.com/
// https://itextpdf.com/resources/api-documentation/itext-7-net
// https://api.itextpdf.com/iText7/dotnet/7.2.4/
// https://github.com/itext/itext7-dotnet
// https://kb.itextpdf.com/home/it7kb
// https://kb.itextpdf.com/home/it7kb/ebooks/itext-7-converting-html-to-pdf-with-pdfhtml
using iText.Layout;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.StyledXmlParser.Css.Util;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Media;
using nsTextSharp7IO = iText.IO;
using nsTextSharp7Pdf = iText.Kernel.Pdf;
using nsTextSharp7Font = iText.Kernel.Font;
using nsTextSharp7FontConstants = iText.IO.Font.Constants.StandardFonts;
using nsTextSharp7Geom = iText.Kernel.Geom;
using nsTextSharp7Element = iText.Layout.Element;

// Install-Package System.Net.WebSockets.Client.Managed -Version 1.0.22
using System.Net.WebSockets;

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

// ---------------------
// TODO ->> TO COMPLETE! 
// --------------------- 

namespace TestConsole7
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
        string CopyFromHtmlFile(string HtmlPath, string PdfPath);
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

    public class PdfPuppeteerBrowserProvider : PdfSharpProviderBase, IPdfBrowserProvider, IDisposable
    {
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
            var pdfDoc = new nsPdfSharp.PdfDocument();
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
                PageSize = PdfSharp.PageSize.A4,
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

        public string CopyFromHtmlFile(string HtmlPath, string PdfPath) {            
            throw new NotImplementedException();
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode = eImageMode.None)
        {
            var pdfDoc = new nsPdfSharp.PdfDocument();

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
            var pdfDoc = new nsPdfSharp.PdfDocument();

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
                client.Encoding = Encoding.GetEncoding("Windows-1255");
                HtmlString = await client.DownloadStringTaskAsync(URL);
            }

            return this.CreateFromHtmlString(HtmlString);
        }

        public override async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            var pdfDoc = new nsPdfSharp.PdfDocument();
            using (var screenshotStream = await base.CreateFromScreenshot(URL, ChromePath, ImageMode))
            using (var image = Image.FromStream(screenshotStream))
                this.DrawImage(pdfDoc, ImageMode, image);

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            pdfDoc.Dispose();

            return stream;
        }

        // --- 

        private void DrawImage(nsPdfSharp.PdfDocument pdfDoc, eImageMode ImageMode, Image image)
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


    // TODO ->> Implement - throw new NotImplementedException()
    // PdfITextSharp5Provider - iTextSharp ver 5.x.x
    /*
    public class PdfITextSharp5Provider : PdfPuppeteerBrowserProvider, IPdfProvider
    {
        public Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings = null)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromHtmlString(string HtmlString)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> CreateFromHtmlFile(string FilePath)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromImage(Image Image, eImageMode ImageMode = eImageMode.None)
        {
            return this.CreateFromBitmap(new Bitmap(Image), ImageMode);
        }

        public async Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
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
                client.Encoding = Encoding.GetEncoding("Windows-1255");
                HtmlString = await client.DownloadStringTaskAsync(URL);
            }

            return this.CreateFromHtmlString(HtmlString);
        }

        public override async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
        }

        // --- 

        private void DrawImage(nsTextSharp7.PdfDocument pdfDoc, eImageMode ImageMode, Image image)
        {
            throw new NotImplementedException();
        }
    }
    */

    // TODO ->> Implement - throw new NotImplementedException()
    /*
        // TODO ->> To Document - Samples 

        // basic table
        var pdfDoc = new nsTextSharp7Pdf.PdfDocument(new nsTextSharp7Pdf.PdfWriter(@"D:\TEMP\PdfManager\table.pdf"));
        var doc = new Document(pdfDoc);

        var table = new nsTextSharp7Element.Table(UnitValue.CreatePercentArray(8)).UseAllAvailableWidth();

        for (int i = 0; i < 16; i++)            
            table.AddCell((i + 1).ToString());            

        doc.Add(table);
        doc.Close();
        -

    */
    public class PdfITextSharp7Provider : PdfPuppeteerBrowserProvider, IPdfProvider
    {        
        public Stream Create(IEnumerable<IDrawingElement> Elements, PdfSettings Settings = null)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromHtmlString(string HtmlString)
        {

            var pdfDoc = new nsTextSharp7Pdf.PdfDocument(new nsTextSharp7Pdf.PdfWriter(@"D:\TEMP\PdfManager\html1.pdf"));
            var doc = new Document(pdfDoc);

            var fontTimesBold = nsTextSharp7Font.PdfFontFactory.CreateFont(nsTextSharp7FontConstants.TIMES_BOLD);
            var fontTimesRoman = nsTextSharp7Font.PdfFontFactory.CreateFont(nsTextSharp7FontConstants.TIMES_ROMAN);
            var fontPermanentMarker = nsTextSharp7Font.PdfFontFactory.CreateFont(nsTextSharp7IO.Font.FontProgramFactory.CreateFont(@"E:\TestProjects\TestHTML\fonts\PermanentMarker-Regular.ttf"));
            
            var p1 = new nsTextSharp7Element.Paragraph("Hello World");
            p1.SetFont(fontTimesBold);

            var p2 = new nsTextSharp7Element.Paragraph("Hello World");
            p2.SetFont(fontPermanentMarker);

            var p3 = new nsTextSharp7Element.Paragraph("Hello World");
            p3.SetFont(fontTimesRoman);

            var p4 = new nsTextSharp7Element.Paragraph("Hello World");
            p4.SetFont(fontTimesRoman).SetFontSize(20);

            var p5 = new nsTextSharp7Element.Paragraph("Hello World");
            p5.SetFontFamily("arial");

            doc.Add(p1);
            doc.Add(p2);
            doc.Add(p3);
            doc.Add(p4);
            doc.Add(p5);
            doc.Add(new nsTextSharp7Element.Paragraph("Hello World"));

            doc.Close();







            /*
            var writer = new nsTextSharp7Pdf.PdfWriter(@"D:\TEMP\PdfManager\html.pdf");
            var pdfDoc = new nsTextSharp7Pdf.PdfDocument(writer);

            var pageSize = nsTextSharp7Geom.PageSize.A4.Rotate();

            // Set the result to be tagged
            pdfDoc.SetTagged();
            pdfDoc.SetDefaultPageSize(pageSize);

            ConverterProperties converterProperties = new ConverterProperties();

            // Set media device description details
            var mediaDescription = new MediaDeviceDescription(MediaType.SCREEN);
            mediaDescription.SetWidth(CssDimensionParsingUtils.ParseAbsoluteLength(pageSize.GetWidth().ToString(CultureInfo.InvariantCulture)));
            converterProperties.SetMediaDeviceDescription(mediaDescription);

            var arialFont = new nsTextSharp7Font.FontProvider("Arial");       
            ///var baseFont = BaseFont.CreateFont("Arial", "UTF-8", false);
            ///var arialFont = new iTextSharp.text.Font(baseFont, 16, 1);       
            converterProperties.SetFontProvider(arialFont);

            // Base URI is required to resolve the path to source files
            converterProperties.SetBaseUri("");

            // Create acroforms from text and button input fields
            converterProperties.SetCreateAcroForm(true);

            HtmlConverter.ConvertToPdf(HtmlString, pdfDoc, converterProperties);

            pdfDoc.Close();
            */

            throw new NotImplementedException();
        }

        public async Task<Stream> CreateFromHtmlFile(string FilePath)
        {
            throw new NotImplementedException();
        }

        public string CopyFromHtmlFile(string HtmlPath, string PdfPath)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromBitmap(Bitmap Bitmap, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
        }

        public Stream CreateFromImage(Image Image, eImageMode ImageMode = eImageMode.None)
        {
            return this.CreateFromBitmap(new Bitmap(Image), ImageMode);
        }

        public async Task<Stream> CreateFromImageUrl(string URL, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
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
                client.Encoding = Encoding.GetEncoding("Windows-1255");
                HtmlString = await client.DownloadStringTaskAsync(URL);
            }

            return this.CreateFromHtmlString(HtmlString);
        }

        public override async Task<Stream> CreateFromScreenshot(string URL, string ChromePath = null, eImageMode ImageMode = eImageMode.None)
        {
            throw new NotImplementedException();
        }

        // --- 

        private void DrawImage(nsTextSharp7Pdf.PdfDocument pdfDoc, eImageMode ImageMode, Image image)
        {
            throw new NotImplementedException();
        }
    }

    /*
        about:
        https://wkhtmltopdf.org/index.html
        https://wkhtmltopdf.org/usage/wkhtmltopdf.txt

        a CLI Tool to generate PDF files and Images from HTML content or URI        
        see E:\Scripts\Utilities\wk-Html-2-Pdf        
    */
    public class WkHtml2PdfProvider : PdfPuppeteerBrowserProvider, IPdfProvider
    {
        private readonly string UtilityPath = $"{AppDomain.CurrentDomain.BaseDirectory}wkhtmltopdf.exe";
        private const string GlobalOptions = "--enable-local-file-access --quiet --encoding windows-1255"; // --encoding utf-8

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

        public string CopyFromHtmlFile(string HtmlPath, string PdfPath)
        {
            return ProcessManager.InteractAsString(this.UtilityPath, $"{GlobalOptions} \"{HtmlPath}\" \"{PdfPath}\"", "");
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
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            }
        }
    }

    // TODO ->> QA (all providers!)
    public class PdfManager : IPdfManager, IDisposable
    {
        private IPdfProvider PdfProvider { get; set; }

        public PdfManager() : this(new PdfSharpProvider()) { }
        public PdfManager(IPdfProvider PdfProvider) {
            this.PdfProvider = PdfProvider;
        }

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

        public string CopyFromHtmlFile(string HtmlPath, string PdfPath)
        {
            throw new NotImplementedException();
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

        public void Dispose()
        {            
            if (this.PdfProvider is IDisposable)
                ((IDisposable)this.PdfProvider).Dispose();
        }
    }
}