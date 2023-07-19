using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace TookanProxy
{
    public class KonimboHookRequestBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var usePostData = actionContext.Request.Content.Headers.ContentType.MediaType == "application/x-www-form-urlencoded";
            if (usePostData)
            {
                var bodyEncoded = actionContext.Request.Content.ReadAsStringAsync().Result;
                var body = HttpUtility.UrlDecode(bodyEncoded);
                var data = body.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);

                bindingContext.Model = new KonimboHookRequest
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
                        }
                    }
                };
            }

            return false;
        }
    }
}