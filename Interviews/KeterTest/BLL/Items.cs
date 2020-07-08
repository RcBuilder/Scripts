using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Items : Entities.IItemsBLL, Entities.IItemsAsyncBLL
    {        
        public IEnumerable<Entities.Item> Get()
        {
            var dal = new DAL.Items();
            return dal.Get();
        }

        public int Save(Entities.Item Item)
        {
            var dal = new DAL.Items();
            return dal.Save(Item);
        }

        public bool Delete(int Code) {
            var dal = new DAL.Items();
            return dal.Delete(Code);
        }

        public async Task<IEnumerable<Entities.Item>> GetAsync() {
            return await Task.Factory.StartNew(() =>
            {
                return this.Get();
            });
        }

        public async Task<int> SaveAsync(Entities.Item Item) {
            return await Task.Factory.StartNew(() =>
            {
                return this.Save(Item);
            });
        }

        public async Task<bool> DeleteAsync(int Code) {
            return await Task.Factory.StartNew(() =>
            {
                return this.Delete(Code);
            });
        }
    }
}
