using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace Services.App_Code
{
    public class LogTraffic : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionContext.ActionDescriptor.ActionName;

            var method = actionContext.Request.Method;
            var path = actionContext.Request.RequestUri.AbsolutePath;

            // get the request post data
            var postData = new Dictionary<string, string>();
            if (actionContext.ActionArguments != null) {
                if (method == HttpMethod.Get || method == HttpMethod.Delete)
                    postData = actionContext.ActionArguments.ToDictionary(x => x.Key, x => x.Value.ToString());
                else if (method == HttpMethod.Post || method == HttpMethod.Put)
                    postData = actionContext.ActionArguments.ToDictionary(x => x.Key, x => JsonConvert.SerializeObject(x.Value));
            }

            // get the querystring data
            var queryData = actionContext.Request.GetQueryNameValuePairs();
            foreach (var p in queryData)
                postData.Add(p.Key, p.Value);

            Logs.WriteInfoLog("[Service Traffic]", 
                string.Format("{0}.{1}", controllerName, actionName),
                string.Format("{0} {1}", method, path),
                postData.Select(x => string.Concat(x.Key, "=", x.Value)).ToList()
            );          
        }        
    }
}