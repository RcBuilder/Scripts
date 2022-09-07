using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace App_Code
{
    public class DAL
    {
        protected string ConnStr { get; set; } = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString.Trim();

        public IEnumerable<Entities.VideoResourceData> GetVideoResources()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var query = @"SELECT Id, [FileName] FROM [dbo].[OnlineMovies] WHERE [FileName] <> ''";
                return conn.Query<Entities.VideoResourceData>(query, param: new { });
            }
        }

        public IEnumerable<Entities.BookResourceData> GetBookResources()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var query = @"SELECT Id, Book as 'FileName' FROM [dbo].[OnlineCourses] WHERE Book <> ''
                              UNION
                              SELECT Id, Book as 'FileName' FROM [dbo].[OnlineChapters] WHERE Book <> ''";
                return conn.Query<Entities.BookResourceData>(query, param: new { });
            }
        }
    }
}
