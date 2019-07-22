using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Configuration;
using Dapper;
using System.Data;

namespace DAL
{
    public class SearchResults
    {
        static string ConnectionString { set; get; }
        static SearchResults() {
            ConnectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString.Trim();
        }

        public static int Add(Entities.SearchResult Result)
        {
            using (var conn = new SqlConnection(SearchResults.ConnectionString))
                return conn.QueryFirst<int>("sp_Search_Add", param: new
                {
                    Title = Result.Title,
                    NameHE = (byte)Result.SearchEngineType,                    
                }, commandType: CommandType.StoredProcedure);
        }
    }
}
