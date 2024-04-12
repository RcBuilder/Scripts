using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace ServicesProxy
{
    public class ItemsProxy : BaseProxy
    {
        public ItemsProxy(HttpExtractor Extractor) : base(Extractor) {
            this.Domain = "https://localhost:59311/";            
        }
        public ItemsProxy(string Domain, string Token) : base(Domain, Token) { }

        public (bool Success, HttpStatusCode StatusCode, string Content, Entities.Item Model) GetItem(string id) {
            return new HttpServiceHelper().GET<Entities.Item>($"{this.Domain}item?id={HttpUtility.UrlEncode(id)}", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, Entities.SearchResult<Entities.Item> Model) SearchItems(Entities.ItemSearchParams SearchParams) {
            return new HttpServiceHelper().POST<Entities.ItemSearchParams, Entities.SearchResult<Entities.Item>>($"{this.Domain}/item/search", SearchParams, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, string Model) CreateItem(Entities.Item item)
        {
            return new HttpServiceHelper().POST<Entities.Item, string>($"{this.Domain}item", item, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}
