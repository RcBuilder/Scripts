using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DAL
{
    public class Pervasive : IDalLayer
    {
        private const string ConnetionString = "Driver={Pervasive ODBC Client Interface};ServerName=CREATIVE-TEST;dbq=NONAME1r2517db;Client_CSet=UTF-8;Server_CSet=CP850;";

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
                                Date = DateTime.MinValue.AddDays(Convert.ToInt32(reader[4])),
                                Details = Encoding.GetEncoding(862).GetString(Encoding.GetEncoding("windows-1255").GetBytes(reader[5].ToString()))
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
                            Date = DateTime.MinValue.AddDays(Convert.ToInt32(reader[4])),
                            Details = Encoding.GetEncoding(862).GetString(Encoding.GetEncoding("windows-1255").GetBytes(reader[5].ToString()))
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