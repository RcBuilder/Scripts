
using Entities;
using System.Collections.Generic;

namespace InventoryDAL
{
    public interface IInventoryDAL
    {
        BulkInsertStatus CreateInventory(Inventory inventory);
        InventoryStatus GetStockStatus();
        InventoryStatus GetStockStatus(string itemId);
        bool StoreItemExists(StoreItem storeItem);
        bool StoreItemUpdate(StoreItem storeItem);        
        bool CreateInventoryTransaction(InventoryTransaction transaction);
        int GetNextDocCounter(eCounterNo counterNo);
        bool SetNextDocCounter(eCounterNo counterNo);
        int GetNextTransactionId(eTransactionProvider provider);
        IEnumerable<Store> GetStores();
    }
}
