using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Install-Package Twilio
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace TwilioBLL
{
    /*
        USING
        -----
        var twilio = new TwilioManager("AC9c7aa7b4d4cb7a6034c694871912ee44", "xxxxxxxxxxxxxxxxxxxxxxxx");
        
        var message = "Hi, have a great and magical day!";
        var messageId = twilio.SendSMS("+1 423 285 6736", "+972 545614020", message);
        Debug.WriteLine(messageId);

        var audioPath = "http://domain.com/Audio/1MM.wav"
        var callId = this.Twilio.MakeACall("+1 423 285 6736", "+972 545614020", audioPath);
        Debug.WriteLine(callId);
        
        var xmlPath = "http://demo.twilio.com/docs/voice.xml";
        var callId = this.Twilio.MakeACall("+1 423 285 6736", "+972 545614020", new Uri(xmlPath));
        Debug.WriteLine(callId);
        
        var whatsappMessageId1 = Twilio.SendWhatsapp("+1 415 523 8886", "+972-54-561-4020", "HELLO FROM TWILIO API");
        Debug.WriteLine(whatsappMessageId1);

        var whatsappMessageId2 = Twilio.SendWhatsapp("+1 415 523 8886", "+972-54-561-4020", "HELLO FROM TWILIO API", new List<string> { 
            "https://static.remove.bg/sample-gallery/graphics/bird-thumbnail.jpg" 
        });
        Debug.WriteLine(whatsappMessageId3);
    */

    public class TwilioManager
    {
        #region TwilioEntities
        public class CallInfo
        {
            public string SId { get; set; }
            public string AccountSId { get; set; }
            public string AnsweredBy { get; set; }  // machine_start, human, fax, unknown (note! must to enable 'machineDetection')
            public string CallerName { get; set; }            
            public string Duration { get; set; }
            public DateTime? EndTime { get; set; }
            public DateTime? StartTime { get; set; }
            public string From { get; set; }
            public string To { get; set; }            
            public string Status { get; set; }  // queued, ringing, in-progress, canceled, completed, failed, busy, no-answer

            public CallInfo(CallResource CallResource) {
                this.SId = CallResource.Sid;
                this.AccountSId = CallResource.AccountSid;
                this.AnsweredBy = CallResource.AnsweredBy;
                this.CallerName = CallResource.CallerName;               
                this.Duration = CallResource.Duration;
                this.EndTime = CallResource.EndTime;
                this.StartTime = CallResource.StartTime;
                this.From = CallResource.From;
                this.To = CallResource.To;                
                this.Status = CallResource.Status.ToString();
            }
        }
        #endregion

        public TwilioManager(string accountSid, string authToken) {            
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

        public string SendWhatsapp(string sTwilioPhone, string sToPhone, string Body, List<string> Images = null) {
            /*
                REFERENCE:                
                https://www.twilio.com/docs/whatsapp/quickstart/csharp
                https://www.twilio.com/docs/whatsapp/api

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

                -------------
                
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

            var fromPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sTwilioPhone)}");  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber($"whatsapp:{CleanPhoneNumber(sToPhone)}"); 

            var message = MessageResource.Create(
                body: Body,

                from: fromPhone,
                to: toPhone,
                mediaUrl: Images == null ? null : Images.Select(x => new Uri(x)).ToList()

            );

            return message.Sid;
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
        public string MakeACallExtended(string sTwilioPhone, string sToPhone, string sTwimlXML, bool useMachineDetection = false) {


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

        public CallInfo GetCallInfo(string Sid)
        {
            // https://www.twilio.com/docs/voice/api/call-resource#fetch-a-call-resource
            var call = CallResource.Fetch(Sid);
            return new CallInfo(call);
        }

        // -------------

        private string CleanPhoneNumber(string PhoneNumber) {
            return PhoneNumber.Replace(" ", string.Empty);
        }
    }
}
