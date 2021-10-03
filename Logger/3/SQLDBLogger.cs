using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Logger
{
    /*
        CREATE TABLE [dbo].[Logs](
	        [RowId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	        [Type] [varchar](20) NOT NULL,
	        [Name] [nvarchar](50) NOT NULL,
	        [Message] [nvarchar](max) NOT NULL DEFAULT '',
	        [Params] [nvarchar](max) NOT NULL DEFAULT '',
	        [CreatedDate] [datetime] NOT NULL DEFAULT GETDATE()
        )

        ---

        CREATE PROCEDURE [dbo].[sp_Log_add]
	        @Type VARCHAR(20),
	        @Name VARCHAR(50), 
	        @Message NVARCHAR(MAX),
	        @Params NVARCHAR(MAX)
        AS
        BEGIN	
	        SET NOCOUNT ON;

	        INSERT INTO [dbo].[Logs] 
	        (			
		        [Type], 
		        [Name], 
		        [Message], 
		        [Params]
	        )
	        VALUES 
	        (
		        @Type,
		        @Name,
		        @Message,
		        @Params
	        )

	        SELECT SCOPE_IDENTITY();	
        END
    */

    public class SQLDBLogger : ILogger, ILoggerAsync
    {
        private const int SyncWaitTimeMS = 3000;

        protected string ConnStr { get; set; }
        protected string ProcedureName { get; set; } = "sp_Log_add";

        public SQLDBLogger(string ConnStr) {
            this.ConnStr = ConnStr;
        }

        public void Error(string LogName, Exception Ex)
        {
            this.ErrorAsync(LogName, Ex).Wait(SyncWaitTimeMS);
        }

        public void Info(string LogName, string Message, List<string> Params = null)
        {
            this.InfoAsync(LogName, Message, Params).Wait(SyncWaitTimeMS);
        }

        public async Task ErrorAsync(string LogName, Exception Ex)
        {
            try {
                var extraParams = new List<string>();
                foreach (DictionaryEntry item in Ex.Data)
                    extraParams.Add($"{item.Key}={item.Value}");
                await this.ErrorAsync(LogName, Ex.Message, extraParams);
            }
            catch (Exception innerEx) {
                Debug.WriteLine($"[SQLDBLogger.ErrorAsync] {innerEx.Message}");
            }
        }

        public async Task ErrorAsync(string LogName, string Message, List<string> Params = null) {
            try {
                await this._LogAsync("ERROR", LogName, Message, Params);                
            }
            catch (Exception innerEx) {
                Debug.WriteLine($"[SQLDBLogger.ErrorAsync] {innerEx.Message}");
            }
        }

        public async Task InfoAsync(string LogName, string Message, List<string> Params = null)
        {
            try {
                await this._LogAsync("INFO", LogName, Message, Params);                
            }
            catch (Exception innerEx) {
                Debug.WriteLine($"[SQLDBLogger.InfoAsync] {innerEx.Message}");
            }
        }

        private async Task _LogAsync(string Type, string Name, string Message, List<string> Params = null) {
            using (var conn = new SqlConnection(ConnStr))
            {
                await conn.ExecuteScalarAsync(
                    this.ProcedureName,
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        Type,
                        Name,
                        Message,
                        Params = string.Join(", ", Params ?? new List<string>())
                    }
                );
            }
        }
    }
}
