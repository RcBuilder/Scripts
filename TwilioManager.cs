using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Install-Package Twilio
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DistributionServiceBLL
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

        public string MakeACall(string sTwilioPhone, string sToPhone, string audioFile) {
            var call = CallResource.Create(                                
                twiml: new Twilio.Types.Twiml($"<Response><Play loop=\"1\">{audioFile}</Play></Response>"), // as inline xml

                from: new Twilio.Types.PhoneNumber(sTwilioPhone),
                to: new Twilio.Types.PhoneNumber(sToPhone)
            );

            return call.Sid;
        }

        public string MakeACall(string sTwilioPhone, string sToPhone, Uri xmlPathURI)
        {
            var call = CallResource.Create(
                url: xmlPathURI,  // as outer xml -  e.g: new Uri("http://demo.twilio.com/docs/voice.xml")

                from: new Twilio.Types.PhoneNumber(sTwilioPhone),
                to: new Twilio.Types.PhoneNumber(sToPhone)
            );

            return call.Sid;
        }
    }
}
