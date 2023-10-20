using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using static TaxesIL.TaxesILEntities;
using Helpers;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;

/*
    REFERENCE
    ---------
    https://openapi-portal.taxes.gov.il/sandbox/product
    https://openapi-portal.taxes.gov.il/sandbox/product/2927/api/1533#/Invoices_v1/overview
    https://openapi-portal.taxes.gov.il/sandbox/product/1776/api/199#/longtimeacces_100/overview


    SUPPORT
    -------
    OpenAPI@taxes.gov.il
    invoices@taxes.gov.il
    lakohot-bt@taxes.gov.il


    DEMO (SOURCES)
    --------------
    https://github.com/ISRTaxesOpenAPI/nodeJSExample
    https://github.com/ISRTaxesOpenAPI/-postmanExample


    PROCESS
    -------
    (steps)
    1. Client Registration        
    2. Developer Registration
    3. CREATE AN APP
    4. ENABLE SERVICES FOR API APP    


    CLIENT REGISTRATION
    -------------------
    https://secapp.taxes.gov.il/srRishum/main/openPage
    https://www.gov.il/he/service/connect-to-shaam

    * register & wait for approval 
    * login the portal
    * add organization 
    * invite developers  
    * create an api app 
    * get the generated key & secret 
    * enable services for api app (demoApp + Invoices) 


    DEVELOPER REGISTRATION
    ----------------------
    * after a client send a developer invitation (see 'Client Registration > invite developers')
    * an email with token will be sent to the developer 
    * need to click and sign-up (if you haven't singed up already) with your id  
    * after the registration, you could find your clients organizations in the taxes api portal

    note! 
    this phase connect the client account with the developer account.


    CREATE AN APP
    -------------
    * login into the taxes portal 
    * choose organization 
    * click on 'Apps' > Create
    * set app name, desc and oauth return uri
    

    ENABLE SERVICES FOR API APP
    ---------------------------
    * login into the taxes portal 
    * choose organization 
    * click on 'API Products' > choose API's to enable > choose plan > save

    note! 
    we can find all enabled APIs under Subscriptions tab in the app page


    INVITE A DEVELOPER
    ------------------
    1. login to the Portal 
    2. click on your company name (top-right corner)
    3. My Organization > Invite 
    4. set email and Role (admin, developer or viewer)


    CLIENT PORTAL
    -------------
    * SANDBOX 
      https://openapi-portal.taxes.gov.il/sandbox

    * PROD
      https://openapi-portal.taxes.gov.il/shaam/production

    note! 
    must be registered to the sandbox prior the prod.  


    SERVICE TYPES
    -------------
    * Open Access (public - no token required) 
    * One Time (requires a token)
    * Long Time (requires a token)


    POSTMAN
    -------
    Taxes API.postman_collection.json


    IMPLEMENTATIONS
    ---------------
    - see 'CODE > TaxesILManager.cs'
    - see 'Creative > TaxesIL'    
    - see 'DocSee > TaxesIL'    

    
    RESEARCH
    --------
    - see 'Scripts > TaxesIL API'


    cURL
    ----
    * AUTHORIZATION CODE:
    
      GET https://openapi.taxes.gov.il/shaam/Tsandbox/longtimetoken/oauth2/authorize

      query params:
      response_type=code
      client_id=<client-id>
      scope=<scope>

    -

    * TOKEN:
    
      POST https://openapi.taxes.gov.il/shaam/Tsandbox/longtimetoken/oauth2/token
      H Authorization: basic base64(clientid:clientsecret)
      H Content-type: application/x-www-form-urlencoded
      
      body params:
      grant_type=authorization_code
      code=<authorizationcode>
      redirect_uri=<redirecturi>
      scope=<scope>


    USING
    -----
    var taxesILConfig = new TaxesILConfig
    {
        ServerURL = "https://openapi.taxes.gov.il/shaam/tsandbox",
        AuthorizeURL = "https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2",
        ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        ApiSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"                    
    };

    // optional
    taxesILConfig.RefreshToken = "AAI1YVCbCLP4CUyWtrL1jiXgEiaIB8CaF-Ima4EnOd0XWpK5URJqPLbnH7LAGqIYYjubIDAVPLj2-QfCRLTMuLAm9zEHhvKwmuoyz8mfClvPUw";
    taxesILConfig.RedirectURI = "http://localhost:9999";

    var taxesILManager = new TaxesILManager(taxesILConfig);

    taxesILManager.TokensUpdated += (sender, eventArgs) => {
        Console.WriteLine($"Tokens Updated: {eventArgs}");
    };            

    /// await taxesILManager.Authorize("xxxxxxxxxxxx");            
    await taxesILManager.RefreshToken();

    var healthResult = await taxesILManager.HealthCheck();
    Console.WriteLine(healthResult); // OK

    var invoiceToCreate = JsonConvert.DeserializeObject<Invoice>(File.ReadAllText($"{AppContext.BaseDirectory}invoice-sample.json", Encoding.UTF8));
    var created = await taxesILManager.CreateInvoice(invoiceToCreate);
    Console.WriteLine(created);

    var details = await taxesILManager.GetInvoiceDetails(new GetInvoiceDetailsRequest { 
        ConfirmationNumber = "20231009105110808282010549",
        CustomerVATNumber = 777777715,
        VatNumber = 18
    });
    Console.WriteLine(details.InvoiceId);

    --
    
    // OAuth
    // process oAuth using browser & http-listener on localhost:9999
    var taxesILManager = new TaxesILManager(taxesILConfig);
    var code = taxesILManager.RequestAuthorizaionCode(9999);
    await taxesILManager.Authorize(code);

    Console.WriteLine("DONE");
    Console.ReadKey();
    return;

    --

    startOAuth:
    // redirect to oauth process - get AuthorizaionCode
    taxesILManager.RequestAuthorizaionCode(HttpContext.Current);

    callback:
    await taxesILManager.Authorize(Request.QueryString["code"]);
    
*/

