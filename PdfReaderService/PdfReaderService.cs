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
    public class PDFExtractor
    {
        public (string Title, string Body, List<Image> Images) Extract(string url)
        {            
            using (var reader = new PdfReader(url))
            {
                /// Console.WriteLine($"{reader.NumberOfPages} Pages Found.");

                // parse text from pdf 
                var content = new PDFContentParser().Parse(reader, 1);
                Console.WriteLine(content.Title);
                Console.WriteLine(content.Body);

                // parse images from pdf
                var images = new PDFImagesParser().Parse(reader);

                return (content.Title, content.Body, images);
            }            
        }
    }
}
