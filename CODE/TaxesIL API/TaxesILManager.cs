﻿using System;
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
 
    Upgrade to V2 Requirement (exp.01/2025)
    1. ID_INVOICE Required
    2. Change in URL - v2 instead of v1
    3. Extra Properties - Invoice model
    4. one of UserId OR UserName is required!
    5. AccountingSoftwareNumber is required!
    6.

    -

    // TODO ->> Implement Multi-invoices service
    Dear Developers,
    We hope this message finds you well. We are writing to inform you of an upcoming change regarding the API endpoint address for our services at the Israel Tax Authority.
    Effective immediately, we are migrating to a new API endpoint address to enhance the efficiency and security of our services. The new API endpoint address will be as follows:
    OLD API Endpoint Address: https://ita-api.taxes.gov.il/shaam/tsandbox/Invoices/v1/MultiApproval
    New API Endpoint Address: https://ita-api.taxes.gov.il/shaam/tsandbox/Multi-invoices/v1/MultiApproval
    Please note that while we are implementing this change, the old API endpoint address will continue to function as usual. However, we strongly encourage you to update your systems and applications to start using the new API endpoint address as soon as possible to ensure uninterrupted access to our services.
    It is important to emphasize that the old API endpoint address will remain operational until April 13, 2024. After this date, it will be deprecated, and all requests made to the old address will no longer be serviced.
    We understand that this change may require adjustments on your end, and we are committed to providing any assistance you may need during this transition period. If you encounter any issues or have any questions regarding the migration process, please do not hesitate to reach out to our support team at [Insert Contact Information].
    We appreciate your cooperation and understanding as we work to improve our services. Thank you for your continued partnership with the Israel Tax Authority.
    Warm regards

    -

    POST {{ServerURL}}/Multi-invoices/v1/MultiApproval
    {
      "Vat_Number": 777777715,
      "Union_Vat_Number": 125847553,
      "Invoices_Amount": 1,
      "Invoices_Payment_Amount": 500,
      "Invoices_Vat_Amount": 85,
      "Invoices_List": [
        {
          "Invoice_ID": "987654321",
          "Invoice_Type": 305,
          "Vat_Number": 777777715,
          "Union_Vat_Number": 125847553,
          "Invoice_Reference_Number": "975626515",
          "Customer_VAT_Number": 18,
          "Customer_Name": "שם הלקוח",
          "Invoice_Date": "2023-04-08",
          "Invoice_Issuance_Date": "2023-04-08",
          "Branch_ID": "533",
          "Accounting_Software_Number": 36955574,
          "Client_Software_Key": "76857",
          "Amount_Before_Discount": 552.75,
          "Discount": 52.75,
          "Payment_Amount": 500,
          "VAT_Amount": 85,
          "Payment_Amount_Including_VAT": 585,
          "Invoice_Note": "הערות",
          "Action": 0,
          "Vehicle_License_Number": 584752145,
          "Phone_Of_Driver": "0505674235",
          "Arrival_Date": "2023-02-26",
          "Estimated_Arrival_Time": "13:25",
          "Transition_Location": 12,
          "Delivery_Address": "כתובת אספקה",
          "Additional_Information": 0,
          "Items": [
            {
              "Index": 7446,
              "Catalog_ID": "5569875437",
              "Category": 15,
              "Description": "תיאור הפריט",
              "Measure_Unit_Description": "קילו",
              "Quantity": 100.5,
              "Price_Per_Unit": 5.5,
              "Discount": 52.75,
              "Total_Amount": 500,
              "VAT_Rate": 17,
              "VAT_Amount": 85
            }
          ]
        }
      ]
    }

    -
    
    // Invoices-v2-portal-sample
    // https://openapi-portal.taxes.gov.il/sandbox/product/19465/api/18168#/Invoices_v2/operation/%2FInvoices%2Fv2%2FApproval/post
    POST {{ServerURL}}/Invoices/v2/Approval
    {
      "invoice_id": "987654321",
      "invoice_type": 305,
      "vat_number": 777777715,
      "union_vat_number": 125847553,
      "authorized_company": 18,
      "user_id": 18,
      "user_name": "שם משתמש",
      "invoice_reference_number": "975626515",
      "customer_vat_number": 18,
      "customer_name": "Curtis Wells",
      "customer_country_code": "UKR",
      "invoice_date": "2024-11-24",
      "invoice_issuance_date": "2024-11-24",
      "branch_id": "533",
      "accounting_software_number": 987654321,
      "client_software_key": "76857",
      "amount_before_discount": 552.75,
      "discount": 52.75,
      "payment_amount": 500,
      "vat_amount": 85,
      "payment_amount_including_vat": 585,
      "invoice_note": "הערות",
      "action": 0,
      "vehicle_license_number": 584752145,
      "phone_of_driver": "0505674235",
      "arrival_date": "2024-11-24",
      "estimated_arrival_time": "13:25",
      "transition_location": 12,
      "delivery_address": "כתובת אספקה",
      "additional_information": 0,
      "additional_information_1": "0",
      "additional_information_2": "0",
      "additional_information_3": 0,
      "items": [
        {
          "index": 999999,
          "catalog_id": "5569875437",
          "category": 15,
          "description": "תיאור הפריט",
          "measure_unit_description": "קילו",
          "quantity": 100.5,
          "price_per_unit": 5.5,
          "discount": 52.75,
          "total_amount": 500,
          "vat_rate": 17,
          "vat_amount": 85
        }
      ]
    }

    ------------------------------------------------------------------------------------



    REFERENCE
    ---------
    https://openapi-portal.taxes.gov.il/sandbox
    https://openapi-portal.taxes.gov.il/sandbox/product    
    https://openapi-portal.taxes.gov.il/sandbox/product/1776/api/199#/longtimeacces_100/overview
    https://openapi-portal.taxes.gov.il/sandbox/product/12366/api/6990#/Invoices_v1/overview
    https://openapi-portal.taxes.gov.il/sandbox/product/12366/api/12360#/multiinvoices_v1/operation/%2FMultiApproval/post

    https://openapi-portal.taxes.gov.il/shaam/production
    https://openapi-portal.taxes.gov.il/shaam/production/product
    https://openapi-portal.taxes.gov.il/shaam/production/forum/3
    https://openapi-portal.taxes.gov.il/sandbox/sites/sandbox.openapi-portal.taxes.gov.il/files/inline-files/portal%20user%20guide.pdf
    https://www.gov.il/BlobFolder/service/connect-to-shaam/he/Service_Pages_shaam_connection-work-process-software-houses.pdf

    https://www.gov.il/he/pages/israel-invoice-160723  
    https://www.gov.il/he/pages/pa181224-1

    SUPPORT
    -------
    OpenAPI@taxes.gov.il
    invoices@taxes.gov.il
    lakohot-bt@taxes.gov.il
    itaOpenApiSupport@taxes.gov.il

    // developers support
    APIsupport@taxes.gov.il
    02-5688444

    SANDBOX USERS
    -------------
    199999996 / HH1773/ QQ1234(OTP)
    199999988 / PK2175 / qq1234(OTP)


    DEMO (SOURCES)
    --------------
    https://github.com/ISRTaxesOpenAPI/nodeJSExample
    https://github.com/ISRTaxesOpenAPI/-postmanExample


    PROCESS
    -------
    (steps)    
    -USER-
    1. Client Registration (Taxes Authority)        
    2. Send API Permissions Request

    -DEVELOPER-    
    1. Client Registration (Taxes Authority)        
    2. Approve API Permissions Request
    3. Developer Registration
    4. CREATE AN APP
    5. ENABLE SERVICES FOR API APP    

    note! 
    need to register to both SANDBOX and PROD portals (with different emails)

    CLIENT REGISTRATION
    -------------------
    https://secapp.taxes.gov.il/srRishum/main/openPage
    https://www.gov.il/he/service/connect-to-shaam
    https://openapi-portal.taxes.gov.il/sandbox/sites/sandbox.openapi-portal.taxes.gov.il/files/inline-files/portal%20user%20guide.pdf

    * register & wait for approval 
    * login the portal
    * add organization 
    * invite developers  
    * create an api app 
    * get the generated key & secret 
    * enable services for api app (demoApp + Invoices) 


    APPROVE API PERMISSIONS REQUEST
    -------------------------------
    once the end user (company owner) has registered the api, a permissions request will be sent to be approved by the developer.
    - https://secapp.taxes.gov.il/srsherutatzmi/#/clientDashboard
    - https://secapp.taxes.gov.il/SrHasmacha/main/MainHasmacha?fromsystem=ezorIshi


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
    must be registered to the sandbox prior the prod. can use the same email.

    API BASE URL
    ------------
    * SANDBOX 
      https://openapi.taxes.gov.il/shaam/tsandbox   // deprecated on 01.01.2024
      https://ita-api.taxes.gov.il/shaam/tsandbox   // new
      https://ita-api.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/token  // new
      https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2
      https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/authorize
            
    * PROD
      https://openapi.taxes.gov.il/shaam/production     // deprecated on 01.01.2024
      https://ita-api.taxes.gov.il/shaam/production     // new
      https://ita-api.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/token  // new
      https://openapi.taxes.gov.il/shaam/production/longtimetoken/oauth2      
      https://openapi.taxes.gov.il/shaam/production/longtimetoken/authorize

    note! 
    oauth2 uses the old base url, the rest of the services will use the new updated url
    https://openapi.taxes.gov.il/shaam/<env>/longtimetoken/oauth2


    SERVICE TYPES
    -------------
    * Open Access (public - no token required) 
    * One Time (requires a token)
    * Long Time (requires a token)


    POSTMAN
    -------
    - see 'ISLTaxesOpenAPI.postman_collection.json'
    - see 'Taxes API.postman_collection.json' 


    DEMO (sources)
    --------------
    https://github.com/ISRTaxesOpenAPI/nodeJSExample
    https://github.com/ISRTaxesOpenAPI/-postmanExample


    REFORM
    ------
    Since the 01 of May 2024, Use this service to broadcast ANY invoice and invoice-receipt in real-time to the taxes authority!
    document types to send: invoice and invoice-receipt. should be applied only to invoices with VAT.
    exempt-dealers businesses don't need to implement it cause no VAT is involved.
    the first limit is for invoices over 25K, shortly this limit will be lowered.
    every business must give permissions its taxes account in order to broadcast data on his behalf. (using oAuth process)
    for every document sent to the taxes authority, an allocation-number is provided. this number represents the validity of the transaction!
    each refresh-token is valid for up to 3 months and needs to be regenerated before its expiry.

    more info:
    - watch 'tax-authority-intro.mp4'
    - https://app.greeninvoice.co.il/settings/tax-authority    


    IMPLEMENTATIONS
    ---------------
    - see 'CODE > TaxesILManager.cs'
    - see 'Creative > TaxesILGateway'    
    - see 'DocSee > TaxesILGateway'    

    
    RESEARCH
    --------
    - see 'Scripts > TaxesIL API'


    cURL
    ----
    * AUTHORIZATION CODE:
    
      GET https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/authorize

      query params:
      response_type=code
      client_id=<client-id>
      scope=<scope>

    -

    * TOKEN:
    
      POST https://ita-api.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/token
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
        ServerURL = "https://ita-api.taxes.gov.il/shaam/tsandbox",
        AuthorizeURL = "https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2",
        ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        ApiSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"                    
    };

    // optional
    taxesILConfig.RefreshToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
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
        ConfirmationNumber = "202310091xxxxxxxxxxxx",
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

    --

    var taxesILManager = new TaxesILManager(taxesILConfig);

    taxesILManager.TokensUpdated += (sender, eventArgs) => {
        Console.WriteLine($"Tokens Updated: {eventArgs}");
        Config.TaxesIL.RefreshToken = eventArgs.RefreshToken;
        UpdateTaxesConfig(Config);
    };

    var healthResult = await taxesILManager.HealthCheck();          
    Console.WriteLine($"Success = {healthResult.Success} | {healthResult.Details}"); // (bool, string) - OK, TOKEN_EXPIRED, ERROR...

    if (!healthResult.Success) {
        var code = taxesILManager.RequestAuthorizaionCode(9999);
        await taxesILManager.Authorize(code);
    }
            
    var created = await taxesILManager.CreateInvoice(invoiceToCreate);
    Console.WriteLine(created);
            
    return created.Confirmation;    
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

            public bool IsRefreashTokenExpired {
                get {
                    return this.InnerMessage.Details?.Contains("refresh_token expired") ?? false;
                }
            }

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
            [JsonProperty(PropertyName = "invoice_id")]
            public string InvoiceId { get; set; }

            [JsonProperty(PropertyName = "invoice_type")]
            public int InvoiceType { get; set; }

            [JsonProperty(PropertyName = "vat_number")]
            public int VatNumber { get; set; }

            [JsonProperty(PropertyName = "union_vat_number")]
            public int UnionVatNumber { get; set; }

            [JsonProperty(PropertyName = "authorized_company")]
            public int AuthorizedCompany { get; set; }

            [JsonProperty(PropertyName = "user_id")]
            public int UserId { get; set; }

            [JsonProperty(PropertyName = "user_name")]
            public string UserName { get; set; }

            [JsonProperty(PropertyName = "invoice_reference_number")]
            public string ReferenceNumber { get; set; }

            [JsonProperty(PropertyName = "customer_vat_number")]
            public int CustomerVATNumber { get; set; }

            [JsonProperty(PropertyName = "customer_name")]
            public string CustomerName { get; set; }

            [JsonProperty(PropertyName = "customer_country_code")]
            public string CustomerCountryCode { get; set; }
            
            [JsonProperty(PropertyName = "invoice_date")]
            public string InvoiceDate { get; set; }

            [JsonProperty(PropertyName = "invoice_issuance_date")]
            public string InvoiceIssuanceDate { get; set; }

            [JsonProperty(PropertyName = "branch_id")]
            public string BranchId { get; set; }

            [JsonProperty(PropertyName = "accounting_software_number")]
            public int AccountingSoftwareNumber { get; set; }

            [JsonProperty(PropertyName = "client_software_key")]
            public string ClientSoftwareKey { get; set; }

            [JsonProperty(PropertyName = "amount_before_discount")]
            public decimal AmountBeforeDiscount { get; set; }

            [JsonProperty(PropertyName = "discount")]
            public decimal Discount { get; set; }

            [JsonProperty(PropertyName = "payment_amount")]
            public decimal PaymentAmount { get; set; }

            [JsonProperty(PropertyName = "vat_amount")]
            public decimal VATAmount { get; set; }

            [JsonProperty(PropertyName = "payment_amount_including_vat")]
            public decimal PaymentAmountIncludingVAT { get; set; }

            [JsonProperty(PropertyName = "invoice_note")]
            public string InvoiceNote { get; set; }

            [JsonProperty(PropertyName = "action")]
            public int Action { get; set; }

            [JsonProperty(PropertyName = "vehicle_license_number")]
            public int LicenseNumber { get; set; }

            [JsonProperty(PropertyName = "phone_of_driver")]
            public string PhoneOfDriver { get; set; } = "0";

            [JsonProperty(PropertyName = "arrival_date")]
            public string ArrivalDate { get; set; }

            [JsonProperty(PropertyName = "estimated_arrival_time")]
            public string EstimatedArrivalTime { get; set; }

            [JsonProperty(PropertyName = "transition_location")]
            public int TransitionLocation { get; set; }

            [JsonProperty(PropertyName = "delivery_address")]
            public string DeliveryAddress { get; set; }

            [JsonProperty(PropertyName = "additional_information")]
            public int Notes { get; set; }

            [JsonProperty(PropertyName = "items")]
            public List<InvoiceItem> Items { get; set; }            
        }

        public class InvoiceItem
        {
            [JsonProperty(PropertyName = "index")]
            public int Index { get; set; }

            [JsonProperty(PropertyName = "catalog_id")]
            public string CatalogId { get; set; }

            [JsonProperty(PropertyName = "category")]
            public int Category { get; set; } = 1; // Must be > 0 

            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; } = "";  // null not allowed 

            [JsonProperty(PropertyName = "measure_unit_description")]
            public string MeasureUnitDescription { get; set; } = "";  // null not allowed 

            [JsonProperty(PropertyName = "quantity")]
            public float Quantity { get; set; }

            [JsonProperty(PropertyName = "price_per_unit")]
            public float UnitPrice { get; set; }

            [JsonProperty(PropertyName = "discount")]
            public decimal Discount { get; set; }

            [JsonProperty(PropertyName = "total_amount")]
            public decimal Total { get; set; }

            [JsonProperty(PropertyName = "vat_rate")]
            public int VATRate { get; set; }

            [JsonProperty(PropertyName = "vat_amount")]
            public decimal VATAmount { get; set; }
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
            public dynamic Message { get; set; }

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
        Task<IEnumerable<CreateInvoiceResponse>> CreateInvoices(IEnumerable<Invoice> Request);
        Task<(bool Success, string Details)> HealthCheck();
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
            Context.Response.Redirect(this.OAuthURL, false);
            Context.ApplicationInstance.CompleteRequest();
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

        public bool SetTokensData(string RefreshToken, string AccessToken)
        {
            this.Config.AccessToken = AccessToken;
            this.Config.RefreshToken = RefreshToken;
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

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(this.Config.RefreshToken))
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<GetInvoiceDetailsRequest, GetInvoiceDetailsResponse>(
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
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model?.Message;
        }

        public async Task<CreateInvoiceResponse> CreateInvoice(Invoice Request) 
        {
            this.NullToEmpty(Request);
            var response = await this.HttpService.POST_ASYNC<Invoice, CreateInvoiceResponse>(
                $"{this.Config.ServerURL}/Invoices/v2/Approval",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                }
            );

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(this.Config.RefreshToken))
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<Invoice, CreateInvoiceResponse>(
                    $"{this.Config.ServerURL}/Invoices/v2/Approval",
                    Request,
                    null,
                    new Dictionary<string, string>
                    {
                        ["Accept"] = "application/json",
                        ["Content-Type"] = "application/json",
                        ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                    }
                );
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<IEnumerable<CreateInvoiceResponse>> CreateInvoices(IEnumerable<Invoice> Request) {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Details)> HealthCheck() 
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

            // Unauthorized (401) - refresh token and try again
            // "OK" \ "TOKEN_EXPIRED" \ "ERROR | {MESSAGE}"
            if (!response.Success && response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(this.Config.RefreshToken))
            {
                try {
                    await this.RefreshToken();
                }
                catch (APIException Ex) {
                    if (Ex.ErrorResponse.IsRefreashTokenExpired)
                        return (false, "TOKEN_EXPIRED");
                    return (false, Ex.ErrorResponse.InnerMessage.Details);
                }

                response = await this.HttpService.GET_ASYNC(
                    $"{this.Config.ServerURL}/Invoices/v1/Health",
                    null,
                    new Dictionary<string, string> {
                        ["Accept"] = "application/json",
                        ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                    }
                );
            }

            if (!response.Success)
                return (false, this.ParseError(response.Content).InnerMessage.Error);
            return (true, "OK");
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

                {                    
                    "error": "invalid_grant",
                    "error_description": "*[e00a251f6f158679906c9de216ab1eb9] refresh_token expired*"
                }

                {
                    "Status": 406,
                    "Message": "Not Acceptable",
                    "Error_Id": "71523446191"
                }

                {
                  "Status": 400,
                  "Message": {
                    "errors": [
                      {
                        "value": " ",
                        "msg": "Value should be numeric of type int ",
                        "param": "Phone_Of_Driver",
                        "location": "body"
                      }
                    ]
                  },
                  "Error_Id": "04565273934"
                }

                {
                  "Status": 200,
                  "Message": {
                    "errors": [
                      {
                        "code": 432,
                        "message": "Customer VAT Number is incorrect",
                        "param": "Customer_VAT_Number",
                        "location": "validation"
                      }
                    ]
                  },
                  "Confirmation_Number": 0
                }
            */
            var errorRawParts = ErrorRaw.Split('|');
            var httpError = errorRawParts[0];
            var requestError = errorRawParts.Length > 1 ? errorRawParts[1] : errorRawParts[0];

            var result = new APIErrorResponse
            {
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
            else if (requestError.Contains("Error_Id"))
            {
                // Single Message (Singular)
                var errorSchema = new
                {
                    Message = string.Empty
                };

                var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
                result.InnerMessage = (
                    exData?.Message?.Trim() ?? string.Empty,
                    exData?.Message?.Trim() ?? string.Empty
                );
            }
            else if (requestError.Contains("errors"))
            {
                // Array of Messages (Plural)
                var itemSchema = new
                {
                    value = string.Empty,
                    msg = string.Empty,
                    message = string.Empty,
                    param = string.Empty
                };

                var messageSchemaErrors = new
                {
                    errors = new[] { itemSchema }
                };

                var errorSchema = new
                {
                    Message = messageSchemaErrors
                };

                var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);

                var message = exData?.Message?.errors.FirstOrDefault().msg?.Trim() ?? exData?.Message?.errors.FirstOrDefault().message?.Trim() ?? string.Empty;
                result.InnerMessage = (
                    message,
                    exData?.Message?.errors.FirstOrDefault().param?.Trim() ?? string.Empty
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

        private void NullToEmpty<T>(T obj)
        {
            if (obj == null) return;

            foreach (var propertyInfo in obj.GetType().GetProperties())
                if (propertyInfo.PropertyType == typeof(string))
                    if (propertyInfo.GetValue(obj, null) == null)
                        propertyInfo.SetValue(obj, string.Empty, null);
        }
    }
}
