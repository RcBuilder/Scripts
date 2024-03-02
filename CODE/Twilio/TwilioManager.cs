using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

// Install-Package Twilio
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Content.V1;
using Twilio.Rest.Verify.V2;
using static Twilio.Rest.Api.V2010.Account.Call.FeedbackSummaryResource;
using static TwilioBLL.TwilioEntities;

namespace TwilioBLL
{
    /*
        --TEMP-- 

        
        - open a new twilio account 
        - set 2FA (two-factor-authentication)
        - Messaging > Senders > whatsapp senders > Get Started
        - upgrade account (to use whatsapp)
        - Upload ID (Passport, Identity or Driving License)
        - check email to provide more details about the business and the using of twilio services
        - Billing > Payment Settings > set credit card & auto-charge
        - Phone Numbers > Active Numbers > Bug number
        - Click on the purchased number > Configue > set Hooks
          tip: Messaging > Try it out > Send SMS (via dashboard for testing purposes)
          tip: within the number we can find tabs 'Calls Log' and 'Messages Log' to monitor all messages (incoming & outgoing)
        - Messaging > Senders > whatsapp senders > Get Started
        - choose 'Twilio phone number' or Buy a new one 
        - (screen) Link WhatsApp Business Account with your number
 


        https://www.twilio.com/docs/whatsapp/tutorial/connect-number-business-profile

        SANDBOX:
        - Messagin > Try it out > Send a WA message >
          https://console.twilio.com/us1/develop/sms/try-it-out/whatsapp-learn
        - Connect to sandbox
          send a whatsapp message as stated on your dashboard 
          join <sandbox-name>  (e.g: join tank-over)
        - send a sample message from twilio sandbox to your registered number 
        - complete the process

        note!
        to add a webhook to the sandbox server, use the 'Sandbox Settings' tab
          


        API keys & tokens:
        https://console.twilio.com/us1/account/keys-credentials/api-keys
        1. Live credentials
        2. Test credentials

        ---
        [add notes to TwilioBLL]
    */

    /*
        // in twilio manager
        public static HttpContent getMessageXML(string body)
        {
            var msg_response = new MessagingResponse();
            msg_response.Message(body);
            return new StringContent(msg_response.ToString(), System.Text.Encoding.UTF8, "application/xml");
        }
        
        // sample usage (mvc-controller)
        public async Task<HttpResponseMessage> WhatsAppHook(HttpRequestMessage request)
        {
            var content = await request.Content.ReadAsStringAsync();
            NameValueCollection formData = HttpUtility.ParseQueryString(content);

            var senderNumber = (new Regex(@"whatsapp:(.+)")).Match(HttpUtility.UrlDecode(formData["From"])).Groups[1].Value;

            var restaurantId = -1;
            try
            {
                var searchNumber = senderNumber.Replace("+972", "0");
                restaurantId = (await new RestaurantsBLL().GetByPhoneNumber(searchNumber)).Details.Id;
            }
            catch(Exception) { }
            
            var messageBody = HttpUtility.UrlDecode(formData["Body"]).Trim();

            ...
            ...
      
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            bool reply_with_response = true;

            if (reply_with_response)
            {
                // return TwiML(replyBody);
                response.Content = TwilioManager.getMessageXML(replyBody);
            }
            else
            {
                var twilioPhone = HttpUtility.UrlDecode(formData["To"]);
                if (!ConfigSingleton.Instance.TwilioPhoneNumber.Equals(twilioPhone, StringComparison.OrdinalIgnoreCase)) 
                    throw new Exception("Unexpected Twilio phone number.");

                var recipientPhone = senderNumber;
                SMSManager.SendSMS(recipientPhone, replyBody);
            }

            return response;
        }
    */

    /*
        Source:
        https://www.twilio.com/docs/verify/whatsapp 

        Verify Service:
        https://www.twilio.com/docs/verify/api/service

        Using:
        var verification = VerificationResource.Create(
            to: "+15017122661",
            channel: "whatsapp",
            pathServiceSid: "VAXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        ); 

        Sample Output:
        {
          "sid": "VEXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
          "service_sid": "VAXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
          "account_sid": "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
          "to": "+15017122661",
          "channel": "whatsapp",
          "status": "pending",
          "valid": false,
          "date_created": "2015-07-30T20:00:00Z",
          "date_updated": "2015-07-30T20:00:00Z",
          "lookup": {},
          "amount": null,
          "payee": null,
          "send_code_attempts": [
            {
              "time": "2015-07-30T20:00:00Z",
              "channel": "whatsapp",
              "attempt_sid": "VLXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
            }
          ],
          "sna": null,
          "url": "https://verify.twilio.com/v2/Services/VAXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX/Verifications/VEXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        }
    */

