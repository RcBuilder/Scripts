using System;
using System.DirectoryServices;

namespace Helpers
{
    /*
        USING
        -----
        var ds = ActiveDirectoryHelper.InitSearcher(AD_DOMAIN, AD_USER, AD_PASSWORD);
        var rs = ActiveDirectoryHelper.FindAccountByUserName(ds, adAccount.UserName);
        var current = rs.GetDirectoryEntry();

        ActiveDirectoryHelper.SetProperty(current, "telephonenumber", adAccount.Phone);
        ActiveDirectoryHelper.SetProperty(current, "mail", adAccount.Email);
    */

    public class ActiveDirectoryHelper
    {
        public static DirectorySearcher InitSearcher(string doamin, string user, string password)
        {
            var ldapAddress = $"LDAP://{doamin}";  // LDAP://rootDSE
            var de = new DirectoryEntry(ldapAddress, user, password);
            var ds = new DirectorySearcher(de);
            ds.SearchScope = SearchScope.Subtree;
            return ds;
        }

        public static SearchResult FindAccountByUserName(DirectorySearcher ds, string userName)
        {
            ds.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(samaccountname={userName}))";  // by userName

            var rs = ds.FindOne();
            if (rs == null) throw new Exception("No Such Account");
            return rs;
        }

        public static SearchResult FindAccountByEmail(DirectorySearcher ds, string email)
        {
            ds.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(mail={email}))";  // by email

            var rs = ds.FindOne();
            if (rs == null) throw new Exception("No Such Account");
            return rs;
        }

        public static void SetProperty(DirectoryEntry entry, string key, string value)
        {
            if (entry.Properties.Contains(key)) // ADD
                entry.Properties[key][0] = value;
            else  // UPDATE
                entry.Properties[key].Add(value);

            entry.CommitChanges(); // save changes
        }
    }
}
