using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    public class ConsoleLogger : BaseLogger
    {
        public override void Error(string logName, Exception Ex)
        {
            Console.WriteLine("{0} [ERROR] {1}", Thread.CurrentThread.ManagedThreadId, Ex.Message);
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
            Console.WriteLine("{0} {1}", Thread.CurrentThread.ManagedThreadId, Message);
        }
    }
}