    /*
        Reference
        ---------          
        -- Messages --
        https://www.twilio.com/docs/messaging        
        https://www.twilio.com/docs/whatsapp
        https://www.twilio.com/docs/whatsapp/quickstart/csharp
        https://www.twilio.com/docs/whatsapp/api
        https://www.twilio.com/docs/whatsapp/tutorial/send-whatsapp-notification-messages-templates
        https://www.twilio.com/docs/whatsapp/api/media-resource                
        -- Hooks --
        https://www.twilio.com/docs/glossary/what-is-a-webhook
        https://www.twilio.com/docs/usage/webhooks/sms-webhooks
        https://www.twilio.com/docs/usage/webhooks        
        -- Content API --
        https://www.twilio.com/docs/content/whatsappauthentication
        https://www.twilio.com/docs/content/send-templates-created-with-the-content-editor#send-messages-with-a-messagingservicesid-field
        https://www.twilio.com/docs/content/content-api-resources#create-templates
        https://console.twilio.com/us1/develop/sms/content-editor
        -- Other --
        https://github.com/twilio/twilio-csharp
        https://www.twilio.com/console/phone-numbers        
        https://www.twilio.com/docs/content/content-types-overview


        Postman
        -------
        Twilio.postman_collection.json           

        Whatsapp Unverified Numbers
        ---------------------------
        see 'Error 63016'

        Error 63016
        -----------
        https://www.twilio.com/docs/api/errors/63016
        this error occurs when trying to use WhatsApp service to send a message to an unverified destination number.
        a number is verified for 24 hours once a message is sent from it to the Twilio-app-number.

        how to verify a number?
        1. use a predefined Template to send a message           
           https://www.twilio.com/docs/whatsapp/tutorial/send-whatsapp-notification-messages-templates

           note! 
           we don't have to use templates, ANY incoming message will activate the permissions window! 
           a template is a predefined message structure verified by Twilio and allowed to be sent to unverified numbers to trigger them to make an incoming request.

        2. wait for an incoming message from that number. 
        3. once arrived > a 24 hours permission window will establish on twilio and during this period, we can send whatsapp messages


        // TODO ->> review hook fields (reference)
        Web Hooks
        ---------
        https://www.twilio.com/docs/messaging/guides/webhook-request

        POST http://localhost:59660/Twilio/Hook

        /// x-www-form-urlencoded
        - sourceComponent:14100
        - httpResponse:500
        - url:https://api.avitanapp.co.il/Avitanz
        - ErrorCode:0
        - LogLevel:INFO
        - Msg:OK
        - EmailNotification:false
        - SmsMessageSid:MM4ec951e52e8a6bf4620817d7bbd7c32b
        - ProfileName:רובי כהן
        - SmsSid:MM4ec951e52e8a6bf4620817d7bbd7c32b
        - WaId:972544861058
        - SmsStatus:received
        - To:whatsapp:+14246552902
        - MessagingServiceSid:MGaa9e09e0bb3294498344516ec949523e
        - MessageSid:MM4ec951e52e8a6bf4620817d7bbd7c32b
        - AccountSid:AC930bcae00a35de321b1ddb5f733dbfe3
        - ApiVersion:2010-04-01
        - From:whatsapp:+972544861058
        - Body:bed945d0-2bfe-42a2-8acc.pdf
        - Forwarded:true
        - ReferralNumMedia:0
        - NumSegments:1
        - NumMedia:1
        - MediaContentType0:application/pdf
        - MediaUrl0:https://api.twilio.com/2010-04-01/Accounts/AC930bcae00a35de321b1ddb5f733dbfe3/Messages/MM4ec951e52e8a6bf4620817d7bbd7c32b/Media/ME5bddad46f21ce53c4118f33084073dfa


        USING
        -----
        var twilio = new TwilioManager("AC9c7aa7b4d4cb7a6034c694871912ee44", "xxxxxxxxxxxxxxxxxxxxxxxx");
                
        // send an sms 
        var messageId = twilio.SendSMS("+1 423 285 6736", "+972 545614020", "HELLO FROM TWILIO API");
        Debug.WriteLine(messageId);

        // get message info by id 
        var message = twilio.GetMessageInfo(messageId);
        Console.WriteLine(JsonConvert.SerializeObject(message));

        // make an audio call
        var audioPath = "http://domain.com/Audio/1MM.wav"
        var callId = this.Twilio.MakeACall("+1 423 285 6736", "+972 545614020", audioPath);
        Debug.WriteLine(callId);
        
        // make an audio call using xml 
        var xmlPath = "http://demo.twilio.com/docs/voice.xml";
        var callId = this.Twilio.MakeACall("+1 423 285 6736", "+972 545614020", new Uri(xmlPath));
        Debug.WriteLine(callId);
        
        // send a whatsapp message 
        var messageId = Twilio.SendWhatsapp("+1 415 523 8886", "+972-54-561-4020", "HELLO FROM TWILIO API");
        Debug.WriteLine(messageId);

        // send a whatsapp message with media files
        var messageId = Twilio.SendWhatsapp("+1 415 523 8886", "+972-54-561-4020", "HELLO FROM TWILIO API", new List<string> { 
            "https://static.remove.bg/sample-gallery/graphics/bird-thumbnail.jpg" 
        });
        Debug.WriteLine(messageId);

        -
   
        var from = "+1 4246552902";
        var to = "+972 545614020";

        var messageId = twilio.SendSMS(from, to, "Hello World");
        var message = twilio.GetMessageInfo(messageId);
        Console.WriteLine(JsonConvert.SerializeObject(message));

        var messageId = twilio.SendWAMessage(from, to, "Hello World");
        var message = twilio.GetMessageInfo(messageId);
        Console.WriteLine(JsonConvert.SerializeObject(message));

        var templates = twilio.GetContentTemplates();
        foreach (var t in templates)
            Console.WriteLine($"#{t.SId} {t.Name}");

        var template = twilio.GetContentTemplate("HX07bd4f3787b27ab1a8107020665792f4");
        Console.WriteLine($"#{template.SId} {template.Name}");

        template = twilio.GetContentTemplate("rc_tmp_6"); // by name
        Console.WriteLine($"#{template.SId} {template.Name}");

        var status = twilio.GetContentTemplateApprovalStatus("HX07bd4f3787b27ab1a8107020665792f4");
        Console.WriteLine(status);

        var messages = twilio.GetMessages(Limit: 5);
        foreach (var m in messages)
            Console.WriteLine($"#{m.SId} {m.Status}");

        var resources = twilio.GetContentResources();
        foreach (var r in resources)
            Console.WriteLine($"#{r.SId} {r.Name}");

        var resource = twilio.GetContentResource("HX07bd4f3787b27ab1a8107020665792f4");
        Console.WriteLine($"#{resource.SId} {resource.Name}");

        var messageId = twilio.SendContentTemplate(from, to, "HXbeb7d9adb751bcf56c0912c4fd9b8e6d", "MGaa9e09e0bb3294498344516ec949523e",
            new Dictionary<string, string> {
                    { "1", "פלוני אלמוני" }
            });
        Console.WriteLine(messageId);

        var messageId = twilio.SendContentTemplate(from, to, "HXd7edf492c44d8bf4322f5fa949624ae2", "MGaa9e09e0bb3294498344516ec949523e");
        Console.WriteLine(messageId);

        var messageId = twilio.SendWAMessage(from, to, "Hello World",
            new List<string> {
                    "https://images.ctfassets.net/yadj1kx9rmg0/wtrHxeu3zEoEce2MokCSi/cf6f68efdcf625fdc060607df0f3baef/quwowooybuqbl6ntboz3.jpg"
            },
            "https://rcbuilder.free.beeceptor.com");
        var message = twilio.GetMessageInfo(messageId);
        Console.WriteLine(JsonConvert.SerializeObject(message));

        var messageId = twilio.ScheduleWAMessage(from, to, "Hello World", "MGaa9e09e0bb3294498344516ec949523e", DateTime.Now.AddSeconds(1000));                
        Console.WriteLine(messageId);

        var messageId = twilio.UnScheduleWAMessage(messageId);                
        Console.WriteLine(messageId);  
                  
        var messageId = twilio.Delete(messageId);                
        Console.WriteLine(messageId);  
                                
        var deleted = twilio.DeleteMessage(messageId);
        Console.WriteLine(deleted);

        var templateByName = twilio.GetContentTemplate("to_delete"); // find by name
        var templateById = twilio.GetContentTemplate("HX02b0773dec0e16b23b23e56357e597cb");  // find by id

        var deleted = twilio.DeleteContentTemplate("HX02b0773dec0e16b23b23e56357e597cb");
        Console.WriteLine(deleted);

        var result = twilio.CreateTextContentTemplate(new TwilioEntities.TemplateCreatorText
        {
            Name = "template_1",
            Payload = new TwilioEntities.TemplateTextPayload("Hi")
        });
        Console.WriteLine($"{result.ContentSid} ({result.ApproveStatus})");

        var result = twilio.CreateTextContentTemplate(new TwilioEntities.TemplateCreatorText
        {
            Name = "template_2",
            Payload = new TwilioEntities.TemplateTextPayload("Hi {{1}}, thanks for contacting {{2}}."),
            ContentVariables = new Dictionary<string, string> { 
                ["1"] = "Roby",
                ["2"] = "RcBuilder"
            }
        });
        Console.WriteLine($"{result.ContentSid} ({result.ApproveStatus})");
    */

