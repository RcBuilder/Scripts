Taxes Open API
--------------

Reference:
https://openapi-portal.taxes.gov.il/sandbox/product
https://openapi-portal.taxes.gov.il/sandbox/product/2927/api/1533#/Invoices_v1/overview
https://openapi-portal.taxes.gov.il/sandbox/product/1776/api/199#/longtimeacces_100/overview

-

Client Registration:
https://secapp.taxes.gov.il/srRishum/main/openPage
https://www.gov.il/he/service/connect-to-shaam

* register & wait for approval 
* login the portal
* add organization 
* invite developers  
* create an api app 
* get the generated key & secret 
* enable services for api app (demoApp + Invoices) 

-

Developer Registration:
* after a client send a developer invitation (see 'Client Registration > invite developers')
* an email with token will be sent to the developer 
* need to click and sign-up (if you haven't singed up already) with your id  
* after the registration, you could find your clients organizations in the taxes api portal

note! 
this phase connect the client account with the developer account.

-

create an app:
* login into the taxes portal 
* choose organization 
* click on 'Apps' > Create
* set app name, desc and oauth return uri

-

enable services for api app:
* login into the taxes portal 
* choose organization 
* click on 'API Products' > choose API's to enable > choose plan > save

note! 
we can find all enabled APIs under Subscriptions tab in the app page

-

Client Portal:
* SANDBOX 
  https://openapi-portal.taxes.gov.il/sandbox
* PROD
  https://openapi-portal.taxes.gov.il/shaam/production

note! 
must be registered to the sandbox prior the prod.  

-

invite a developer:
1. login to the Portal 
2. click on your company name (top-right corner)
3. My Organization > Invite 
4. set email and Role (admin, developer or viewer)

-
 
Service Types:
* Open Access (public - no token required) 
* One Time (requires a token)
* Long Time (requires a token)

-

IMPLEMENTATIONS:
* see 'CODE > TaxesILManager.cs'
* see 'Creative > TaxesIL'    
* see 'DocSee > TaxesIL'    

-

RESEARCH:
* see 'Scripts > TaxesIL API'

-

Postman:
* see 'ISLTaxesOpenAPI.postman_collection.json'
* see 'Taxes API.postman_collection.json' 

- 

sandbox users: 
199999996 / HH1773/ QQ1234 (OTP)
199999988 / PK2175 / qq1234 (OTP)

-

support:
OpenAPI@taxes.gov.il
invoices@taxes.gov.il
lakohot-bt@taxes.gov.il

-

demo (sources):
https://github.com/ISRTaxesOpenAPI/nodeJSExample
https://github.com/ISRTaxesOpenAPI/-postmanExample

-

cURL:

// AUTHORIZATION CODE
GET https://openapi.taxes.gov.il/shaam/Tsandbox/longtimetoken/oauth2/authorize

query params:
response_type=code
client_id=<client-id>
scope=<scope>


// TOKEN
POST https://openapi.taxes.gov.il/shaam/Tsandbox/longtimetoken/oauth2/token
H Authorization: basic base64(clientid:clientsecret)
H Content-type: application/x-www-form-urlencoded

body params:
grant_type=authorization_code
code=<authorizationcode>
redirect_uri=<redirecturi>
scope=<scope>



