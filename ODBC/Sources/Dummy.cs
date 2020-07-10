using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DAL
{
    public class Dummy : IDalLayer
    {        
        private static IEnumerable<Entities.Transaction> TRANSACTIONS_TABLE = new List<Entities.Transaction> {
            new Entities.Transaction { TrsNo = 1, accNo = "1001", Asmac1 = "Asmac1", Date = DateTime.Now, Sum = 300, Details = "bla bla bla" },
            new Entities.Transaction { TrsNo = 2, accNo = "1002", Asmac1 = "Asmac1", Date = DateTime.Now, Sum = 450, Details = "bla bla bla" },
            new Entities.Transaction { TrsNo = 3, accNo = "1003", Asmac1 = "Asmac1", Date = DateTime.Now, Sum = 500, Details = "bla bla bla" },
            new Entities.Transaction { TrsNo = 4, accNo = "1004", Asmac1 = "Asmac1", Date = DateTime.Now, Sum = 520, Details = "bla bla bla" }
        };

        public IEnumerable<Entities.Transaction> GetTransactions()
        {
            return TRANSACTIONS_TABLE;
        }

        public Entities.Transaction GetTransactionDetails(int TrsNo)
        {
            return TRANSACTIONS_TABLE.FirstOrDefault(x => x.TrsNo == TrsNo);
        }
    }
}