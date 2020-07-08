using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Item
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }
        [JsonProperty(PropertyName = "saleDate")]
        public DateTime SaleDate { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string ImageData { get; set; }
    }
}
