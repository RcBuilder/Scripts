
using Entities;
using System.Collections.Generic;

namespace AccountsDAL
{
    public interface IAccountsDAL {
        int CreateAccount(Account account);
        int UpdateAccount(Account account);
        IEnumerable<Account> Find(AccountSearchParams searchParams);
    }
}
