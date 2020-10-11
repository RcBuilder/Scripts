using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HTTP
    {
        #region POST:
        public static string POST(string url, string vars)
        {
            CookieContainer COOKIES = new CookieContainer();
            return POST(url, vars, COOKIES);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return POST(url, vars, COOKIES, null, out STATUS);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES, out HttpStatusCode STATUS)
        {
            return POST(url, vars, COOKIES, null, out STATUS);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES, Dictionary<string, string> Headers)
        {
            HttpStatusCode STATUS;
            return POST(url, vars, COOKIES, Headers, out STATUS);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES, Dictionary<string, string> Headers, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                byte[] buffer = Encoding.ASCII.GetBytes(vars);

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Trident/5.0)";
                WebReq.CookieContainer = COOKIES;

                if (Headers != null && Headers.Count > 0)
                    foreach (var header in Headers)
                        WebReq.Headers.Add(header.Key, header.Value);

                // send post vars - stream
                using(var PostData = WebReq.GetRequestStream())
                    PostData.Write(buffer, 0, buffer.Length);                

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                foreach (Cookie cookie in WebResp.Cookies)
                    COOKIES.Add(cookie);

                using (Stream response = WebResp.GetResponseStream())
                using (StreamReader content = new StreamReader(response))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                var webResponse = ((HttpWebResponse)ex.Response);
                STATUS = webResponse == null ? HttpStatusCode.SeeOther : webResponse.StatusCode;
                return ex.Message;
            }
            finally { }
        }
        #endregion

        #region GET:
        public static string GET(string url, string vars)
        {
            CookieContainer COOKIES = new CookieContainer();
            return GET(url, vars, COOKIES);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return GET(url, vars, COOKIES, null, out STATUS);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES, out HttpStatusCode STATUS)
        {
            return GET(url, vars, COOKIES, null, out STATUS);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES, Dictionary<string, string> Headers, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                if (vars != string.Empty)
                    url = string.Concat(url, "?", vars);

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = "GET";
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                WebReq.CookieContainer = COOKIES;

                if (Headers != null && Headers.Count > 0)
                    foreach (var header in Headers)
                        WebReq.Headers.Add(header.Key, header.Value);

                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                foreach (Cookie cookie in WebResp.Cookies)
                    COOKIES.Add(cookie);

                using (Stream response = WebResp.GetResponseStream())
                using (StreamReader content = new StreamReader(response))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                var webResponse = ((HttpWebResponse)ex.Response);
                STATUS = webResponse == null ? HttpStatusCode.SeeOther : webResponse.StatusCode;
                return ex.Message;
            }
            finally { }
        }
        #endregion
    }
}
