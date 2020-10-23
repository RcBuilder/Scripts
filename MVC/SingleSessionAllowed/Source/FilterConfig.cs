using System.Web;
using System.Web.Mvc;

namespace Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleSessionStateAttribute());
            filters.Add(new HandleErrorAttribute());            
        }
    }
}