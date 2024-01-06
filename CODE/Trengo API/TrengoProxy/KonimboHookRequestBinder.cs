using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace TrengoProxy
{
    /*
        [HttpPost]
        [Route("konimbo/hook")]
        public async Task<HttpResponseMessage> KonimboOrderAfterPaymentHook([ModelBinder(typeof(KonimboHookRequestBinder))]KonimboHookRequest HookData){
            ...
        }

        --

        order[cart][shipping][id]=45998
        order[cart][shipping][integration_type]=
        order[cart][shipping][price]=35.0
        order[cart][shipping][price_with_intrest]=35.0
        order[cart][shipping][quantity]=1
        order[cart][shipping][shipping_type]=
        order[cart][shipping][store_default]=true
        order[cart][shipping][title]=תל אביב
        order[cart][shipping][type]=shipping
        order[cart][lines][][category_title]=מגשי פירות בלבד 
        order[cart][lines][][code]=2
        order[cart][lines][][image_url]=https://konimboimages.s3.amazonaws.com/system/photos/10887886/cart/d7f9435c69ec087175aa1c61902ca817.jpg?1671648916
        order[cart][lines][][item_id]=3174746
        order[cart][lines][][line_item_id]=121960594
        order[cart][lines][][offer_code]=
        order[cart][lines][][price]=198.0
        order[cart][lines][][quantity]=1
        order[cart][lines][][second_code]=
        order[cart][lines][][title]=מגש פירות  מתאים  לעד 4 אנשים 
        order[cart][lines][][type]=line_item
        order[cart][lines][][unit_price]=198.0
        order[cart][total_price]=233.0
        order[additional_inputs][extra_field_apartment_num]=5
        order[additional_inputs][extra_field_bill_name]=
        order[additional_inputs][extra_field_bliss]=הורים יקרים שלנו  יום נשואין שמח  מאחלים לך המון בריאות ונחת מאיתנו אוהבים אותכם המון הילדים ונכדים 
        order[additional_inputs][extra_field_bn_number]=
        order[additional_inputs][extra_field_city]=תל אביב - יפו
        order[additional_inputs][extra_field_date]=17/11/2023
        order[additional_inputs][extra_field_floor_num]=3
        order[additional_inputs][extra_field_hour]=תיאום מול מקבל המשלוח
        order[additional_inputs][extra_field_house_num]=1
        order[additional_inputs][extra_field_name]=אתי מצי 
        order[additional_inputs][extra_field_notes]=
        order[additional_inputs][extra_field_phone_num]=0549553526
        order[additional_inputs][extra_field_phone_num2]=
        order[additional_inputs][extra_field_street]=חיים בר לב 132 
        order[affillate_field]=
        order[customer_id]=11061971
        order[customer_orders_size]=
        order[email]=Nmhachmon@gmail.com
        order[event]=order_after_payment
        order[full_url]=https://www.fruitfactory.co.il
        order[id]=14204837
        order[name]= מעיין חכמון 
        order[newsletter]=true
        order[payment_status]=שולם
        order[phone]=0522501586
        order[store_id]=5435
    */
    public class KonimboHookRequestBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var step = 1;
            try
            {
                var usePostData = actionContext.Request.Content.Headers.ContentType.MediaType == "application/x-www-form-urlencoded";

                var bodyEncoded = actionContext.Request.Content.ReadAsStringAsync().Result;

                step = 2;

                var body = HttpUtility.UrlDecode(bodyEncoded);
                ///File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}Payloads\\Exceptions.txt", $"{body}{Environment.NewLine}");

                step = 3;

                KonimboHookRequest model = null;

                if (usePostData)
                {
                    step = 4;

                    var matchIndex = 0;
                    body = Regex.Replace(body, "\\[\\]", match => $"[{matchIndex++}]");
                    var data = body.Split('&')
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                        .Distinct()
                        .ToDictionary(x => x.key, x => x.value);

                    step = 5;

                    model = new KonimboHookRequest
                    {
                        Order = new KonimboHookRequest.OrderDetails
                        {
                            StoreId = Convert.ToInt32(data["order[store_id]"]),
                            CustomerId = data["order[customer_id]"]?.Trim(),
                            Email = data["order[email]"]?.Trim(),
                            EventName = data["order[event]"]?.Trim(),
                            Phone = data["order[phone]"]?.Trim(),
                            OrderId = data["order[id]"]?.Trim(),
                            Name = data["order[name]"]?.Trim(),
                            URL = data["order[full_url]"]?.Trim(),
                            PaymentStatus = data["order[payment_status]"]?.Trim(),
                            ExtraInfo = new KonimboHookRequest.OrderExtraInfo
                            {
                                BillName = data["order[additional_inputs][extra_field_bill_name]"]?.Trim(),
                                Bliss = data["order[additional_inputs][extra_field_bliss]"]?.Trim(),
                                Apartment = data["order[additional_inputs][extra_field_apartment_num]"]?.Trim(),
                                City = data["order[additional_inputs][extra_field_city]"]?.Trim(),
                                BusinessId = data["order[additional_inputs][extra_field_bn_number]"]?.Trim(),
                                Floor = data["order[additional_inputs][extra_field_floor_num]"]?.Trim(),
                                Hour = data["order[additional_inputs][extra_field_hour]"]?.Trim(),
                                House = data["order[additional_inputs][extra_field_house_num]"]?.Trim(),
                                Name = data["order[additional_inputs][extra_field_name]"]?.Trim(),
                                Notes = data["order[additional_inputs][extra_field_notes]"]?.Trim(),
                                Phone = data["order[additional_inputs][extra_field_phone_num]"]?.Trim(),
                                sDate = data["order[additional_inputs][extra_field_date]"]?.Trim(),
                                Street = data["order[additional_inputs][extra_field_street]"]?.Trim(),
                            },
                            CartDetails = new KonimboHookRequest.CartDetails { 
                                TotalPrice = data["order[cart][total_price]"]?.Trim()
                            }
                        }
                    };
                }
                else
                {
                    step = 6;
                    model = JsonConvert.DeserializeObject<KonimboHookRequest>(body);
                }

                step = 7;

                bindingContext.Model = model;
                return true;
            }
            catch (Exception ex) {
                try
                {
                    File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}Payloads\\Exceptions.txt", $"{ex.Message} (step={step}){Environment.NewLine}");
                }
                catch { }

                return false;
            }
            
        }
    }
}