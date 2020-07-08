using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IItemsBLL
    {
        IEnumerable<Item> Get();
        int Save(Item Item);
        bool Delete(int Code);
    }

    public interface IItemsAsyncBLL
    {
        Task<IEnumerable<Item>> GetAsync();
        Task<int> SaveAsync(Item Item);
        Task<bool> DeleteAsync(int Code);
    }
}
