using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Routing;

namespace Helpers
{
    // TODO ->> implement
    public class HttpExtractor : IHttpExtractor
    {
        public HttpCookieCollection ExtractCookies(HttpContext context)
        {
            return context.Request.Cookies;
        }

        public Dictionary<string, string> ExtractCookiesAsDictionary(HttpContext context)
        {            
            try
            {
                var cookies = this.ExtractCookies(context);
                return cookies.AllKeys.ToDictionary(k => k, k => cookies[k].Value);
            }
            catch
            {
                return null;
            }
        }

        public NameValueCollection ExtractHeaders(HttpContext context) {
            return context.Request.Headers;
        }

        public Dictionary<string, string> ExtractHeadersAsDictionary(HttpContext context)
        {
            try
            {
                var headers = this.ExtractHeaders(context);
                return headers.AllKeys.ToDictionary(k => k, k => headers[k]);
            }
            catch
            {
                return null;
            }
        }

        public string ExtractHttpMethod(HttpContext context)
        {
            return context.Request.HttpMethod;
        }

        public string ExtractPayload(HttpContext context)
        {            
            var stream = context.Request.InputStream;
            stream.Position = 0;

            // note! DO NOT use "using" clause - it prevents another payload read later on the pipeline 
            var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();            
        }

        public T ExtractPayloadAsT<T>(HttpContext context)
        {
            try
            {
                var sBody = this.ExtractPayload(context);
                if (string.IsNullOrEmpty(sBody)) return default(T);

                return JsonConvert.DeserializeObject<T>(sBody);
            }
            catch {
                return default(T);
            }
        }

        public string ExtractQuery(HttpContext context)
        {
            return context.Request.Url.Query;
        }

        public Dictionary<string, string> ExtractQueryAsDictionary(HttpContext context)
        {
            try
            {
                var query = this.ExtractQuery(context);
                query = query.Replace("?", string.Empty).Trim();
                if (string.IsNullOrEmpty(query)) return new Dictionary<string, string>();

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

        public RouteData ExtractRouteData(HttpContext context)
        {
            return context.Request.RequestContext.RouteData;
        }

        public Dictionary<string, string> ExtractRouteAsDictionary(HttpContext context)
        {
            try
            {
                var routeData = this.ExtractRouteData(context);
                return routeData.Values.ToDictionary(x => x.Key, x => x.Value.ToString());
            }
            catch {
                return null;
            }
        }

        public string ExtractDomain(HttpContext context)
        {
            return $"{context.Request.Url.Scheme}://{context.Request.Url.Host}";
        }

        public int ExtractPort(HttpContext context)
        {
            return context.Request.Url.Port;
        }

        public string ExtractServerURI(HttpContext context) {
            if (context.Request.Url.Port == 80) return this.ExtractDomain(context);
            return $"{this.ExtractDomain(context)}:{context.Request.Url.Port}";
        }

        public string ExtractAuthorizationHeader(HttpContext context)
        {
            var headerKey = "Authorization";
            var headers = this.ExtractHeadersAsDictionary(context);
            if(headers.ContainsKey(headerKey))
                return headers[headerKey];
            return null;
        }

        public string ExtractBearerToken(HttpContext context) {
            var header = this.ExtractAuthorizationHeader(context);
            if (header == null || !header.StartsWith("Bearer")) return null;
            return header.Replace("Bearer", string.Empty).Trim();
        }
    }
}