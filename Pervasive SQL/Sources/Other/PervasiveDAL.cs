using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.PervasiveDBHelper;

namespace BrokersMonitorService
{
    public class PervasiveDAL
    {
        protected string ConnStr { get; set; }

        public PervasiveDAL(string ConnStr) {
            this.ConnStr = ConnStr;
        }

        public IEnumerable<CompanyDetails> GetClientCompanies()
        {
            try
            {
                var companies = new List<CompanyDetails>();

                using (var connection = new OdbcConnection(this.ConnStr))
                {
                    connection.Open();

                    var query = $@"
                        SELECT C.*, CDB.OdbcDBname 
                        FROM COMPANY C 
                        INNER JOIN 
                        COMPANYDB CDB 
                        ON (C.CompNo = CDB.CompNo)
                    ";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            companies.Add(new CompanyDetails
                            {
                                Id = SafeConvert.ToInt32(reader["CompNo"]),                                
                                Name = SafeConvert.ToString(reader["Name"]),
                                Path = SafeConvert.ToString(reader["Dir"]),
                                /// Company = SafeConvert.ToString(reader["Company"]),
                                OdbcName = SafeConvert.ToString(reader["OdbcDBname"])      
                            });
                        }
                    }

                    return companies;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("PervasiveDAL", ex.Message);
                throw;
            }
        }
    }
}
