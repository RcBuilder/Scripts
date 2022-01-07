using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using Entities;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{ 
    public class MSSQLDBProvider : ISystemBrokersDAL
    {
        protected string ConnetionString { get; set; }
        public MSSQLDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }
       
        public bool DeleteBroker(string name)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_Delete",
                    commandType: CommandType.StoredProcedure,
                    param: new { name }
                ) > 0;
            }
        }

        public BrokerData GetBroker(string name)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Query<BrokerData>(
                    "sp_Broker_Get",
                    commandType: CommandType.StoredProcedure,
                    param: new { name }
                ).FirstOrDefault();
            }
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

        public bool SaveBroker(BrokerData brokerData)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Execute(
                    "sp_Broker_Save",
                    commandType: CommandType.StoredProcedure,
                    param: new {
                        brokerData.Name,
                        brokerData.DBProviderName,
                        brokerData.ConnectionString,
                        brokerData.Server,
                        brokerData.IsActive,
                        brokerData.Permissions
                    }
                ) > 0;                
            }
        }

        // --

        public IEnumerable<QueryData> GetQueries()
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return conn.Query<QueryData>(
                    "sp_Queries_Get",
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public int CreateQuery(QueryData queryData)
        {
            using (var conn = new SqlConnection(this.ConnetionString))
            {
                return Convert.ToInt32(conn.ExecuteScalar(
                    "sp_Query_Save",
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        queryData.Id,
                        queryData.BrokerName,
                        queryData.Title,
                        queryData.Body
                    }
                ));
            }
        }

        public bool DeleteQuery(int id)
        {
            using (var conn = new SqlConnection(this.ConnetionString)) {
                return conn.Execute(
                    "sp_Query_Delete",
                    commandType: CommandType.StoredProcedure,
                    param: new { id }
                ) > 0;
            }
        }
    }
}
