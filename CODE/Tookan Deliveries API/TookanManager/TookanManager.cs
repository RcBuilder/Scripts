using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tookan
{
    /*
        Reference:
        https://tookanapi.docs.apiary.io/#reference
        https://tookanapi.docs.apiary.io/#reference/task/create-task/create-a-pickup-and-delivery-task

        Dashboard:
        https://app.tookanapp.com/

        Postman:
        Tookan deliveries

        -

        Custom Fields:
        in order to pass custom fields, we need to create a template via the tookan's dashboard! 
        add some custom fields to the created template. 
        once created use the very same name as the 'meta_data' and set the template name as 'custom_field_template' 
        
        ...
        ...
        "custom_field_template": "Konimbo-Order",
        "meta_data": [
            {
                "label": "Price",
                "data": "100"
            },
            {
                "label": "Quantity",
                "data": "3"
            }
        ]
        ...
        ...

        -

        Using:        
        var manager = new TookanManager(new TookanConfig
        {
            ApiUrl = "https://api.tookanapp.com/",            
            ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxx"            
        });

        var response = await manager.CreatePickupDeliveryTask(
            new PickupDeliveryTaskRequest{ 
                ...
            }
        );

        Console.WriteLine($"[{response.status}] {response.message}");

        -

        * Samle Request:
        POST https://api.tookanapp.com/v2/create_task
        H Content-Type:application/json
        {
          "api_key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
          "order_id": "12345",
          "team_id": "",
          "auto_assignment": "0",
          "job_description": "משלוח בדיקה",
          "job_pickup_phone": "+97254-561-4020",
          "job_pickup_name": "חנות ההפתעות של רובי",
          "job_pickup_email": "",
          "job_pickup_address": "וייצמן 6 כפר סבא",
          "job_pickup_latitude": "32.178090",
          "job_pickup_longitude": "34.895440",
          "job_pickup_datetime": "2023-06-01 12:00:00",
          "customer_email": "john@example.com",
          "customer_username": "John Doe",
          "customer_phone": "",
          "customer_address": "קניון ערים כפר סבא",
          "latitude": "32.187809",
          "longitude": "34.936199",
          "job_delivery_datetime": "2023-06-01 13:00:00",
          "has_pickup": "1",
          "has_delivery": "1",
          "layout_type": "0",
          "tracking_link": 1,
          "timezone": "+180",
          "custom_field_template": "Konimbo-Order",
          "meta_data": [
            {
              "label": "Price",
              "data": "100"
            },
            {
              "label": "Quantity",
              "data": "3"
            }
          ],
          "pickup_custom_field_template": "",
          "pickup_meta_data": [],
          "fleet_id": "",
          "p_ref_images": [],
          "ref_images": [
            "https://www.kasandbox.org/programming-images/avatars/old-spice-man.png",
            "https://www.kasandbox.org/programming-images/avatars/old-spice-man-blue.png"
          ],
          "notify": 1,
          "tags": "",
          "geofence": 0,
          "ride_type": 0
        } 

        -

        * Sample Response:   // success      
        {
          "message": "The task has been created.",
          "status": 200,
          "data": {
            "job_id": 481987441,
            "pickup_job_id": 481987441,
            "delivery_job_id": 481987442,
            "job_hash": "xxxxxxxxxxxxxxxxxxxxxxxxx",
            "pickup_job_hash": "xxxxxxxxxxxxxxxxxxxxxx",
            "delivery_job_hash": "xxxxxxxxxxxxxxxxxxxxxxx",
            "customer_name": "John Doe",
            "customer_address": "קניון ערים כפר סבא",
            "job_pickup_name": "חנות ההפתעות של רובי",
            "job_pickup_address": "וייצמן 6 כפר סבא",
            "job_token": "xxxxxxxxxxxxxxxxxxxxxxxx",
            "pickup_tracking_link": "https://jngl.ml/E2aD31a82",
            "delivery_tracing_link": "https://jngl.ml/8fE4474Vc",
            "order_id": "12345",
            "pickupOrderId": "12345",
            "deliveryOrderId": "12345",
            "pickupAddressNotFound": false,
            "deliveryAddressNotFound": false
          }
        }

        -

        * Sample Response:  // failure
        {
            "message":"Invalid API Key.",
            "status":101,
            "data":{}
        }
    */

    public class TookanManager : ITookanManager
    {
        protected TookanConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public TookanManager(TookanConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        public async Task<PickupDeliveryTaskResponse> CreatePickupDeliveryTask(PickupDeliveryTaskRequest Request)
        {
            Request.ApiKey = this.Config.ApiKey; // SET KEY
            var response = await this.HttpService.POST_ASYNC<PickupDeliveryTaskRequest, PickupDeliveryTaskResponse>(
                $"{this.Config.ApiUrl}v2/create_task", 
                Request, 
                null, 
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            return response.Model;
        }
    }
}
