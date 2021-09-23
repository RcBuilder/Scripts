using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    public class ConsoleLogger : ILogger, ILoggerAsync
    {
        public void Error(string LogName, Exception Ex)
        {
            Console.WriteLine("{0} [ERROR] {1}", Thread.CurrentThread.ManagedThreadId, Ex.Message);
        }

        public void Info(string LogName, string Message, List<string> Params = null)
        {
            Console.WriteLine("{0} {1}", Thread.CurrentThread.ManagedThreadId, Message);
        }

        public async Task ErrorAsync(string LogName, Exception Ex)
        {
            this.Error(LogName, Ex);
        }

        public Task ErrorAsync(string LogName, string Message, List<string> Params = null) {
            throw new NotImplementedException();
        }

        public async Task InfoAsync(string LogName, string Message, List<string> Params = null)
        {
            this.Info(LogName, Message, Params);
        }
    }
}
