using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeComm
{
    #region Entities
    public enum eOrderType { PICKUP, TABLE, DELIVERY }
    public enum ePaymentType { CARD = 1, CASH = 2, EXTERNAL = 6, SODEXO = 10, BIS10 = 12, GOODI = 27 } 

    public class Customer
    {
        [JsonProperty(PropertyName = "customerName")]
        public string CustomerName { get; set; }

        [JsonProperty(PropertyName = "branchName")]
        public string BranchName { get; set; }

        [JsonProperty(PropertyName = "branchId")]
        public string CustomerId { get; set; }
    }

    public class Order
    {
        [JsonProperty(PropertyName = "branchId")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "orderInfo")]
        public OrderDetails Details { get; set; }        
    }

    public class OrderDetails {
        [JsonProperty(PropertyName = "orderType")]
        public eOrderType OrderType { get; set; } = eOrderType.PICKUP;

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "discountSum")]
        public float Discount { get; set; }

        [JsonProperty(PropertyName = "outerCompId")]
        public int SenderCompanyId { get; set; }

        [JsonProperty(PropertyName = "outerCompOrderId")]
        public string SenderOrderId { get; set; }

        [JsonProperty(PropertyName = "dinners")]
        public int Dinners { get; set; } = 1;

        [JsonProperty(PropertyName = "arrivalTime")]
        public string ArrivalTime { get; set; }  // dd/MM/yyyy HH:mm:ss

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
       
        [JsonProperty(PropertyName = "items")]
        public IEnumerable<OrderItem> Items { get; set; }

        [JsonProperty(PropertyName = "payments")]
        public IEnumerable<OrderPayment> Payments { get; set; }

        ///[JsonProperty(PropertyName = "deliveryInfo")]
        ///public OrderDeliveryInfo DeliveryInfo { get; set; }
    }

    public class OrderItem {
        [JsonProperty(PropertyName = "netId")]  // Item Id (Business) 
        public int ItemId { get; set; }

        [JsonProperty(PropertyName = "itemName")]
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public float Quantity { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "unitPrice")]
        public float UnitPrice { get; set; }

        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "belongTo")]
        public string BelongTo { get; set; }

        [JsonProperty(PropertyName = "billRemarks")]
        public string BillRemarks { get; set; }       
    }

    public class OrderPayment {

        public ePaymentType PaymentType { get; set; } = ePaymentType.EXTERNAL;

        [JsonProperty(PropertyName = "paymentSum")]
        public float Amount { get; set; }

        [JsonProperty(PropertyName = "paymentName")]
        public string PaymentName { get; set; } // קרדקום

        [JsonProperty(PropertyName = "creditCard")]
        public string CreditCardNumber { get; set; }

        [JsonProperty(PropertyName = "creditCardTokef")] 
        public string CreditCardExpiry { get; set; }

        [JsonProperty(PropertyName = "creditCardCvv")]
        public string CreditCardCvv { get; set; }

        [JsonProperty(PropertyName = "creditCardHolderID")]
        public string CreditCardHolderID { get; set; }

        [JsonProperty(PropertyName = "paymentRemark")]
        public string PaymentRemark { get; set; }
    }

    public class OrderDeliveryInfo {
        [JsonProperty(PropertyName = "deliveryCost")]
        public float DeliveryCost { get; set; }

        [JsonProperty(PropertyName = "deliveryRemarks")]
        public string DeliveryRemarks { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "homeNum")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "apartment")]
        public string Apartment { get; set; }

        [JsonProperty(PropertyName = "floor")]
        public string Floor { get; set; }

        [JsonProperty(PropertyName = "companyName")]
        public string CompanyName { get; set; }
    }



    public class OrderResponse
    {
        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "orderCenterId")]
        public string OrderId { get; set; }
    }

    public class OrderStatus
    {
        [JsonProperty(PropertyName = "orderCenterId")]
        public string OrderId { get; set; }

        public OrderStatus(string OrderId) {
            this.OrderId = OrderId;
        }
    }

    public class OrderStatusResponse
    {
        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }

        [JsonProperty(PropertyName = "posOrderId")]
        public int PosOrderId { get; set; }
        
        [JsonProperty(PropertyName = "posError")]
        public OrderStatusError PosError { get; set; }        
    }

    public class OrderStatusError { 
        [JsonProperty(PropertyName = "posError")]
        public string Error { get; set; }
    }
    #endregion

    public class BeeCommManager
    {
        private const string BASE_URL = "https://biapp.beecomm.co.il:8094";
        private const string VERSION = "v2";

        private static readonly string SERVER_URL = $"{BASE_URL}/api/{VERSION}";
        private static readonly string OAUTH_URL = $"{BASE_URL}/{VERSION}/oauth/token";

        protected string ClientId { get; set; }
        protected string ClientSecret { get; set; }
        protected HttpServiceHelper HttpService { get; set; }
        protected string AccessToken { get; set; } = "";

        public BeeCommManager(string ClientId, string ClientSecret)
        {
            this.ClientId = ClientId;
            this.ClientSecret = ClientSecret;
            this.HttpService = new HttpServiceHelper();
        }

        public async Task<bool> GenerateToken()
        {
            var response = await this.HttpService.POST_DATA_ASYNC($"{OAUTH_URL}", new List<string> {
                $"client_id={this.ClientId}",
                $"client_secret={this.ClientSecret}"
            }, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/x-www-form-urlencoded",
                ["Accept"] = "application/json"
            });

            if (!response.Success)
                return false;

            var modelSchema = new
            {
                result = false,
                message = string.Empty,
                access_token = string.Empty
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            this.AccessToken = responseData.access_token;            
            return true;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Customer>>($"{SERVER_URL}/services/orderCenter/customers", headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",                
                ["access_token"] = $"{this.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.GenerateToken();

                response = await this.HttpService.GET_ASYNC<IEnumerable<Customer>>($"{SERVER_URL}/services/orderCenter/customers", headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["access_token"] = $"{this.AccessToken}"
                });
            }

            if (!response.Success)
                throw new Exception(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<OrderStatusResponse> GetOrderStatus(string OrderId)
        {
            var orderStatus = new OrderStatus(OrderId);
            var response = await this.HttpService.POST_ASYNC<OrderStatus, OrderStatusResponse>($"{SERVER_URL}/services/orderCenter/getOrderStatus", orderStatus, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["access_token"] = $"{this.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.GenerateToken();

                response = await this.HttpService.POST_ASYNC<OrderStatus, OrderStatusResponse>($"{SERVER_URL}/services/orderCenter/getOrderStatus", orderStatus, headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["access_token"] = $"{this.AccessToken}"
                });
            }

            if (!response.Success)
                throw new Exception(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<OrderResponse> CreateOrder(Order Order)
        {            
            var response = await this.HttpService.POST_ASYNC<Order, OrderResponse>($"{SERVER_URL}/services/orderCenter/pushOrder", Order, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["access_token"] = $"{this.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.GenerateToken();

                response = await this.HttpService.POST_ASYNC<Order, OrderResponse>($"{SERVER_URL}/services/orderCenter/pushOrder", Order, headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["access_token"] = $"{this.AccessToken}"
                });
            }
            
            if (!response.Success)
                throw new Exception(this.ParseError(response.Content));

            return response.Model;
        }

        // --- 

        private string ParseError(string ErrorRaw)
        {
            if (!ErrorRaw.Contains("|"))
                return ErrorRaw;

            /*
                The remote server returned an error: (401)Unauthorized.|
                {
                    "status": 401,
                    "message": "token expired, please refresh your token."
                }
            */

            var errorRawParts = ErrorRaw.Split('|');

            var errorSchema = new
            {
                status = 0,
                message = string.Empty 
            };

            var exData = JsonConvert.DeserializeAnonymousType(errorRawParts[1], errorSchema);
            return $"{errorRawParts[0].Trim()} ({exData.message})";            
        }
    }
}