using BLL;
using Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Controllers
{    
    public class CelebritiesController : ApiController
    {
        [HttpGet]
        [Route("celebrities/who")]
        public HttpResponseMessage WhoAmI() {
            return Request.CreateResponse(HttpStatusCode.OK,
                new List<string> {
                    "GET celebrities",
                    "DELETE celebrities/{id}",
                    "GET celebrities/reload",
                    "POST celebrities",
                    "PUT celebrities"
                }
            );
        }

        [HttpGet]
        [Route("celebrities")]
        public async Task<HttpResponseMessage> Get() {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await new CelebritiesBLL().GetAsync());
            }
            catch(Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("celebrities/{id}")]
        public async Task<HttpResponseMessage> Delete(string id) {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await new CelebritiesBLL().DeleteAsync(id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("celebrities/reload")]
        public async Task<HttpResponseMessage> Reload() {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await new CelebritiesBLL().ReloadAsync());
            }
            catch(Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("celebrities")]
        public async Task<HttpResponseMessage> Create([FromBody]CelebrityCard card)
        {
            try
            {                
                var response = Request.CreateResponse(HttpStatusCode.OK, await new CelebritiesBLL().CreateAsync(card));
                response.Headers.Add("Created_Id", card.Id);
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("celebrities")]
        public async Task<HttpResponseMessage> Update([FromBody]CelebrityCard card)
        {
            try
            {                
                return Request.CreateResponse(HttpStatusCode.OK, await new CelebritiesBLL().UpdateAsync(card));                                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
