using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCNUtils.enums;

namespace PCNUtils
{
    public interface ITransactionEntry
    {
        string buildTransactionRow();
        eTransactionType TransactionType { get;} // 1 digit
        string IdentificationNumber  { get; set; }      // 9 digits         
        string InvoiceDate { get; set; }         // 8 digits
        string ReferenceGroup { get; set; }       // 4 digits
        string InvoiceReferenceNumber { get; set; }   // 9 digits
        string TotalVatAmount { get; set; }           // 9 digits
        string CreditSymbol { get; set; }           // 1 digits  +/-  Cancellation/credit from supplier or customer – always in minus

        string TotalInvoiceAmount { get; set; }    // 10 digits   
        string FutureData { get; set; }           // 9 digits  (all zero)
    }
}
