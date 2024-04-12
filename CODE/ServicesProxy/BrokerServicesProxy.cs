using Entities;
using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ServicesProxy
{
    public class BrokerServicesProxy : BaseProxy
    {        
        public BrokerServicesProxy(HttpExtractor Extractor, string ServiceURL) : base(Extractor) {
            this.Domain = ServiceURL;
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, string Model) GenerateDocToken(GenerateTokenRequest request) {            
            return new HttpServiceHelper().POST<GenerateTokenRequest, string>($"{this.Domain}token/doc", request, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, QueryData Model) GetQuery(int queryId)
        {
            return new HttpServiceHelper().GET<QueryData>($"{this.Domain}query/{queryId}", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}