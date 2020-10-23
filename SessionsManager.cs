using System;
using System.Web;

namespace WebsiteBLL
{
    /*
        public enum eAccountType : byte { Subscription, Affiliate }

        public class LoggedInAccount {
            public eAccountType AccountType { get; protected set; }
            public int AccountId { get; protected set; }

            public LoggedInAccount(eAccountType AccountType, int AccountId) {
                this.AccountType = AccountType;
                this.AccountId = AccountId;
            }
        }
    */

    public class SessionsManager
    {
        public const string KEY_ACCOUNT = "Account";

        public static void SetAccount(HttpContext Context, Entities.LoggedInAccount Account) {
            Context.Session[KEY_ACCOUNT] = Account;            
        }

        public static Entities.LoggedInAccount GetAccount(HttpContext Context)
        {
            try
            {
                // get from session
                var accountSession = Context.Session[KEY_ACCOUNT];
                if (accountSession != null) return (Entities.LoggedInAccount)accountSession;

                return null;
            }
            catch { return null; }
        }

        public static void ClearAccount(HttpContext Context)
        {
            Context.Session[KEY_ACCOUNT] = "";
            Context.Session.Remove(KEY_ACCOUNT);           
        }

        public static bool IsLoggedIn(HttpContext Context)
        {
            return GetAccount(Context) != null;
        }
    }
}
