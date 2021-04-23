using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

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

        public Dictionary<string, string> Headers2Dictionary(HttpRequestBase Request) {
            return this.Headers2Dictionary(Request.Headers);
        }
        public Dictionary<string, string> Headers2Dictionary(NameValueCollection headers)
        {
            try
            {
                return headers.AllKeys.ToDictionary(k => k, k => headers[k]);
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

        public static dynamic ModelStateToJson(ModelStateDictionary ModelState)
        {
            var errorList = (
                from item in ModelState
                where item.Value.Errors.Any()
                select new
                {
                    key = item.Key,
                    errors = item.Value.Errors.Select(e => e.ErrorMessage)
                }
            );

            return errorList;
        }

        public static IEnumerable<(string Key, List<string> Errors)> ModelStateToList(ModelStateDictionary ModelState)
        {
            var errorList = (
                from item in ModelState
                where item.Value.Errors.Any()
                select (item.Key, item.Value.Errors.Select(e => e.ErrorMessage).ToList())
            );

            return errorList;
        }
    }
}
