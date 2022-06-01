using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
    DEPENDENCY
    ----------
    SignServer by PrimeKey (see 'SignServer by PrimeKey')
  
    USING
    -----
    <appSettings>
        <add key="WorkerName" value="PDFSigner" />
        <add key="SrcFolder" value="D:\TEMP" />
        <add key="DestFolder" value="D:\TEMP\Signed" />
        <add key="Filter" value="*.pdf" />
        <add key="DeleteSrcFiles" value="0" />
        <add key="ServerURI" value="https://localhost/signserver/process" />    
    </appSettings> 
    -
    static async Task Main(string[] args) 
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => { return true; };

        var signer = new BLL.DocumentSignerManager(Config.ServerURI);
        var files = Directory.GetFiles(Config.SrcFolder, Config.Filter);
        Console.WriteLine($"{files.Length} files found.");

        foreach (var file in files) {
            try
            {
                Console.WriteLine(file);

                var extension = Path.GetExtension(file);
                var name = Path.GetFileNameWithoutExtension(file);

                var signed = await signer.Sign(file, Config.WorkerName);
                File.WriteAllBytes($"{Config.DestFolder}\\{name}_signed{extension}", signed);

                if (Config.DeleteSrcFiles) File.Delete(file);
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] {file} | {ex.Message}");
            }
        }

        Console.ReadKey();
    }
*/

namespace BLL
{
    public interface IDocumentSignerManager {
        Task<byte[]> Sign(string FilePath, string WorkerName);
    }

    public class DocumentSignerManager : IDocumentSignerManager
    {
        private class FileUpload
        {            
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }
        
        private string ServerURI { get; set; }

        public DocumentSignerManager(string ServerURI) {
            this.ServerURI = ServerURI;
        }

        public async Task<byte[]> Sign(string FilePath, string WorkerName) {

            using (var stream = File.Open(FilePath, FileMode.Open))
            {
                var file = new FileUpload
                {
                    Name = "file",
                    Filename = Path.GetFileName(FilePath),
                    ContentType = "text/plain",
                    Stream = stream
                };

                var formData = new NameValueCollection
                {
                    { "processType", "signDocument" },
                    { "workerName", WorkerName },
                };

                return await this.UploadFiles(this.ServerURI, file, formData);                
            }            
        }

        // --------------

        private async Task<byte[]> UploadFiles(string URL, FileUpload File, NameValueCollection FormData) {
            return await this.UploadFiles(URL, new List<FileUpload>{ File }, FormData);
        }

        private async Task<byte[]> UploadFiles(string URL, IEnumerable<FileUpload> Files, NameValueCollection FormData)
        {            
            var request = WebRequest.Create(URL);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in FormData.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(FormData[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in Files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            using (var response = await request.GetResponseAsync())
            using (var responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                return stream.ToArray();
            }
        }
    }   
}
