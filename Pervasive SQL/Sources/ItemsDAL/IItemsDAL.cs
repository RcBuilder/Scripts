
using Entities;
using System.Collections.Generic;

namespace ItemsDAL
{
    public interface IItemsDAL {
        string CreateItem(Item item);
        string UpdateItem(Item item);
        int GetStock(int id, int storeId);
        IEnumerable<Item> Find(ItemSearchParams searchParams);
    }
}
