using Logger;

namespace BLL
{
    public sealed class LoggerSingleton
    {
        private static ILogger _Instance = null;
        public static ILogger Instance
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
