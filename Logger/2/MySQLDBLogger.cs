using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Logger
{
    public class MySQLDBLogger : BaseLogger
    {
        protected string ConnStr { get; set; }

        public MySQLDBLogger(string ConnStr) {
            this.ConnStr = ConnStr;
        }

        public override void Error(string logName, Exception Ex)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var connection = new MySqlConnection(this.ConnStr))
                    {
                        connection.Open();

                        string query = $@"
                            INSERT INTO tbl_Logs(logName, createdDate, message, error)
                            VALUES('{logName}', '{ToMySQLDate(DateTime.Now)}', '', '{Ex.Message}');
                        ";

                        var cmd = new MySqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        var changesCount = cmd.ExecuteNonQueryAsync().Result;
                    }
                }
                catch (Exception ex) { }
            });
        }

        public override void Info(string logName, string Message, List<string> Params)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var connection = new MySqlConnection(this.ConnStr))
                    {
                        connection.Open();

                        string query = $@"
                            INSERT INTO tbl_Logs(logName, createdDate, message, error)
                            VALUES('{logName}', '{ToMySQLDate(DateTime.Now)}', '{Message}', '');
                        ";

                        var cmd = new MySqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        var changesCount = cmd.ExecuteNonQueryAsync().Result;
                    }
                }
                catch (Exception ex) { }
            });
        }

        public static string ToMySQLDate(DateTime me)
        {
            return me.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
