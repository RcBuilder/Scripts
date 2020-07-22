using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public interface ICelebritiesBLL
    {
        List<CelebrityCard> Get();
        int Save(IEnumerable<CelebrityCard> Cards);
        bool Delete(string CardId);
        List<CelebrityCard> Reload();
        bool Create(CelebrityCard Card);
        bool Update(CelebrityCard Card);
    }

    public interface ICelebritiesAsyncBLL
    {
        Task<List<CelebrityCard>> GetAsync();
        Task<int> SaveAsync(IEnumerable<CelebrityCard> Cards);
        Task<bool> DeleteAsync(string CardId);
        Task<List<CelebrityCard>> ReloadAsync();
        Task<bool> CreateAsync(CelebrityCard Card);
        Task<bool> UpdateAsync(CelebrityCard Card);
    }
}
