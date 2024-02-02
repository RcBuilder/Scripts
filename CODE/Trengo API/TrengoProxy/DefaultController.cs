using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using TrengoApi;

namespace TrengoProxy.Controllers
{
    public class DefaultController : ApiController
    {
        private static readonly string PAYLOADS_FOLDER = $"{AppDomain.CurrentDomain.BaseDirectory}Payloads\\";

        [HttpGet]
        [Route("")]
        public HttpResponseMessage Who()
        {
            return Request.CreateResponse("TrengoProxy");
        }
        
        [HttpPost]
        [Route("konimbo/hook")]
        public async Task<HttpResponseMessage> KonimboOrderAfterPaymentHook([ModelBinder(typeof(KonimboHookRequestBinder))]KonimboHookRequest HookData)
        {
            var step = 0;

            var orderId = HookData.Order?.OrderId ?? "0";

            /* DUMP */
            // log payload

            var logId = Guid.NewGuid();
            try
            {
                if (!Directory.Exists(PAYLOADS_FOLDER))
                    Directory.CreateDirectory(PAYLOADS_FOLDER);

                var payload = "";
                using (var stream = await Request.Content.ReadAsStreamAsync())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(stream))
                        payload = sr.ReadToEnd();
                }

                File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_IN.txt", payload);
                File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_model_IN.txt", JsonConvert.SerializeObject(HookData));
            }
            catch { }

            try
            {
                step = 1;

                if (!HookData.IsPaid)
                    throw new Exception("Order is not paid!");

                step = 2;

                /*
                if (!HookData.IsNewsletterEnabled)
                    throw new Exception("Newsletter flag is disabled!");
                */

                step = 3;
            
                var manager = new TrengoApiManager(new TrengoApiConfig
                {
                    ApiUrl = ConfigurationManager.AppSettings["TrengoApiUrl"].Trim(),
                    ApiKey = ConfigurationManager.AppSettings["TrengoApiKey"].Trim()
                });

                step = 4;

                /// 1306497 = wa_business
                const int WA_BUSINESS_CHANNEL = 1306497;
                const int KONIMBO_PROFILE = 13714855;

                var contactRequest = new CreateContactRequest(HookData.PhoneFormatted, HookData.Order.Name, WA_BUSINESS_CHANNEL);
                var contactId = await manager.CreateContact(contactRequest);

                step = 5;

                /// 609842 = contact_name
                /// 609835 = phone
                /// 609825 = customer number
                /// 609992 = customer_123
                /// 614854 = contact email

                var customField1Request = new SetContactCustomFieldRequest(contactId, 609842, HookData.Order.Name);
                var customField1Id = await manager.SetContactCustomField(customField1Request);

                step = 6;

                var customField2Request = new SetContactCustomFieldRequest(contactId, 609835, HookData.PhoneFormatted);
                var customField2Id = await manager.SetContactCustomField(customField2Request);

                step = 7;

                var customField3Request = new SetContactCustomFieldRequest(contactId, 609825, HookData.Order.CustomerId);
                var customField3Id = await manager.SetContactCustomField(customField3Request);

                step = 8;

                var customField4Request = new SetContactCustomFieldRequest(contactId, 614854, HookData.Order.Email);
                var customField4Id = await manager.SetContactCustomField(customField4Request);

                step = 9;

                var contactNoteRequest = new CreateContactNoteRequest(contactId, HookData.ToString());
                var contactNoteId = await manager.CreateContactNote(contactNoteRequest);

                step = 10;
                
                var profileRequest = new SetContactProfileRequest(contactId, KONIMBO_PROFILE);
                var profileSuccess = await manager.SetContactProfile(profileRequest);

                step = 11;

                var ticketRequest = new CreateTicketRequest(contactId, WA_BUSINESS_CHANNEL, $"KONIMBO INCOMING ORDER #{orderId}");
                var ticketId = await manager.CreateTicket(ticketRequest);

                step = 12;

                /// 1683642 = Urgent
                /// 1701915 = Konimbo
                /// 1708877 = NoSubscription

                if(!HookData.IsNewsletterEnabled)
                    await manager.LabelATicket(new LabelATicketRequest(ticketId, 1708877));                

                var ticketLabelRequest = new LabelATicketRequest(ticketId, 1701915); 
                var ticketLabelSuccess = await manager.LabelATicket(ticketLabelRequest);

                step = 13;

                var ticketMessageRequest = new CreateTicketMessageRequest(ticketId, HookData.ToString(), true);
                var ticketMessageId = await manager.CreateTicketMessage(ticketMessageRequest);

                var result = new
                {
                    contactRequest,
                    contactId,
                    customField1Request,
                    customField1Id,
                    customField2Request,
                    customField2Id,
                    customField3Request,
                    customField3Id,
                    customField4Request,
                    customField4Id,
                    contactNoteRequest,
                    contactNoteId,
                    profileRequest,
                    profileSuccess,
                    ticketRequest,
                    ticketId,
                    ticketLabelRequest,
                    ticketLabelSuccess,
                    ticketMessageRequest,
                    ticketMessageId
                };

                try
                {
                    File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_model_OUT.txt", JsonConvert.SerializeObject(result));
                }
                catch { }

                step = 14;

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                var error = $"[#{orderId}] {ex.Message} [step={step}]";
                this.WriteLog(error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        // ----

        private bool WriteLog(string Message)
        {
            try
            {
                File.AppendAllText($"{PAYLOADS_FOLDER}ErrorLog_{DateTime.Now.ToString("yyyyMMdd")}.txt", Message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
