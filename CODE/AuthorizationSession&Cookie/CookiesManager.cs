using Newtonsoft.Json;
using System;
using System.Web;

namespace BLL
{
    public class CookiesManager
    {
        public const string KEY_ACCOUNT = "MNEW_Account";

        public static void SetAccount(HttpContext Context, Entities.LoggedInAccount Account) {
            Context.Response.Cookies.Add(new HttpCookie(KEY_ACCOUNT, JsonConvert.SerializeObject(Account))
            {
                Expires = DateTime.Now.AddDays(60),
                Secure = true,
            });
        }

        public static void ClearAccount(HttpContext Context)
        {
            var cookie = Context.Request.Cookies[KEY_ACCOUNT];
            if (cookie == null) return;
            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-1);
            Context.Response.Cookies.Add(cookie);
        }

        public static Entities.LoggedInAccount GetAccount(HttpContext Context)
        {
            var cookie = Context.Request.Cookies[KEY_ACCOUNT];
            if (cookie == null) return null;
            return JsonConvert.DeserializeObject<Entities.LoggedInAccount>(cookie.Value);
        }
    }
}
