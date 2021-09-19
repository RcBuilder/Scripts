using BrokersEngine;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Authorization
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method)]
    public class CreativeAuthorizeAttribute : AuthorizeAttribute
    {
        private string JWTSecretKey { get; set; }
        public CreativeAuthorizeAttribute() {
            this.JWTSecretKey = ConfigurationManager.AppSettings["JWTSecretKey"].Trim();
        }

        private string GetTokenFromQuery(HttpActionContext actionContext) {
            var prmsMap = actionContext.Request.GetQueryNameValuePairs();
            if (prmsMap == null) return null; // no parameters
            return prmsMap.FirstOrDefault(p => p.Key == "token").Value;
        }

        private string GetTokenFromHeader(HttpActionContext actionContext)
        {
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization == null)
                throw new Exception("No Authorization Header");

            if (authorization.Scheme != "Bearer")
                throw new Exception("Not a Bearer Authorization");

            if (string.IsNullOrEmpty(this.JWTSecretKey))
                throw new Exception("No JWT Secret Key");

            return authorization.Parameter;
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            try
            {
                // [AllowAnonymous]
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                    return base.OnAuthorizationAsync(actionContext, cancellationToken);

                /*
                    [Header]
                    Authorization: Bearer xxxxxxxxxxxxxxxxxxxxxxxxxx

                    - OR -

                    [Query]
                    ?token=xxxxxxxxxxxxxxxxxxxxxxxxxx
                */
                var token = GetTokenFromQuery(actionContext);
                if(string.IsNullOrEmpty(token))
                    token = GetTokenFromHeader(actionContext);

                var generator = new JWTGenerator(this.JWTSecretKey);
                if (!generator.VerifyToken(token))                
                    throw new Exception("Unauthorized Access");                 

                // -- OK (Authorized) --

                var tokenPayload = generator.GetTokenPayload(token);

                /*
                   payload:
                   { brokerName, role, refName }
                   
                    roles:
                    Document | Guest | Broker | System

                   sample:
                   {  
                      "brokerName": "ShakedBroker",
                      "role": "Broker",
                      "refName": ""
                   }
                */

                var schema = new
                {
                    BrokerName = "",
                    Role = "",
                    RefName = "" 
                };
                var tokenPayloadModel = JsonConvert.DeserializeAnonymousType(tokenPayload, schema);

                // set brokerName from the JWT payload                
                var identity = new GenericIdentity(tokenPayloadModel.BrokerName, "BrokerName");
                actionContext.RequestContext.Principal = new GenericPrincipal(identity, new string[] {
                    tokenPayloadModel.Role // roles = Document | Guest | Broker | System 
                });
                actionContext.ActionArguments["TokenData"] = tokenPayload;

                var contextIdentity = actionContext.RequestContext.Principal.Identity;
                Debug.WriteLine($"Context Identity -> {contextIdentity.Name}");

                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }
            catch (Exception ex)
            {
                /*
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) {
                        Content = new StringContent(ex.Message)
                    };
                */

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, $"[CreativeAuthorize] {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }
}
