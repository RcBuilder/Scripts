using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DAL
{
    public class Items : Entities.IItemsDAL
    {
        public IEnumerable<Entities.Item> Get()
        {
            using (var conn = new SqlConnection(Settings.Instance.StoreConnectionString)) {
                return conn.Query<Entities.Item>("sp_Items_Get", commandType: CommandType.StoredProcedure);
            }
        }

        public int Save(Entities.Item Item)
        {
            using (var conn = new SqlConnection(Settings.Instance.StoreConnectionString))
            {
                return conn.Query<int>("sp_Item_Save", param: Item, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public bool Delete(int Code)
        {
            using (var conn = new SqlConnection(Settings.Instance.StoreConnectionString))
            {
                return conn.Execute("sp_Item_Delete", param: new { Code }, commandType: CommandType.StoredProcedure) > 0;                
            }
        }
    }
}
