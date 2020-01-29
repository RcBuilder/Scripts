using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Core.PdfReaderService
{
    public class PDFRenderListener : IRenderListener
    {
        private StringBuilder _Content { get; set; } = new StringBuilder();
        private List<Image> _Images { get; set; } = new List<Image>();

        private const int MIN_IMAGE_SIZE = 150;

        public List<Image> Images {
            get {
                _Images.Sort(new ImageSizeComparer());
                return _Images;
            }
        }

        [Obsolete("Use PdfTextExtractor Instead")]
        public string Content {
            get {
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

        public void EndTextBlock() {
            ///_Content.Append(Environment.NewLine);
        }

        public void RenderText(TextRenderInfo renderInfo) {
            ///_Content.Append(renderInfo.GetText());
        }
    }
}
