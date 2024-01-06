Taxes Open API
--------------

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
itaOpenApiSupport@taxes.gov.il


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
need to register to both SANDBOX and PROD portals

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
must be registered to the sandbox prior the prod.  

API BASE URL
------------
* SANDBOX 
    https://openapi.taxes.gov.il/shaam/tsandbox   // deprecated on 01.01.2024
    https://ita-api.taxes.gov.il/shaam/tsandbox   // new
    https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2
            
* PROD
    https://openapi.taxes.gov.il/shaam/production     // deprecated on 01.01.2024
    https://ita-api.taxes.gov.il/shaam/production     // new
    https://openapi.taxes.gov.il/shaam/production/longtimetoken/oauth2

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
    
    POST https://openapi.taxes.gov.il/shaam/tsandbox/longtimetoken/oauth2/token
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
Console.WriteLine(healthResult); // OK

if (healthResult != "OK") {
    var code = taxesILManager.RequestAuthorizaionCode(9999);
    await taxesILManager.Authorize(code);
}
            
var created = await taxesILManager.CreateInvoice(invoiceToCreate);
Console.WriteLine(created);
            
return created.Confirmation;    

