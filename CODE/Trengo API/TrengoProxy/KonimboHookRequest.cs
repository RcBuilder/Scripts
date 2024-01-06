using Newtonsoft.Json;
using System;
using System.Globalization;

namespace TrengoProxy
{
    // TODO ->> Implement - Set Models

    /*       
        {
          "order": {
            "customer_id": 11213910,
            "data_record_var": {},
            "store_id": 3002,
            "id": 14394243,
            "phone": "0546765695",
            "additional_inputs": null,
            "customer_orders_size": null,
            "event": "order_after_payment",
            "email": "lirazsofrin@gmail.com",
            "cart": {
              "lines": [
                {
                  "quantity": 1,
                  "item_id": 6236606,
                  "title": "תיק גב The EcoScenic™ ProX ",
                  "unit_price": 810,
                  "second_code": "https://detail.1688.com/offer/733277529845.html",
                  "offer_code": "",
                  "code": "BZ-CM01451",
                  "price": 810,
                  "line_item_id": 123564401,
                  "image_url": "https://konimboimages.s3.amazonaws.com/system/photos/13078316/cart/fa6261cfcfd93758cc8eef2849ff25d0.jpg",
                  "brand_title": "Camel Mountain",
                  "category_title": "תיקי גב",
                  "type": "line_item"
                },
                {
                  "quantity": 1,
                  "item_id": 6236606,
                  "title": "כמות",
                  "unit_price": 0,
                  "topic_title": "מלאי",
                  "inventory_code": "1",
                  "price": 0, 
                  "line_item_id": 123564401,
                  "type": "upgrade"
                }
              ],
              "shipping": {
                "price_with_intrest": 0,
                "quantity": 1,
                "title": "משלוח לנק' איסוף קרוב לכתובת (8-12 ימי עסקים)",
                "id": 48189,
                "data": {
                  "days": "10"
                },
                "var": {},
                "price": 0,
                "shipping_type": "not_cart_shipping",
                "store_default": false,
                "integration_type": "chita shipping Door2Door",
                "type": "shipping"
              },
              "total_price": 810
            },
            "newsletter": true,
            "payment_status": "אשראי - מלא",
            "name": "לירז סופרין",
            "full_url": "https://www.camelmountain.co.il",
            "affillate_field": ""
          }
          ---
          {
              "order": {
                "full_url": "https://www.camelmountain.co.il",
                "newsletter": true,
                "cart": {
                  "total_price": 299,
                  "lines": [
                    {
                      "code": "BZ-CM01255",
                      "offer_code": "",
                      "brand_title": "Camel Mountain",
                      "category_title": "תיקי גב",
                      "price": 299,
                      "type": "line_item",
                      "image_url": "https://konimboimages.s3.amazonaws.com/system/photos/12705938/cart/3964b42dc5412630be8b36d5c0a7ee2e.jpg",
                      "quantity": 1,
                      "item_id": 6074396,
                      "unit_price": 299,
                      "line_item_id": 123607360,
                      "title": "תיק גב The ProPulse™ Ultra",
                      "second_code": "https://detail.1688.com/offer/614809038474.html"
                    },
                    {
                      "price": 0,
                      "type": "upgrade",
                      "quantity": 1,
                      "inventory_code": "1",
                      "item_id": 6074396,
                      "unit_price": 0,
                      "line_item_id": 123607360,
                      "title": "כמות",
                      "topic_title": "מלאי"
                    }
                  ],
                  "shipping": {
                    "var": {},
                    "price": 0,
                    "store_default": false,
                    "type": "shipping",
                    "quantity": 1,
                    "id": 48189,
                    "price_with_intrest": 0,
                    "data": {
                      "days": "10"
                    },
                    "shipping_type": "not_cart_shipping",
                    "title": "משלוח לנק' איסוף קרוב לכתובת (8-12 ימי עסקים)",
                    "integration_type": "chita shipping Door2Door"
                  }
                },
                "store_id": 3002,
                "email": "arielsav77@gmail.com",
                "customer_id": 11219461,
                "id": 14399976,
                "name": "אריאל סביר",
                "data_record_var": {},
                "affillate_field": "35",
                "additional_inputs": null,
                "event": "order_after_payment",
                "payment_status": "אשראי - מלא",
                "phone": "0544662625",
                "customer_orders_size": null
              }
            }
        }
        ---
        {
          "order": {
            "name": "יפה אלצק",
            "customer_id": 11377873,
            "newsletter": false,
            "id": 14577197,
            "full_url": "https://www.camelmountain.co.il",
            "payment_status": "אשראי - מלא",
            "status_option_title": "הזמנה חדשה",
            "store_id": 3002,
            "customer_orders_size": null,
            "affillate_field": "",
            "email": "yafa774@gmail.com",
            "event": "order_after_payment",
            "additional_inputs": null,
            "phone": "0545830977",
            "cart": {
              "shipping": {
                "type": "shipping",
                "integration_type": "chita shipping",
                "id": 52484,
                "store_default": true,
                "data": {
          
                },
                "var": {
          
                },
                "quantity": 1,
                "price": 0.0,
                "shipping_type": "not_cart_shipping",
                "title": "משלוח חינם!  ePost למקום נח לבחירתכם (עד 5 ימי עסקים)",
                "price_with_intrest": 0.0
              },
              "lines": [
                {
                  "type": "line_item",
                  "brand_title": "Camel Mountain",
                  "image_url": "https://konimboimages.s3.amazonaws.com/system/photos/4569353/cart/853de601bf9011ab0976bac74bc40273.jpg?1581951922",
                  "category_title": "תיקי גב",
                  "unit_price": 178.0,
                  "second_code": "https://tigernu.en.alibaba.com/product/62062747995-805707138/Tigernu_T_B3090B_double_layer_waterproof_laptop_computer_sleeve_light_weight_men_waterproof_smart_fashion_bags_bag_backpack.html?spm=a2700.shop_plser.41413.13.6b6c14f0n58dk3",
                  "quantity": 1,
                  "price": 178.0,
                  "title": "Camel Mountain® Dooney 22L 15.6\" Laptop Pack",
                  "code": "BZ-CM7762",
                  "item_id": 2718179,
                  "line_item_id": 125067362,
                  "offer_code": ""
                },
                {
                  "type": "upgrade",
                  "topic_title": "מלאי",
                  "unit_price": 0.0,
                  "inventory_code": "1",
                  "quantity": 1,
                  "price": 0.0,
                  "title": "כמות",
                  "line_item_id": 125067362,
                  "item_id": 2718179
                }
              ],
              "total_price": 178.0
            },
            "data_record_var": {
      
            }
          }
        }
    */
    public class KonimboHookRequest
    {
        [JsonProperty(PropertyName = "order")]
        public OrderDetails Order { get; set; }