namespace TaxesIL
{
    public class TaxesILEntities
    {
        public class TaxesILConfig 
        {
            public string ServerURL { get; set; }
            public string AuthorizeURL { get; set; }            
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }

            public string RedirectURI { get; set; }

            // OPTIONAL - init tokens
            public string RefreshToken { get; set; }
            public string AccessToken { get; set; }
        }

        public class TokensData
        {
            [JsonProperty(PropertyName = "token_type")]
            public string TokenType { get; set; }

            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }

            [JsonProperty(PropertyName = "scope")]
            public string Scope { get; set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty(PropertyName = "refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty(PropertyName = "refresh_token_expires_in")]
            public int RefreshTokenExpiresIn { get; set; }            
        }

        public class APIException : Exception
        {
            public APIErrorResponse ErrorResponse { get; protected set; }
            public APIException(APIErrorResponse ErrorResponse) : base(ErrorResponse.Message)
            {
                this.ErrorResponse = ErrorResponse;
            }

            public override string ToString()
            {
                return $"{this.ErrorResponse}";
            }
        }

        public class APIErrorResponse
        {
            public string Message { get; set; }
            public (string Error, string Details) InnerMessage { get; set; }

            public override string ToString()
            {
                return $"{this.Message} | {this.InnerMessage.Error} | {this.InnerMessage.Details}";
            }
        }

        public class TokensUpdatedEventArgs : EventArgs
        {
            public string ApiKey { get; set; }
            public string RefreshToken { get; set; }
            public string AccessToken { get; set; }

            public TokensUpdatedEventArgs(string ApiKey, string RefreshToken, string AccessToken)
            {
                this.ApiKey = ApiKey;
                this.RefreshToken = RefreshToken;
                this.AccessToken = AccessToken;
            }

            public override string ToString()
            {
                return $"{this.ApiKey} | {this.RefreshToken} | {this.AccessToken}";
            }
        }

        public class APIResponse<T>
        {
            public string Status { get; set; }
            public T Message { get; set; }
        }

        public class CreateInvoiceRequest {}

        public class Invoice : CreateInvoiceRequest
        {
            [JsonProperty(PropertyName = "Invoice_ID")]
            public string InvoiceId { get; set; }

            [JsonProperty(PropertyName = "Invoice_Type")]
            public int InvoiceType { get; set; }

            [JsonProperty(PropertyName = "Vat_Number")]
            public int VatNumber { get; set; }

            [JsonProperty(PropertyName = "Union_Vat_Number")]
            public int UnionVatNumber { get; set; }

            [JsonProperty(PropertyName = "Invoice_Reference_Number")]
            public string ReferenceNumber { get; set; }

            [JsonProperty(PropertyName = "Customer_VAT_Number")]
            public int CustomerVATNumber { get; set; }

            [JsonProperty(PropertyName = "Customer_Name")]
            public string CustomerName { get; set; }

            [JsonProperty(PropertyName = "Invoice_Date")]
            public string InvoiceDate { get; set; }

            [JsonProperty(PropertyName = "Invoice_Issuance_Date")]
            public string InvoiceIssuanceDate { get; set; }

            [JsonProperty(PropertyName = "Amount_Before_Discount")]
            public double AmountBeforeDiscount { get; set; }
            public double Discount { get; set; }

            [JsonProperty(PropertyName = "Payment_Amount")]
            public double PaymentAmount { get; set; }

            [JsonProperty(PropertyName = "VAT_Amount")]
            public double VATAmount { get; set; }

            [JsonProperty(PropertyName = "Payment_Amount_Including_VAT")]
            public double PaymentAmountIncludingVAT { get; set; }

            [JsonProperty(PropertyName = "Invoice_Note")]
            public string InvoiceNote { get; set; }

            [JsonProperty(PropertyName = "Branch_ID")]
            public string BranchId { get; set; }

            [JsonProperty(PropertyName = "Accounting_Software_Number")]
            public int AccountingSoftwareNumber { get; set; }

            [JsonProperty(PropertyName = "Client_Software_Key")]
            public string ClientSoftwareKey { get; set; }        
            
            public int Action { get; set; }

            [JsonProperty(PropertyName = "Vehicle_License_Number")]
            public int LicenseNumber { get; set; }

            [JsonProperty(PropertyName = "Phone_Of_Driver")]
            public string PhoneOfDriver { get; set; }

            [JsonProperty(PropertyName = "Arrival_Date")]
            public string ArrivalDate { get; set; }

            [JsonProperty(PropertyName = "Estimated_Arrival_Time")]
            public string EstimatedArrivalTime { get; set; }

            [JsonProperty(PropertyName = "Transition_Location")]
            public int TransitionLocation { get; set; }

            [JsonProperty(PropertyName = "Delivery_Address")]
            public string DeliveryAddress { get; set; }
            public List<InvoiceItem> Items { get; set; }
        }

        public class InvoiceItem
        {
            public int Index { get; set; }

            [JsonProperty(PropertyName = "Catalog_ID")]
            public string CatalogId { get; set; }
            public int Category { get; set; }
            public string Description { get; set; }

            [JsonProperty(PropertyName = "Measure_Unit_Description")]
            public string MeasureUnitDescription { get; set; }
            public double Quantity { get; set; }

            [JsonProperty(PropertyName = "Price_Per_Unit")]
            public double UnitPrice { get; set; }
            public double Discount { get; set; }

            [JsonProperty(PropertyName = "Total_Amount")]
            public double Total { get; set; }

            [JsonProperty(PropertyName = "VAT_Rate")]
            public int VATRate { get; set; }

            [JsonProperty(PropertyName = "VAT_Amount")]
            public double VATAmount { get; set; }
        }

        public class GetInvoiceDetailsRequest
        {
            [JsonProperty(PropertyName = "Customer_VAT_Number")]
            public int CustomerVATNumber { get; set; }

            [JsonProperty(PropertyName = "Confirmation_Number")]
            public string ConfirmationNumber { get; set; }

            [JsonProperty(PropertyName = "Vat_Number")]
            public int VatNumber { get; set; }
        }

        public class GetInvoiceDetailsResponse : APIResponse<Invoice> { }

        public class CreateInvoiceResponse
        {            
            public int Status { get; set; }            
            public string Message { get; set; }

            [JsonProperty(PropertyName = "Confirmation_Number")]
            public string Confirmation { get; set; }

            public override string ToString()
            {
                return $"{this.Status} | {this.Message} | {this.Confirmation}";
            }
        }
    }

