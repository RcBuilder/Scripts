using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace TestConsole9
{    
    public class PdfSharpTests
    {
        public static void Test1()
        {
            // https://www.nuget.org/packages/HtmlRenderer.PdfSharp/
            // Install-Package HtmlRenderer.PdfSharp -Version 1.5.0.6

            var sDocument = @"
                <h1>Header<h1>
                <p>Content</p>
            ";

            // save to file
            var pdfDoc1 = PdfGenerator.GeneratePdf(sDocument, PageSize.A4);
            pdfDoc1.Save("D:\\1.pdf");
            Console.WriteLine("converted HTML has saved as file (D:\\1.pdf)");

            // save to stream
            var pdfDoc2 = PdfGenerator.GeneratePdf(sDocument, PageSize.A4);
            var stream = new MemoryStream();
            pdfDoc2.Save(stream);
            Console.WriteLine("converted HTML has saved to stream");
        }
    }
}
