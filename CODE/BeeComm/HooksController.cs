using BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WEB.API
{
    public class HooksController : ApiController
    {
        /*
        {
          "restaurantName": "ג.א.א יעוץ וניהול בע\"מ",
          "businessNumber": "123456789",
          "orderID": 12354,
          "amountPaid": 58.0,
          "orderInfo": {
            "actionType": null,
            "OrderType": 0,
            "FirstName": "Flora Leandra B",
            "LastName": null,
            "Phone": "0",
            "Remarks": "Wolt  מספר הזמנה:850 Flora Leandra B  זמן להכנה : 20:00",
            "DiscountSum": 0.0,
            "OuterCompId": 66,
            "OuterCompOrderId": "61d4876c4c7996a5c5813263",
            "Items": [
              {
                "NetID": 203,
                "ItemName": "קוורי",
                "Quantity": 1.0,
                "Price": 46.0,
                "UnitPrice": 46.0,
                "Remarks": null,
                "BelongTo": "Flora Leandra B",
                "BillRemarks": null,
                "SubItems": [],
                "Toppings": [],
                "outerUniqeId": null,
                "cancelItem": false,
                "updateItem": false
              },
              {
                "NetID": 813,
                "ItemName": "ספרייט זירו",
                "Quantity": 1.0,
                "Price": 12.0,
                "UnitPrice": 12.0,
                "Remarks": null,
                "BelongTo": "Flora Leandra B",
                "BillRemarks": null,
                "SubItems": [],
                "Toppings": [],
                "outerUniqeId": null,
                "cancelItem": false,
                "updateItem": false
              }
            ],
            "Payments": [
      
            ],
            "DeliveryInfo": null,
            "Dinners": 1,
            "ArrivalTime": null,
            "Email": null,
            "Tip": 0.0,
            "TipPercent": 0.0,
            "tableNumber": 0,
            "cancelOrder": false,
            "updateOrder": false,
            "updateDiscount": false,
            "recoverOrder": false,
            "relatedOrderId": 0
          }
        }
        */


        [HttpPost]
        [Route("api/hook/beecomm")]
        public async Task<HttpResponseMessage> BeeCommHook([FromBody]BeeComm.OrderHook model)
        {
            try
            {
                LoggerSingleton.Instance.Info("BeeCommHook", $"Order Arrived");

                /* DUMP */
                try
                {
                    var payload = "";
                    using (var stream = await Request.Content.ReadAsStreamAsync())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(stream))
                            payload = sr.ReadToEnd();
                    }
                    File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}OrdersBK\\payload_{Guid.NewGuid()}.txt", payload);
                }
                catch { }

                if (model.Details.SenderCompanyId == ConfigSingleton.Instance.BeecommSenderCompanyId) // MNEW 
                    return Request.CreateResponse(HttpStatusCode.OK);
                
                if(string.IsNullOrEmpty(model.CustomerId))
                    model.CustomerId = "5fc517cb898f9173dbf4adc6"; // TODO ->> Temporary

                var order = await OrdersBLL.BeeCommOrder2Order(model);
                var orderId = await new OrdersBLL().Create(order, false);
                LoggerSingleton.Instance.Info("BeeCommHook", $"Order {orderId} has created", new List<string> {
                    JsonConvert.SerializeObject(model)
                });

                File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}OrdersBK\\order_{Guid.NewGuid()}.txt", JsonConvert.SerializeObject(model));                
                
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LoggerSingleton.Instance.Error("BeeCommHook", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
