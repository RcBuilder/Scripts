using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public class DBLogs
    {        
        #region WriteErrorLog:
        public static void WriteErrorLog(string logName, string method, Exception ex)
        {
            try
            {
                var moreInfo = string.Empty;
                if (ex.Data.Keys.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (object key in ex.Data.Keys)
                    {
                        object value = ex.Data[key];
                        sb.AppendFormat("{0} = {1}", key, value);
                    }

                    moreInfo = sb.ToString();
                }

                DAL.Repository.DBLogs.Add(logName, method, ex.Message, moreInfo, true);
            }
            catch{
                Logs.WriteErrorLog(logName, method, ex);
            }
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
                var moreInfo = string.Empty;
                if (extra != null && extra.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (string param in extra)
                        sb.Append(param);

                    moreInfo = sb.ToString();
                }

                DAL.Repository.DBLogs.Add(logName, method, data, moreInfo, false);
            }
            catch
            {
                Logs.WriteInfoLog(logName, method, data, extra);
            }
        }
        #endregion
    }
}