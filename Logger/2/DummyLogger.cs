using System;
using System.Collections.Generic;
using System.Threading;

namespace Logger
{
    public class DummyLogger : BaseLogger
    {
        public override void Error(string logName, Exception Ex)
        {
            
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
           
        }
    }
}
