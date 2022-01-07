
using Entities;
using System.Collections.Generic;

namespace ItemsDAL
{
    public interface IItemsDAL {
        string CreateItem(Item item);
        string UpdateItem(Item item);
        int GetStock(string id, int storeId);
        int GetStockInOrders(string id);
        IEnumerable<Item> Find(ItemSearchParams searchParams);
        IEnumerable<ItemBarcode> GetBarcodes(string Id);
        IEnumerable<ItemBarcode> GetBarcodes();
    }
}