    /*
        [WHATSAPP]

        REFERENCE:                
        https://www.twilio.com/docs/whatsapp/quickstart/csharp
        https://www.twilio.com/docs/whatsapp/api
        https://www.twilio.com/docs/whatsapp/tutorial/send-whatsapp-notification-messages-templates
        https://www.twilio.com/docs/whatsapp/api/media-resource
        -- Content API --
        https://www.twilio.com/docs/content/whatsappauthentication
        https://www.twilio.com/docs/content/send-templates-created-with-the-content-editor#send-messages-with-a-messagingservicesid-field
        https://www.twilio.com/docs/content/content-api-resources#create-templates
        https://console.twilio.com/us1/develop/sms/content-editor            


        Signup Process
        --------------
        Enable WhatsApp feature on twilio:                
        https://www.twilio.com/docs/whatsapp/api#connect-your-meta-business-manager-account
        WA uses your (Facebook) Meta Business Manager account to identify your business and associate your WA Business Account (WABA) with it.

        to create or connect a Meta Business Manager account to Twilio, use a Signup process
        1. Self-Signup process -> for an end-user
            https://www.twilio.com/docs/whatsapp/self-sign-up
        2. Global-Signup process -> for ISV (Independent Software Vendor)
            https://www.twilio.com/docs/whatsapp/tutorial/connect-number-business-profile

        create a Meta Business Manager account:
        https://www.facebook.com/business/help/1710077379203657?id=180505742745347

        get your Meta Business Manager ID:                
        Business Settings > Business Info > #Id

        Self-Signup process:
        1.  Create WA Sender:
            Messaging > Senders > WhatsApp senders > New
        2.  Link WA Account with Twilio Account
            Click on 'Continue With Facebook' button > OAuth Popup 
            note! if Must have a 'Meta Business Account'.
        3. Verify your WA Number

        WA Content Process
        ------------------
        (Steps)
        1. Messaging > Content Editor > Create Templates > Submit for Approval
        2. Messaging > Services > Create a Messaging Service
        3. Use API to send a Template 

            POST {{Server}}{{Version}}/Accounts/{{SID}}/Messages.json
            H Authorization:Basic {{SID}}{{Token}}
            ContentVariables:{"1":"Roby"}
            From:whatsapp:{{From}}
            To:whatsapp:+972xxxxxxxxxx
            ContentSid:HXxxxxxxxxxxxxxxxxxxxxxxxxx
            MessagingServiceSid:MGxxxxxxxxxxxxxxxxxxxx

        4. once a user sent a message, we'll get a 24-hour window to send ANY message to this user. 
            for new users or existing users who have not sent you a message for more than 24 hours - we MUST use Templates.                       
            templates are used to contact a user with pre-defined & approved messages by the Twilio team. 
            use these techniques to send reminders, alerts and etc. OR to cause the user to reply and by that, to gain the 24h permission's allocation

            note!
            the use consent also known as 24-hour window or 24-hour session.
        -

        Params:
        1. ContentSid  (HXxxxxxxxxxxxxxxxxxxxxxxx)
        2. ContentVariables  ({"<ph-num>":"<value>"})  e.g: {"1":"AAA", "2":"BBB"}

            note: use {{1}}... {{N}} to add variables 
            e.g: "hello {{1}}"

            to set variables, use the ContentVariables post-data parameter
            e.g: ContentVariables: {"1":"Roby"}  // replace ph-1 with the value "Roby" -> "hello Roby"

            note!
            can use Content API
            https://www.twilio.com/docs/content/send-templates-created-with-the-content-editor#send-messages-with-a-messagingservicesid-field                

        3. MessagingServiceSid   (MGxxxxxxxxxxxxxxxxxxxxxx) 
            https://www.twilio.com/docs/content/create-and-send-your-first-content-api-template

            https://console.twilio.com/us1/develop/sms/services
            > Messaging > Services > Create a Messaging Service > Set Name > Create
            > Add Senders > Choose Phone Number

            > Messaging > Services > Choose Service > Copy SID (MessagingServiceSid)

            note!                
            we can remove the 'MessagingServiceSid' param and set the MessagingServiceSid in the 'from' param
            From:MGxxxxxxxxxxxxxxxxxxxx

        Content Template
        ----------------
        aka as conversation starter. use templates to create pre-defined & approved messages. 
        each template must be verified by the Twilio team and once approved - can be sent to any user via WhatsApp. 
        note that WhatsApp API is restricted from sending messages to users without their consent. we can use a template to gain consent for 24 hours. 
        after this period, regular messages will NOT pass through until a new consent has taken place. 
        because templates are verified and allowed to be sent without a user's consent, we should use it to trigger a reply from a user in order to get 24-hours consent. 
        any outgoing message from the end-user provides consent to send messages to the user for up to 24 hours!

        https://console.twilio.com/us1/develop/sms/content-editor
        > Messaging > Content Editor > Create New > Set Name > Select Content-Type > Create 
        > Set Template Body 

        https://www.twilio.com/docs/content/content-types-overview
        https://console.twilio.com/us1/develop/sms/content-editor/template/create
        Content-Types:
        - Text
        - Media
        - Quick reply
        - Call to action
        - List picker                
        - Card 
        - Authentication (OTP)

        https://www.twilio.com/docs/content/content-types-overview#whatsapp-template-approval-statuses
        Approval Status:
        - Unsubmitted
        - Received
        - Pending
        - Approved
        - Rejected
        - Paused
        - Disabled


        Create a Content Template via API
        ---------------------------------
        https://www.twilio.com/docs/content/content-api-resources#create-templates
        https://www.twilio.com/docs/content/content-types-overview

        Params:
        - friendly_name
        - language
        - ContentVariables
        - types  // https://www.twilio.com/docs/content/content-types-overview


        Usage:
        POST {{ServerContent}}v1/Content
        H Content-Type:application/json
        H Authorization:Basic {{SID}}{{Token}}
        {
            "friendly_name": "rc_tmp_8",
            "language": "en",
            "variables": {"1":"John Doe", "2":"RcBuilder"},
            "types": {
                "twilio/text":{
                "body": "Hi {{1}},\n Thanks for contacting {{2}}..."        
                }
            }
        }


        Get Content Templates
        ---------------------
        GET {{ServerContent}}v1/Content                
        H Authorization:Basic {{SID}}{{Token}}


        Get a specific Content Template
        -------------------------------
        GET {{ServerContent}}v1/Content/{{ContentSID}}                
        H Authorization:Basic {{SID}}{{Token}}


        Delete a specific Content Template
        ----------------------------------
        DELETE {{ServerContent}}v1/Content/{{ContentSID}}                
        H Authorization:Basic {{SID}}{{Token}}


        Get Template Approval Status
        ----------------------------
        GET {{ServerContent}}v1/Content/{{ContentSID}}/ApprovalRequests
        H Authorization:Basic {{SID}}{{Token}}

        Pricing
        -------
        https://www.twilio.com/en-us/whatsapp/pricing

        Send Message
        ------------            
        - body
        - from
        - to
        - mediaUrl
        - statusCallback

        usage:
        POST {{Server}}{{Version}}/Accounts/{{SID}}/Messages.json
        H Authorization:Basic {{SID}}{{Token}}
        Body:Hello from Twilio
        From:whatsapp:{{From}}
        To:whatsapp:+972xxxxxxxxxx                
        mediaUrl: ["https://domain.com/a.jpg"]
        StatusCallback: https://rcbuilder.free.beeceptor.com


        Callback Status Notifications
        -----------------------------
        twilio supports callback notifications when sending an SMS or WA message. 
        use 'StatusCallback' param to set a callback URL to receive the status notifications. 

        e.g:
        for WA message we'll get 'sent' and 'delivered' callback requests. 
        any status change triggers a new status notification to be sent to the callback URL defined in the StatusCallback param. 

        usage:
        POST {{Server}}{{Version}}/Accounts/{{SID}}/Messages.json
        H Authorization:Basic {{SID}}{{Token}}
        Body:Hello from Twilio
        From:whatsapp:{{From}}
        To:whatsapp:+972xxxxxxxxxx                
        StatusCallback: https://rcbuilder.free.beeceptor.com


        Schedule a Message
        ------------------
        - ScheduleType  // fixed
        - SendAt        // sent UTC time (ISO-8601 format - yyyy-MM-ddThh:mm:ssZ)

        usage:
        POST {{Server}}{{Version}}/Accounts/{{SID}}/Messages.json
        H Authorization:Basic {{SID}}{{Token}}
        Body:Hello from Twilio
        From:whatsapp:{{From}}
        To:whatsapp:+972xxxxxxxxxx                
        MessagingServiceSid:MGxxxxxxxxxxxxxxxxxxxxx
        ScheduleType:fixed
        SendAt:2023-07-01T10:30:27Z


        Cancel a Scheduled Message
        --------------------------
        {{Server}}{{Version}}/Accounts/{{SID}}/Messages/SMxxxxxxxxxxxxxxxxxxxxxxxxxx.json
        Status:canceled


        Fetch a specific Message
        ------------------------                
        usage:
        GET {{Server}}{{Version}}/Accounts/{{SID}}/Messages/SMxxxxxxxxxxxxxxxxxxxxxxxx.json
        H Authorization:Basic {{SID}}{{Token}}


        Fetch Messages
        --------------
        search-engine for messages. optional filters
        - pageSize 
        - accountSid
        - from
        - to
        - dateSent

        usage:
        GET {{Server}}{{Version}}/Accounts/{{SID}}/Messages.json?pageSize=10
        H Authorization:Basic {{SID}}{{Token}}


        Delete a Message
        ----------------
        usage:
        DELETE {{Server}}{{Version}}/Accounts/{{SID}}/Messages/SMxxxxxxxxxxxxxxxxxxxxxxxx.json
        H Authorization:Basic {{SID}}{{Token}}

        -------------

        DASHBOARD:                
        Home -> Programmable Messaging

        JOIN (SANDBOX):
        1. https://www.twilio.com/console/sms/whatsapp/sandbox    
        2. invite users to the sendbox by sending a whatsapp message to your twilio number with the provided code (e.g: "join least-dull")
            notes: 
            we can also use invitation link (whatsapp://send?phone=<TwilioNumber>&text=<Code(URL-Encoded)>)
            we can also use QRCode to generate the invitation link

        WEBHOOK (SANDBOX):
        1. https://www.twilio.com/console/sms/whatsapp/sandbox
        2. set a webhook URL in both "WHEN A MESSAGE COMES IN" and\or "STATUS CALLBACK URL" options                    


        PRODUCTION:                
        (1) Facebook > Verify your Facebook Business Manager in order to setup WhatsApp Business API using Twilio.
            business.facebook > Business settings > Business Information > Business Verification Status 
            note! if status is not verified > click on "start verification" and follow the instructions 
            https://business.facebook.com/
            https://business.facebook.com/settings/info
            https://business.facebook.com/settings/security

        (2) Twilio > Purchase a new mobile phone number for WhatsApp Business API
            https://www.twilio.com/console/phone-numbers/search

        (3) To purchase the phone number, you'll need to submit a regulation-document fit to your country.
            https://www.twilio.com/guidelines/regulatory

        (4) Then, you'll have to fill-out an access-request-form and send it to whatsapp team for approval.
            note! twilio will contact you within 10 days with the outcome.
            https://www.twilio.com/whatsapp/request-access

            to get Facebook Business Id: 
            FB > Setting > Business Manager
            https://business.facebook.com/settings/info

            to approve twilio on facebook bussiness:
            FB > Settings > Received > "Approve" Twilio
            https://business.facebook.com/settings/requests

        (5) add a new WhatsApp Sender using the approved phone number
            https://www.twilio.com/console/sms/whatsapp/senders

        sources:
        - https://support.salescandy.com/hc/en-us/articles/360053590431-How-to-apply-and-set-up-WhatsApp-Business-API-number-in-Twilio-
        - https://www.twilio.com/docs/whatsapp/tutorial/connect-number-business-profile    
        - https://www.twilio.com/docs/whatsapp/api
        - https://www.twilio.com/docs/whatsapp/tutorial/connect-number-business-profile
    */

