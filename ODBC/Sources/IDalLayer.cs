using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL
{
    public interface IDalLayer
    {
        IEnumerable<Entities.Transaction> GetTransactions();
        Entities.Transaction GetTransactionDetails(int TrsNo);
    }
}