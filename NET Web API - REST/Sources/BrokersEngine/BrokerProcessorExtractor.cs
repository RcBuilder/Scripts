using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace BrokersEngine
{
    // TODO ->> implement
    public class BrokerProcessorExtractor : IBrokerProcessorExtractor
    {
        public IRequestHelper RequestHelper { get; protected set; }

        public BrokerProcessorExtractor(IRequestHelper RequestHelper) {
            this.RequestHelper = RequestHelper;
        }

        public string ExtractActionName(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.ActionName;
        }

        public string ExtractControllerName(HttpActionContext actionContext)
        {
            return actionContext.ControllerContext.ControllerDescriptor.ControllerName;
        }

        public string ExtractBrokerName(HttpActionContext actionContext)
        {
            return actionContext.RequestContext.Principal.Identity.Name;
        }

        public string ExtractCookies(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string ExtractHeaders(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string ExtractHttpMethod(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string ExtractPayload(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string ExtractQuery(HttpActionContext actionContext)
        {
            return actionContext.Request.RequestUri.Query;
        }

        public Dictionary<string, string> ExtractQueryParameters(HttpActionContext actionContext)
        {
            return this.RequestHelper.Query2Dictionary(actionContext.Request.RequestUri.Query);
        }

        public string ExtractRouteParameters(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}