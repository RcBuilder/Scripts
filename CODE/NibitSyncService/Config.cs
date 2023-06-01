using System;
using System.Configuration;

namespace NibitSyncService
{
    public class Config
    {                        
        public static string ConnStr
        {
            get { return ConfigurationManager.AppSettings["ConnStr"].Trim(); }
        }

        public static string SrcFolder
        {
            get { return ConfigurationManager.AppSettings["SrcFolder"].Trim(); }
        }

        public static bool IsTestMode
        {
            get { return ConfigurationManager.AppSettings["IsTestMode"].Trim() == "1"; }
        }
    }
}