    public interface ITaxesILManager
    {        
        void RequestAuthorizaionCode(HttpContext Context);
        string RequestAuthorizaionCode(int ListenerPort = 9999);
        Task<bool> Authorize(string AuthorizationCode);
        Task<bool> RefreshToken();

        Task<Invoice> GetInvoiceDetails(GetInvoiceDetailsRequest Request);
        Task<CreateInvoiceResponse> CreateInvoice(Invoice Request);
        Task<string> HealthCheck();
    }

    public class TaxesILManager : ITaxesILManager
    {
        protected static readonly string ASSEMBLY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
        protected TaxesILConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public EventHandler<TokensUpdatedEventArgs> TokensUpdated;

        protected string OAuthURL
        {
            get 
            { 
                var url = $"{this.Config.AuthorizeURL}/authorize?response_type=code&client_id={this.Config.ApiKey}&scope=scope";

                if (!string.IsNullOrEmpty(this.Config.RedirectURI)) 
                    url = $"{url}&redirect_uri={this.Config.RedirectURI}";

                return url;
            }
        }

        public TaxesILManager(TaxesILConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        protected void OnTokensUpdated(TokensUpdatedEventArgs args)
        {
            if (TokensUpdated == null) return;
            TokensUpdated(null, args);
        }

        // redirects the user to the oAuth process 
        // authorization_code
        public void RequestAuthorizaionCode(HttpContext Context) {
            Context.Response.Redirect(this.OAuthURL, true);
        }

        // launches a browser and set an http-listener to monitor the callback response
        // must use localhost as the redirected url 
        // also fits for Console & Desktop apps  
        public string RequestAuthorizaionCode(int ListenerPort = 9999) 
        {
            var authorizaionCode = string.Empty;

            using (var process = Process.Start(this.OAuthURL))
            {
                using (var listener = new HttpListener())
                {
                    // callback 
                    listener.Prefixes.Add($"http://localhost:{ListenerPort}/");
                    listener.Start();

                    //wait for server captures redirect_uri  
                    var context = listener.GetContext();
                    var request = context.Request;

                    var requestedURL = request.RawUrl.TrimStart('/');
                    /// Debug.WriteLine(requestedURL == "" ? "-" : requestedURL);
                    
                    authorizaionCode = request.QueryString.Get("code");
                    Console.WriteLine(authorizaionCode);

                    context.Response.Close();
                    listener.Stop();
                }
            }

            return authorizaionCode;
        }

        /*
            GET https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/authorize?response_type=code&client_id=xxxxxxxx&scope=scope
         
            --

            // error
            {
                "error": "invalid_grant",
                "error_description": "*[xxxxxxxxxxxxx] code expired*"
            }

            // error
            {
                "error": "invalid_request",
                "error_description": "Redirect URI specified in the request is not configured in the client subscription"
            }

            // success
            {
                "token_type": "Bearer",
                "access_token": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                "scope": "scope",
                "expires_in": 601,
                "consented_on": 1696871420,
                "refresh_token": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                "refresh_token_expires_in": 7776000
            } 
        */
        public async Task<bool> Authorize(string AuthorizationCode) 
        {
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.AuthorizeURL}/token", new List<string> {
                "grant_type=authorization_code",
                $"code={AuthorizationCode}",
                "scope=scope",
                $"redirect_uri={this.Config.RedirectURI}",                
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/x-www-form-urlencoded",
                ["Authorization"] = this.HttpService.GenerateBasicAuthorizationValue(this.Config.ApiKey, this.Config.ApiSecret)
            });

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            var tokensData = JsonConvert.DeserializeObject<TokensData>(response.Content);
            this.UpdateTokens(tokensData);

