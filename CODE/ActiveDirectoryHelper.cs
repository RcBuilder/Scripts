using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Helpers
{
    /*
        USING
        -----

        // update account data
        // best for single property
        var ds = ActiveDirectoryHelper.InitSearcher(AD_DOMAIN, AD_USER, AD_PASSWORD);
        var rs = ActiveDirectoryHelper.FindAccountByUserName(ds, adAccount.UserName);
        var current = rs.GetDirectoryEntry();

        ActiveDirectoryHelper.SetProperty(current, "telephonenumber", adAccount.Phone);
        ActiveDirectoryHelper.SetProperty(current, "mail", adAccount.Email);

        current.Close();

        -

        // update account data 
        // best for multiple properties
        var ds = ActiveDirectoryHelper.InitSearcher(AD_DOMAIN, AD_USER, AD_PASSWORD);
        var rs = ActiveDirectoryHelper.FindAccountByUserName(ds, adAccount.UserName);
        var current = rs.GetDirectoryEntry();

        var properties = new Dictionary<string, string> {
            { "telephonenumber", adAccount.Phone },
            { "mail", adAccount.Email }
        };

        ActiveDirectoryHelper.SetProperties(current, properties);                

        current.Close();
       
        -

        // reset account password
        var ds = ActiveDirectoryHelper.InitSearcher(AD_DOMAIN, AD_USER, AD_PASSWORD);
        var rs = ActiveDirectoryHelper.FindAccountByUserName(ds, adAccountPassword.UserName);
        var current = rs.GetDirectoryEntry();

        ActiveDirectoryHelper.SetProperty(current, "userAccountControl", "0x200"); // Enable Account (if disabled)                
        current.Invoke("SetPassword", new object[] { adAccountPassword.Password });

        // Force the user to change password at next logon
        if (adAccountPassword.Force)
            ActiveDirectoryHelper.SetProperty(current, "pwdlastset", "0");

        current.Close();

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

        public static void SetProperties(DirectoryEntry entry, Dictionary<string, string> dicKeyValue) 
        {            
            foreach (var kv in dicKeyValue)
                SetProperty(entry, kv.Key, kv.Value);

            entry.CommitChanges(); // save changes
        }
    }
}
