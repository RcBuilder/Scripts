using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Logger
{
    public class DebugLogger : BaseLogger
    {
        public override void Error(string logName, Exception Ex)
        {
            Debug.WriteLine("{0} [ERROR] {1}", Thread.CurrentThread.ManagedThreadId, Ex.Message);
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
            Debug.WriteLine("{0} {1}", Thread.CurrentThread.ManagedThreadId, Message);
        }
    }
}
