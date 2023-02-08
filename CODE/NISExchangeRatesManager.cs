using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ExchangeRates
{
    /*
         var rates = await new NISExchangeRatesManager().GetLatestRates();
         foreach(var rate in rates)
            Console.WriteLine(rate);
    */
    public class NISExchangeRatesManager
    {
        public class ExchangeRatesResponse
        {
            [JsonProperty(PropertyName = "exchangeRates")]
            public ExchangeRate[] Rates { get; set; }
        }

        public class ExchangeRate
        {
            [JsonProperty(PropertyName = "key")]
            public string CurrencyCode { get; set; }

            [JsonProperty(PropertyName = "currentExchangeRate")]
            public float RateValue { get; set; }

            [JsonProperty(PropertyName = "currentChange")]
            public float ChangeValue { get; set; }

            [JsonProperty(PropertyName = "lastUpdate")]
            public DateTime UpdatedDate { get; set; }

            public override string ToString()
            {
                return $"{this.CurrencyCode} = {this.RateValue} NIS";
            }
        }

        // -------

        const string SERVER_URL = "http://www.boi.org.il/currency.xml";

        public async Task<IEnumerable<ExchangeRate>> GetLatestRates()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var sJson = await client.GetStringAsync(SERVER_URL);
                var response = JsonConvert.DeserializeObject<ExchangeRatesResponse>(sJson);
                return response?.Rates;
            }

            /*
            using(var client = new System.Net.WebClient()) {    	    
                client.Headers["Content-Type"] = "application/json";	
                
                var sJson = client.DownloadString(SERVER_URL);	  
                var response = JsonConvert.DeserializeObject<ExchangeRatesResponse>(sJson);
                return response?.Rates;
            }
            */
        }
    }
}
