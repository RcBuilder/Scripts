using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BigQueryRelay
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRelayRequestAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            try
            {
                // [AllowAnonymous]
                if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                {
                    base.OnAuthorization(context);
                    return;
                }

                var xApiKey = context.Request.Headers.GetValues("x-api-key")?.FirstOrDefault()?.Trim();
                if (xApiKey == null)
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "No x-api-key Header");

                var configApiKey = ConfigurationManager.AppSettings["RelayApiKey"].Trim();
                if (xApiKey != configApiKey)
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized Access");

                base.OnAuthorization(context);
            }
            catch (HttpException ex)
            {
                context.Response = new HttpResponseMessage((HttpStatusCode)ex.GetHttpCode())
                {
                    Content = new StringContent(ex.Message)
                };
            }
            catch (Exception ex)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}