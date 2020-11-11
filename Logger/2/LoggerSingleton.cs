using Logger;
using System;
using System.Configuration;

namespace WebsiteBLL
{
    public sealed class LoggerSingleton
    {
        private static SQLDBLogger _Instance = null;
        public static SQLDBLogger Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new SQLDBLogger(ConfigSingleton.Instance.ConnStr);  // lazy loading 
                return _Instance;
            }
        }

        private LoggerSingleton(){ }
    }
}
