using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ServicesProxy
{
    public class DocumentsProxy : BaseProxy
    {
        public DocumentsProxy(HttpExtractor Extractor) : base(Extractor) {            
            this.Domain = "https://localhost:59312/";            
        }
        public DocumentsProxy(string Domain, string Token) : base(Domain, Token) {}

        public (bool Success, HttpStatusCode StatusCode, string Content, Entities.DocumentResult Model) CreateOrderMas(Entities.Order order) {            
            return new HttpServiceHelper().POST<Entities.Order, Entities.DocumentResult>($"{this.Domain}order", order, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}
