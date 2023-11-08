using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL
{
    public class SampleBaseDAL
    {
        protected string ConnetionString { get; set; }

        public SampleBaseDAL(string ConnetionString) {
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
                    var sJsonResult = this.ReadAsJson(reader);
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

        // --

        public string ReadAsJson(OdbcDataReader dr)
        {
            if (dr == null || !dr.HasRows) return "[]";

            var results = new List<Dictionary<string, string>>();
            while (dr.Read())
            {
                var result = new Dictionary<string, string>();
                for (var i = 0; i < dr.VisibleFieldCount; i++)
                    result.Add(dr.GetName(i), SafeConvert.ToString(dr[i]));
                results.Add(result);
            }

            return JsonConvert.SerializeObject(results);
        }

        public T ReadAsT<T>(OdbcDataReader dr)
        {
            return JsonConvert.DeserializeObject<T>(ReadAsJson(dr));
        }
    }
}
