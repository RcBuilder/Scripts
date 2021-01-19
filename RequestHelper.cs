using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Helpers
{
    public class RequestHelper : IRequestHelper
    {
        public Dictionary<string, string> Query2Dictionary(string query)
        {
            try
            {
                query = query.Replace("?", string.Empty);

                return query.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> Query2Dictionary(HttpRequestBase Request)
        {
            try
            {
                var query = Request.QueryString.ToString().Replace("?", string.Empty);

                return query.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> Form2Dictionary(HttpRequestBase Request)
        {
            try
            {
                return Request.Form.AllKeys
                    .Select(x => new { key = x, value = Request.Form[x].ToString() })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch
            {
                return null;
            }
        }

        public static NameValueCollection ParseResponse(string sResponse)
        {
            try
            {
                var responseText = HttpUtility.UrlDecode(sResponse);
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(sResponse));
                return responseParsed;
            }
            catch
            {
                return null;
            }
        }
    }
}
