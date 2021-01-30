using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BLL
{
    /*
        API:
        https://github.com/binance/binance-spot-api-docs/blob/master/rest-api.md
    */

    namespace BinanceConnectorEntities 
    {
        public class RateLimit
        {
            public string rateLimitType { get; set; }
            public string interval { get; set; }
            public int intervalNum { get; set; }
            public int limit { get; set; }
        };

        public class Filter
        {
            public string filterType { get; set; }
            public decimal minPrice { get; set; }
            public decimal maxPrice { get; set; }
            public decimal tickSize { get; set; }
        };

        public class ExchangeFilter
        {
            public string filterType { get; set; }
            public int maxNumOrders { get; set; }
        };

        public class Symbol
        {
            public string symbol { get; set; }
            public string status { get; set; }
            public string baseAsset { get; set; }
            public int baseAssetPrecision { get; set; }
            public string quoteAsset { get; set; }
            public int quoteAssetPrecision { get; set; }
            public int baseCommissionPrecision { get; set; }
            public int quoteCommissionPrecision { get; set; }
            public List<string> orderTypes { get; set; }
            public bool icebergAllowed { get; set; }
            public bool ocoAllowed { get; set; }
            public bool quoteOrderQtyMarketAllowed { get; set; }
            public bool isSpotTradingAllowed { get; set; }
            public bool isMarginTradingAllowed { get; set; }
            public List<Filter> filters { get; set; }
            public List<string> permissions { get; set; }
        };


        public class ExchangeInfo {
            public string timezone { get; set; }
            public long serverTime { get; set; }
            public List<RateLimit> rateLimits { get; set; }
            public List<ExchangeFilter> exchangeFilters { get; set; }
            public List<Symbol> symbols { get; set; }
        }
    };

    public class BinanceConnector : IPlatformConnector
    {        
        private const string SERVER = "https://api.binance.com/api/v3/";
        private readonly string API_KEY;
        private readonly string SECRET_KEY;
        private IHttpServiceHelper HttpService { get; set; } = new HttpServiceHelper();

        public BinanceConnector(string API_KEY, string SECRET_KEY) {
            this.API_KEY = API_KEY;
            this.SECRET_KEY = SECRET_KEY;
        }

        // ---

        public long BUY(TradeCommand Command) 
        {
            var formVariables = new List<string> {
                    $"symbol={Command.Symbol}",
                    "side=BUY",     // SELL, BUY
                    "type=MARKET",   // LIMIT, MARKET, STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, TAKE_PROFIT_LIMIT, LIMIT_MAKER
                    $"quantity={Math.Round(Command.Quantity, 4)}",
                    ///$"price={Math.Round(Command.Price, 4)}",
                    ///"timeInForce=GTC",
                    "recvWindow=60000",                    
                    $"timestamp={ToUnixTimeStamp(DateTime.UtcNow)}"
                };

            // sign the request
            var signature = this.SignThis(SECRET_KEY, string.Join("&", formVariables)).signature;
            formVariables.Add($"signature={signature}");

            var response = this.HttpService.POST_DATA($"{SERVER}order", formVariables, headers: new Dictionary<string, string>
                {
                    { "X-MBX-APIKEY", API_KEY },
                    { "Content-Type", "application/x-www-form-urlencoded" }
                });

            var schema = new
            {
                orderId = 0L,
                clientOrderId = "",
                transactTime = 0L,
                status = "",
                type = "",
                side = ""
            };

            if (!response.Success)
                throw new Exception(response.Content);

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);
            return model.orderId;
        }

        public long SELL(TradeCommand Command)
        {
            var formVariables = new List<string> {
                    $"symbol={Command.Symbol}",
                    "side=SELL",     // SELL, BUY
                    "type=MARKET",   // LIMIT, MARKET, STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, TAKE_PROFIT_LIMIT, LIMIT_MAKER
                    $"quantity={Math.Round(Command.Quantity, 4)}",
                    ///$"price={Math.Round(Command.Price, 4)}",
                    ///"timeInForce=GTC",
                    "recvWindow=60000",                    
                    $"timestamp={ToUnixTimeStamp(DateTime.UtcNow)}"
                };

            if(Command.OrderId > 0)
                formVariables.Add($"newClientOrderId={Command.OrderId}");

            // sign the request
            var signature = this.SignThis(SECRET_KEY, string.Join("&", formVariables)).signature;
            formVariables.Add($"signature={signature}");

            var response = this.HttpService.POST_DATA($"{SERVER}order", formVariables, headers: new Dictionary<string, string>
                {
                    { "X-MBX-APIKEY", API_KEY },
                    { "Content-Type", "application/x-www-form-urlencoded" }
                });

            var schema = new
            {
                orderId = 0L,
                clientOrderId = "",
                transactTime = 0L,
                status = "",
                type = "",
                side = ""
            };

            if (!response.Success)
                throw new Exception(response.Content);

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);
            return model.orderId;
        }

        public decimal GET_PRICE(string Symbol) {
            var response = this.HttpService.GET($"{SERVER}ticker/price", $"symbol={Symbol}");

            var schema = new
            {
                symbol = "",
                price = 0.0M
            };

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);
            return model.price;
        }

        [Obsolete("USE GET_PRICE INSTEAD")]
        public (decimal AvgUnitPrice, decimal MinUnitPrice) GET_INFO(string Symbol) {
            /*
                    // symbol list
                    var temp = this.ExchangeInfo();
                    foreach (var s in temp.symbols)
                        File.AppendAllText("D:\\symbols.txt", s.symbol + "\n");
                 */

            var response = this.HttpService.GET($"{SERVER}avgPrice", $"symbol={Symbol}");

            var schema = new
            {
                mins = 0,
                price = 0.0M
            };

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);

            // info.symbols.Find(x => x.symbol == Symbol).filters.Find(x => x.filterType == "PRICE_FILTER")
            // info.symbols.Find(x => x.symbol == Symbol).filters.Find(x => x.filterType == "PERCENT_PRICE")
            var info = this.ExchangeInfo();
            var priceRange = info.symbols.First(x => x.symbol == Symbol).filters.FirstOrDefault(x => x.filterType == "PRICE_FILTER");

            return (model.price, priceRange?.minPrice ?? 0);
        }

        public IEnumerable<long> FIND_ORDERS(string Symbol, eTradeCommandType CommandType, string Status = "NEW") {
            var query = $"symbol={Symbol}&timestamp={ToUnixTimeStamp(DateTime.UtcNow)}";                

            // sign the request
            var signature = this.SignThis(SECRET_KEY, query).signature;
            query += $"&signature={signature}";

            var response = this.HttpService.GET($"{SERVER}allOrders", query, headers: new Dictionary<string, string>
                {
                    { "X-MBX-APIKEY", API_KEY },
                    { "Content-Type", "application/x-www-form-urlencoded" }
                });

            var itemSchema = new {
                symbol = "",
                orderId = 0L,
                orderListId = 0L,
                clientOrderId = "",
                price = 0.0M,
                origQty = 0.0M,
                executedQty = 0.0M,
                cummulativeQuoteQty = 0.0M,
                status = "",    // see 'Order status'
                timeInForce = "",
                type = "",      // LIMIT, MARKET, STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, TAKE_PROFIT_LIMIT, LIMIT_MAKER
                side = "",      // SELL, BUY 
                stopPrice = 0.0M,
                icebergQty = 0.0M,
                time = 0L,
                updateTime = 0L,
                isWorking = false,
                origQuoteOrderQty = 0.0M
            };

            var schema = new[] { itemSchema };

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);
            return model.Where(x => x.side == CommandType.ToString() & string.Equals(x.status, Status, StringComparison.OrdinalIgnoreCase)).Select(x => x.orderId);
        }

        public IEnumerable<long> OPEN_ORDERS(string Symbol, eTradeCommandType CommandType)
        {
            var query = $"symbol={Symbol}&timestamp={ToUnixTimeStamp(DateTime.UtcNow)}";

            // sign the request
            var signature = this.SignThis(SECRET_KEY, query).signature;
            query += $"&signature={signature}";

            var response = this.HttpService.GET($"{SERVER}openOrders", query, headers: new Dictionary<string, string>
                {
                    { "X-MBX-APIKEY", API_KEY },
                    { "Content-Type", "application/x-www-form-urlencoded" }
                });

            var itemSchema = new
            {
                symbol = "",
                orderId = 0L,
                orderListId = 0L,
                clientOrderId = "",
                price = 0.0M,
                origQty = 0.0M,
                executedQty = 0.0M,
                cummulativeQuoteQty = 0.0M,
                status = "",    // see 'Order status'
                timeInForce = "",
                type = "",      // LIMIT, MARKET, STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, TAKE_PROFIT_LIMIT, LIMIT_MAKER
                side = "",      // SELL, BUY 
                stopPrice = 0.0M,
                icebergQty = 0.0M,
                time = 0L,
                updateTime = 0L,
                isWorking = false,
                origQuoteOrderQty = 0.0M
            };

            var schema = new[] { itemSchema };

            var model = JsonConvert.DeserializeAnonymousType(response.Content, schema);
            return model.Where(x => x.side == CommandType.ToString()).Select(x => x.orderId);
        }

        // --

        private BinanceConnectorEntities.ExchangeInfo ExchangeInfo() {
            var response = this.HttpService.GET($"{SERVER}exchangeInfo");

            #region Anonymous Schema:
            /*
                var rateLimitSchema = new {
                    rateLimitType = "",
                    interval = "",
                    intervalNum = 0,
                    limit = 0
                };

                var filterSchema = new {
                    filterType = "",
                    minPrice = 0.0,
                    maxPrice = 0.0,
                    tickSize = 0.0
                };

                var exchangeFilterSchema = new
                {
                    filterType = "",
                    maxNumOrders = 0
                };

                var symbolSchema = new {
                    symbol = "",
                    status = "",
                    baseAsset = "",
                    baseAssetPrecision = 0,
                    quoteAsset = "",                    
                    quoteAssetPrecision = 0,
                    baseCommissionPrecision = 0,
                    quoteCommissionPrecision = 0,
                    orderTypes = new List<string>(),
                    icebergAllowed = true,
                    ocoAllowed = true,
                    quoteOrderQtyMarketAllowed = true,
                    isSpotTradingAllowed = true,
                    isMarginTradingAllowed = true,
                    filters = new[] { filterSchema },
                    permissions = new List<string>()
                };

                var schema = new
                {
                    timezone = "",
                    serverTime = 0.0,
                    rateLimits = new[] { rateLimitSchema },
                    exchangeFilters = new[] { exchangeFilterSchema },
                    symbols = new[] { symbolSchema }
                }; 
            */
            #endregion

            var model = JsonConvert.DeserializeObject<BinanceConnectorEntities.ExchangeInfo>(response.Content);
            return model;
        }

        // ---

        private (string signature, string signatureBase64) SignThis(string Key, string Value)
        {
            using (var hmacSha256 = new HMACSHA256(Encoding.ASCII.GetBytes(Key)))
            {
                byte[] dataToHmac = Encoding.ASCII.GetBytes(Value);
                var hash = hmacSha256.ComputeHash(dataToHmac);
                var signature = HashEncode(hash);
                return (signature, Convert.ToBase64String(hash));
            }
        }

        private string HashEncode(byte[] hash) {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static string GetTimestamp(DateTime value) {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        private long ToUnixTimeStamp(DateTime date) {
            DateTime point = new DateTime(1970, 1, 1);
            TimeSpan time = date.Subtract(point);

            return (long)time.TotalMilliseconds;
        }
    }
}
