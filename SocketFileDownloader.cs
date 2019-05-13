using Common;
using Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Common
{
    public class SocketFileDownloader : IFileDownloader, IFileDownloaderAsync, IProxy
    {
        private string REQUEST_HEADERS(Uri url) {
            return
            string.Format("GET {0} HTTP/1.1\r\n", url.ToString()) +
            string.Format("Host: {0}\r\n", url.Host) +
            "Connection: keep-alive\r\n" +
            "Accept: */*\r\n" +
            "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36\r\n" +
            "Accept-Encoding: gzip, deflate, sdch\r\n" +
            "Accept-Language: en-US,en;q=0.9\r\n" +
            "Cache-Control: no-cache\r\n" +
            "\r\n"; // important! empty row - spacer
        }

        public event EventHandler<FileDownloadEventArgs> OnDownloadToDiskAsyncCompleted;

        public string ProxyURL { get; set; }

        public SocketFileDownloader() { }
        public SocketFileDownloader(string ProxyURL) {
            if (!ProxyURL.StartsWith("http"))
                ProxyURL = "http://" + ProxyURL;

            this.ProxyURL = ProxyURL;            
        }

        private string ReadStream(Socket client, string FileURL) {
            var url = new Uri(FileURL);
            var connectionUrl = url;            

            // override connection details - use the proxy
            if (!string.IsNullOrEmpty(ProxyURL))
                connectionUrl = new Uri(ProxyURL);                          

            client.Connect(connectionUrl.Host, connectionUrl.Port);
            client.ReceiveTimeout = 10000;
            
            byte[] writeBytes = Encoding.Default.GetBytes(REQUEST_HEADERS(url));
            client.Send(writeBytes);

            var bytesRead = new List<byte>(10000000);
            var buffer = new byte[Int16.MaxValue];
            var bytesReceived = 0;
            
            do
            {
                bytesReceived = client.Receive(buffer, buffer.Length, SocketFlags.None);
                bytesRead.AddRange(buffer.Take(bytesReceived));
                Thread.Sleep(500); // wait for stream - stability
            }
            while (client.Available > 0);

            /*
            bytesReceived = client.Receive(buffer, buffer.Length, SocketFlags.None);            
            while (bytesReceived > 0)
            {                
                bytesReceived = client.Receive(buffer, buffer.Length, SocketFlags.None);
                bytesRead.AddRange(buffer.Take(bytesReceived));
                Thread.Sleep(50); // wait for stream - stability
            }            
            */

            if (bytesRead.Count == 0) {
                Console.WriteLine("NO DATA FOR '{0}'", url);
                return null;
            }

            var response = Encoding.Default.GetString(bytesRead.ToArray(), 0, bytesRead.Count - 1);
            return response;
        }

        public byte[] Download(string FileURL) {
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                return Download(client, FileURL);
        }

        public byte[] Download(Socket client, string FileURL) {
            var response = ReadStream(client, FileURL);

            var dataIndex = response.IndexOf("\r\n\r\n") + 4;
            var data = response.Substring(dataIndex);
            var headers = response.Substring(0, dataIndex);

            var dataBytes = Encoding.Default.GetBytes(data);
            return dataBytes;
        }

        public void DownloadToDisk(string FileURL, string LocalFilePath) {
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                DownloadToDisk(client, FileURL, LocalFilePath);
        }

        [Obsolete]
        public void DownloadToDisk___(Socket client, string FileURL, string LocalFilePath) {
            Uri url = new Uri(FileURL);

            IPAddress[] addresses = System.Net.Dns.GetHostAddresses(url.Host);
            IPEndPoint hostep = new IPEndPoint(addresses[0], 80);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sock.Connect(hostep);

            string request_txt = // File.ReadAllText("req.txt");
                string.Format("GET {0} HTTP/1.1\r\n", url.ToString()) +
                string.Format("Host: {0}\r\n", url.Host) +
                "Connection: keep-alive\r\n" +
                "Upgrade-Insecure-Requests: 1\r\n" +
                "User-Agent: Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36\r\n" +
                "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n" +
                "Accept-Encoding: gzip, deflate, sdch\r\n" +
                "Accept-Language: en-US,en;q=0.8\r\n" +
                "Cache-Control: no-cache\r\n\r\n";
            // request_txt = string.Format(request_txt, url.ToString(), url.Host);
            int response = sock.Send(Encoding.UTF8.GetBytes(request_txt));
            const int MAX_BYTES_RECEIVED = 10000000; // 10M max image size.
            byte[] bytesReceived = new byte[MAX_BYTES_RECEIVED];
            int n_bytes_received = sock.Receive(bytesReceived, bytesReceived.Length, SocketFlags.None);

            string packet_str = System.Text.Encoding.Default.GetString(bytesReceived, 0, n_bytes_received);
            int start_of_data_i = packet_str.IndexOf("\r\n\r\n") + 4;
            int content_length = int.Parse(Regex.Match(packet_str, @"^Content-Length:\s+(?<len>\d+)\s*$", RegexOptions.Multiline).Groups["len"].Value);
            Console.WriteLine("content_length {0}", content_length);
            bool is_gzip = Regex.IsMatch(packet_str, @"Content-Encoding:.*\bgzip\b", RegexOptions.Multiline);
            int retries = 0;
            while (content_length == 0 || n_bytes_received - start_of_data_i < content_length)
            {
                sock.ReceiveTimeout = 10000;
                int n = 0;
                try
                {
                    n = sock.Receive(bytesReceived, n_bytes_received, bytesReceived.Length - n_bytes_received, SocketFlags.None);
                }
                catch (Exception e)
                {
                    if (retries++ < 2)
                    {
                        continue;
                    }
                    break;
                }
                if (n == 0)
                {
                    break;
                }
                n_bytes_received += n;
            }
            sock.Close();

            byte[] res_data = new byte[n_bytes_received - start_of_data_i];
            Array.Copy(bytesReceived, start_of_data_i, res_data, 0, n_bytes_received - start_of_data_i);

            if (is_gzip)
            {
                GZipStream decompressionStream = new GZipStream(new MemoryStream(res_data), CompressionMode.Decompress);
                MemoryStream ms = new MemoryStream();
                decompressionStream.CopyTo(ms);
                res_data = ms.ToArray();
            }

            Stream dataStream = new MemoryStream(res_data);
            using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                dataStream.CopyTo(fs);
        }

        public void DownloadToDisk(Socket client, string FileURL, string LocalFilePath) {
            var response = ReadStream(client, FileURL);

            var dataIndex = response.IndexOf("\r\n\r\n") + 4;
            var data = response.Substring(dataIndex);

            /*
                HTTP/1.1 200 OK
                Accept-Ranges: bytes
                Content-Type: image/jpeg
                ETag: "f207e7-9c728-587f85825cd52"
                Last-Modified: Fri, 03 May 2019 09:26:24 GMT
                MyHeader: D=425 t=1556995161421281
                Server: Apache
                Content-Length: 640808
                Date: Tue, 07 May 2019 13:04:10 GMT
                Connection: keep-alive 
            */
            var headers = response.Substring(0, dataIndex);

            Match match = null;
            match = Regex.Match(headers, @"^HTTPs?/[0-9\.]+ \s+ (?<code>\d+) \s+ (?<text>.*?)$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match != null && match.Success) {
                var statusCode = match.Groups["code"].Value.Trim();
                if (statusCode != "200")
                    throw new Exception(string.Format("Request Error! {0} ({1})", match.Groups["text"].Value.Trim().Trim('\r'), statusCode));
            }

            var isGZip = false;
            match = Regex.Match(headers, @"^Content-Encoding: \s+ (?<enc>.*?)$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match != null && match.Success) {
                var encoding = match.Groups["enc"].Value;
                isGZip = encoding.Contains("gzip");
            }

            var dataBytes = Encoding.Default.GetBytes(data);
            Stream dataStream = new MemoryStream(dataBytes);

            if (isGZip)
                dataStream = new GZipStream(dataStream, CompressionMode.Decompress);

            /*
                using (var content = new StreamReader(dataStream, Encoding.Default))
                    response = content.ReadToEnd();
            */

            /*
                var bitmap = new Bitmap(dataStream);
                bitmap.Save(LocalFilePath);
            */

            if (dataStream.Length == 0)
                return;

            using (var fs = new FileStream(LocalFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                dataStream.CopyTo(fs);     
        }

        public void DownloadToDiskAsync(string FileURL, string LocalFilePath) {
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                DownloadToDiskAsync(client, FileURL, LocalFilePath);
        }

        public void DownloadToDiskAsync(Socket client, string FileURL, string LocalFilePath) {           
            Task.Factory.StartNew<Exception>(() => {
                try {
                    DownloadToDisk(FileURL, LocalFilePath);
                    return null;
                }
                catch (Exception ex) {
                    return ex;
                }
            }).ContinueWith(prev => {
                OnDownloadToDiskAsyncCompleted(null, new FileDownloadEventArgs
                {
                    Error = prev.Result,
                    FileURL = FileURL,
                    LocalFilePath = LocalFilePath
                });
            });
        }
    }
}
