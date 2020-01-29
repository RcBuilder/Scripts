using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Core.PdfReaderService
{
    public class PDFImagesParser
    {
        public List<Image> Parse(PdfReader reader)
        {
            var images = new List<Image>();

            for (var i = 1; i <= reader.NumberOfPages; i++)
                images.AddRange(this.Parse(reader, i));

            images.Sort(new ImageSizeComparer());
            return images;
        }

        public List<Image> Parse(PdfReader reader, int pageNumber)
        {
            var listener = new PDFRenderListener();

            var parser = new PdfReaderContentParser(reader);
            parser.ProcessContent(pageNumber, listener);
            ///Console.WriteLine(listener.Images.Count);

            return listener.Images;
        }
    }
}
