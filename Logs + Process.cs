using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MHCommon
{
    public class Logs
    {
        #region LOGS BASE PATH:
        private static string PATH = "D:\\Logs\\"; //string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Logs\\");
        private static object _syncRoot = new object();  
        #endregion

        #region WriteErrorLog:
        public static void WriteErrorLog(string logName, string method, Exception ex)
        {
            try
            {
                lock (_syncRoot)
                {
                    var process = System.Diagnostics.Process.GetCurrentProcess();
                    var processId = process == null ? "NULL" : process.Id.ToString();

                    using (var fs = new StreamWriter(string.Concat(PATH, logName, "_", processId, "_", DateTime.Now.ToString("yyyyMMdd"), ".error"), true, Encoding.UTF8))
                    {
                        fs.WriteLine("Method : " + method);
                        fs.WriteLine("Time : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        foreach (object key in ex.Data.Keys)
                        {
                            object value = ex.Data[key];
                            fs.WriteLine(string.Format("{0} = {1}", key, value));
                        }
                        fs.WriteLine(ex.Message);
                        fs.WriteLine("-------------------------------");
                    }
                }
            }
            catch { }
            finally { }
        }
        #endregion

        #region WriteInfoLog:
        public static void WriteInfoLog(string logName, string method, string data)
        {
            WriteInfoLog(logName, method, data, null);
        }
        public static void WriteInfoLog(string logName, string method, string data, List<string> extra)
        {
            try
            {
                lock (_syncRoot)
                {
                    var process = System.Diagnostics.Process.GetCurrentProcess();
                    var processId = process == null ? "NULL" : process.Id.ToString();

                    using (var fs = new StreamWriter(string.Concat(PATH, logName, "_", processId, "_", DateTime.Now.ToString("yyyyMMdd"), ".info"), true, Encoding.UTF8))
                    {
                        fs.WriteLine("Method : " + method);
                        fs.WriteLine("Time : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        if (extra != null)
                        {
                            foreach (string param in extra)
                                fs.WriteLine(param);
                        }
                        fs.WriteLine(data);
                        fs.WriteLine("-------------------------------");
                    }
                }
            }
            catch { }
            finally { }
        }
        #endregion
    }
}