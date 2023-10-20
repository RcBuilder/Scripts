using System;
using System.Collections.Generic;

using PCN874;
using static PCN874.PCN874Entities;

namespace PCN874Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExport();
            TestExportFromCSV();
            Console.ReadKey();
        }

        static void TestExportFromCSV() {
            var manager = new PCN874Manager();

            var destPath = $"{AppContext.BaseDirectory}pcn784_{DateTime.Now.Ticks}.txt";
            var csvPath = $"{AppContext.BaseDirectory}pcn_csv_input1.txt";

            var config = new ExportConfig(destPath, "514457282", (2023, 8));
            manager.Export(csvPath, config);

            Console.WriteLine($"exported to {destPath}");
        }

        static void TestExport() {
            var manager = new PCN874Manager();

            var destPath = $"{AppContext.BaseDirectory}pcn784_{DateTime.Now.Ticks}.txt";

            var transactionEntries = new List<TransactionEntry>();
            transactionEntries.Add(new L_SalesUnidentifiedTrn
            { 
                IdentificationNumber = "001495706".PadLeft(9, '0'),
                InvoiceDate = "20230811",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "101".PadLeft(9, '0'),
                TotalVatAmount = "0".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "558505".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });
            transactionEntries.Add(new S_RegularSalesTrn {
                IdentificationNumber = "515219731".PadLeft(9, '0'),
                InvoiceDate = "20230811",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "201".PadLeft(9, '0'),
                TotalVatAmount = "20400".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "120000".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });
            transactionEntries.Add(new T_InputRegularTrn {
                IdentificationNumber = "515948321".PadLeft(9, '0'),
                InvoiceDate = "20230511",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "301".PadLeft(9, '0'),
                TotalVatAmount = "8500".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "50000".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });
            transactionEntries.Add(new T_InputRegularTrn {
                IdentificationNumber = "514486547".PadLeft(9, '0'),
                InvoiceDate = "20230411",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "401".PadLeft(9, '0'),
                TotalVatAmount = "6800".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "40000".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });
            transactionEntries.Add(new T_InputRegularTrn {
                IdentificationNumber = "013360714".PadLeft(9, '0'),
                InvoiceDate = "20230311",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "501".PadLeft(9, '0'),
                TotalVatAmount = "5100".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "30000".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });            
            transactionEntries.Add(new T_InputRegularTrn {
                IdentificationNumber = "515948321".PadLeft(9, '0'),
                InvoiceDate = "20230215",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "701".PadLeft(9, '0'),
                TotalVatAmount = "7650".PadLeft(9, '0'),
                CreditSymbol = "+",
                TotalInvoiceAmount = "45000".PadLeft(10, '0'),
                FutureData = "".PadLeft(9, '0')
            });

            var config = new ExportConfig(destPath, "514457282", (2023, 10));
            manager.Export(transactionEntries, config);

            Console.WriteLine($"exported to {destPath}");
        }
    }
}
