using System;

// Install-Package Twilio -Version 5.53.1
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Twilio
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
            var fromPhone = new Twilio.Types.PhoneNumber(sTwilioPhone);  // https://www.twilio.com/console/phone-numbers
            var toPhone = new Twilio.Types.PhoneNumber(sToPhone); 

            var message = MessageResource.Create(
                body: Body,

                from: fromPhone,
                to: toPhone
            );

            return message.Sid;
        }

        public string MakeACall(string sTwilioPhone, string sToPhone, string audioFile, bool useMachineDetection = false) {
            var call = CallResource.Create(                                
                twiml: new Twilio.Types.Twiml($"<Response><Play loop=\"1\">{audioFile}</Play></Response>"), // as inline xml

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
    }
}
