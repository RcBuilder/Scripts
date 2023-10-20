using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCNUtils.enums;

namespace PCNUtils.Transactions
{
    public class S_RegularSalesTrn : AbstractTransaction
    {
        public override eTransactionType TransactionType => eTransactionType.S;
        public string IdentificationNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string ReferenceGroup { get; set; }
        public string InvoiceReferenceNumber { get; set; }
        public string TotalVatAmount { get; set; }
        public string CreditSymbol { get; set; }
        public string TotalInvoiceAmount { get; set; }
        public string FutureData { get; set; }
    }
}
