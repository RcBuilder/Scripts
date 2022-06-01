using System.Collections.Generic;
using System.Threading.Tasks;
using HConnectEntities;

namespace HConnectProxy
{
    public interface IHConnectManager
    {                
        Task<bool> SaveAccount(Account Account);
        Task<bool> SaveJournalEntry(JournalEntry JournalEntry);
        Task<bool> SaveDocument(Document Document);
        Task<IEnumerable<ExportAccount>> GetAccounts(string DataFile);
    }
}
