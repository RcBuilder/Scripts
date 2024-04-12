using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ServicesProxy
{
    public class AccountsProxy : BaseProxy
    {
        public AccountsProxy(HttpExtractor Extractor) : base(Extractor) {            
            this.Domain = "https://localhost:59310/";            
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) AccountExists(int id) {            
            return new HttpServiceHelper().GET($"{this.Domain}account/{id}/exists", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, Entities.Account Model) GetAccount(int id) {            
            return new HttpServiceHelper().GET<Entities.Account>($"{this.Domain}account/{id}", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, Entities.AccountResult Model) CreateAccount(Entities.Account account) {
            return new HttpServiceHelper().POST<Entities.Account, Entities.AccountResult>($"{this.Domain}account", account, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}
