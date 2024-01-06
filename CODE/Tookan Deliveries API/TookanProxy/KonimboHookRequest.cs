using Newtonsoft.Json;
using System;
using System.Globalization;

namespace TookanProxy
{
    /*
        {
          "order": {
            "id": 13134655,
            "store_id": 5435,
            "additional_inputs": {
              "extra_field_bill_name": "שידורי קשת בע\"מ ",
              "extra_field_bn_number": "511786352",
              "extra_field_notes": "להתקשר למקבל השליחות ",
              "extra_field_street": "מיכל לייב ",
              "extra_field_city": "פתח תקווה",
              "extra_field_date": "31/05/2023",
              "extra_field_bliss": "חופי היקר, מאחלים לך שתהיה חזק, תצלח את התקופה הקרובה ותחזור אלינו בריא וחזק מתמיד. מאחלים לך בריאות, אתה חסר לנו. מערכת mako",
              "extra_field_name": "אביב חופי / פזית חופי ",
              "extra_field_hour": "תיאום מול מקבל המשלוח",
              "extra_field_house_num": "39/2",
              "extra_field_phone_num": "0525604882"
            },
            "full_url": "https://www.fruitfactory.co.il",
            "customer_orders_size": null,
            "newsletter": false,
            "email": "sapir.tuby@mako.co.il",
            "name": "ספיר סגל",
            "event": "order_after_payment",
            "payment_status": "שולם",
            "customer_id": 10101693,
            "cart": null,
            "affillate_field": "",
            "phone": "0546250662"
          }
        } 
        --
        {
          "order": {
            "customer_id": 10112613,
            "name": "רוני ליברמן",
            "cart": null,
            "store_id": 5435,
            "full_url": "https://www.fruitfactory.co.il",
            "payment_status": "שולם",
            "id": 13147474,
            "email": "roni.liberman@gmail.com",
            "affillate_field": null,
            "phone": "0546605017",
            "event": "order_after_payment",
            "additional_inputs": {
              "extra_field_name": "נטע אמיתי ",
              "extra_field_date": "02/06/2023",
              "extra_field_phone_num": "0546303274",
              "extra_field_street": "מחלקת יולדות בית חולים איכילוב",
              "extra_field_floor_num": "",
              "extra_field_bill_name": "רוני ליברמן פרונובו",
              "extra_field_city": "תל אביב ",
              "extra_field_bliss": "נטע וגיא היקרים ברכות על הרחבת המשפחה . שתגדלו את הקטנה החדשה בנחת ואושר . אוהבים רוני , אורה, שי, ליהי, מור ולאון ומכל משפחת ליברמן",
              "extra_field_phone_num2": "0547917616",
              "extra_field_apartment_num": "חדר 2",
              "extra_field_bn_number": "031909310",
              "extra_field_notes": "המתנה היא עבור יולדת שנמצאת במחלקת יולדות - בית חולים איכילות ",
              "extra_field_hour": "שעות הצהריים 13:00-17:30",
              "extra_field_house_num": "מחלקה 3 - קומה 3"
            },
            "newsletter": true,
            "customer_orders_size": null
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

        public class OrderDetails {
            [JsonProperty(PropertyName = "id")]
            public string OrderId { get; set; }

            [JsonProperty(PropertyName = "store_id")]
            public int StoreId { get; set; }

            [JsonProperty(PropertyName = "additional_inputs")]
            public OrderExtraInfo ExtraInfo { get; set; }

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
                    var result = $"{this.City} {this.Street}";
                    if (!string.IsNullOrWhiteSpace(this.House))
                        result = $"{result} {this.House}";
                    if (!string.IsNullOrWhiteSpace(this.Apartment))
                        result = $"{result}, {this.Apartment}";
                    if (!string.IsNullOrWhiteSpace(this.Floor))
                        result = $"{result}, {this.Floor}";
                    return result.Trim(',');
                }
            }
        }
    }
}