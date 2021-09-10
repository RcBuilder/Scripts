using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace RestAPITests.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("api/export/html")]
        public HttpResponseMessage ExportAsHTML()
        {
            var sDocument = @"
                <h1>Header<h1>
                <p>Content</p>
            ";

            var response = new HttpResponseMessage();
            response.Content = new StringContent(sDocument);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "export.html";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        [HttpGet]
        [Route("api/export/pdf")]
        public HttpResponseMessage ExportAsPDF()
        {
            // https://www.nuget.org/packages/HtmlRenderer.PdfSharp/
            // Install-Package HtmlRenderer.PdfSharp -Version 1.5.0.6

            var sDocument = @"
                <h1>Header<h1>
                <p>Content</p>
            ";

            var pdfDoc = PdfGenerator.GeneratePdf(sDocument, PageSize.A4);
            var stream = new MemoryStream();
            pdfDoc.Save(stream);
            ///pdfDoc.Save("D:\\1.pdf");

            var response = new HttpResponseMessage();
            response.Content = new ByteArrayContent(stream.ToArray());
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "export.pdf";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;
        }

        [HttpGet]
        [Route("api/render")]
        public HttpResponseMessage RenderHTML()
        {
            var sDocument = @"
                <h1>Header<h1>
                <p>Content</p>
            ";

            var response = new HttpResponseMessage();
            response.Content = new StringContent(sDocument);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route("api/render/razor")]
        public HttpResponseMessage RenderHTMLByRazor()
        {
            var model = new SampleModel
            {
                FirstName = "John",
                LastName = "Doe"                
            };
           
            var sDocument = RazorViewRenderer.RenderView("~/views/Sample.cshtml", model);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(sDocument);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}