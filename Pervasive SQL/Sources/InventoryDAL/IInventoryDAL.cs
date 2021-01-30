
using Entities;
using System.Collections.Generic;

namespace InventoryDAL
{
    public interface IInventoryDAL
    {
        BulkInsertStatus CreateInventory(Inventory inventory);
        InventoryStatus GetStockStatus();
    }
}