    public class TwilioEntities
    {
        public enum eScheduleType : byte {
            Fixed
        }

        public enum eTemplateType : byte
        {
            Text,
            Media,
            Location,
            List,
            CallToAction,
            QuickReply,
            Card,
            Authentication            
        }

        public class ContentTemplateCategory {
            public const string UTILITY = "UTILITY";
            public const string MARKETING = "MARKETING";
            public const string AUTHENTICATION = "AUTHENTICATION";
        }


        public abstract class TemplateCreatorBase<T> where T: ITemplatePayload
        {
            [JsonProperty(PropertyName = "friendly_name")]
            public string Name { get; set; } = "Template Creator";

            [JsonProperty(PropertyName = "language")]
            public string Language { get; set; } = "en";

            [JsonProperty(PropertyName = "variables")]
            public Dictionary<string, string> ContentVariables { get; set; }   
            
            [JsonProperty(PropertyName = "types")]
            public T Payload { get; set; }
        }

        /*
            POST {{ServerContent}}v1/Content
            {
                "friendly_name": "rc_tmp_8",
                "language": "en",
                "variables": {"1":"John Doe", "2":"RcBuilder"},
                "types": {
                    "twilio/text":{
                    "body": "Hi {{1}},\n Thanks for contacting {{2}}..."        
                    }
                }
            }
        */
        public class TemplateCreatorText : TemplateCreatorBase<TemplateTextPayload> {}

