using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DAL
{
    public class Access : IDalLayer
    {
        private static readonly string App_Data = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\");
        private static readonly string ConnetionString = $"Driver={{Microsoft Access Driver (*.mdb)}};Dbq={App_Data}\\testDB.mdb;";

        public IEnumerable<Entities.Transaction> GetTransactions()
        {
            var result = new List<Entities.Transaction>();
            try
            {
                using (var connection = new OdbcConnection(ConnetionString))
                {
                    connection.Open();
                    var query = "select top 100 TrsNo, accNo, Asmac1, \"Sum\", Date, Details from AccTrs";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Entities.Transaction
                            {
                                TrsNo = Convert.ToInt32(reader[0]),
                                accNo = reader[1].ToString(),
                                Asmac1 = reader[2].ToString(),
                                Sum = Convert.ToDouble(reader[3]),
                                Date = Convert.ToDateTime(reader[4]),
                                Details = reader[5].ToString()
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Entities.Transaction GetTransactionDetails(int TrsNo)
        {
            try
            {
                using (var connection = new OdbcConnection(ConnetionString))
                {
                    connection.Open();
                    var query = $"select TrsNo, accNo, Asmac1, \"Sum\", Date, Details from AccTrs where TrsNo = {TrsNo}";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return new Entities.Transaction
                        {
                            TrsNo = Convert.ToInt32(reader[0]),
                            accNo = reader[1].ToString(),
                            Asmac1 = reader[2].ToString(),
                            Sum = Convert.ToDouble(reader[3]),
                            Date = Convert.ToDateTime(reader[4]),
                            Details = reader[5].ToString()
                        };
                    }
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