        public bool IsPaid
        {
            get
            {
                return this.Order?.PaymentStatus == "שולם";
            }
        }

        public bool IsNewsletterEnabled
        {
            get
            {
                return this.Order?.Newsletter ?? false;
            }
        }

        public class OrderDetails {
            [JsonProperty(PropertyName = "id")]
            public string OrderId { get; set; }

            [JsonProperty(PropertyName = "store_id")]
            public int StoreId { get; set; }

            [JsonProperty(PropertyName = "additional_inputs")]
            public OrderExtraInfo ExtraInfo { get; set; }

            [JsonProperty(PropertyName = "cart")]
            public CartDetails CartDetails { get; set; }

            [JsonProperty(PropertyName = "full_url")]
            public string URL { get; set; }

            [JsonProperty(PropertyName = "newsletter")]
            public bool Newsletter { get; set; }

            [JsonProperty(PropertyName = "email")]
            public string Email { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "event")]
            public string EventName { get; set; }  // order_after_payment

            [JsonProperty(PropertyName = "payment_status")]
            public string PaymentStatus { get; set; }

            [JsonProperty(PropertyName = "customer_id")]
            public string CustomerId { get; set; }

            [JsonProperty(PropertyName = "phone")]
            public string Phone { get; set; }
        }

        public class OrderExtraInfo
        {            
            [JsonProperty(PropertyName = "extra_field_bill_name")]
            public string BillName { get; set; }

            [JsonProperty(PropertyName = "extra_field_bn_number")]
            public string BusinessId { get; set; }

            [JsonProperty(PropertyName = "extra_field_notes")]
            public string Notes { get; set; }

            [JsonProperty(PropertyName = "extra_field_street")]
            public string Street { get; set; }

            [JsonProperty(PropertyName = "extra_field_city")]
            public string City { get; set; }

            [JsonProperty(PropertyName = "extra_field_date")]
            public string sDate { get; set; }            
            public DateTime Date {
                get {
                    DateTime converted;
                    return DateTime.TryParse(this.sDate, CultureInfo.GetCultureInfo("he-IL"), DateTimeStyles.None, out converted) ? converted : DateTime.Now;
                }
            }

            [JsonProperty(PropertyName = "extra_field_bliss")]
            public string Bliss { get; set; }

            [JsonProperty(PropertyName = "extra_field_name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "extra_field_hour")]
            public string Hour { get; set; }

            [JsonProperty(PropertyName = "extra_field_house_num")]
            public string House { get; set; }

            [JsonProperty(PropertyName = "extra_field_phone_num")]
            public string Phone { get; set; }

            [JsonProperty(PropertyName = "extra_field_floor_num")]
            public string Floor { get; set; }

            [JsonProperty(PropertyName = "extra_field_apartment_num")]
            public string Apartment { get; set; }

            public string Address
            {
                get {                    
                    var result = $"Israel, {this.City} {this.Street}";
                    if (!string.IsNullOrWhiteSpace(this.House))
                        result = $"{result} {this.House}";
                    /*                    
                    if (!string.IsNullOrWhiteSpace(this.Apartment))
                        result = $"{result}, {this.Apartment}";
                    if (!string.IsNullOrWhiteSpace(this.Floor))
                        result = $"{result}, {this.Floor}";
                    */
                    return result.Trim(',');
                }
            }
        }

        public class CartDetails
        {
            [JsonProperty(PropertyName = "total_price")]
            public string TotalPrice { get; set; }
        }

        public override string ToString()
        {
            return $""; // TODO ->> override ToString
        }
    }
}