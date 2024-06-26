﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyServer
{
    /*
        GET http://localhost:2346 --> GET https://jsonplaceholder.typicode.com
        GET http://localhost:2346/todos --> GET https://jsonplaceholder.typicode.com/todos
        GET http://localhost:2346/posts/1 --> GET https://jsonplaceholder.typicode.com/posts/1
    */

    public class SampleProxyMessageHandler : DelegatingHandler
    {
        private string TargetURI { get; set; }

        public SampleProxyMessageHandler(string TargetURI) : base() {
            this.TargetURI = TargetURI;
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            var ms = new MemoryStream();
            if (request.Content != null)
            {
                await request.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;

                if ((ms.Length > 0 || request.Content.Headers.Any()) && clone.Method != HttpMethod.Get)
                {
                    clone.Content = new StreamContent(ms);

                    if (request.Content.Headers != null)
                        foreach (var h in request.Content.Headers)
                            clone.Content.Headers.Add(h.Key, h.Value);
                }
            }

            clone.Version = request.Version;

            foreach (var prop in request.Properties)
                clone.Properties.Add(prop);

            foreach (var header in request.Headers)
            {
                switch (header.Key?.ToLower())
                {
                    case "host":
                        // change destination host-name
                        clone.Headers.TryAddWithoutValidation(header.Key, $"{this.TargetURI}");
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
            using (var client = new HttpClient())
            {                
                var clonedRequest = await CloneHttpRequestMessageAsync(request);
                clonedRequest.RequestUri = new Uri($"{this.TargetURI}{request.RequestUri.LocalPath}{request.RequestUri.Query}");                                
                return await client.SendAsync(clonedRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return ForwardRequest(request, cancellationToken);
        }
    }
}