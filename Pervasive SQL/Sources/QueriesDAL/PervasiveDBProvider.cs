using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;

namespace QueriesDAL
{
    public class PervasiveDBProvider : IQueriesDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }
                
        public string Run(string query)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();                    
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                        return PervasiveDBHelper.ReadAsJson(reader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}