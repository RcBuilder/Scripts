using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServicesProxy
{
    public class AccountsProxy : BaseProxy
    {
        public AccountsProxy(HttpExtractor Extractor) : base(Extractor) {
            this.Domain = "https://127.0.0.1:59310/";              
        }

        public (bool Success, string Content) AccountExists(int id) {            
            return new HttpServiceHelper().GET($"{this.Domain}account/{id}/exists", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, string Content, Entities.Account Model) GetAccount(int id) {            
            return new HttpServiceHelper().GET<Entities.Account>($"{this.Domain}account/{id}", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, string Content, int Model) CreateAccount(Entities.Account account) {
            return new HttpServiceHelper().POST<Entities.Account, int>($"{this.Domain}account", account, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        // TODO ->> QA
        public (bool Success, string Content, IEnumerable<Entities.Account> Model) GetDataUpdates(Entities.DataUpdatesConfig config)
        {
            return new HttpServiceHelper().POST<Entities.DataUpdatesConfig, IEnumerable<Entities.Account>>($"{this.Domain}account/updates", config, headers: new Dictionary<string, string> {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}
