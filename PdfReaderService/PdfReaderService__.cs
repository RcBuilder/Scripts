using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
    iTextSharp
    ----------
    Nuget:
    Install-Package iTextSharp -Version 5.5.13.1

    Namespaces:
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;

    Sources:
    https://www.nuget.org/packages/iTextSharp/
*/

namespace TestConsole7
{
    public class PdfReaderService
    {
        public class ImageSizeComparer : IComparer<Image>
        {
            public int Compare(Image a, Image b)
            {
                var sizeA = (a.Height * a.Width);
                var sizeB = (b.Height * b.Width);

                if (sizeA == sizeB) return 0;
                return sizeA > sizeB ? -1 : 1;
            }
        }

        public class PDFRenderListener : IRenderListener
        {
            private StringBuilder _Content { get; set; } = new StringBuilder();
            private List<Image> _Images { get; set; } = new List<Image>();

            private const int MIN_IMAGE_SIZE = 20;

            public List<Image> Images
            {
                get
                {
                    _Images.Sort(new ImageSizeComparer());
                    return _Images;
                }
            }

            [Obsolete("Use PdfTextExtractor Instead")]
            public string Content
            {
                get
                {
                    return _Content.ToString();
                }
            }

            public void RenderImage(ImageRenderInfo renderInfo)
            {
                var imageInfo = renderInfo.GetImage();
                var image = imageInfo.GetDrawingImage();

                if (image.Size.IsEmpty || image.Height < MIN_IMAGE_SIZE || image.Width < MIN_IMAGE_SIZE)
                    return;

                _Images.Add(image);
            }

            public void BeginTextBlock() { }

            public void EndTextBlock()
            {
                ///_Content.Append(Environment.NewLine);
            }

            public void RenderText(TextRenderInfo renderInfo)
            {
                ///_Content.Append(renderInfo.GetText());
            }
        }

        public class PDFContentParser {
            public (string Title, string Body) Parse(PdfReader reader, int pageNumber) {
                var body = PdfTextExtractor.GetTextFromPage(reader, pageNumber, new LocationTextExtractionStrategy());
                
                string title = null;
                if (reader.Info != null && reader.Info.ContainsKey("Title"))
                    title = reader.Info["Title"];
                if (string.IsNullOrWhiteSpace(title))
                    title = new string(body.Take(60).ToArray());

                return (title, body);
            } 
        }

        public class PDFImagesParser
        {
            public List<Image> Parse(PdfReader reader) {
                var images = new List<Image>();

                for (var i = 1; i <= reader.NumberOfPages; i++)
                    images.AddRange(this.Parse(reader, i));

                images.Sort(new ImageSizeComparer());
                return images;
            }

            public List<Image> Parse(PdfReader reader, int pageNumber) {
                var listener = new PDFRenderListener();

                var parser = new PdfReaderContentParser(reader);
                parser.ProcessContent(pageNumber, listener);
                /// Console.WriteLine(listener.Images.Count);

                return listener.Images;
            }
        }

        public static void Run() {
            var numOfImagesToSave = 10;

            using (var reader = new PdfReader("http://www.seebo.com/wp-content/uploads/2016/12/Guide-to-Successful-IoT-Product-Strategy.pdf"))
            {
                Console.WriteLine($"{reader.NumberOfPages} Pages Found.");

                // parse text from pdf 
                var content = new PDFContentParser().Parse(reader, 1);
                Console.WriteLine(content.Title);
                Console.WriteLine(content.Body);

                // parse images from pdf
                var images = new PDFImagesParser().Parse(reader);
                foreach (var image in images.Take(numOfImagesToSave))
                    image.Save($@"C:\Users\Roby\Downloads\AAA\{Guid.NewGuid()}.jpg");
            }
        }
    }
}
