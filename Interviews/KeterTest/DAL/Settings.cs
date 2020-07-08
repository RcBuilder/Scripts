using System;
using System.Configuration;

namespace DAL
{
    public class Settings
    {
        private static volatile Settings _instance;
        private static object syncRoot = new Object();

        public string StoreConnectionString { get; private set; }

        private Settings() { 
            this.StoreConnectionString = ConfigurationManager.ConnectionStrings["StoreConnectionString"].ConnectionString;
        }

        public static Settings Instance
        {
            get
            {                
                if (_instance == null)
                {
                    lock (syncRoot)
                    { 
                        if (_instance == null)
                            _instance = new Settings();
                    }
                }

                return _instance;
            }
        }
    }
}
