using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace BrokersEngine
{
    public class BrokerProcessorContext
    {
        public HttpActionContext ActionContext { get; set; }        
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}