using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Core.PdfReaderService
{
    public class PDFContentParser
    {
        public (string Title, string Body) Parse(PdfReader reader, int pageNumber)
        {
            var body = PdfTextExtractor.GetTextFromPage(reader, pageNumber, new LocationTextExtractionStrategy());

            string title = null;
            if (reader.Info != null && reader.Info.ContainsKey("Title"))
                title = reader.Info["Title"];
            if (string.IsNullOrWhiteSpace(title))
                title = new string(body.Take(60).ToArray());

            return (title, body);
        }
    }
}
