using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogger
    {
        void Error(string logName, Exception Ex);
        void Info(string logName, string Message, List<string> Params);
    }
}
