using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.PervasiveDBHelper;

namespace AccountStatusSenderService
{
    public class PervasiveDAL
    {
        protected string ConnetionString { get; set; }

        public PervasiveDAL(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public string ExecuteAsJson(string query) {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsJson(query, command);
            }
        }

        public string ExecuteAsJson(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    var sJsonResult = PervasiveDBHelper.ReadAsJson(reader);
                    return sJsonResult;
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }

        public DataTable ExecuteAsDataTable(string query)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsDataTable(query, command);
            }
        }

        public DataTable ExecuteAsDataTable(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;

                var dt = new DataTable();
                using (var adapter = new OdbcDataAdapter(command))
                    adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }

        public object ExecuteAsScalar(string query)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsScalar(query, command);
            }
        }

        public object ExecuteAsScalar(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }
    }
}
