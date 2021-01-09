using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Helpers
{
    public class HttpServiceHelper : IHttpServiceHelper
    {
        private const double TimeOutSec = 30;
        private WebClient client { get; set; } = new WebClient {
            Proxy = null
        };

        public (bool Success, string Content) GET(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                client.Headers.Clear();
                foreach (var header in headers)
                    client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                return (true, client.DownloadString(url));
            }
            catch(Exception ex) {
                return (false, ex.Message);
            }
        }

        public (bool Success, string Content, T Model) GET<T>(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                client.Headers.Clear();
                foreach (var header in headers)
                    client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                var content = client.DownloadString(url);
                return (true, content, JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, default(T));
            }
        }

        public (bool Success, string Content) POST<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, querystring, headers, "POST");
        }

        public (bool Success, string Content) PUT<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, querystring, headers, "PUT");
        }

        public (bool Success, string Content) DELETE<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, querystring, headers, "DELETE");
        }

        protected (bool Success, string Content) UPLOAD<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null, string method = "POST")
        {            
            try
            {
                client.Headers.Clear();
                if (headers != null)
                    foreach (var header in headers)
                        client.Headers.Add(header.Key, header.Value);
                    
                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                var payloadType = payload.GetType();
                string sPayload;
                if (payloadType.IsPrimitive || payloadType == typeof(System.String))
                    sPayload = payload.ToString();                
                else 
                    sPayload = JsonConvert.SerializeObject(payload);

                return (true, client.UploadString(url, method, sPayload));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
