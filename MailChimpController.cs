using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Services.App_Code;
using Newtonsoft.Json;
using Services.Models;

namespace Services.Controllers
{
    /*     
        ## MailChimp ##
    
        Authorization header:
        Authorization: Basic base64(name:api key)          

        control panel:
        https://us5.admin.mailchimp.com
      
        api server 
        https://us5.api.mailchimp.com/3.0/   

        documentation:
        http://developer.mailchimp.com/documentation/mailchimp/reference/overview/
     
        playground:
        https://us1.api.mailchimp.com/playground/?_ga=2.122965901.788719496.1497604357-1049574962.1495999186         
    */

    [EnableCors(
           origins: "*",
           headers: "*",
           methods: "*"
       )]
    [LogTraffic]
    [ErrorTraffic]
    public class MailChimpController : ApiController
    {
        [HttpGet]
        [Route("api/MailChimp/lists")]
        public MailChimpList GetLists()
        {
            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));

            var response = Helper.GET("https://us5.api.mailchimp.com/3.0/lists", string.Empty, Headers, out STATUS);            
            
            if(STATUS != HttpStatusCode.OK) {
                return null; 
            }

            return JsonConvert.DeserializeObject<MailChimpList>(response);
        }

        [HttpGet]
        [Route("api/MailChimp/lists/{id}/categories")]
        public MailChimpListCategories GetListCategories(string id)
        {
            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));

            var response = Helper.GET(string.Format("https://us5.api.mailchimp.com/3.0/lists/{0}/interest-categories", id), string.Empty, Headers, out STATUS);            
            
            if(STATUS != HttpStatusCode.OK) {
                return null; 
            }

            return JsonConvert.DeserializeObject<MailChimpListCategories>(response);
            
        }

        [HttpGet]
        [Route("api/MailChimp/lists/{listId}/categories/{categoryId}/emails")]
        public IEnumerable<string> GetCategoryEmails(string listId, string categoryId)
        {
            var members = GetCategoryMembers(listId);
            var category = GetCategory(listId, categoryId);

            return (from m in members.Members
                   let intrsts = m.Interests
                   from intrst in intrsts
                   join g in category.SubCategories on intrst.Key equals g.Id 
                   where intrst.Value == true
                   select m.Email).Distinct();
        }

        [HttpPost]
        [Route("api/MailChimp/process")]
        /*
            POST http://localhost:4338/api/MailChimp/process
         
            User-Agent: Fiddler
            Host: localhost:4338
            Content-Length: 129
            content-type: application/json
            key: Um9ieTowNWM3OWVkOGJjNzhmZWZlOWVjYmRkNjBkNjk5NmQzNS11czU= 
         
            {
                listId: '2cd6eea34b',
                categoryId:'c81e87b7ce',
                campaignSubject: "test", 
                fromName: "roby", 
                replyTo: "shuki@lixfix.com"
            }
        */

        public bool ProcessCampaign(ProcessCampaignRequest request)
        {
            var segmentId = CreateSegment(request.ListId, request.CategoryId);
            var campaignId = CreateCampaign(request.ListId, segmentId, request.Subject, request.FromName, request.ReplyTo);
            var result = SendCampaign(campaignId);

            return result;
        }
        

        public MailChimpMembers GetCategoryMembers(string listId)
        {
            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));

            // get all the members within the list
            var responseMembers = Helper.GET(string.Format("https://us5.api.mailchimp.com/3.0/lists/{0}/members", listId), string.Empty, Headers, out STATUS);

            if (STATUS != HttpStatusCode.OK)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MailChimpMembers>(responseMembers);            
        }

        // TODO get category instead of sub categories
        public MailChimpCategory GetCategory(string listId, string categoryId)
        {
            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));
           
            // get all the interests within the category 
            var responseGroups = Helper.GET(string.Format("https://us5.api.mailchimp.com/3.0/lists/{0}/interest-categories/{1}/interests", listId, categoryId), string.Empty, Headers, out STATUS);

            if (STATUS != HttpStatusCode.OK)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MailChimpCategory>(responseGroups);
        }

        public bool SendCampaign(string campaignId)
        {
            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));

            var response = Helper.POST(string.Format("https://us5.api.mailchimp.com/3.0/campaigns/{0}/actions/send", campaignId), string.Empty, null, null, Headers, out STATUS);
            return STATUS == HttpStatusCode.OK;
        }

        public string CreateCampaign(string listId, string segmentId, string campaignSubject, string fromName, string replyTo)
        {
            /*  note:
                MUST Verify the replyTo domain! 
                Account -> Settings -> Verified domains ->  Verify a Domain
             
            */

            var request = new {
                type = "regular", // regular, plaintext, rss, variate
                settings = new {
                    title = string.Format("testCampaign_{0}", DateTime.Now.ToString("yyyyMMddHHmmss")),
                    subject_line = campaignSubject,  
                    from_name = fromName,
                    reply_to = replyTo
                },
                recipients = new {
                    list_id = listId,
                    saved_segment_id = segmentId // note: can create a new one by suppling both match and conditions options
                }
            };

            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));

            var response = Helper.POST("https://us5.api.mailchimp.com/3.0/campaigns", JsonConvert.SerializeObject(request), "application/json", null, Headers, out STATUS);

            if (STATUS != HttpStatusCode.OK)
            {
                return string.Empty;
            }

            dynamic responseObj = JsonConvert.DeserializeObject(response);
            var campaignId = responseObj.id; //campaignId

            // TODO mail body

            var content = new {
                // plain_text = "Hello World",
                html = "<p>Hello World</p>" // note: throw an exception for plaintext type 
            };

            response = Helper.POST(string.Format("https://us5.api.mailchimp.com/3.0/campaigns/{0}/content", campaignId), JsonConvert.SerializeObject(content), "application/json", "PUT", Headers, out STATUS);

            if (STATUS != HttpStatusCode.OK)
            {
                return string.Empty;
            }
           
            return campaignId;
        }

        public string CreateSegment(string listId, string categoryId)
        {
            var category = GetCategory(listId, categoryId);
            if (category == null)
                return string.Empty;

            var interests = category.SubCategories.Select(x => x.Id).ToArray();

            var request = new { 
                name = string.Format("testSegment_{0}", DateTime.Now.ToString("yyyyMMddHHmmss")),
                options = new {
                    match = "all", // any, all
                    conditions = new [] {
                        new{                        
                            condition_type = "Interests",    
                            field = string.Format("interests-{0}", categoryId),
                            op = "interestcontains",                
                            value = interests
                        }
                    }
                }
            };

            HttpStatusCode STATUS;
            var Headers = new NameValueCollection();
            Headers.Add("Authorization", string.Format("Basic {0}", GetAPIKey()));            

            var response = Helper.POST(string.Format("https://us5.api.mailchimp.com/3.0/lists/{0}/segments", listId), JsonConvert.SerializeObject(request), "application/json", null, Headers, out STATUS);

            if (STATUS != HttpStatusCode.OK)
            {
                return string.Empty;
            }

            dynamic responseObj = JsonConvert.DeserializeObject(response);
            return responseObj.id; //segmentId           
        }

        private string GetAPIKey() {
            return Request.Headers.GetValues("key").FirstOrDefault();
        }
    }
}