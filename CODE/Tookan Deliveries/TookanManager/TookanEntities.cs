using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tookan
{   
    public class TookanConfig
    {
        public string ApiUrl { get; set; }                
        public string ApiKey { get; set; }               
    }

    public abstract class TookanBaseRequest
    {
        [JsonProperty(PropertyName = "api_key")]
        public string ApiKey { get; set; }
    }

    public abstract class TookanBaseResponse<T>
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }

    public class MetaDataItem
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Value { get; set; }
        
        public MetaDataItem(string Label, string Value) {
            this.Label = Label;
            this.Value = Value;
        }
    }

    /*
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
          "custom_field_template": "",
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
    */
    public class PickupDeliveryTaskRequest : TookanBaseRequest {
        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "team_id")]
        public int TeamId { get; set; }

        [JsonProperty(PropertyName = "auto_assignment")]
        public byte AutoAssignment { get; set; } = 0;

        [JsonProperty(PropertyName = "job_description")]
        public string JobDescription { get; set; }

        [JsonProperty(PropertyName = "job_pickup_phone")]
        public string JobPickupPhone { get; set; }

        [JsonProperty(PropertyName = "job_pickup_name")]
        public string JobPickupName { get; set; }

        [JsonProperty(PropertyName = "job_pickup_email")]
        public string JobPickupEmail { get; set; }

        [Required]
        [JsonProperty(PropertyName = "job_pickup_address")]
        public string JobPickupAddress { get; set; }

        [JsonProperty(PropertyName = "job_pickup_latitude")]
        public string JobPickupLatitude { get; set; }

        [JsonProperty(PropertyName = "job_pickup_longitude")]
        public string JobPickupLongitude { get; set; }

        [Required]
        [JsonProperty(PropertyName = "job_pickup_datetime")]
        public string JobPickupDatetime { get; set; }

        [JsonProperty(PropertyName = "customer_email")]
        public string CustomerEmail { get; set; }

        [JsonProperty(PropertyName = "customer_username")]
        public string CustomerUsername { get; set; }

        [JsonProperty(PropertyName = "customer_phone")]
        public string CustomerPhone { get; set; }

        [Required]
        [JsonProperty(PropertyName = "customer_address")]
        public string CustomerAddress { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [Required]
        [JsonProperty(PropertyName = "job_delivery_datetime")]
        public DateTime JobDeliveryDatetime { get; set; }

        [Required]
        [JsonProperty(PropertyName = "has_pickup")]
        public byte HasPickup { get; set; } = 1;

        [Required]
        [JsonProperty(PropertyName = "has_delivery")]
        public byte HasDelivery { get; set; } = 1;

        [Required]
        [JsonProperty(PropertyName = "layout_type")]
        public int LayoutType { get; set; } = 0;

        [JsonProperty(PropertyName = "tracking_link")]
        public byte TrackingLink { get; set; } = 1;

        [Required]
        [JsonProperty(PropertyName = "timezone")]
        public int Timezone { get; set; } = 180; // +180 minutes (UTC+3)

        [JsonProperty(PropertyName = "custom_field_template")]
        public string CustomFieldTemplate { get; set; }

        [JsonProperty(PropertyName = "meta_data")]
        public IEnumerable<MetaDataItem> MetaDataCollection { get; set; }

        [JsonProperty(PropertyName = "pickup_custom_field_template")]
        public string PickupCustomFieldTemplate { get; set; }

        [JsonProperty(PropertyName = "pickup_meta_data")]
        public IEnumerable<MetaDataItem> PickupMetaData { get; set; }

        [JsonProperty(PropertyName = "fleet_id")]
        public string FleetId { get; set; }

        [JsonProperty(PropertyName = "p_ref_images")]
        public IEnumerable<string> PickupRefImages { get; set; }

        [JsonProperty(PropertyName = "ref_images")]
        public IEnumerable<string> DeliveryRefImages { get; set; }

        [JsonProperty(PropertyName = "notify")]
        public int Notify { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }

        [JsonProperty(PropertyName = "geofence")]
        public int Geofence { get; set; }

        [JsonProperty(PropertyName = "ride_type")]
        public int RideType { get; set; }
    }

    /*
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

        {
            "message":"Invalid API Key.",
            "status":101,
            "data":{}
        }
    */
    public class PickupDeliveryTaskResponse : TookanBaseResponse<PickupDeliveryTaskData> { }

    public class PickupDeliveryTaskData {
        [JsonProperty(PropertyName = "job_id")]
        public int JobId { get; set; }

        [JsonProperty(PropertyName = "pickup_job_id")]
        public int PickupJobId { get; set; }

        [JsonProperty(PropertyName = "delivery_job_id")]
        public int DeliveryJobId { get; set; }

        [JsonProperty(PropertyName = "job_hash")]
        public string JobHash { get; set; }

        [JsonProperty(PropertyName = "pickup_job_hash")]
        public string PickupJobHash { get; set; }

        [JsonProperty(PropertyName = "delivery_job_hash")]
        public string DeliveryJobHash { get; set; }

        [JsonProperty(PropertyName = "customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty(PropertyName = "customer_address")]
        public string CustomerAddress { get; set; }

        [JsonProperty(PropertyName = "job_pickup_name")]
        public string JobPickupName { get; set; }
        
        [JsonProperty(PropertyName = "job_pickup_address")]
        public string JobPickupAddress { get; set; }

        [JsonProperty(PropertyName = "job_token")]
        public string JobToken { get; set; }

        [JsonProperty(PropertyName = "pickup_tracking_link")]
        public string PickupTrackingLink { get; set; }

        [JsonProperty(PropertyName = "delivery_tracing_link")]
        public string DeliveryTracingLink { get; set; }

        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "pickupOrderId")]
        public string PickupOrderId { get; set; }

        [JsonProperty(PropertyName = "deliveryOrderId")]
        public string DeliveryOrderId { get; set; }

        [JsonProperty(PropertyName = "pickupAddressNotFound")]
        public bool PickupAddressNotFound { get; set; }

        [JsonProperty(PropertyName = "deliveryAddressNotFound")]
        public bool DeliveryAddressNotFound { get; set; }
    }
}
