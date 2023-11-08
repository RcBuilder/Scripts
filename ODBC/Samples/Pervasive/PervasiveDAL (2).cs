using Entities;
using Helpers;
using Newtonsoft.Json;
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
    public class PervasiveBaseDAL
    {
        protected string ConnetionString { get; set; }

        public PervasiveBaseDAL(string ConnetionString) {
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

        protected string ReadAsJson(OdbcDataReader dr)
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
    }

    public class PervasiveDAL : PervasiveBaseDAL {
        public PervasiveDAL(string ConnetionString) : base(ConnetionString) { }

        public IEnumerable<AccountData> GetAccountData() {
            try
            {
                var headerRows = new List<AccountDataHeader>();
                var bodyRows = new List<AccountDataBodyRow>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    // Load AccCard2MailHDR Rows
                    command.CommandText = "SELECT * FROM AccCard2MailHDR";
                    
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            headerRows.Add(new AccountDataHeader
                            {
                                DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                                AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                                Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                                Email = SafeConvert.ToString(reader["EmailHDR"])
                            });


                    // Load AccCard2MailBDY Rows
                    command.CommandText = "SELECT * FROM AccCard2MailBDY";

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            bodyRows.Add(new AccountDataBodyRow
                            {                                
                                DocumentId = Convert.ToInt32(reader["DOCCODE"]),                                
                                OpAcc = SafeConvert.ToString(reader["INACCBDY"]),
                                Details = SafeConvert.ToString(reader["ITEMNAMEBDY"]),
                                Asmac = SafeConvert.ToString(reader["ITEMNOBDY"]),
                                Asmac2 = SafeConvert.ToString(reader["BRIFNOBDY"]),
                                Credit = Convert.ToSingle(reader["ITEMPRCBDY"]),
                                Debit = Convert.ToSingle(reader["ITEMQTYBDY"]),                                 
                                Balance = Convert.ToSingle(reader["ITEMSUMBDY"]),
                                CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["LSTORENOBDY"])),
                                ValueDate = FromPervasiveDate(Convert.ToDouble(reader["PRICENOBDY"]))
                            });
                }

                var result = new List<AccountData>();

                foreach(var headerRow in headerRows)
                    result.Add(new AccountData
                    {
                        Header = headerRow,
                        Body = new AccountDataBody { 
                            Rows = bodyRows.Where(r => r.DocumentId == headerRow.DocumentId) 
                        }
                    });

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public CompanyInfo GetCompanyInfo()
        {
            var companyInfo = new CompanyInfo();
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;

                // CompLines
                command.CommandText = "SELECT * FROM CompLines";

                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var lineId = Convert.ToInt32(reader["RecNo"]);
                        companyInfo.CompanyLines[lineId] = new CompanyLine
                        {
                            LineId = lineId,
                            LineDesc = SafeConvert.ToString(reader["Desc"]),
                            LineToPrint = SafeConvert.ToString(reader["LineToPrint"], false),
                        };
                    }


                // CompDet
                command.CommandText = "SELECT * FROM CompDet";

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    companyInfo.CompanyName = SafeConvert.ToString(reader["Name"]);                    
                    companyInfo.OsekNo = SafeConvert.ToString(reader["OSEKNO"]);

                    // load ONLY if column exists                    
                    if (this.IsColumnExists($"CompDet", "LogoUrl1"))
                        companyInfo.LogoPath1 = SafeConvert.ToString(reader["LogoUrl1"]);
                    if (this.IsColumnExists($"CompDet", "LogoUrl2"))
                        companyInfo.LogoPath2 = SafeConvert.ToString(reader["LogoUrl2"]);
                    if (this.IsColumnExists($"CompDet", "LogoUrl3"))
                        companyInfo.LogoPath3 = SafeConvert.ToString(reader["LogoUrl3"]);
                    if (this.IsColumnExists($"CompDet", "LogoUrl4"))
                        companyInfo.LogoPath4 = SafeConvert.ToString(reader["LogoUrl4"]);
                }
            }

            return companyInfo;
        }

        private bool IsColumnExists(string tableName, string columnName)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@" SELECT  1
                                    FROM    X$File,X$Field
                                    WHERE   Xf$Id=Xe$File
                                    AND     Xf$Name = '{tableName}'
                                    AND     Xe$Name = '{columnName}'";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
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
