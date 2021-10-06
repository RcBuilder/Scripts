using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PropertySuiteAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRequestAttribute : AuthorizationFilterAttribute //ActionFilterAttribute
    {
        private const string KEY = "api_key";
        public override void OnAuthorization(HttpActionContext context)
        {
            /*
                [Header]
                Authorization: api_key xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxx

                - OR -

                [Query]
                ?api_key=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxx
            */

            try
            {
                // [AllowAnonymous]
                if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()) {
                    base.OnAuthorization(context);
                    return;
                }
                
                var apiKey = GetFromQuery(); // [from Query] swagger UI
                if (string.IsNullOrEmpty(apiKey))
                    apiKey = GetFromHeader(context); // [from Header] propertySuite servers

                if (string.IsNullOrEmpty(apiKey) || apiKey != Config.Keys.AuthorizationKey)                    
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

        private string GetFromHeader(HttpActionContext context) {
            var authorization = context.Request.Headers.Authorization;
            if (authorization == null)
                throw new HttpException((int)HttpStatusCode.Unauthorized, "No Authorization Header");

            if (authorization.Scheme != KEY)
                throw new HttpException((int)HttpStatusCode.Unauthorized, string.Format("Not a {0} Authorization Scheme", KEY));

            var apiKey = authorization.Parameter;
            return apiKey;
        }

        private string GetFromQuery() {
            var apiKey = HttpContext.Current.Request.QueryString[KEY];
            return apiKey;
        }
    }
}


/*

/// Authorization: Basic1 Base64([User]:[Password])

try {
    // [AllowAnonymous]
    if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()) {
        base.OnAuthorization(context);
        return;
    }

    var authorization = context.Request.Headers.Authorization;
    if (authorization == null)
        throw new HttpException((int)HttpStatusCode.Unauthorized, "No Authorization Header");

    if (authorization.Scheme != "Basic1")
        throw new HttpException((int)HttpStatusCode.Unauthorized, "Not a Basic1 Authorization");

    var encodedCredentials = authorization.Parameter;
    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
    var user = credentials.Split(':')[0];
    var password = credentials.Split(':')[1];

    if (user != Config.Keys.AuthorizationKey || password != Config.Keys.AuthorizationPassword)                    
        throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized Access");

    base.OnAuthorization(context);
}
catch (HttpException ex) {
    context.Response = new HttpResponseMessage((HttpStatusCode)ex.GetHttpCode()) {
        Content = new StringContent(ex.Message)
    };
}
catch (Exception ex) {
    context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) {
        Content = new StringContent(ex.Message)
    };
} 
*/