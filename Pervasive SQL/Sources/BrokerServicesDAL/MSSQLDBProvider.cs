using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{     
    public class MSSQLDBProvider : IBrokerServicesDAL
    {
        protected string ConnetionString { get; set; }
        public MSSQLDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public IEnumerable<BrokerData> GetBrokers()
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Query<BrokerData>(
                    "sp_Brokers_Get",
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public bool RegisterWebHook(WebHookData data)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_WebHook_Register",
                    commandType: CommandType.StoredProcedure,
                    param: new {
                        data.BrokerName,
                        data.ServiceName,
                        data.HookURL
                    }
                ) > 0;
            }
        }

        public bool DeleteWebHook(int id, string brokerName)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_WebHook_Delete",
                    commandType: CommandType.StoredProcedure,
                    param: new { 
                        id, 
                        brokerName 
                    }
                ) > 0;
            }
        }

        public bool UpdateWebHook(WebHookData data)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_WebHook_Update",
                    commandType: CommandType.StoredProcedure,
                    param: new { 
                        data.Id, 
                        data.BrokerName, 
                        data.HookURL 
                    }
                ) > 0;
            }
        }

        public bool UpdateWebHookLastExecutionTime(WebHookData data)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_WebHook_LastExecutionTime_Update",
                    commandType: CommandType.StoredProcedure,
                    param: new { 
                        data.Id, 
                        data.BrokerName, 
                        data.LastExecutionTime 
                    }
                ) > 0;
            }
        }

        public IEnumerable<WebHookData> GetBrokerWebHooks(string brokerName)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Query<WebHookData>(
                    "sp_Broker_WebHooks_Get",
                    commandType: CommandType.StoredProcedure,
                    param: new {
                        brokerName
                    }
                );
            }
        }

        public IEnumerable<WebHookData> GetServiceWebHooks(string brokerName, string serviceName)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Query<WebHookData>(
                    "sp_Broker_WebHooks_ByService_Get",
                    commandType: CommandType.StoredProcedure,
                    param: new {
                        brokerName,
                        serviceName
                    }
                );
            }
        }
    }
}
