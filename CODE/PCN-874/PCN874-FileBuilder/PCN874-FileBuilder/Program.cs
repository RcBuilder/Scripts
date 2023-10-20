using System;
using System.Collections.Generic;
using PCNUtils;
using PCNUtils.enums;
using PCNUtils.Transactions;

namespace PCN874_FileBuilder
{
    class Program
    {
        static void Main(string[] args)
        {


            Test_ExportPCN874();

            //Test_ExportPCN874_From_CsvFile();

        }

        private static void Test_ExportPCN874_From_CsvFile()
        {

            string identificationNumber = "514457282";
            PCNData pcn = new PCNData(identificationNumber, 2023, 8);
            pcn.ExportPCN874FromCsv($@"{AppContext.BaseDirectory}pcn_csv_input1.txt", $@"{AppContext.BaseDirectory}out_pcn784.txt");
        }

        private static void Test_ExportPCN874()
        {

            List<ITransactionEntry> transactionsData = new List<ITransactionEntry>();
            ITransactionEntry trnEntry = null;

            trnEntry = new L_SalesUnidentifiedTrn();
            trnEntry.IdentificationNumber = "001495706".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230811";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "101".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "0".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "558505".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);

            trnEntry = new S_RegularSalesTrn();
            trnEntry.IdentificationNumber = "515219731".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230811";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "201".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "20400".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "120000".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);

            trnEntry = new T_InputRegularTrn();
            trnEntry.IdentificationNumber = "515948321".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230511";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "301".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "8500".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "50000".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);


            trnEntry = new T_InputRegularTrn();
            trnEntry.IdentificationNumber = "514486547".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230411";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "401".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "6800".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "40000".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);


            trnEntry = new T_InputRegularTrn();
            trnEntry.IdentificationNumber = "013360714".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230311";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "501".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "5100".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "30000".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);


            trnEntry = new T_InputRegularTrn();
            trnEntry.IdentificationNumber = "515948321".PadLeft(9, '0');
            trnEntry.InvoiceDate = "20230215";
            trnEntry.ReferenceGroup = "0000";
            trnEntry.InvoiceReferenceNumber = "701".PadLeft(9, '0');
            trnEntry.TotalVatAmount = "7650".PadLeft(9, '0');
            trnEntry.CreditSymbol = "+";
            trnEntry.TotalInvoiceAmount = "45000".PadLeft(10, '0');
            trnEntry.FutureData = "".PadLeft(9, '0');
            transactionsData.Add(trnEntry);

            /**********************************************************************************************/
            // ExportPCN874 

            string identificationNumber = "514457282";
            PCNData pcn = new PCNData(identificationNumber, 2023, 10);
            pcn.ExportPCN874(transactionsData,@"C:\Dev\Tasks\PCN874-creation\out_pcn784.txt");

        }
    }
}
