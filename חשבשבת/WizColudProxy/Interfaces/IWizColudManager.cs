using System.Collections.Generic;
using System.Threading.Tasks;
using WizColudEntities;

namespace WizColudProxy
{
    public interface IWizColudManager
    {
        Task<bool> RefreshToken();
        Task<CompanyList> GetCompanyList();
        Task<bool> SaveAccount(Account Account);
        Task<int> SaveJournalEntry(JournalEntry JournalEntry);
        Task<IEnumerable<ExportAccount>> GetAccounts(string DataFile);
    }
}
