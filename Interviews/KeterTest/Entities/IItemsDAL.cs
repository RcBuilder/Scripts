using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IItemsDAL
    {
        IEnumerable<Item> Get();
        int Save(Item Item);
        bool Delete(int Code);
    }
}
