using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Logger
{
    public class SQLDBLogger : BaseLogger
    {
        protected string ConnStr { get; set; }

        public SQLDBLogger(string ConnStr) {
            this.ConnStr = ConnStr;
        }

        public override void Error(string logName, Exception Ex)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var extraParams = new List<string>();
                    foreach (DictionaryEntry item in Ex.Data)
                        extraParams.Add($"{item.Key}={item.Value}");

                    using (var conn = new SqlConnection(ConnStr))
                    {
                        await conn.ExecuteAsync(
                            "sp_Log_add",
                            commandType: CommandType.StoredProcedure,
                            param: new { 
                                Type = "ERROR",
                                Name = logName,
                                Ex.Message,
                                Params = string.Join(", ", extraParams)
                            }
                        );                        
                    }                    
                }
                catch {}
            });
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    using (var conn = new SqlConnection(ConnStr))
                    {
                        await conn.ExecuteAsync(
                            "sp_Log_add",
                            commandType: CommandType.StoredProcedure,
                            param: new { 
                                Type = "INFO",
                                Name = logName,
                                Message,
                                Params = string.Join(", ", Params)
                            }
                        );
                    }
                }
                catch {}
            });
        }
    }
}
