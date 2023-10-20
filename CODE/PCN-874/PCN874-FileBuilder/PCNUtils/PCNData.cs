using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCNUtils.enums;
using PCNUtils.Transactions;

namespace PCNUtils
{
    public class PCNData
    {        
        private List<ITransactionEntry> _transactionsData;
        private string _identificationNumber;
        private int _yearReportPeriod;
        private int _monthReportPeriod; 
        private HeaderEntry _headerEntry;

        public PCNData(string identificationNumber, int yearReportPeriod,int monthReportPeriod)
        {
            _identificationNumber = identificationNumber;
            _yearReportPeriod = yearReportPeriod;
            _monthReportPeriod = monthReportPeriod;
            _headerEntry = new HeaderEntry(_identificationNumber);
        }

        public bool ExportPCN874(List<ITransactionEntry> transactionsData,string targetFilePath)
        {
            try
            {
                StringBuilder pcn874Data = new StringBuilder();

                _transactionsData = transactionsData;
                string headerRow = _headerEntry.buildHeaderRow(_transactionsData, _yearReportPeriod, _monthReportPeriod);


                using (StreamWriter writer = new StreamWriter(targetFilePath))
                {
                    writer.WriteLine(headerRow);
                    foreach (ITransactionEntry transEntry in transactionsData)
                    {
                        string transactionRow = transEntry.buildTransactionRow();
                        writer.WriteLine(transactionRow);
                    }

                    string closingEntry = $"X{_identificationNumber}";
                    writer.WriteLine(closingEntry);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in ExportPCN874 {e}");
                return false;
            }
    
            return true;
        }


        public bool ExportPCN874FromCsv(string csvFilePath,string targetFilePath)
        {
            bool status = true;
            try
            {
                List<ITransactionEntry> transactionsData = new List<ITransactionEntry>();
                ITransactionEntry trnEntry = null;

                var filestream = new System.IO.FileStream(csvFilePath,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read,
                    System.IO.FileShare.ReadWrite);

                var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
                string line = "";

                while ((line = file.ReadLine()) != null)
                {
                    List<string> listOfTransValues = line.Split(',').ToList();

                    eTransactionType enumValue = (eTransactionType)
                        Enum.Parse(typeof(eTransactionType), listOfTransValues[0]);

                    switch (enumValue)
                    {
                        case eTransactionType.L:
                            trnEntry = new L_SalesUnidentifiedTrn();
                            break;
                        case eTransactionType.S:
                            trnEntry = new S_RegularSalesTrn();
                            break;
                        case eTransactionType.T:
                            trnEntry = new T_InputRegularTrn();
                            break;

                        default:
                            break;
                    }

                    trnEntry.IdentificationNumber = listOfTransValues[1].PadLeft(9, '0');
                    trnEntry.InvoiceDate = listOfTransValues[2];
                    trnEntry.ReferenceGroup = listOfTransValues[3];
                    trnEntry.InvoiceReferenceNumber = listOfTransValues[4].PadLeft(9, '0');
                    trnEntry.TotalVatAmount = listOfTransValues[5].PadLeft(9, '0');
                    trnEntry.CreditSymbol = listOfTransValues[6];
                    trnEntry.TotalInvoiceAmount = listOfTransValues[7].PadLeft(10, '0');
                    trnEntry.FutureData = "".PadLeft(9, '0');

                    // build the transaction list 
                    transactionsData.Add(trnEntry);

                   
                }

                status = ExportPCN874(transactionsData, targetFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in ExportPCN874FromCsv {e}");
                return false;
            }

            return status;
        }





  

    }
}
