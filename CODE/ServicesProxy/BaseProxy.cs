using BrokersEngine;
using Helpers;
using System;
using System.Web;

namespace ServicesProxy
{
    public abstract class BaseProxy
    {        
        public string Domain { get; protected set; }
        public string Token { get; protected set; }
        public HttpExtractor Extractor { get; protected set; }

        public BaseProxy(HttpExtractor Extractor) {
            this.Extractor = Extractor;                        
            this.Domain = Extractor.ExtractDomain(HttpContext.Current);
            this.Token = Extractor.ExtractBearerToken(HttpContext.Current);
        }

        public BaseProxy(string Domain, string Token) {
            this.Domain = Domain;
            this.Token = Token;            
        }
    }
}
