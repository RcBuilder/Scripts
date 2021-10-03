using Entities;
using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServicesProxy
{
    public class BrokerServicesProxy : BaseProxy
    {        
        public BrokerServicesProxy(HttpExtractor Extractor, string ServiceURL) : base(Extractor) {
            this.Domain = ServiceURL;
        }

        public (bool Success, string Content, string Model) GenerateDocToken(GenerateTokenRequest request) {            
            return new HttpServiceHelper().POST<GenerateTokenRequest, string>($"{this.Domain}token/doc", request, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}