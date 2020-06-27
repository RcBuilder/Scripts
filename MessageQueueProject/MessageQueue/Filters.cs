using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace MessageQueue
{
    public class ActionLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            Console.WriteLine("{2} {0}, Query: {1}", 
                actionExecutedContext.ActionContext.ActionDescriptor.ActionName, 
                actionExecutedContext.Request.RequestUri.PathAndQuery,
                actionExecutedContext.Request.Method
            );
        }
    }
}
