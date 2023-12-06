using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    /*
        REFERENCE:
        https://exchangerate.host/
        https://exchangerate.host/#/#docs

        POSTMAN:
        DATA APIs.postman_collection.json

        LEGEND:
        - base      // source currency (e.g: ILS)
        - symbols   // dest currencies (e.g: USD,EUR,GBP)
        - amount    // specific amount to be converted (default: 1) 
        - places    // round to decimal places (x places after the floating point)
        - format    // response output format (supports: XML, CSV and TSV)

        DATE-FORMAT:
        yyyy-MM-dd  // e.g: 2023-01-01        

        LICENCE:
        free! public API

        Tester:
        ExchangeRatesManagerTester.cs

        ---

        USING:

        var exchangeRatesManager = new ExchangeRatesManager();
        
        // exchange-rates by NIS (Default)
        var latestRatesByNIS = await exchangeRatesManager.GetLatestRates();
        foreach (var rate in latestRatesByNIS)
            Console.WriteLine(rate);

        -

        // exchange-rates by USD
        var latestRatesByUSD = await exchangeRatesManager.GetLatestRates(new GetLatestRatesRequest
        {
            SrcCurrency = "USD"
        });
        foreach (var rate in latestRatesByUSD)
            Console.WriteLine(rate);

        -

        // exchange-rates by NIS with Currencies filter
        var latestRatesWithFilter = await exchangeRatesManager.GetLatestRates(new GetLatestRatesRequest
        {
            FilterCurrencies = new string[] { "USD", "EUR", "GBP" }
        });
        foreach (var rate in latestRatesWithFilter)
            Console.WriteLine(rate);

        -

        // exchange-rates as CSV format 
        var latestRatesAsCSV = await exchangeRatesManager.GetLatestRates(eOutputFormats.CSV);
        File.WriteAllText($"{OUTPUT_FOLDER}latestRatesAsCSV.csv", latestRatesAsCSV);
        Console.WriteLine($"{OUTPUT_FOLDER}latestRatesAsCSV.csv");

        -

        // exchange-rates as sJSON format 
        var latestRatesAsJSONString = await exchangeRatesManager.GetLatestRates(eOutputFormats.JSON);
        File.WriteAllText($"{OUTPUT_FOLDER}latestRatesAsJSONString.json", latestRatesAsJSONString);
        Console.WriteLine($"{OUTPUT_FOLDER}latestRatesAsJSONString.json");

        -

        // convert NIS to USD (Default)
        var convertedRateNisUsd = await exchangeRatesManager.ConvertRates();            
        Console.WriteLine(convertedRateNisUsd);

        -

        // convert USD to NIS
        var convertedRateUsdNis = await exchangeRatesManager.ConvertRates(new ConvertRatesRequest {
            SrcCurrency = "USD",
            DestCurrency = "ILS"
        });
        Console.WriteLine(convertedRateUsdNis);

        -

        // convert USD to NIS by specific date
        var date = new DateTime(2022, 1, 1);
        var convertedRateUsdNisByDate = await exchangeRatesManager.ConvertRates(new ConvertRatesRequest
        {
            SrcCurrency = "USD",
            DestCurrency = "ILS",
            Date = date
        });
        Console.WriteLine($"{convertedRateUsdNisByDate} [{date.ToString(ExchangeRates.Consts.DATE_FORMAT)}]");

        -

        // exchange-rates by NIS (Default)
        var ratesByNIS = await exchangeRatesManager.GetRates();
        foreach (var rate in ratesByNIS)
            Console.WriteLine(rate);

        -

        // exchange-rates by USD
        var ratesByUSD = await exchangeRatesManager.GetRates(new GetRatesRequest
        {
            SrcCurrency = "USD"
        });
        foreach (var rate in ratesByUSD)
            Console.WriteLine(rate);

        -

        // compare NIS rates between now and last year
        var dateTo = DateTime.Now;
        var dateFrom = dateTo.AddYears(-1);
        var compareRatesNISYear = await exchangeRatesManager.CompareRates(new CompareRatesRequest
        {
            SrcCurrency = "ILS",
            StartDate = dateFrom,
            EndDate = dateTo,
            FilterCurrencies = new string[] { "USD", "EUR", "GBP" }               
        });
        foreach (var rate in compareRatesNISYear)
            Console.WriteLine(rate);

        -

        // get currencies
        var currencies = await exchangeRatesManager.GetCurrencies();
        foreach (var currency in currencies)
            Console.WriteLine(currency);

        -

        // get currencies as CSV format
        var currenciesAsCSV = await exchangeRatesManager.GetCurrencies(eOutputFormats.CSV);
        File.WriteAllText($"{OUTPUT_FOLDER}currenciesAsCSV.csv", currenciesAsCSV);
        Console.WriteLine($"{OUTPUT_FOLDER}currenciesAsCSV.csv");

        -

        // get vat rates with Countries filter
        var vatRatesWithFilter = await exchangeRatesManager.GetVatRates(new GetVatRatesRequest { 
            FilterCountries = new string[] { "IT", "DE", "CY" }
        });
        foreach (var vatRate in vatRatesWithFilter)
            Console.WriteLine(vatRate);

        -

        // series of rates between now and 7 days ago
        var seriesToDate = DateTime.Now;
        var seriesFromDate = dateTo.AddDays(-7);
        var seriesOfRatesByDates = await exchangeRatesManager.GetSeriesOfRates(new GetSeriesOfRatesRequest
        {
            SrcCurrency = "ILS",
            StartDate = seriesFromDate,
            EndDate = seriesToDate,
            FilterCurrencies = new string[] { "USD", "EUR", "GBP" }
        });
        foreach (var series in seriesOfRatesByDates)
        {
            Console.WriteLine(series.Date.ToString(Consts.DATE_FORMAT));
            foreach (var rate in series.Rates)
                Console.WriteLine($"-- {rate}");
        }

        -

        // series of rates as CSV format 
        var seriesOfRatesByDatesAsCSV = await exchangeRatesManager.GetSeriesOfRates(eOutputFormats.CSV);
        File.WriteAllText($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsCSV.csv", seriesOfRatesByDatesAsCSV);
        Console.WriteLine($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsCSV.csv");

        -

        // series of rates as sJSON format 
        var seriesOfRatesByDatesAsJSONString = await exchangeRatesManager.GetSeriesOfRates(eOutputFormats.JSON);
        File.WriteAllText($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsJSONString.json", seriesOfRatesByDatesAsJSONString);
        Console.WriteLine($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsJSONString.json");
    */

    #region Consts
    public class Consts
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";
    }
    #endregion

    #region Entities
    public enum eOutputFormats : byte { JSON, XML, CSV, TSV }

    public class Currency
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{this.Code}";
        }
    }

    public class VatRate
    {
        public string CountryCode { get; set; }
        public float DefaultRate { get; set; }
        public IEnumerable<float> ReducedRates { get; set; }

        public override string ToString()
        {
            return $"{this.CountryCode} = {this.DefaultRate}%";
        }
    }

    public class ExchangeRate
    {
        public string SrcCurrency { get; set; }
        public string DestCurrency { get; set; }
        public float RateValue { get; set; }

        public override string ToString()
        {
            return $"1 {this.SrcCurrency} = {this.RateValue} {this.DestCurrency}";
        }
    }

    public class ExchangeRatesComparison : ExchangeRate
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public float StartRate { get; set; }
        public float EndRate { get; set; }

        public float RatePercentage { get; set; }

        public override string ToString()
        {
            return $@"[{this.SrcCurrency}] {StartDate.ToString(Consts.DATE_FORMAT)} = {this.StartRate} {this.DestCurrency} | {EndDate.ToString(Consts.DATE_FORMAT)} = {this.EndRate} {this.DestCurrency} | {this.RateValue} Difference ({this.RatePercentage}%)";
        }
    }

    public class ExchangeRateSeries {
        public DateTime Date { get; set; }
        public List<ExchangeRate> Rates { get; set; }
    }
    #endregion

    #region DTOs
    public interface IAPIRequest {
        List<string> GetAsParams();
    }

    public abstract class BaseRatesRequest : IAPIRequest
    {
        public string SrcCurrency { get; set; } = "ILS";
        public IEnumerable<string> FilterCurrencies { get; set; }
        public byte RoundValue { get; set; } = 6;
        public float Amount { get; set; } = 1;

        public virtual List<string> GetAsParams() {
            return new List<string> {
                    $"base={this.SrcCurrency.ToUpper()}",
                    $"symbols={string.Join(",", this.FilterCurrencies ?? Enumerable.Empty<string>())}",
                    $"places={this.RoundValue}",
                    $"amount={this.Amount}"
                };
        }
    }

    public class GetLatestRatesRequest : BaseRatesRequest {}

    public class ConvertRatesRequest : BaseRatesRequest
    {
        public string DestCurrency { get; set; } = "USD";
        public DateTime Date { get; set; } = DateTime.Now;

        public override List<string> GetAsParams()
        {
            var prms = base.GetAsParams();
            prms.Add($"from={this.SrcCurrency.ToUpper()}");
            prms.Add($"to={this.DestCurrency.ToUpper()}");
            prms.Add($"date={this.Date.ToString(Consts.DATE_FORMAT)}");
            return prms;
        }
    }

    public class GetRatesRequest : BaseRatesRequest {
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class CompareRatesRequest : BaseRatesRequest {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.Now;

        public override List<string> GetAsParams()
        {
            var prms = base.GetAsParams();            
            prms.Add($"start_date={this.StartDate.ToString(Consts.DATE_FORMAT)}");
            prms.Add($"end_date={this.EndDate.ToString(Consts.DATE_FORMAT)}");
            return prms;
        }
    }

    public class GetSeriesOfRatesRequest : CompareRatesRequest {}

    public class GetVatRatesRequest : IAPIRequest
    {
        public IEnumerable<string> FilterCountries { get; set; }

        public virtual List<string> GetAsParams()
        {
            return new List<string> {                    
                $"symbols={string.Join(",", this.FilterCountries ?? Enumerable.Empty<string>())}"                    
            };
        }
    }
    #endregion

    #region ExchangeRatesManager
    public interface IExchangeRatesManager {
        Task<IEnumerable<ExchangeRate>> GetLatestRates();
        Task<IEnumerable<ExchangeRate>> GetLatestRates(GetLatestRatesRequest Request);
        Task<string> GetLatestRates(eOutputFormats OutputFormat);
        Task<string> GetLatestRates(GetLatestRatesRequest Request, eOutputFormats OutputFormat);  
        
        Task<ExchangeRate> ConvertRates();
        Task<ExchangeRate> ConvertRates(ConvertRatesRequest Request);
        Task<string> ConvertRates(eOutputFormats OutputFormat);
        Task<string> ConvertRates(ConvertRatesRequest Request, eOutputFormats OutputFormat);

        Task<IEnumerable<ExchangeRate>> GetRates();
        Task<IEnumerable<ExchangeRate>> GetRates(GetRatesRequest Request);
        Task<string> GetRates(eOutputFormats OutputFormat);
        Task<string> GetRates(GetRatesRequest Request, eOutputFormats OutputFormat);

        Task<IEnumerable<ExchangeRatesComparison>> CompareRates();
        Task<IEnumerable<ExchangeRatesComparison>> CompareRates(CompareRatesRequest Request);
        Task<string> CompareRates(eOutputFormats OutputFormat);
        Task<string> CompareRates(CompareRatesRequest Request, eOutputFormats OutputFormat);

        Task<IEnumerable<Currency>> GetCurrencies();
        Task<string> GetCurrencies(eOutputFormats OutputFormat);

        Task<IEnumerable<VatRate>> GetVatRates();
        Task<IEnumerable<VatRate>> GetVatRates(GetVatRatesRequest Request);
        Task<string> GetVatRates(eOutputFormats OutputFormat);
        Task<string> GetVatRates(GetVatRatesRequest Request, eOutputFormats OutputFormat);

        Task<IEnumerable<ExchangeRateSeries>> GetSeriesOfRates();
        Task<IEnumerable<ExchangeRateSeries>> GetSeriesOfRates(GetSeriesOfRatesRequest Request);
        Task<string> GetSeriesOfRates(eOutputFormats OutputFormat);
        Task<string> GetSeriesOfRates(GetSeriesOfRatesRequest Request, eOutputFormats OutputFormat);
    }

    public class ExchangeRatesManager : IExchangeRatesManager
    {
        const string SERVER_URL = "https://api.exchangerate.host/";

        // Latest Rates
        public async Task<IEnumerable<ExchangeRate>> GetLatestRates() {
            return await this.GetLatestRates(new GetLatestRatesRequest());
        }
        public async Task<IEnumerable<ExchangeRate>> GetLatestRates(GetLatestRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}latest?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);

                var ModelSchema = new
                {
                    success = false,                    
                    date = "",
                    rates = new Dictionary<string, float>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.rates.Select(x => new ExchangeRate 
                    { 
                        SrcCurrency = Request.SrcCurrency,
                        DestCurrency = x.Key.ToUpper(),
                        RateValue = x.Value
                    });

                return null;
            }
        }

        public async Task<string> GetLatestRates(eOutputFormats OutputFormat) {
            return await this.GetLatestRates(new GetLatestRatesRequest(), OutputFormat);
        }
        public async Task<string> GetLatestRates(GetLatestRatesRequest Request, eOutputFormats OutputFormat) 
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();                
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}latest?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }
        
        // Convert Rates
        public async Task<ExchangeRate> ConvertRates()
        {
            return await this.ConvertRates(new ConvertRatesRequest());
        }
        public async Task<ExchangeRate> ConvertRates(ConvertRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}convert?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                
                var ModelSchema = new {
                    query = new {
                        from = "",
                        to = "",
                        amount = 0.0F
                    },
                    success = false,
                    date = "",
                    result = 0.0F
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)                     
                    return new ExchangeRate
                    {
                        SrcCurrency = response?.query?.from?.ToUpper(),
                        DestCurrency = response?.query?.to?.ToUpper(),
                        RateValue = response?.result ?? 0
                    };

                return null;
            }
        }

        public async Task<string> ConvertRates(eOutputFormats OutputFormat)
        {
            return await this.ConvertRates(new ConvertRatesRequest(), OutputFormat);
        }
        public async Task<string> ConvertRates(ConvertRatesRequest Request, eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}convert?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }

        // Rates
        public async Task<IEnumerable<ExchangeRate>> GetRates()
        {
            return await this.GetRates(new GetRatesRequest());
        }
        public async Task<IEnumerable<ExchangeRate>> GetRates(GetRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}{Request.Date.ToString(Consts.DATE_FORMAT)}?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);

                var ModelSchema = new
                {
                    success = false,
                    date = "",
                    rates = new Dictionary<string, float>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.rates.Select(x => new ExchangeRate
                    {
                        SrcCurrency = Request.SrcCurrency,
                        DestCurrency = x.Key.ToUpper(),
                        RateValue = x.Value
                    });

                return null;
            }
        }

        public async Task<string> GetRates(eOutputFormats OutputFormat)
        {
            return await this.GetRates(new GetRatesRequest(), OutputFormat);
        }
        public async Task<string> GetRates(GetRatesRequest Request, eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}{Request.Date.ToString(Consts.DATE_FORMAT)}?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }

        // Rates Comparison By Dates
        public async Task<IEnumerable<ExchangeRatesComparison>> CompareRates()
        {
            return await this.CompareRates(new CompareRatesRequest());
        }
        public async Task<IEnumerable<ExchangeRatesComparison>> CompareRates(CompareRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}fluctuation?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);

                var RateSchema = new
                {
                    start_rate = 0.0F,
                    end_rate = 0.0F,
                    change = 0.0F,
                    change_pct = 0.0F
                };

                var ModelSchema = new
                {
                    success = false,
                    start_date = "",
                    end_date = "",
                    rates = new Dictionary<string, dynamic>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.rates.Select(x => {
                        var rateInfo = JsonConvert.DeserializeAnonymousType(x.Value.ToString(), RateSchema);

                        return new ExchangeRatesComparison
                        {
                            SrcCurrency = Request.SrcCurrency,
                            DestCurrency = x.Key.ToUpper(),
                            RateValue = rateInfo.change,
                            RatePercentage = rateInfo.change_pct,
                            StartDate = Request.StartDate,
                            EndDate = Request.EndDate,
                            StartRate = rateInfo.start_rate,
                            EndRate = rateInfo.end_rate
                        };
                    });

                return null;
            }
        }

        public async Task<string> CompareRates(eOutputFormats OutputFormat)
        {
            return await this.CompareRates(new CompareRatesRequest(), OutputFormat);
        }
        public async Task<string> CompareRates(CompareRatesRequest Request, eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}fluctuation?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }

        // Currencies
        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var endpoint = $"{SERVER_URL}symbols";
                var sJson = await client.GetStringAsync(endpoint);

                var CurrencySchema = new
                {
                    code = "",
                    description = "",                    
                };

                var ModelSchema = new
                {
                    success = false,
                    symbols = new Dictionary<string, dynamic>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.symbols.Select(x => {
                        var currencyInfo = JsonConvert.DeserializeAnonymousType(x.Value.ToString(), CurrencySchema);

                        return new Currency
                        {
                            Code = currencyInfo.code,
                            Description = currencyInfo.description
                        };
                    });

                return null;
            }
        }

        public async Task<string> GetCurrencies(eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = new List<string>();
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}symbols?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }

        // VAT Rates
        public async Task<IEnumerable<VatRate>> GetVatRates() {
            return await this.GetVatRates(new GetVatRatesRequest());
        }
        public async Task<IEnumerable<VatRate>> GetVatRates(GetVatRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}vat_rates?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);

                /*
                 "rates": 
                 {
                    "AT": {
                        "country_name": "Austria",
                        "standard_rate": 20,
                        "reduced_rates": [
                            10,
                            13
                        ],
                        "super_reduced_rates": [],
                        "parking_rates": [
                            13
                        ]
                    },
                    ...
                    ...
                 }
                */
                var VatRateSchema = new
                {
                    country_name = "",
                    standard_rate = 0.0F
                };

                var ModelSchema = new
                {
                    success = false,
                    rates = new Dictionary<string, dynamic>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.rates.Select(x => {
                        var rateInfo = JsonConvert.DeserializeAnonymousType(x.Value.ToString(), VatRateSchema);

                        return new VatRate
                        {
                            CountryCode = x.Key,
                            DefaultRate = rateInfo.standard_rate,                            
                        };
                    });

                return null;
            }
        }

        public async Task<string> GetVatRates(eOutputFormats OutputFormat) {
            return await this.GetVatRates(new GetVatRatesRequest(), OutputFormat);
        }
        public async Task<string> GetVatRates(GetVatRatesRequest Request, eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();                
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}vat_rates?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }

        // Series Of Rates
        public async Task<IEnumerable<ExchangeRateSeries>> GetSeriesOfRates()
        {
            return await this.GetSeriesOfRates(new GetSeriesOfRatesRequest());
        }
        public async Task<IEnumerable<ExchangeRateSeries>> GetSeriesOfRates(GetSeriesOfRatesRequest Request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                var endpoint = $"{SERVER_URL}timeseries?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                
                var ModelSchema = new
                {
                    success = false,
                    start_date = "",
                    end_date = "",
                    rates = new Dictionary<string, Dictionary<string, float>>()
                };

                var response = JsonConvert.DeserializeAnonymousType(sJson, ModelSchema);

                if (response?.success ?? false)
                    return response?.rates.Select(x => new ExchangeRateSeries { 
                        Date = DateTime.Parse(x.Key),
                        Rates = x.Value.Select(r => new ExchangeRate
                        {
                            SrcCurrency = Request.SrcCurrency,
                            DestCurrency = r.Key.ToUpper(),
                            RateValue = r.Value
                        }).ToList()
                });
                
                return null;
            }
        }

        public async Task<string> GetSeriesOfRates(eOutputFormats OutputFormat)
        {
            return await this.GetSeriesOfRates(new GetSeriesOfRatesRequest(), OutputFormat);
        }
        public async Task<string> GetSeriesOfRates(GetSeriesOfRatesRequest Request, eOutputFormats OutputFormat)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var lstParams = Request.GetAsParams();
                lstParams.Add($"format={OutputFormat}");

                var endpoint = $"{SERVER_URL}timeseries?{string.Join("&", lstParams)}";
                var sJson = await client.GetStringAsync(endpoint);
                return sJson;
            }
        }
    }
    #endregion
}
