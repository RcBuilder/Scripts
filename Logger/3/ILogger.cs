using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogger
    {
        void Error(string logName, Exception Ex);        
        void Info(string logName, string Message, List<string> Params = null);
    }

    public interface ILoggerAsync
    {
        Task ErrorAsync(string LogName, Exception Ex);
        Task ErrorAsync(string LogName, string Message, List<string> Params = null);
        Task InfoAsync(string LogName, string Message, List<string> Params = null);
    }
}
