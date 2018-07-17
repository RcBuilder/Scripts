using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestConsole
{
    /*
        -- USING --
        var response = HttpRequest.UseTcpClient();
    */

    public class HttpRequest
    {
        public static string UseWebBrowser() {
            var response = string.Empty;

            var waitEvent = new AutoResetEvent(false);
            var webBrowseThread = new Thread(() =>
            {
                using (var webBrowser = new WebBrowser())
                {
                    webBrowser.ScriptErrorsSuppressed = true; // disable the script errors popup
                    webBrowser.DocumentCompleted += (s, e) =>
                    {
                        string html = (s as WebBrowser).DocumentText;
                        response = html;
                        waitEvent.Set();
                    };

                    webBrowser.Navigate("http://www.sothebys.com/en.html");

                    while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                        Application.DoEvents();
                }
            });
            webBrowseThread.SetApartmentState(ApartmentState.STA); // required for webBrowser
            webBrowseThread.Start();
            waitEvent.WaitOne(); // wait for the site response

            return response;
        }

        public static string UseSocket() {
            var response = string.Empty;

            Uri url = new Uri("http://www.sothebys.com/en.html");
            var requestTXT = 
                string.Format("GET {0} HTTP/1.1\r\n", url.PathAndQuery) +
                string.Format("Host: {0}\r\n", url.Host) +
                "Connection: keep-alive\r\n" +
                "Accept: */*\r\n" +
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36\r\n" +
                "Accept-Encoding: gzip, deflate\r\n" +
                "Accept-Language: en-US,en;q=0.9\r\n" +
                "\r\n"; // important! empty row - spacer

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sock.Connect(url.Host, 80);
                byte[] writeBytes = Encoding.UTF8.GetBytes(requestTXT);
                sock.Send(writeBytes);

                var readBytes = new byte[Int16.MaxValue];
                var bytesReceived = sock.Receive(readBytes, readBytes.Length, SocketFlags.None);
                response = Encoding.Default.GetString(readBytes, 0, bytesReceived);

                var dataIndex = response.LastIndexOf("\r\n") + 2;
                var data = response.Substring(dataIndex);
                var headers = response.Substring(0, dataIndex);

                var dataBytes = Encoding.Default.GetBytes(data);

                var isGZip = false;
                var match = Regex.Match(headers, @"^Content-Encoding: \s+ (?<enc>.*?)$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (match != null && match.Success)
                {
                    var encoding = match.Groups["enc"].Value;
                    isGZip = encoding.Contains("gzip");
                }

                // gzip
                Stream dataStream = new MemoryStream(dataBytes);

                if (isGZip)
                    dataStream = new GZipStream(dataStream, CompressionMode.Decompress);

                using (var content = new StreamReader(dataStream, Encoding.Default))
                    response = content.ReadToEnd();

                var gzipStream = new GZipStream(new MemoryStream(dataBytes), CompressionMode.Decompress);
                using (var content = new StreamReader(gzipStream, Encoding.Default))
                    response = content.ReadToEnd();
            }
            finally {
                sock.Close();
            }

            return response;
        }

        public static string UseTcpClient() {
            var response = string.Empty;

            Uri url = new Uri("http://www.sothebys.com/en.html");
            var requestTXT =
                string.Format("GET {0} HTTP/1.1\r\n", url.PathAndQuery) +
                string.Format("Host: {0}\r\n", url.Host) +
                "Connection: keep-alive\r\n" +
                "Accept: */*\r\n" +
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36\r\n" +
                "Accept-Encoding: gzip, deflate\r\n" +
                "Accept-Language: en-US,en;q=0.9\r\n" +
                "\r\n"; // important! empty row - spacer

            using (var client = new TcpClient()) {
                client.Connect(url.Host, 80);

                client.SendTimeout = 500;
                //client.ReceiveTimeout = 1000;

                using (NetworkStream stream = client.GetStream())
                {
                    byte[] writeBytes = Encoding.UTF8.GetBytes(requestTXT);
                    stream.Write(writeBytes, 0, writeBytes.Length);
                    stream.Flush();

                    var readBytes = new byte[client.ReceiveBufferSize];
                    var bytes = stream.Read(readBytes, 0, readBytes.Length);
                    response = Encoding.Default.GetString(readBytes, 0, bytes);
                }
            }

            Console.WriteLine("original: {0}", response);

            var dataIndex = response.LastIndexOf("\r\n") + 2;
            var data = response.Substring(dataIndex);
            var headers = response.Substring(0, dataIndex);

            var dataBytes = Encoding.Default.GetBytes(data);

            var isGZip = false;
            var match = Regex.Match(headers, @"^Content-Encoding: \s+ (?<enc>.*?)$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match != null && match.Success)
            {
                var encoding = match.Groups["enc"].Value;
                isGZip = encoding.Contains("gzip");
            }

            // gzip
            Stream dataStream = new MemoryStream(dataBytes);

            if (isGZip)
                dataStream = new GZipStream(dataStream, CompressionMode.Decompress);

            using (var content = new StreamReader(dataStream, Encoding.Default))
                response = content.ReadToEnd();

            return response;
        }

        public static string UseRestClient() {
            var response = string.Empty;

            var client = new RestClient("http://www.sothebys.com/en.html");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Host", "www.sothebys.com");
            //request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9");

            IRestResponse restResponse = client.Execute(request);

            // code here ...

            return response;
        }

        public static string UseHttpClient()
        {
            var response = string.Empty;

            try {
                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Host", "www.sothebys.com");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                    response = client.GetStringAsync("http://www.sothebys.com/en.html").Result;
                }
            }
            catch (WebException ex)
            {
                response = "ERROR";
                var errorResponse = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// dynamic obj = JsonConvert.DeserializeObject(errorResponse);
                Console.WriteLine(errorResponse);
            }
            finally { }

            return response;
        }

        public static string UseWebClient()
        {
            var response = string.Empty;

            try {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Host", "www.sothebys.com");
                    client.Headers.Add("Connection", "keep-alive");
                    client.Headers.Add("Accept", "*/*");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
                    client.Headers.Add("Accept-Encoding", "gzip, deflate");
                    client.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                    response = client.DownloadString("http://www.sothebys.com/en.html");
                }
            }
            catch (WebException ex) {
                response = "ERROR";
                var errorResponse = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// dynamic obj = JsonConvert.DeserializeObject(errorResponse);
                Console.WriteLine(errorResponse);
            }
            finally { }

            return response;
        }

        public static string UseHttpWebRequest()
        {
            var response = string.Empty;

            try {
                var WebReq = (HttpWebRequest)WebRequest.Create("http://www.sothebys.com/en.html");
                WebReq.Headers.Clear();

                WebReq.Method = "GET";
                WebReq.Host = "www.sothebys.com";
                WebReq.Accept = "*/*";
                WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

                WebReq.Headers.Add("Accept-Encoding", "gzip, deflate");
                WebReq.Headers.Add("Accept-Language", "en-US,en;q=0.9");

                WebReq.Headers.GetType().InvokeMember(
                    "ChangeInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    WebReq.Headers,
                    new object[] { "Connection", "keep-alive" }
                );

                var WebResp = (HttpWebResponse)WebReq.GetResponse();

                using (var stream = WebResp.GetResponseStream())
                {
                    var decodedStream = stream;

                    if (WebResp.ContentEncoding.ToLower().Contains("gzip"))
                        decodedStream = new GZipStream(stream, CompressionMode.Decompress);

                    using (var content = new StreamReader(decodedStream, Encoding.Default))
                        response = content.ReadToEnd();
                    response = "OK!";
                }
            }
            catch (WebException ex)
            {
                response = "ERROR";
                var errorResponse = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// dynamic obj = JsonConvert.DeserializeObject(errorResponse);
                Console.WriteLine(errorResponse);
            }
            finally { }

            return response;
        }

        public static bool UseHttpWebRequestToDownloadImage()
        {
            try {
                var WebReq = (HttpWebRequest)WebRequest.Create("http://www.sothebys.com/content/dam/sothebys-pages/home-page-slides/2018/7/watches-0710-hp-380.jpg");

                WebReq.Accept = "*/*";
                WebReq.Method = "GET";
                WebReq.Host = "www.sothebys.com";
                WebReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

                WebReq.Headers = new WebHeaderCollection();
                WebReq.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                WebReq.Headers.Add("Accept-Encoding", "gzip, deflate");

                var WebResp = (HttpWebResponse)WebReq.GetResponse();

                using (var stream = WebResp.GetResponseStream()) {
                    using (var ms = new MemoryStream()) {
                        stream.CopyTo(ms);
                        byte[] readBytes = ms.ToArray();

                        using (FileStream fs = new FileStream("downloadedImage.jpg", FileMode.Create))
                            fs.Write(readBytes, 0, readBytes.Length);
                    }
                }

                return true;
            }
            catch (WebException ex)
            {
                var errorResponse = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// dynamic obj = JsonConvert.DeserializeObject(errorResponse);
                Console.WriteLine(errorResponse);
                return false;
            }
            finally { }
        }
    }
}