            return true;
        }

        public async Task<bool> RefreshToken()
        {
            /// if (string.IsNullOrEmpty(this.Config.RefreshToken))
                /// throw new Exception("No RefreshToken");

            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.AuthorizeURL}/token", new List<string> {
                "grant_type=refresh_token",
                $"refresh_token={this.Config.RefreshToken}",
                "scope=scope",
                $"client_id={this.Config.ApiKey}",
                $"client_secret={this.Config.ApiSecret}"
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/x-www-form-urlencoded"
            });

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));
         
            var tokensData = JsonConvert.DeserializeObject<TokensData>(response.Content);
            this.UpdateTokens(tokensData);

            return true;
        }
        
        public async Task<Invoice> GetInvoiceDetails(GetInvoiceDetailsRequest Request) 
        {
            var response = await this.HttpService.POST_ASYNC<GetInvoiceDetailsRequest, GetInvoiceDetailsResponse>(
                $"{this.Config.ServerURL}/invoice-information/v1/details",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                }
            );

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model?.Message;
        }

        public async Task<CreateInvoiceResponse> CreateInvoice(Invoice Request) 
        {
            var response = await this.HttpService.POST_ASYNC<Invoice, CreateInvoiceResponse>(
                $"{this.Config.ServerURL}/Invoices/v1/Approval",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                }
            );

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<string> HealthCheck() 
        {
            var response = await this.HttpService.GET_ASYNC(
                $"{this.Config.ServerURL}/Invoices/v1/Health",
                null,                
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",                    
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                }
            );

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Content;
        }

        // ---

        private APIErrorResponse ParseError(string ErrorRaw)
        {
            /*
                // schema
                <Http-Error>|<Request-Error>

                -

                // sample
                The remote server returned an error: (401)Unauthorized.|{
                    "error": "invalid_request",
                    "error_description": "Redirect URI specified in the request is not configured in the client subscription"
                }

                -

                // schema types
                {
                    "httpCode": "401",
                    "httpMessage": "Unauthorized",
                    "moreInformation": "Invalid client id or secret."
                }

                {
                    "error": "invalid_request",
                    "error_description": "Redirect URI specified in the request is not configured in the client subscription"
                }

            */
            var errorRawParts = ErrorRaw.Split('|');
            var httpError = errorRawParts[0];
            var requestError = errorRawParts[1];

            var result = new APIErrorResponse {
                Message = httpError?.Trim()                
            };

            // parse by Schema type
            if (requestError.Contains("error_description"))
            {
                var errorSchema = new
                {
                    error = string.Empty,
                    error_description = string.Empty
                };

                var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
                result.InnerMessage = (
                    exData?.error?.Trim() ?? string.Empty,
                    exData?.error_description?.Trim() ?? string.Empty
                );
            }
            else if (requestError.Contains("httpCode")) 
            {
                var errorSchema = new
                {
                    httpCode = 0,
                    httpMessage = string.Empty,
                    moreInformation = string.Empty
                };

                var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
                result.InnerMessage = (
                    $"{exData?.httpMessage?.Trim() ?? string.Empty} ({exData?.httpCode ?? -1})",
                    exData?.moreInformation?.Trim() ?? string.Empty
                );
            }

            return result;
        }

        private void UpdateTokens(TokensData TokensData) {            
            this.Config.AccessToken = TokensData.AccessToken;
            this.Config.RefreshToken = TokensData.RefreshToken;

            // raise an event
            this.OnTokensUpdated(new TokensUpdatedEventArgs(this.Config.ApiKey, this.Config.RefreshToken, this.Config.AccessToken));
        }
    }
}
