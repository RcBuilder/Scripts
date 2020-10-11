
namespace Common
{
    public class Logger
    {
        private static BaseLogger _instance = null;
        private Logger() { }
        private static object _lockThis = new object();

        public static BaseLogger Instance
        {
            get
            {
                // performance-related:
                // lock is a costly action, check whether you already have a populated instance or not and save unnecessary lockings!
                if (_instance == null)
                {                      
                    lock (_lockThis)
                    {
                        if (_instance == null)
                            _instance = new ConsoleAndFileLogger();
                    }
                }

                return _instance;
            }
        }
    }
}
