using System;
using System.Configuration;

namespace DistributionServiceBLL
{
    public sealed class ConfigSingleton
    {
        private static ConfigSingleton _Instance = null;
        public static ConfigSingleton Instance {
            get {
                if (_Instance == null)
                    _Instance = new ConfigSingleton();  // lazy loading 
                return _Instance;
            }
        }

        private ConfigSingleton() {            
            this.TwilioAccountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].Trim();
            this.TwilioAuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"].Trim();
            this.TwilioPhoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"].Trim();
            this.AudioServer = ConfigurationManager.AppSettings["AudioServer"].Trim();
            this.MessagesServer = ConfigurationManager.AppSettings["MessagesServer"].Trim();
            this.ServicesOnOff = ConfigurationManager.AppSettings["ServicesOnOff"].Trim().ToUpper() == "ON";
            this.WPConnStr = ConfigurationManager.ConnectionStrings["WPConnStr"].ConnectionString.Trim();
            this.HangfireConnStr = ConfigurationManager.ConnectionStrings["HangfireConnStr"].ConnectionString.Trim();
        }

        // ---------------------------
        
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string TwilioPhoneNumber { get; set; }     
        public string AudioServer { get; set; }
        public string MessagesServer { get; set; }
        public string WPConnStr { get; set; }
        public string HangfireConnStr { get; set; }
        public bool ServicesOnOff { get; set; }
    }
}
