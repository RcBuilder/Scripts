
using Entities;
using System.Collections.Generic;

namespace AccountsDAL
{
    public interface IAccountsDAL {
        Account GetAccount(int id);
        int CreateAccount(Account account);
        int UpdateAccount(Account account);
        IEnumerable<Account> Find(AccountSearchParams searchParams);
        bool AccountExists(int accountId);
        bool UpdateAccountingTransactionTotals(int accountId);
        ///int GetNextAccountId(int accountType);
        int GetNextAccountId(IdentityRange range);
        IdentityRange GetIdentityRange(int accountType);
        string AccountType2AccountName(int accountType);
    }
}
