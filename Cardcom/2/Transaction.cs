using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Transaction
    {         
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public int OrderId { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; } = -1;

        [JsonProperty(PropertyName = "statusCodeDetails")]
        public int StatusCodeDetails { get; set; } = -1;

        [JsonProperty(PropertyName = "statusCodeCard")]
        public int StatusCodeCard { get; set; } = -1;

        [JsonProperty(PropertyName = "cardOwnerId")]
        public string CardOwnerId { get; set; }

        [JsonProperty(PropertyName = "cardExpiry")]
        public string CardExpiry { get; set; }

        [JsonProperty(PropertyName = "cardSuffix")]
        public string CardSuffix { get; set; }

        [JsonProperty(PropertyName = "invoiceStatusCode")]
        public int InvoiceStatusCode { get; set; } = -1;

        [JsonProperty(PropertyName = "invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty(PropertyName = "invoiceType")]
        public int InvoiceType { get; set; }

        [JsonProperty(PropertyName = "numOfPayments")]
        public int NumOfPayments { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "raw")]
        public string Raw { get; set; }

        [JsonProperty(PropertyName = "rawDetails")]
        public string RawDetails { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime? CreatedDate { get; set; }        
    }
}