        public interface ITemplatePayload { }
        public class TemplateTextPayload : ITemplatePayload
        {
            public class TemplateTextPayloadBody
            {
                [JsonProperty(PropertyName = "body")]
                public string Body { get; set; }

                public TemplateTextPayloadBody(string Body){
                    this.Body = Body;
                }
            }

            public TemplateTextPayload(string Body) : this(new TemplateTextPayloadBody(Body)) { }
            public TemplateTextPayload(TemplateTextPayloadBody Content) {
                this.Content = Content;
            }

            [JsonProperty(PropertyName = "twilio/text")]
            public TemplateTextPayloadBody Content { get; set; }
        }

        public class ApproveContentTemplateConfig {            
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "category")]
            public string Category { get; set; } = ContentTemplateCategory.UTILITY;
        }

        public class CallInfo
        {
            public string SId { get; set; }
            public string AccountSId { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string Status { get; set; }  // queued, ringing, in-progress, canceled, completed, failed, busy, no-answer
            public string AnsweredBy { get; set; }  // machine_start, human, fax, unknown (note! must to enable 'machineDetection')
            public string CallerName { get; set; }
            public string Duration { get; set; }
            public DateTime? EndTime { get; set; }
            public DateTime? StartTime { get; set; }

            public CallInfo(CallResource Resource)
            {
                this.SId = Resource.Sid;
                this.AccountSId = Resource.AccountSid;
                this.From = Resource.From;
                this.To = Resource.To;
                this.Status = Resource.Status.ToString();
                this.AnsweredBy = Resource.AnsweredBy;
                this.CallerName = Resource.CallerName;
                this.Duration = Resource.Duration;
                this.EndTime = Resource.EndTime;
                this.StartTime = Resource.StartTime;
            }
        }

        public class MessageInfo
        {
            public string SId { get; set; }
            public string AccountSId { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string Status { get; set; }  // queued, in-progress, completed, failed
            public string Body { get; set; }
            public string Price { get; set; }
            public string PriceUnit { get; set; }  // usd, eur, jpy, etc. (https://www.iso.org/iso/home/standards/currency_codes.htm)
            public int? ErrorCode { get; set; }  // failed, undelivered or null (successful)
            public string ErrorMessage { get; set; }
            public string Uri { get; set; }
            public DateTime? DateUpdated { get; set; }
            public DateTime? DateSent { get; set; }
            public DateTime? DateCreated { get; set; }
            public int NumMedia { get; set; }  // the number of media files associated with the message
            public int NumSegments { get; set; }  // the number of segments that make up the complete message, up to 160 characters per a single message-part

            public MessageInfo(MessageResource Resource)
            {
                this.SId = Resource.Sid;
                this.AccountSId = Resource.AccountSid;
                this.From = Resource.From.ToString();
                this.To = Resource.To;
                this.Status = Resource.Status.ToString();
                this.Body = Resource.Body;
                this.Price = Resource.Price;
                this.PriceUnit = Resource.PriceUnit;
                this.ErrorCode = Resource.ErrorCode;
                this.ErrorMessage = Resource.ErrorMessage;
                this.Uri = Resource.Uri;
                this.DateUpdated = Resource.DateUpdated;
                this.DateSent = Resource.DateSent;
                this.DateCreated = Resource.DateCreated;

                int numMedia, numSegments;
                this.NumMedia = int.TryParse(Resource.NumMedia, out numMedia) ? numMedia : 0;
                this.NumSegments = int.TryParse(Resource.NumSegments, out numSegments) ? numSegments : 1;
            }
        }

        [Obsolete("Use ResourceInfo Instead")]
        public class TemplateInfo
        {
            public string SId { get; set; }
            public string AccountSId { get; set; }
            public string Name { get; set; }
            public List<string> Channels { get; set; }  // sms, voice, etc.
            public Translation BodyEN { get; set; }
            public Translation BodyHE { get; set; }

            public string Body
            {
                get
                {
                    return this.BodyHE?.Text ?? this.BodyEN?.Text ?? "";
                }
            }

            public TemplateInfo(TemplateResource Resource)
            {
                this.SId = Resource.Sid;
                this.AccountSId = Resource.AccountSid;
                this.Name = Resource.FriendlyName;
                this.Channels = Resource.Channels;

                var translations = JsonConvert.DeserializeObject<Dictionary<string, Translation>>(Resource.Translations.ToString());
                this.BodyEN = translations.ContainsKey("en") ? translations["en"] : null;
                this.BodyHE = translations.ContainsKey("he") ? translations["he"] : null;
            }
        }

        public class ResourceInfo
        {
            public string SId { get; set; }
            public string AccountSId { get; set; }
            public string Name { get; set; }
            public string Language { get; set; }
            public string Body { get; set; }
            public IEnumerable<string> Channels { get; set; }  // twilio/text, twilio/media, etc.
            public Dictionary<string, string> ContentVariables { get; set; }
            public string ApprovalStatus { get; set; }

            public ResourceInfo(ContentResource Resource) {
                this.SId = Resource.Sid;
                this.AccountSId = Resource.AccountSid;
                this.Name = Resource.FriendlyName;
                this.Language = Resource.Language;

                var dicChannels = JsonConvert.DeserializeObject<Dictionary<string, object>>(Resource.Types.ToString());
                this.Channels = dicChannels.Keys;

                var bodySchema = new
                {
                    body = ""
                };

                var converted = JsonConvert.DeserializeAnonymousType(dicChannels.FirstOrDefault().Value.ToString(), bodySchema);
                this.Body = converted.body;

                this.ContentVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resource.Variables.ToString());
            }

            public ResourceInfo(ContentAndApprovalsResource Resource)
            {
                this.SId = Resource.Sid;
                this.AccountSId = Resource.AccountSid;
                this.Name = Resource.FriendlyName;
                this.Language = Resource.Language;

                var dicChannels = JsonConvert.DeserializeObject<Dictionary<string, object>>(Resource.Types.ToString());
                this.Channels = dicChannels.Keys;

                var bodySchema = new {
                    body = ""
                };

                var converted = JsonConvert.DeserializeAnonymousType(dicChannels.FirstOrDefault().Value.ToString(), bodySchema); 
                this.Body = converted.body;

                this.ContentVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resource.Variables.ToString());

                var dicApprovalRequests = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resource.ApprovalRequests.ToString());
                this.ApprovalStatus = dicApprovalRequests.ContainsKey("status") ? dicApprovalRequests["status"] : "";
            }
        }

        public class Translation
        {
            /*                                
                {
                    "en": {
                    "is_default_translation": true,
                    "status": "approved",
                    "to_be_deleted": false,
                    "reject_reason": "",
                    "locale": "en",
                    "text": "Your {{friendly_name}} verification code is: {{code}}. This code will expire in {{ttl}} minutes.",
                    "date_updated": "2021-07-29T20:38:28.641258955Z",
                    "temporal_text": "",
                    "date_created": "2021-07-29T20:38:28.219170887Z"
                    },
                    "nl": {
                    "is_default_translation": false,
                    "status": "approved",
                    "to_be_deleted": false,
                    "reject_reason": "",
                    "locale": "nl",
                    "text": "Uw {{friendly_name}}-verificatiecode is: {{code}}.. Deze code verloopt over {{ttl}} minuten. ",
                    "date_updated": "2022-12-02T21:05:56.521280234Z",
                    "temporal_text": "",
                    "date_created": "2022-12-02T21:05:07.607927375Z"
                    },
                    "ja": {
                    "is_default_translation": false,
                    "status": "approved",
                    "to_be_deleted": false,
                    "reject_reason": "",
                    "locale": "ja",
                    "text": "あなたの{{friendly_name}} 認証コード： {{code}}. このコードは、{{ttl}} 分間有効です。",
                    "date_updated": "2022-12-02T21:05:56.521280234Z",
                    "temporal_text": "",
                    "date_created": "2022-12-02T21:05:07.607927375Z"
                    }
                }
            */

            [JsonProperty(PropertyName = "is_default_translation")]
            public bool IsDefault { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "reject_reason")]
            public string RejectReason { get; set; }

            [JsonProperty(PropertyName = "locale")]
            public string Locale { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            [JsonProperty(PropertyName = "date_updated")]
            public DateTime? DateUpdated { get; set; }

            [JsonProperty(PropertyName = "date_created")]
            public DateTime? DateCreated { get; set; }
        }
    }

    // TODO ->> Async Version
    public interface ITwilioManager {
        string SendSMS(string sTwilioPhone, string sToPhone, string Body);
        string SendWAMessage(string sTwilioPhone, string sToPhone, string Body, List<string> Images, string StatusCallbackURL);
        string ScheduleWAMessage(string sTwilioPhone, string sToPhone, string Body, string MessagingServiceSid, DateTime SendAt);
        string UnScheduleWAMessage(string Sid);
        bool DeleteMessage(string Sid);
        string MakeACall(string sTwilioPhone, string sToPhone, string audioFile, bool useMachineDetection);
        string MakeACall(string sTwilioPhone, string sToPhone, IEnumerable<string> audioFiles, bool useMachineDetection);
        string MakeACall(string sTwilioPhone, string sToPhone, Uri xmlPathURI, bool useMachineDetection);
        string MakeACallExtended(string sTwilioPhone, string sToPhone, string sTwimlXML, bool useMachineDetection);
        CallInfo GetCallInfo(string Sid);
        MessageInfo GetMessageInfo(string Sid);
        IEnumerable<MessageInfo> GetMessages(int PageSize, long Limit, DateTime? DateSent);
        ResourceInfo GetContentTemplate(string Filter);
        IEnumerable<ResourceInfo> GetContentTemplates(string Filter, int PageSize, long Limit);
        ResourceInfo GetContentResource(string Sid);
        IEnumerable<ResourceInfo> GetContentResources(int PageSize, long Limit);
        bool DeleteContentTemplate(string Sid);
        string SendContentTemplate(string sTwilioPhone, string sToPhone, string ContentSid, string MessagingServiceSid, Dictionary<string, string> ContentVariables);        
        string GetContentTemplateApprovalStatus(string ContentSid);
        (string ContentSid, string ApproveStatus) CreateTextContentTemplate(TemplateCreatorText TemplateCreator);
        string ApproveContentTemplate(string ContentSid, ApproveContentTemplateConfig Config);
    }

    public class TwilioManager : ITwilioManager
    {
        private string accountSid { get; set; }
        private string authToken { get; set; }
        private WebClient client { get; set; } = new WebClient {
            Proxy = null,
            Encoding = Encoding.UTF8
        };

        public TwilioManager(string accountSid, string authToken) {
            this.accountSid = accountSid;
            this.authToken = authToken;

            TwilioClient.Init(accountSid, authToken);
        }

        public string SendSMS(string sTwilioPhone, string sToPhone, string Body) {
            /*
                REFERENCE:                
                https://www.twilio.com/docs/glossary/what-is-a-webhook
                https://www.twilio.com/docs/usage/webhooks/sms-webhooks
                https://www.twilio.com/docs/usage/webhooks

                DASHBOARD:                
                Home -> Programmable Messaging
                
                WEBHOOK:
                1. https://www.twilio.com/console/phone-numbers
                2. choose a Twilio Number 
                3. (tab) Messaging > set "CONFIGURE WITH" to "WebHooks" > set WebHook URL                
            */

            var fromPhone = new Twilio.Types.PhoneNumber(CleanPhoneNumber(sTwilioPhone));  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber(CleanPhoneNumber(sToPhone)); 

            var message = MessageResource.Create(
                body: Body,
                from: fromPhone,
                to: toPhone
            );

            return message.Sid;
        }

        public string SendWAMessage(string sTwilioPhone, string sToPhone, string Body, List<string> Images = null, string StatusCallbackURL = null) {
            var fromPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sTwilioPhone)}");  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sToPhone)}"); 

            var message = MessageResource.Create(
                body: Body,
                from: fromPhone,
                to: toPhone,
                mediaUrl: Images == null ? null : Images.Select(x => new Uri(x)).ToList(),
                statusCallback: StatusCallbackURL == null ? null : new Uri(StatusCallbackURL)
            );

            return message.Sid;
        }

        public string ScheduleWAMessage(string sTwilioPhone, string sToPhone, string Body, string MessagingServiceSid, DateTime SendAt)
        {
            var ts = SendAt - DateTime.Now;
            if (ts.TotalSeconds < 900) throw new Exception("Invalid schedule time, must be at least 900 sec from now");

            var fromPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sTwilioPhone)}");  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sToPhone)}");

            var message = MessageResource.Create(
                body: Body,
                from: fromPhone,
                to: toPhone,
                messagingServiceSid: MessagingServiceSid,
                sendAt: SendAt.ToUniversalTime(),
                scheduleType: MessageResource.ScheduleTypeEnum.Fixed
            );

            return message.Sid;
        }

        public string UnScheduleWAMessage(string Sid)
        {
            var message = MessageResource.Update(
                status: MessageResource.UpdateStatusEnum.Canceled,
                pathSid: Sid
            );
            return message.Sid;
        }

        public bool DeleteMessage(string Sid)
        {            
            var sucess = MessageResource.Delete(                
                pathSid: Sid
            );
            return sucess;
        }

        public string MakeACall(string sTwilioPhone, string sToPhone, string audioFile, bool useMachineDetection = false) {
            var sTwimlXML = $"<Response><Play loop=\"1\">{audioFile}</Play></Response>";
            return MakeACallExtended(sTwilioPhone, sToPhone, sTwimlXML, useMachineDetection);
        }
        public string MakeACall(string sTwilioPhone, string sToPhone, IEnumerable<string> audioFiles, bool useMachineDetection = false)
        {
            var sTwimlXML = new StringBuilder();
            sTwimlXML.Append("<Response>");
            foreach(var audioFile in audioFiles)
                sTwimlXML.Append($"<Play loop=\"1\">{audioFile}</Play>");
            sTwimlXML.Append("</Response>");

            return MakeACallExtended(sTwilioPhone, sToPhone, sTwimlXML.ToString(), useMachineDetection);
        }
        public string MakeACall(string sTwilioPhone, string sToPhone, Uri xmlPathURI, bool useMachineDetection = false)
        {
            var call = CallResource.Create(               
                url: xmlPathURI,  // as outer xml -  e.g: new Uri("http://demo.twilio.com/docs/voice.xml")

                // detects who answered the call (machine_start, human, fax, unknown)
                // https://www.twilio.com/docs/voice/answering-machine-detection
                machineDetection: useMachineDetection ? "Enable" : null,

                from: new Twilio.Types.PhoneNumber(sTwilioPhone),
                to: new Twilio.Types.PhoneNumber(sToPhone)
            );

            return call.Sid;
        }

        /*
           source:
           https://www.twilio.com/docs/voice/twiml

           <Say> - Read provided text 
           <Play> - Play an audio file
           <Dial> - Add another party to the call
           <Record> - Record the caller's voice
           <Gather> - Collect digits the caller types on their keypad

           ---

           sample:
           <Response>
               <Say voice=""alice"">Hello World</Say>
               <Play loop=""1"">https://media-server.com/sample1.wav</Play>
               <Play loop=""2"">https://media-server.com/sample2.wav</Play>
           </Response> 
       */
        public string MakeACallExtended(string sTwilioPhone, string sToPhone, string sTwimlXML, bool useMachineDetection = false)
        {
            var call = CallResource.Create(
                twiml: new Twilio.Types.Twiml(sTwimlXML), // as inline xml

                // detects who answered the call (machine_start, human, fax, unknown)
                // https://www.twilio.com/docs/voice/answering-machine-detection
                machineDetection: useMachineDetection ? "Enable" : null,

                // phone must include country prefix (e.g: +972)
                from: new Twilio.Types.PhoneNumber(sTwilioPhone),
                to: new Twilio.Types.PhoneNumber(sToPhone)

                /// status callback
                /// https://support.twilio.com/hc/en-us/articles/223132547-What-are-the-Possible-Call-Statuses-and-What-do-They-Mean-
                /// statusCallback: new Uri("https://webhook.site/dae1c747-f7f7-405c-9941-bf49bf76ad00")  
            );

            return call.Sid;
        }

        public CallInfo GetCallInfo(string Sid)
        {
            // https://www.twilio.com/docs/voice/api/call-resource
            // https://www.twilio.com/docs/voice/api/call-resource#fetch-a-call-resource
            var call = CallResource.Fetch(Sid);
            return new CallInfo(call);
        }

        public MessageInfo GetMessageInfo(string Sid)
        {
            // https://www.twilio.com/docs/sms/api/message-resource
            // https://www.twilio.com/docs/sms/api/message-resource#fetch-a-message-resource
            var message = MessageResource.Fetch(Sid);
            return new MessageInfo(message);
        }

        public IEnumerable<MessageInfo> GetMessages(int PageSize = 20, long Limit = 200, DateTime? DateSent = null)
        {
            /*
                filters:
                - pageSize 
                - accountSid
                - from
                - to
                - dateSent
            */

            var messages = MessageResource.Read(new ReadMessageOptions { 
                PageSize = PageSize,
                Limit = Limit,
                DateSent = DateSent
            });

            return messages.Select(x => new MessageInfo(x));
        }

        public ResourceInfo GetContentTemplate(string Filter) {
            return this.GetContentTemplates(Filter)?.FirstOrDefault();
        }
        
        public IEnumerable<ResourceInfo> GetContentTemplates(string Filter = null, int PageSize = 20, long Limit = 200)
        {
            var templates = this.GetContentResources(PageSize, Limit);
            if (Filter != null) return templates?.Where(t => t.SId == Filter || t.Name.Equals(Filter, StringComparison.OrdinalIgnoreCase)); // search by name or id
            return templates;
        }

        public ResourceInfo GetContentResource(string Sid) {
            var resource = ContentResource.Fetch(Sid);
            return new ResourceInfo(resource);
        }

        public IEnumerable<ResourceInfo> GetContentResources(int PageSize = 20, long Limit = 200) {
            /// ContentResource
            /// ContentAndApprovalsResource
            
            var resources = ContentAndApprovalsResource.Read(new ReadContentAndApprovalsOptions
            {
                PageSize = PageSize,
                Limit = Limit
            });

            return resources.Select(c => new ResourceInfo(c));
        }

        public bool DeleteContentTemplate(string Sid)
        {
            return ContentResource.Delete(Sid);
        }

        public string SendContentTemplate(string sTwilioPhone, string sToPhone, string ContentSid, string MessagingServiceSid, Dictionary<string, string> ContentVariables = null)
        {
            var fromPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sTwilioPhone)}");  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sToPhone)}");

            var message = MessageResource.Create(                
                from: fromPhone,
                to: toPhone,
                contentVariables: JsonConvert.SerializeObject(ContentVariables ?? new Dictionary<string, string>()), 
                contentSid: ContentSid,
                messagingServiceSid: MessagingServiceSid
            );

            return message.Sid;
        }
   
        public string GetContentTemplateApprovalStatus(string ContentSid)
        {
            /// this.GetContentResource(ContentSid);
            
            var resource = this.GetContentTemplate(ContentSid);            
            return resource?.ApprovalStatus ?? "";
        }

        // TODO ->> Implement More Types
        public (string ContentSid, string ApproveStatus) CreateTextContentTemplate(TemplateCreatorText TemplateCreator) 
        {
            /*
                https://www.twilio.com/docs/content/twilio-text
                https://www.twilio.com/docs/content/twilio-media
                https://www.twilio.com/docs/content/twilio-location
                https://www.twilio.com/docs/content/twiliolist-picker
                https://www.twilio.com/docs/content/twilio-call-to-action
                https://www.twilio.com/docs/content/twilio-quick-reply
                https://www.twilio.com/docs/content/twiliocard
                https://www.twilio.com/docs/content/whatsappauthentication
            */
            
            var result = this.POST($"https://content.twilio.com/v1/Content", TemplateCreator, Headers: new Dictionary<string, string> {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{accountSid}:{authToken}"))}"
            });

            var resultSchema = new {
                sid = ""
            };

            var templateId = JsonConvert.DeserializeAnonymousType(result.Content, resultSchema)?.sid;

            var approveStatus = this.ApproveContentTemplate(templateId, new ApproveContentTemplateConfig { 
                Name = TemplateCreator.Name,
                Category = ContentTemplateCategory.UTILITY
            });

            return (templateId, approveStatus);
        }
        
        public string ApproveContentTemplate(string ContentSid, ApproveContentTemplateConfig Config) {
            /*
                https://www.twilio.com/docs/content/content-api-resources#submit-template-approval-for-whatsapp        
                -
                Response:
                {
                  "category": "UTILITY",
                  "status": "received",
                  "name": "template_101",
                  "allow_category_change": true,
                  "content_type": "twilio/text",
                  "rejection_reason": ""
                }
            */

            var result = this.POST($"https://content.twilio.com/v1/Content/{ContentSid}/ApprovalRequests/whatsapp", Config, Headers: new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{accountSid}:{authToken}"))}"
            });

            var resultSchema = new {
                status = ""
            };

            return JsonConvert.DeserializeAnonymousType(result.Content, resultSchema)?.status;
        }

        // -------------

        private string CleanPhoneNumber(string PhoneNumber) {
            return PhoneNumber.Replace(" ", string.Empty);
        }
        
        private (bool Success, HttpStatusCode StatusCode, string Content) POST<T>(string URL, T Payload, string PayloadMode = "JSON" /*JSON|DATA*/, string Method = "POST", string QueryString = null, Dictionary<string, string> Headers = null) {
            try
            {
                client.Headers.Clear();
                if (Headers != null)
                    foreach (var header in Headers)
                        client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(QueryString))
                    URL = string.Concat(URL, "?", QueryString);

                string response = null;

                // as form-variables (aka post-data)
                if (PayloadMode == "DATA")
                {
                    response = Encoding.UTF8.GetString(client.UploadData(URL, Method, Encoding.UTF8.GetBytes(Payload.ToString())));
                }
                else // as json payload                
                {
                    var payloadType = Payload.GetType();
                    string sPayload;
                    if (payloadType.IsPrimitive || payloadType == typeof(System.String))
                        sPayload = Payload.ToString();
                    else
                        sPayload = JsonConvert.SerializeObject(Payload);

                    response = client.UploadString(URL, Method, sPayload);
                }

                return (true, HttpStatusCode.OK, response);
            }
            catch (WebException ex)
            {
                var webRespEX = ((HttpWebResponse)ex.Response);
                var statusCode = webRespEX?.StatusCode ?? HttpStatusCode.BadRequest;

                var stream = ex?.Response?.GetResponseStream();
                if (stream == null) return (false, statusCode, $"{ex.Message}");

                using (var reader = new StreamReader(stream))
                    return (false, statusCode, $"{ex.Message} | {reader.ReadToEnd()}");
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
