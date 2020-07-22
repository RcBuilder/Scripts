using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TestRest.Filters;

using Microsoft.IdentityModel.Tokens; // Base64UrlEncoder
using System.Security.Cryptography;  // HMACSHA256 
using System.Text;  // Encoding

namespace TestRest.Controllers
{
    [AuthorizeRequest]
    public class JWTController : ApiController
    {

        /*
            POST api/token
            REQ: grant_type=password&username=&password=
            RES: { access_token }
        */

        [AllowAnonymous]
        [HttpPost]
        [Route("api/token")]
        public HttpResponseMessage Token()
        {
            // validate credentials here...             

            return Request.CreateResponse(HttpStatusCode.OK, new {
                access_token = new JWTGenerator("my-secret-key").GenerateToken("1234567890", "John Doe")
            });                    
        }
        
        /*
            POST api/resource
            H: Authorization: Bearer <token>
            H: Content-Type: application/json;charset=UTF-8 
        */

        [HttpGet]
        [Route("api/resource")]
        public string GetResource()
        {
            return "resource";
        }
    }
}