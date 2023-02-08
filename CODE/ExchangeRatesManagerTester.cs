using ExchangeRates;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TestConsole7
{
    public class ExchangeRatesManagerTester
    {
        private const string OUTPUT_FOLDER = @"D:\TEMP\ExchangeRates\";

        /*
            NISExchangeRatesManager

            // common exchange-rates by NIS
            var rates = await new NISExchangeRatesManager().GetLatestRates();
            foreach (var rate in rates)
                Console.WriteLine(rate); 
        */


        // ExchangeRatesManager
        public async static Task Run() {
            var exchangeRatesManager = new ExchangeRatesManager();

            // exchange-rates by NIS (Default)
            var latestRatesByNIS = await exchangeRatesManager.GetLatestRates();
            foreach (var rate in latestRatesByNIS)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates by USD
            var latestRatesByUSD = await exchangeRatesManager.GetLatestRates(new GetLatestRatesRequest
            {
                SrcCurrency = "USD"
            });
            foreach (var rate in latestRatesByUSD)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates by NIS with Currencies filter
            var latestRatesWithFilter = await exchangeRatesManager.GetLatestRates(new GetLatestRatesRequest
            {
                FilterCurrencies = new string[] { "USD", "EUR", "GBP" }
            });
            foreach (var rate in latestRatesWithFilter)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates as CSV format 
            var latestRatesAsCSV = await exchangeRatesManager.GetLatestRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}latestRatesAsCSV.csv", latestRatesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}latestRatesAsCSV.csv");

            Console.WriteLine("-");

            // exchange-rates as sJSON format 
            var latestRatesAsJSONString = await exchangeRatesManager.GetLatestRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}latestRatesAsJSONString.json", latestRatesAsJSONString);
            Console.WriteLine($"{OUTPUT_FOLDER}latestRatesAsJSONString.json");

            Console.WriteLine("-");

            // convert NIS to USD (Default)
            var convertedRateNisUsd = await exchangeRatesManager.ConvertRates();            
            Console.WriteLine(convertedRateNisUsd);

            Console.WriteLine("-");

            // convert USD to NIS
            var convertedRateUsdNis = await exchangeRatesManager.ConvertRates(new ConvertRatesRequest {
                SrcCurrency = "USD",
                DestCurrency = "ILS"
            });
            Console.WriteLine(convertedRateUsdNis);

            Console.WriteLine("-");

            // convert USD to NIS by specific date
            var date = new DateTime(2022, 1, 1);
            var convertedRateUsdNisByDate = await exchangeRatesManager.ConvertRates(new ConvertRatesRequest
            {
                SrcCurrency = "USD",
                DestCurrency = "ILS",
                Date = date
            });
            Console.WriteLine($"{convertedRateUsdNisByDate} [{date.ToString(ExchangeRates.Consts.DATE_FORMAT)}]");

            Console.WriteLine("-");

            // converted-rate as CSV format 
            var convertedRateAsCSV = await exchangeRatesManager.ConvertRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}convertedRateAsCSV.csv", convertedRateAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}convertedRateAsCSV.csv");

            Console.WriteLine("-");

            // converted-rate as sJSON format 
            var convertedRateAsJSONString = await exchangeRatesManager.ConvertRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}convertedRateAsJSONString.json", convertedRateAsJSONString);
            Console.WriteLine($"{OUTPUT_FOLDER}convertedRateAsJSONString.json");

            Console.WriteLine("-");

            // exchange-rates by NIS (Default)
            var ratesByNIS = await exchangeRatesManager.GetRates();
            foreach (var rate in ratesByNIS)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates by USD
            var ratesByUSD = await exchangeRatesManager.GetRates(new GetRatesRequest
            {
                SrcCurrency = "USD"
            });
            foreach (var rate in ratesByUSD)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates by NIS with Currencies filter
            var ratesWithFilter = await exchangeRatesManager.GetRates(new GetRatesRequest
            {
                FilterCurrencies = new string[] { "USD", "EUR", "GBP" }
            });
            foreach (var rate in ratesWithFilter)
                Console.WriteLine(rate);

            Console.WriteLine("-");

            // exchange-rates by NIS with Currencies filter & specific date
            var ratesByDateWithFilter = await exchangeRatesManager.GetRates(new GetRatesRequest
            {
                FilterCurrencies = new string[] { "USD", "EUR", "GBP" },
                Date = date
            });
            foreach (var rate in ratesByDateWithFilter)
                Console.WriteLine($"{rate} [{date.ToString(ExchangeRates.Consts.DATE_FORMAT)}]");

            Console.WriteLine("-");

            // exchange-rates as CSV format 
            var ratesAsCSV = await exchangeRatesManager.GetRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}ratesAsCSV.csv", ratesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}ratesAsCSV.csv");

            Console.WriteLine("-");

            // exchange-rates as sJSON format 
            var ratesAsJSONString = await exchangeRatesManager.GetRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}ratesAsJSONString.json", ratesAsJSONString);
            Console.WriteLine($"{OUTPUT_FOLDER}ratesAsJSONString.json");

            Console.WriteLine("-");

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

            Console.WriteLine("-");

            // compare NIS rates as CSV format 
            var compareRatesAsCSV = await exchangeRatesManager.CompareRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}compareRatesAsCSV.csv", compareRatesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}compareRatesAsCSV.csv");

            Console.WriteLine("-");

            // compare NIS rates as sJSON format 
            var compareRatesAsJSONString = await exchangeRatesManager.CompareRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}compareRatesAsJSONString.json", compareRatesAsJSONString);
            Console.WriteLine($"{OUTPUT_FOLDER}compareRatesAsJSONString.json");

            Console.WriteLine("-");

            // get currencies
            var currencies = await exchangeRatesManager.GetCurrencies();
            foreach (var currency in currencies)
                Console.WriteLine(currency);

            Console.WriteLine("-");

            // get currencies as CSV format
            var currenciesAsCSV = await exchangeRatesManager.GetCurrencies(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}currenciesAsCSV.csv", currenciesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}currenciesAsCSV.csv");

            // get currencies as sJSON format
            var currenciesAsJSON = await exchangeRatesManager.GetCurrencies(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}currenciesAsJSON.json", currenciesAsJSON);
            Console.WriteLine($"{OUTPUT_FOLDER}currenciesAsJSON.json");

            Console.WriteLine("-");

            // get vat rates
            var vatRates = await exchangeRatesManager.GetVatRates();
            foreach (var vatRate in vatRates)
                Console.WriteLine(vatRate);

            Console.WriteLine("-");

            // get vat rates with Countries filter
            var vatRatesWithFilter = await exchangeRatesManager.GetVatRates(new GetVatRatesRequest { 
                FilterCountries = new string[] { "IT", "DE", "CY" }
            });
            foreach (var vatRate in vatRatesWithFilter)
                Console.WriteLine(vatRate);

            Console.WriteLine("-");

            // get vat rates as CSV format
            var vatRatesAsCSV = await exchangeRatesManager.GetVatRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}vatRatesAsCSV.csv", vatRatesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}vatRatesAsCSV.csv");

            // get vat rates as sJSON format
            var vatRatesAsJSON = await exchangeRatesManager.GetVatRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}vatRatesAsJSON.json", vatRatesAsJSON);
            Console.WriteLine($"{OUTPUT_FOLDER}vatRatesAsJSON.json");

            // get vat rates as sJSON format with Countries filter
            var vatRatesWithFilterAsJSON = await exchangeRatesManager.GetVatRates(new GetVatRatesRequest
            {
                FilterCountries = new string[] { "IT", "DE", "CY" }
            }, eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}vatRatesWithFilterAsJSON.json", vatRatesWithFilterAsJSON);
            Console.WriteLine($"{OUTPUT_FOLDER}vatRatesWithFilterAsJSON.json");

            Console.WriteLine("-");

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

            Console.WriteLine("-");

            // series of rates as CSV format 
            var seriesOfRatesByDatesAsCSV = await exchangeRatesManager.GetSeriesOfRates(eOutputFormats.CSV);
            File.WriteAllText($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsCSV.csv", seriesOfRatesByDatesAsCSV);
            Console.WriteLine($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsCSV.csv");

            Console.WriteLine("-");

            // series of rates as sJSON format 
            var seriesOfRatesByDatesAsJSONString = await exchangeRatesManager.GetSeriesOfRates(eOutputFormats.JSON);
            File.WriteAllText($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsJSONString.json", seriesOfRatesByDatesAsJSONString);
            Console.WriteLine($"{OUTPUT_FOLDER}seriesOfRatesByDatesAsJSONString.json");
        }
    }
}
