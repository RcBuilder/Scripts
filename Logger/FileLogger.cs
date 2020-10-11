using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Common
{
    public class FileLogger : BaseLogger
    {
        protected static string PATH { set; get; } = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Logs\\");        
        protected Encoding Encoding { set; get; } = Encoding.UTF8;

        private object _lockThis = new object();

        public FileLogger() {
            if (!Directory.Exists(FileLogger.PATH))
                Directory.CreateDirectory(FileLogger.PATH);
        }

        public override void Error(string logName, Exception Ex) {
            try {
                lock (_lockThis) {
                    
                    /*
                        var process = System.Diagnostics.Process.GetCurrentProcess();
                        var processId = process == null ? "NULL" : process.Id.ToString();
                    */

                    using (var fs = new StreamWriter(string.Concat(FileLogger.PATH, logName, "_", DateTime.Now.ToString("yyyyMMdd"), ".error"), true, Encoding)) {
                        // fs.WriteLine("Method : " + method);
                        fs.WriteLine("Time : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        fs.WriteLine("Thread ID : " + Thread.CurrentThread.ManagedThreadId.ToString());
                        fs.WriteLine("InnerException : " + Ex.InnerException);
                        fs.WriteLine("StackTrace : " + Ex.StackTrace);
                        
                        foreach (object key in Ex.Data.Keys) {
                            object value = Ex.Data[key];
                            fs.WriteLine(string.Format("{0} = {1}", key, value));
                        }

                        fs.WriteLine(Ex.Message);
                        fs.WriteLine("-------------------------------");
                    }
                }
            }
            catch { }
            finally { }
        }

        public override void Info(string logName, string Message, List<string> Params) {
            try {
                lock (_lockThis) {
                    using (var fs = new StreamWriter(string.Concat(FileLogger.PATH, logName, "_", DateTime.Now.ToString("yyyyMMdd"), ".info"), true, Encoding))
                    {
                        // fs.WriteLine("Method : " + method);
                        fs.WriteLine("Time : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        fs.WriteLine("Thread ID : " + Thread.CurrentThread.ManagedThreadId.ToString());

                        if (Params != null)                        
                            foreach (string param in Params)
                                fs.WriteLine(param);                        

                        fs.WriteLine(Message);
                        fs.WriteLine("-------------------------------");
                    }
                }
            }
            catch { }
            finally { }
        }
    }
}
