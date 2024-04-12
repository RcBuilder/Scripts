using Entities;
using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ServicesProxy
{
    public class SystemBrokersProxy : BaseProxy
    {
        public SystemBrokersProxy(string ServiceURL, string Token) : base(ServiceURL, Token) { }

        public (bool Success, HttpStatusCode StatusCode, string Content, BrokerData Model) GetBroker(string brokerName)
        {
            return new HttpServiceHelper().GET<BrokerData>($"{this.Domain}/broker/{brokerName}", headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, string Model) CreateBroker(BrokerData brokerData)
        {
            return new HttpServiceHelper().POST<BrokerData, string>($"{this.Domain}/broker", brokerData, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, string Model) UpdateBroker(BrokerData brokerData)
        {
            return new HttpServiceHelper().PUT<BrokerData, string>($"{this.Domain}/broker", brokerData, headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Token}"
            });
        }
    }
}