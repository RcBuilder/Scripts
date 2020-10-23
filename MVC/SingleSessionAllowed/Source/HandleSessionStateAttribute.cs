using BLL;
using System.Web.Mvc;

namespace Website
{
    public class HandleSessionStateAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            ConnectedSessionsSingleTon.Instance.ProcessCurrentContext(filterContext.HttpContext);
            base.OnResultExecuted(filterContext);
        }
    }
}