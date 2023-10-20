using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCNUtils.enums;

namespace PCNUtils.Transactions
{
    public abstract class AbstractTransaction : ITransactionEntry
    {
        public string buildTransactionRow()
        {
            StringBuilder sbRow = new StringBuilder();
            sbRow.Append(TransactionType.ToString());
            sbRow.Append(IdentificationNumber);
            sbRow.Append(InvoiceDate);
            sbRow.Append(ReferenceGroup);
            sbRow.Append(InvoiceReferenceNumber);
            sbRow.Append(TotalVatAmount);
            sbRow.Append(CreditSymbol);
            sbRow.Append(TotalInvoiceAmount);
            sbRow.Append(FutureData);
            return sbRow.ToString();
        }

        public abstract eTransactionType TransactionType { get;}
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
