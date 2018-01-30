using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public enum eMethodType { POST, GET, DELETE, PUT }

    public class HTTPHelper
    {
        public enum ContentTypes : byte
        {
            [Description("application/x-www-form-urlencoded")]
            BASIC,
            [Description("application/json")]
            JSON,
            [Description("application/xml")]
            XML
        }

        #region POST:
        public static string POST(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return POST(url, vars, COOKIES, null, out STATUS);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES, out HttpStatusCode STATUS)
        {
            return POST(url, vars, COOKIES, null, out STATUS);
        }

        public static string POST(string url, string vars, CookieContainer COOKIES, NameValueCollection Headers, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                byte[] buffer = Encoding.ASCII.GetBytes(vars);

                var WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = eMethodType.POST.ToString();
                WebReq.ContentType = "application/x-www-form-urlencoded"; // ContentTypes.BASIC.GetDesc();
                WebReq.ContentLength = buffer.Length;
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                WebReq.CookieContainer = COOKIES;

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
                using (var content = new StreamReader(response))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    STATUS = HttpStatusCode.ExpectationFailed;
                else
                    STATUS = ((HttpWebResponse)ex.Response).StatusCode;
                return ex.Message;
            }
            finally { }
        }
        #endregion

        #region GET:
        public static string GET(string url, string vars, CookieContainer COOKIES)
        {
            HttpStatusCode STATUS;
            return GET(url, vars, COOKIES, null, out STATUS);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES, out HttpStatusCode STATUS)
        {
            return GET(url, vars, COOKIES, null, out STATUS);
        }

        public static string GET(string url, string vars, CookieContainer COOKIES, NameValueCollection Headers, out HttpStatusCode STATUS)
        {
            try
            {
                STATUS = HttpStatusCode.OK;

                if (vars != string.Empty)
                    url = string.Concat(url, "?", vars);

                var WebReq = (HttpWebRequest)WebRequest.Create(url);

                WebReq.Accept = "*/*";
                WebReq.Method = eMethodType.GET.ToString();
                WebReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                WebReq.CookieContainer = COOKIES;

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
                using (var content = new StreamReader(response))
                    return content.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    STATUS = HttpStatusCode.ExpectationFailed;
                else
                    STATUS = ((HttpWebResponse)ex.Response).StatusCode;
                return ex.Message;
            }
            finally { }
        }
        #endregion
    }
}
