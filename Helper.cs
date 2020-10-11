using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace MHCommon
{
    public class Helper
    {
        public static string GetFileContentFromWeb(string httpFilePath)
        {
            using (var client = new System.Net.WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var response = client.DownloadString(httpFilePath);
                return response;
            }
        }

        public static T ConvertToEnum<T>(string value) where T : struct
        {
            try
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T is not an ENUM!");
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch{
                return default(T);
            }
        }

        public static Dictionary<string, string> Query2Dictionary(string query)
        {
            try
            {
                query = query.Replace("?", string.Empty);

                return query.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch {
                return null;
            }
        }

        #region POST:
        public static string POST(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return POST(url, vars, null, null, COOKIES, null, null, out STATUS);
        }

        public static string POST(string url, string vars, string ContentType, string Method, NameValueCollection Headers, out HttpStatusCode STATUS)
        {
            return POST(url, vars, ContentType, Method, null, Headers, null, out STATUS);
        }

        public static string POST(string url, string vars, string ContentType, string Method, CookieContainer COOKIES, NameValueCollection Headers, string Proxy, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                byte[] buffer = Encoding.ASCII.GetBytes(vars);

                var WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = Method ?? "POST";

                // application/x-www-form-urlencoded
                // application/json
                WebReq.ContentType = ContentType ?? "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                WebReq.CookieContainer = COOKIES;

                if (!string.IsNullOrEmpty(Proxy))
                    WebReq.Proxy = new WebProxy(Proxy, true);

                if (Headers != null && Headers.Count > 0)
                {
                    if (WebReq.Headers == null)
                        WebReq.Headers = new WebHeaderCollection();
                    WebReq.Headers.Add(Headers);
                }

                // send post vars - stream
                var PostData = WebReq.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();

                var WebResp = (HttpWebResponse)WebReq.GetResponse();

                foreach (Cookie cookie in WebResp.Cookies)
                    COOKIES.Add(cookie);

                using (var response = WebResp.GetResponseStream())
                using (var content = new StreamReader(response, Encoding.UTF8))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var WebRespEX = ((HttpWebResponse)ex.Response);
                    STATUS = WebRespEX.StatusCode;
                    using (var response = WebRespEX.GetResponseStream())
                    using (var content = new StreamReader(response, Encoding.UTF8))
                        return content.ReadToEnd();
                }
                else
                    STATUS = HttpStatusCode.ExpectationFailed;
                return string.Empty;
            }
            finally { }
        }
        #endregion

        #region GET:
        public static string GET(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return GET(url, vars, COOKIES, null, null, out STATUS);
        }

        public static string GET(string url, string vars, NameValueCollection Headers, out HttpStatusCode STATUS)
        {
            return GET(url, vars, null, Headers, null, out STATUS);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES, NameValueCollection Headers, string Proxy, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                if (vars != string.Empty)
                    url = string.Concat(url, "?", vars);

                var WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = "GET";
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                WebReq.CookieContainer = COOKIES;

                if (!string.IsNullOrEmpty(Proxy))
                    WebReq.Proxy = new WebProxy(Proxy, true);

                if (Headers != null && Headers.Count > 0)
                {
                    if (WebReq.Headers == null)
                        WebReq.Headers = new WebHeaderCollection();
                    WebReq.Headers.Add(Headers);
                }

                var WebResp = (HttpWebResponse)WebReq.GetResponse();

                foreach (Cookie cookie in WebResp.Cookies)
                    COOKIES.Add(cookie);

                using (var response = WebResp.GetResponseStream())
                using (var content = new StreamReader(response, Encoding.UTF8))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    STATUS = ((HttpWebResponse)ex.Response).StatusCode;
                else
                    STATUS = HttpStatusCode.ExpectationFailed;
                return string.Empty;
            }
            finally { }
        }
        #endregion
    }
}
