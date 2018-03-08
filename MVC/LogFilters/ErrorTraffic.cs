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
    public class ErrorTraffic : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var actionContext = actionExecutedContext.ActionContext;

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

            // e.g: POST /api/SpreadSheet/31519/tickets
            actionExecutedContext.Exception.Data.Add("Action", string.Format("{0} {1}", method, path));

            // convert post data to exception data
            foreach (var p in postData)
                actionExecutedContext.Exception.Data.Add(p.Key, p.Value);

            Logs.WriteErrorLog("[Service Error Traffic]",
                string.Format("{0}.{1}", controllerName, actionName),                
                actionExecutedContext.Exception
            );          
        }
    }
}