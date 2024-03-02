using GS1;
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

namespace GS1Proxy.Controllers
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
                if (HookData.IsPartialPaid)
                    throw new Exception("Order is partial paid!");

                step = 2;

                /*
                var gs1Manager = new GS1Manager(new GS1Entities.GS1Config
                {
                    ServerURL = ConfigurationManager.AppSettings["GS1ServerURL"].Trim(),
                    ApiUserName = ConfigurationManager.AppSettings["GS1ApiUserName"].Trim(),
                    ApiPassword = ConfigurationManager.AppSettings["GS1ApiPassword"].Trim()
                });
                */

                var gs1Manager = new GS1Manager(new GS1Entities.GS1Config
                {
                    ServerURL = "https://retailer.gs1ildigital.org/external",
                    ApiUserName = "xxxxxxxxxxxxxxx",
                    ApiPassword = "xxxxxxxxxxxxxxx"
                });

                var result = "TODO";

                step = 3;

                try
                {
                    File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_model_OUT.txt", JsonConvert.SerializeObject(result));
                }
                catch { }

                step = 4;

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
