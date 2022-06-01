using Authorization;
using BrokersEngine;
using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ProxyServer
{
    public class CreativeProxyMessageHandler : DelegatingHandler
    {
        private string BrokerName { get; set; }
        private string TargetURL { get; set; }
        private int TargetPort { get; set; }

        private string JWTSecretKey { get; set; }
        private ILoggerAsync Logger { get; set; }
        private TimeSpan Timeout = TimeSpan.FromSeconds(90);

        public CreativeProxyMessageHandler(string JWTSecretKey, ILoggerAsync Logger) {
            this.Logger = Logger;
            this.JWTSecretKey = JWTSecretKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await this.InitBroker(request);
                await this.InitTargetServer(request);
                await this.LogTraffic(request);
                return await ForwardRequest(request, cancellationToken);
            }
            catch (StatusException ex)
            {
                await this.LogError($"[{(int)ex.StatusCode}] {ex.Message}", request);
                return request.CreateResponse(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                await this.LogError($"[{(int)HttpStatusCode.InternalServerError}] {ex.Message}", request);                
                return request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
                
        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            var ms = new MemoryStream();
            if (request.Content != null)
            {
                await request.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;

                if ((ms.Length > 0 || request.Content.Headers.Any()) && clone.Method != HttpMethod.Get) {
                    clone.Content = new StreamContent(ms);

                    if (request.Content.Headers != null)
                        foreach (var h in request.Content.Headers)
                            clone.Content.Headers.Add(h.Key, h.Value);
                }
            }

            clone.Version = request.Version;

            foreach (var prop in request.Properties)
                clone.Properties.Add(prop);

            foreach (var header in request.Headers) {
                switch (header.Key?.ToLower()) {
                    case "host": 
                        // change destination host-name
                        clone.Headers.TryAddWithoutValidation(header.Key, $"{this.TargetURL}:{this.TargetPort}");
                        break;
                    default:
                        clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        break;
                }                
            }

            return clone;
        }

        private async Task<HttpResponseMessage> ForwardRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {        
            using (var client = new HttpClient()) {
                client.Timeout = this.Timeout; 
                var clonedRequest = await CloneHttpRequestMessageAsync(request);
                clonedRequest.RequestUri = this.BuildTargetRequestUri(request);
                return await client.SendAsync(clonedRequest, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }                        
        }

        private async Task InitBroker(HttpRequestMessage request)
        {
            // note! broker is mandatory in order to pull the server location

            try
            {                
                var token = JWTGenerator.Utilities.GetTokenFromQuery(request);
                if (string.IsNullOrEmpty(token))
                    token = JWTGenerator.Utilities.GetTokenFromHeader(request);

                var generator = new JWTGenerator(this.JWTSecretKey);
                var tokenPayload = generator.GetTokenPayload(token);
                var schema = new
                {
                    BrokerName = "",
                    Role = "",
                    RefName = ""
                };
                var tokenPayloadModel = JsonConvert.DeserializeAnonymousType(tokenPayload, schema);
                this.BrokerName = tokenPayloadModel.BrokerName;
            }
            catch (Exception ex) {
                throw new StatusException(HttpStatusCode.Unauthorized, ex.Message);
            }
        }

        private async Task InitTargetServer(HttpRequestMessage request) {

            var brokerFactory = new ProxyBrokerFactory("SystemBrokersServiceURI");
            var broker = brokerFactory.Produce(this.BrokerName);

            this.TargetURL = broker.Server;
            this.TargetPort = request.RequestUri.Port; // 59310-59315
            
            if (string.IsNullOrEmpty(this.TargetURL))
                throw new Exception($"No TargetURL ({this.BrokerName})");
        }

        private async Task LogTraffic(HttpRequestMessage request) {
            if (this.Logger == null) return;
            await this.Logger.InfoAsync("Traffic", $"Source: '{request.RequestUri}' | Target: '{this.BuildTargetRequestUri(request)}'", await this.GetRequestInfoToLog(request));
        }
        private async Task LogError(string Message, HttpRequestMessage request)
        {
            if (this.Logger == null) return;
            await this.Logger.ErrorAsync("Traffic", Message, await this.GetRequestInfoToLog(request));
        }

        private Uri BuildTargetRequestUri(HttpRequestMessage request) {
            return new Uri($"{this.TargetURL}:{this.TargetPort}{request.RequestUri.LocalPath}{request.RequestUri.Query}");
        }

        private async Task<List<string>> GetRequestInfoToLog(HttpRequestMessage request) {
            var requestInfo = new List<string>();

            try {
                var ip = string.Empty;
                if (request.Properties.ContainsKey("MS_HttpContext"))
                    ip = (request.Properties["MS_HttpContext"] as HttpContextWrapper)?.Request.UserHostAddress;

                var method = request.Method;

                // get the request post data
                var postData = new Dictionary<string, string>();
                var sSontent = await request.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(sSontent))
                    postData.Add("sContent", sSontent);

                // get the querystring data
                var queryData = request.GetQueryNameValuePairs();
                foreach (var p in queryData)
                    postData.Add(p.Key, p.Value);

                requestInfo.AddRange(new List<string> {
                    $"Broker:{this.BrokerName}",
                    $"IP:{ip ?? ""}",
                    $"Verb:{request.Method.Method}"
                });

                requestInfo.AddRange(postData.Select(x => string.Concat(x.Key, "=", x.Value)));                
            }
            catch (Exception ex) {
                requestInfo.Add($"Exception: {ex.Message}");
            }
            finally {}

            return requestInfo;
        }
    }
}