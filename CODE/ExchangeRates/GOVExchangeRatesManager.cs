using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ExchangeRates
{
    /*
        Source:
        https://www.boi.org.il/roles/markets/exchangerates/

        -

        Reference:
        edge_guide_he.pdf

        -

        Params:
        1. c%5BDATA_TYPE%5D         // data-type (e.g: OF00)
        2. c%5BBASE_CURRENCY%5D     // base currency (e.g: EUR,USD)
        3. c%5BSERIES_CODE%5D       // series_code (e.g: RER_EUR_ILS)
        4. format                   // result format type (e.g: csv)
        5. startperiod              // from date (e.g: 2022-06-01)
        6. endperiod                // to date (e.g: 2022-12-31)
        7. lastNObservations        // top-N (e.g: lastNObservations=2)

        data-type:
        1. OF00 - שערים יציגים

        series_code:
        RER_<Currency>_<Currency> // (e.g: RER_USD_ILS)

        lastNObservations:
        top-N, from latest to older.
        the N value is a numeric to represents the number of results PER CURRENCY to fetch. 
        for example, BBASE_CURRENCY=EUR,USD and lastNObservations=1 means total of 2 results (latest USD rate & latest EUR rate).

        supported base_currency:
        see eCurrency. 
        note! can add more currency codes as per need. 

        format:
        see FormatTypeValues. 
        note! in order to use XML format, we must remove this param. (format=xml DOES NOT work!)

        rates by specific date/ date-range:
        use 'startperiod' and 'endperiod' to define the date range to fetch. 

        -

        Csv Sample: 
        https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/?c%5BDATA_TYPE%5D=OF00&c%5BBASE_CURRENCY%5D=EUR&locale=he&format=csv
        SERIES_CODE,FREQ,BASE_CURRENCY,COUNTER_CURRENCY,UNIT_MEASURE,DATA_TYPE,DATA_SOURCE,TIME_COLLECT,CONF_STATUS,PUB_WEBSITE,UNIT_MULT,COMMENTS,TIME_PERIOD,OBS_VALUE,RELEASE_STATUS
        RER_EUR_ILS,D,EUR,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,1999-01-04,4.896,YP
        RER_EUR_ILS,D,EUR,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,1999-01-05,4.8427,YP
        ...
        ...

        https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/?c%5BDATA_TYPE%5D=OF00&c%5BSERIES_CODE%5D=RER_EUR_ILS,RER_USD_ILS&locale=he&format=csv
        SERIES_CODE,FREQ,BASE_CURRENCY,COUNTER_CURRENCY,UNIT_MEASURE,DATA_TYPE,DATA_SOURCE,TIME_COLLECT,CONF_STATUS,PUB_WEBSITE,UNIT_MULT,COMMENTS,TIME_PERIOD,OBS_VALUE,RELEASE_STATUS
        RER_USD_ILS,D,USD,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,1948-05-15,0.000025,YP
        RER_USD_ILS,D,USD,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,1949-09-19,0.0000357,YP
        ...
        ...

        https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/?c%5BDATA_TYPE%5D=OF00&c%5BBASE_CURRENCY%5D=EUR&locale=he&format=csv&lastNObservations=2
        SERIES_CODE,FREQ,BASE_CURRENCY,COUNTER_CURRENCY,UNIT_MEASURE,DATA_TYPE,DATA_SOURCE,TIME_COLLECT,CONF_STATUS,PUB_WEBSITE,UNIT_MULT,COMMENTS,TIME_PERIOD,OBS_VALUE,RELEASE_STATUS
        RER_EUR_ILS,D,EUR,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,2023-11-23,4.0716,YP
        RER_EUR_ILS,D,EUR,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,2023-11-24,4.0783,YP
        ...
        ...

        https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/?c%5BDATA_TYPE%5D=OF00&c%5BBASE_CURRENCY%5D=EUR,USD&locale=he&format=csv&lastNObservations=1
        SERIES_CODE,FREQ,BASE_CURRENCY,COUNTER_CURRENCY,UNIT_MEASURE,DATA_TYPE,DATA_SOURCE,TIME_COLLECT,CONF_STATUS,PUB_WEBSITE,UNIT_MULT,COMMENTS,TIME_PERIOD,OBS_VALUE,RELEASE_STATUS
        RER_USD_ILS,D,USD,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,2023-11-24,3.739,YP
        RER_EUR_ILS,D,EUR,ILS,ILS,OF00,BOI_MRKT,V,F,Y,0,,2023-11-24,4.0783,YP

        -

        USING:
        var rates = await new GOVExchangeRatesManager().GetLatestRates();
        foreach(var rate in rates)
            Console.WriteLine(rate);
        ...
        ...

        var rates = await new GOVExchangeRatesManager().GetLatestRates(new List<eCurrency> {
            eCurrency.USD,
            eCurrency.GBP,            
            eCurrency.EUR            
        });
        foreach(var rate in rates)
            Console.WriteLine(rate);
        ...
        ...
    */
    public class GOVExchangeRatesManager
    {        
        public class ExchangeRate
        {            
            [JsonProperty(PropertyName = "currencySrc")]
            public string CurrencyCodeSrc { get; set; }

            [JsonProperty(PropertyName = "currencyDest")]
            public string CurrencyCodeDest { get; set; }

            [JsonProperty(PropertyName = "rate")]
            public float RateValue { get; set; }
            
            [JsonProperty(PropertyName = "date")]
            public DateTime UpdatedDate { get; set; }

            public override string ToString()
            {
                return $"1 {this.CurrencyCodeSrc} = {this.RateValue} {this.CurrencyCodeDest}";
            }
        }

        public sealed class DataTypeValues {
            internal const string OF00 = "OF00";
        }
        public sealed class FormatTypeValues
        {
            internal const string JSON = "sdmx-json";            
            internal const string CSV = "csv";            
            internal const string EXCEL = "excel";            
        }
        public sealed class LocaleValues {
            internal const string HE = "he";
        }

        public enum eCurrency {
            USD,
            EUR,
            GBP,
            JPY,
            CHF,
            AUD,
            CAD,
            DKK,
            SEK,
            ZAR,
            EGP,
            JOD,
            NOK,
            LBP
        }


        // -------

        const string SERVER_BASE_URL = "https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/";
        static IEnumerable<eCurrency> DEFAULT_CURRENCIES { get {
                return new List<eCurrency> {
                    eCurrency.USD,
                    eCurrency.EUR,
                    eCurrency.GBP,
                    eCurrency.JPY,
                    eCurrency.CHF,
                    eCurrency.AUD,
                    eCurrency.CAD,
                    eCurrency.DKK,
                    eCurrency.SEK,
                    eCurrency.ZAR,
                    eCurrency.EGP,
                    eCurrency.JOD,
                    eCurrency.NOK,
                    eCurrency.LBP
                };
            } 
        }

        public async Task<IEnumerable<ExchangeRate>> GetLatestRates() {
            return await GetLatestRates(DEFAULT_CURRENCIES);
        }
        public async Task<IEnumerable<ExchangeRate>> GetLatestRates(IEnumerable<eCurrency> Currencies)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var endpoint = $@"{SERVER_BASE_URL}BOI.STATISTICS/EXR/1.0/?c%5BDATA_TYPE%5D={DataTypeValues.OF00}&c%5BBASE_CURRENCY%5D={string.Join(",", Currencies ?? Enumerable.Empty<eCurrency>())}&locale={LocaleValues.HE}&format={FormatTypeValues.CSV}&lastNObservations=1";                
                var sCSV = await client.GetStringAsync(endpoint);

                var rates = new List<ExchangeRate>();

                foreach (var line in sCSV.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1)) {
                    var columns = line.Split(',');
                    rates.Add(new ExchangeRate
                    {
                        CurrencyCodeDest = columns[2],
                        CurrencyCodeSrc = columns[3],
                        RateValue = Convert.ToSingle(columns[13]),
                        UpdatedDate = DateTime.Parse(columns[12])
                    });
                }

                return rates;
            }
        }

        // TODO ->>        
        // GET AS XML
        // GET AS EXCEL
        // GET AS CSV        
    }
}
