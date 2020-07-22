using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TestRest.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRequestAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            // Authorization: Bearer <token>       

            try
            {
                // [AllowAnonymous]
                if (context.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                {
                    base.OnAuthorization(context);
                    return;
                }

                var authorization = context.Request.Headers.Authorization;
                if (authorization == null)
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "No Authorization Header");

                if (authorization.Scheme != "Bearer")
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Not a Bearer Authorization");

                var token = authorization.Parameter;
                var generator = new JWTGenerator("my-secret-key");
                if (!generator.VerifyToken(token))
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized Access");

                var header = generator.GetTokenHeader(token);
                var payload = generator.GetTokenPayload(token);

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