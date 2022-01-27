using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BLL
{
    /*
        SOURCES:
        https://zcreditwc.docs.apiary.io/#reference
        https://zcreditwc.docs.apiary.io/#introduction/getting-started
        https://zcreditwc.docs.apiary.io/#reference/0/create-webcheckout-session/createsession

        --
        More Info:
        see 'ZCredit > ZCredit WebCheckout API'

        --        
        Payment Settings:
        go to 'ZCredit Dashboard' > 'Settings' > (tab) 'Clearing Settings'
        (הגדרות עסק - הגדרות סליקה)

        --        
        Api_Secret:
        note! aka Token
        1. go to the payment sertings page (see 'Payment Settings')
        2. under 'WebCheckout Settings' > Generate 'Web Checkout ID'

        --
        Api URL:
        note! aka ServerURL
        https://pci.zcredit.co.il/webcheckout/api

        Endpoints:
        {{ServerURL}}WebCheckout/CreateSession/

        --
        Supported Features: 
        to activate these features, check them on the dashboard's Settings page 
        - GooglePay
        - ApplePay
        - Bit

        --
        PROCESS:        
        1. Create a new session (Payment-URL)
           - Define the SuccessUrl and CallbackUrl Urls
        2. Use the SuccessUrl as a ThankYou page after purchase.
        3. Use the CallbackUrl for the IPN from ZCredit. this should be a server handler to process the transaction.
        4. Create a Listener to receive the IPNs comming from ZCredit servers.
           - Store the transaction details 
           - Supply the item/s to the custoemr and save a receipt if needed.

        --
        Languages:
        use the payload property 'Local' to set the page language
        - He = Hebrew
        - En = English
        - Ru = Russian

        Currencies:
        - ILS
        - USD
        - EUR

        --
        IPN:
        - SessionId
        - ReferenceNumber
        - HolderId
        - Total
        - Currency
        - Installments
        - CardNum
        - CardName
        - UniqueID
        - Token
        - ApprovalNumber
        - CustomerName
        - CustomerPhone
        - CustomerEmail
        - ExpDate_MMYY
        - FirstInstallentAmount
        - OtherInstallmentsAmount
        - CreditType
        - J
        - CardIssuerCode
        - CardFinancerCode
        - CardBrandCode
        - VoucherNumber
        - InvoiceRecieptDocumentNumber
        - InvoiceRecieptNumber
        
        --
        USING:
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        var response = new ZCreditManager(
                "xxxxxxxxxxxxxxxxxxxxxxxx", 
                "https://rcbuilder.free.beeceptor.com", 
                "https://rcbuilder.free.beeceptor.com"
            ).CreateSession(new ZCreditCreateSessionRequest {
            AdditionalText = "some extra details",
            UniqueId = "1001",
            CreateInvoice = false,
            Customer = new ZCreditCustomer {
                Email = "RcBuilder@walla.com",
                Name = "RcBuilder"
            },
            Installments = new ZCreditInstallments {
                Type = eZCreditInstallmentsType.regular,
                MinQuantity = 1,
                MaxQuantity = 5
            },
            CartItems = new List<ZCreditCartItem> {
                new ZCreditCartItem {
                    Amount = 10,
                    Description = "bla bla bla",
                    Name = "Item 1",
                    Quantity = 1
                }
            }
        });

        if (response.HasError)
            Debug.WriteLine($"[ERROR] {response.Data?.ReturnMessage}");
        else 
            Debug.WriteLine(response.Data.SessionUrl);
        -
        <iframe class="zcredit-frame" src="@SessionUrl" frameborder="0" scrolling="no"></iframe>        
        -
        [HttpPost]
        public string ZCreditIPNHandler(BLL.IPNRequest Request)
        {            
            try {                
                BLL.Logs.Add(JsonConvert.SerializeObject(Request));
            }
            catch { }        
            
            // CODE HERE ...

            return "OK";
        }
        -
        // PayThanksFrame.cshtml
        // use frame to refresh parent (must be at the same domain to enforce same-domain policy)

        @{ Layout = null; }

        <!DOCTYPE html>
        <html>
        <head>    
            <title>סיום רכישה</title>    
            <script>
                parent.location.href = '/Cart/PayThanks';
            </script>
        </head>
        <body></body>
        </html>        
    */

    #region Entities
    public class ZCreditCreateSessionRequest {        
        public string UniqueId { get; set; }
        public bool CreateInvoice { get; set; }
        public string AdditionalText { get; set; }
        public bool ShowCart { get; set; }
        public ZCreditInstallments Installments { get; set; }
        public ZCreditCustomer Customer { get; set; }
        public IEnumerable<ZCreditCartItem> CartItems { get; set; } = new List<ZCreditCartItem>();
    }

    public class ZCreditCreateSessionResponse {
        public bool HasError { get; set; }
        public ZCreditCreateSessionResponseData Data { get; set; }
    }

    public class ZCreditCreateSessionResponseData
    {
        public bool HasError { get; set; }        
        public int ReturnCode { get; set; }   
        public string ReturnMessage { get; set; }      
        public string SessionId { get; set; }
        public string SessionUrl { get; set; }
    }
    
    public enum eZCreditLanguage { He, En, Ru }
    public enum eZCreditCurrency { ILS, USD, EUR }
    public enum eZCreditPaymentType { regular, authorize, validate } // case sensitive 
    public enum eZCreditInstallmentsType { none, regular, credit } // case sensitive
    public enum eZCreditCreditType { Regular = 1, Installments = 8, CreditInstallments = 3 }
    public enum eZCreditTransactionCharge { Regular = 0, Validate = 2, Authorize = 5 }
    public enum eZCreditCardCompany { InternationalCard, Isracard, VisaCAL, Diners, Amex, JCB, Max } // see 'Credit Card Companies'
    public enum eZCreditCardBrand{ PrivateLabelBrand, Mastercard, Visa, Diners, Amex, Isracard, JCB, Discover, Maestro } // see 'Credit Card Brands'

    // visa payments (תשלומים)
    public class ZCreditInstallments  
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public eZCreditInstallmentsType Type { get; set; } = eZCreditInstallmentsType.none;
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }

    public class ZCreditCustomer
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ZCreditCartItem
    {
        public float Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public eZCreditCurrency Currency { get; set; } = eZCreditCurrency.ILS;
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool IsTaxFree { get; set; }
    }

    public class ZCreditSession : ZCreditCreateSessionRequest
    {
        public string Key { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public eZCreditLanguage Local { get; set; } = eZCreditLanguage.He;
        [JsonConverter(typeof(StringEnumConverter))]
        public eZCreditPaymentType PaymentType { get; set; } = eZCreditPaymentType.regular;
        public string SuccessUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string CancelUrl { get; set; }
               
        public static ZCreditSession Clone(ZCreditCreateSessionRequest Source)
        {
            var serialized = JsonConvert.SerializeObject(Source);
            var clone = JsonConvert.DeserializeObject<ZCreditSession>(serialized);
            return clone;
        }
    }

    public class IPNRequest
    {
        public string SessionId { get; set; }
        public string ReferenceNumber { get; set; }
        public string UniqueID { get; set; }
        public string Token { get; set; }
        public string ApprovalNumber { get; set; }
        public string VoucherNumber { get; set; }
        public int InvoiceRecieptDocumentNumber { get; set; }
        public int InvoiceRecieptNumber { get; set; }

        public decimal Total { get; set; }
        public eZCreditCurrency Currency { get; set; }
        public int Installments { get; set; }
        public float FirstInstallentAmount { get; set; }
        public float OtherInstallmentsAmount { get; set; }

        public string HolderId { get; set; } // TZ

        [JsonProperty(PropertyName = "CardNum")]
        public string CardSuffix { get; set; }
        public string CardName { get; set; }
        public eZCreditCardCompany CardIssuerCode { get; set; }
        public eZCreditCardCompany CardFinancerCode { get; set; }
        public eZCreditCardBrand CardBrandCode { get; set; }

        [JsonProperty(PropertyName = "ExpDate_MMYY")]
        public string CardExpiry { get; set; } // MM/YY

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }    
                                       
        [JsonProperty(PropertyName = "J")]    
        public eZCreditTransactionCharge TransactionCharge { get; set; }
        public eZCreditCreditType CreditType { get; set; }
    }

    #endregion

    public class ZCreditManager
    {
        private const string SERVER = "https://pci.zcredit.co.il/webcheckout/api";
        private static readonly string CREATE_SESSION_ENDPOINT = $"{SERVER}/WebCheckout/CreateSession/";
        
        protected string Token { get; set; }          
        protected string NotifyURL { get; set; }
        protected string SuccessURL { get; set; }  
        protected string CancelURL { get; set; }                  

        public ZCreditManager(string Token, string NotifyURL = "", string SuccessURL = "", string CancelURL = "") {            
            this.Token = Token;            

            this.NotifyURL = NotifyURL;
            this.SuccessURL = SuccessURL;
            this.CancelURL = CancelURL;           
        }

        #region CreateSession        
        /*
            POST {{ServerURL}}/WebCheckout/CreateSession/
            H Content-Type: application/json
            {
                "Key": "{{Token}}",
                "Local": "He",
                "UniqueId": "#100",
                "SuccessUrl": "{{SuccessUrl}}",
                "CallbackUrl": "{{CallbackUrl}}",
                "PaymentType": "regular",
                "CreateInvoice": false,
                "AdditionalText": "some extra details",
                "ShowCart": true,
                "Installments": {
                    "Type": "regular",
                    "MinQuantity": 1,
                    "MaxQuantity": 5
                },
                "Customer": {
                    "Email": "RcBuilder@walla.com",
                    "Name": "RcBuilder",
                    "PhoneNumber": ""
                },
                "CartItems": [
                    {
                        "Amount": 10,
                        "Currency": "ILS",
                        "Name": "Item 1",
                        "Description": "bla bla bla",
                        "Quantity": 1,
                        "IsTaxFree": false
                    }
                ]    
            }
        */
        public ZCreditCreateSessionResponse CreateSession(ZCreditCreateSessionRequest Request) 
        {
            try
            {
                var session = ZCreditSession.Clone(Request);
                session.Key = this.Token;
                session.CallbackUrl = this.NotifyURL;
                session.SuccessUrl = this.SuccessURL;                
                session.CancelUrl = this.CancelURL;

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;

                    client.Headers.Clear();
                    client.Headers.Add("Accept", "*/*");                    
                    client.Headers.Add("Content-Type", "application/json");

                    var response = client.UploadString(CREATE_SESSION_ENDPOINT, JsonConvert.SerializeObject(session));
                    return JsonConvert.DeserializeObject<ZCreditCreateSessionResponse>(response);
                }
            }
            catch (Exception Ex) {
                return new ZCreditCreateSessionResponse
                {

                };
            }
        }
        #endregion

        #region ParseIPN
        /*
            {   
                    "SessionId": "333a569cf6a95e294d55d36fc33172",     
                    "ReferenceNumber": "12345679",     
                    "HolderId": "123456780",            
                    "Total": 10,                         
                    "Currency": "ILS",                   
                    "Installments": 3,                
                    "CardNum": "9001",                   
                    "CardName": "ויזה רגיל",           
                    "UniqueID": "A23fJKS23443",         
                    "Token": "b12163d2-d1a2-4cf9-b71c-1151825b450a",    
                    "ApprovalNumber": "0543140",      
                    "CustomerName": "43434",            
                    "CustomerPhone": "",              
                    "CustomerEmail": "",                
                    "ExpDate_MMYY": "07/20",             
                    "FirstInstallentAmount": 3.34,       
                    "OtherInstallmentsAmount": 3.33,      
                    "CreditType": 2,                    
                    "J": 0,                            
                    "CardIssuerCode": "2",               
                    "CardFinancerCode": "1",             
                    "CardBrandCode": "2",                
                    "VoucherNumber": "31001037",      
                    "InvoiceRecieptDocumentNumber": 1718,
                    "InvoiceRecieptNumber": 12
            }    
        */
        public IPNRequest ParseIPN(HttpRequestBase Request) {
            /*
            var payload = "";
            using (var stream = Request.Content.ReadAsStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(stream))
                    payload = sr.ReadToEnd();
            }
            */
            return null;
        }
        #endregion
    }
}
