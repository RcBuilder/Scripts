using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Tookan;

namespace TookanProxy.Controllers
{
    public class DefaultController : ApiController
    {
        private static readonly string PAYLOADS_FOLDER = $"{AppDomain.CurrentDomain.BaseDirectory}Payloads\\";        
        private static readonly (string Address, string Coordinates) BUSINESS_LOCATION = (
            ConfigurationManager.AppSettings["BusinessAddress"].Trim(), 
            ConfigurationManager.AppSettings["BusinessCoordinates"].Trim()
        );        

        [HttpGet]
        [Route("")]
        public HttpResponseMessage Who()
        {
            return Request.CreateResponse("TookanProxy");
        }

        [HttpPost]
        [Route("konimbo/hook")]
        public async Task<HttpResponseMessage> KonimboOrderAfterPaymentHook([ModelBinder(typeof(KonimboHookRequestBinder))]KonimboHookRequest HookData)
        {            
            var step = 1;

            var orderId = HookData.Order?.OrderId ?? "0";

            /* DUMP */
            // log payload
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

                var logId = Guid.NewGuid();
                File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}.txt", payload);
                File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_model.txt", JsonConvert.SerializeObject(HookData));
            }
            catch { }

            step = 2;

            if (!HookData.IsPaid)
                throw new Exception("Order is not paid");

            step = 3;

            // send order to tookan delivery system via api 
            try
            {
                var manager = new TookanManager(new TookanConfig
                {
                    ApiUrl = ConfigurationManager.AppSettings["TookanApiUrl"].Trim(),
                    ApiKey = ConfigurationManager.AppSettings["TookanApiKey"].Trim()
                });

                step = 4;

                var response = await manager.CreatePickupDeliveryTask(
                    new PickupDeliveryTaskRequest{                        
                        OrderId = orderId,                        
                        JobDescription = HookData.Order.ExtraInfo.Notes,
                        JobPickupPhone = HookData.Order.Phone,
                        JobPickupName = HookData.Order.Name,
                        JobPickupEmail = HookData.Order.Email,
                        JobPickupAddress = BUSINESS_LOCATION.Address, // restaurant address 
                        JobPickupLatitude = BUSINESS_LOCATION.Coordinates.Split(',')[0],  // coordinates
                        JobPickupLongitude = BUSINESS_LOCATION.Coordinates.Split(',')[1], // coordinates
                        JobPickupDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        JobDeliveryDatetime = HookData.Order.ExtraInfo.Date,
                        CustomerAddress = HookData.Order.ExtraInfo.Address,
                        CustomerEmail = "",
                        CustomerPhone = HookData.Order.ExtraInfo.Phone,
                        CustomerUsername = HookData.Order.ExtraInfo.Name,
                        Notify = 1,
                        MetaDataCollection = new List<MetaDataItem> {                             
                            new MetaDataItem("Status", HookData.Order.PaymentStatus),
                            new MetaDataItem("StoreId", HookData.Order.StoreId.ToString()),
                            new MetaDataItem("CustomerId", HookData.Order.CustomerId)
                        }
                    }
                );

                step = 5;

                Debug.WriteLine($"[{response.Status}] {response.Message}");

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var error = $"[#{orderId}] {ex.Message} [step={step}]";
                this.WriteLog(error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        // ----

        private bool WriteLog(string Message) {
            try {
                File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}Log_{DateTime.Now.ToString("yyyyMMdd")}.txt", Message);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
