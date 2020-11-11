using Entities;
using System;
using System.Collections.Generic;

namespace Common
{
    public class ConsoleAndFileLogger : BaseLogger
    {
        protected ILogger ConsoleLogger = new ConsoleLogger();
        protected ILogger FileLogger = new FileLogger();

        public override void Error(string logName, Exception Ex)
        {
            ConsoleLogger.Error(logName, Ex);
            FileLogger.Error(logName, Ex);
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
            ConsoleLogger.Info(logName, Message, Params);
            FileLogger.Info(logName, Message, Params);
        }
    }
}
