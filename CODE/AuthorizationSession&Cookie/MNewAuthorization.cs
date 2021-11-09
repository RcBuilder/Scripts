using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace WEB.ActionFilters
{
    public class MNewAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = HttpContext.Current;
            var accountSession = SessionsManager.GetAccount(context);

            // no session - try to load cookie
            if (accountSession == null)
            {
                var accountCookie = CookiesManager.GetAccount(context);
                if (accountCookie != null)
                {
                    // update session
                    SessionsManager.SetAccount(context, accountCookie);
                    accountSession = accountCookie;
                }
            }

            if (accountSession != null)
            {
                GenericIdentity identity = null;
                var roles = new List<string>();

                var roleName = "Restaurant";
                identity = new GenericIdentity(accountSession.Id.ToString(), roleName);
                roles.Add(roleName);

                if (roles.Count > 0)
                    context.User = new GenericPrincipal(identity, roles.ToArray());
            }

            base.OnAuthorization(filterContext);
        }
    }
}