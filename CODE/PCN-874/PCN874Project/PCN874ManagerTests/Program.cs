using System;
using System.Collections.Generic;
using System.IO;
using PCN874;
using static PCN874.PCN874Entities;

namespace PCN874Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExport(); return;
            TestExportFromCSV();            

            TestGenerate();
            TestGenerateFromCSV();

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
            var transactionEntries = GetEntries();

            var config = new ExportConfig(destPath, "514457282", (2023, 10));
            manager.Export(transactionEntries, config);

            Console.WriteLine($"exported to {destPath}");
        }
        
        static void TestGenerateFromCSV() {
            var manager = new PCN874Manager();

            var destPath = $"{AppContext.BaseDirectory}pcn784_{DateTime.Now.Ticks}.txt";
            var csvPath = $"{AppContext.BaseDirectory}pcn_csv_input1.txt";

            var config = new ExportConfig(destPath, "514457282", (2023, 8));
            var sContent = manager.Generate(csvPath, config);
            
            Console.WriteLine($"generated to {destPath}");
            File.WriteAllText(destPath, sContent);
        }

        static void TestGenerate() {
            var manager = new PCN874Manager();

            var destPath = $"{AppContext.BaseDirectory}pcn784_{DateTime.Now.Ticks}.txt";
            var transactionEntries = GetEntries();

            var config = new ExportConfig(destPath, "514457282", (2023, 10));
            var sContent = manager.Generate(transactionEntries, config);

            Console.WriteLine($"generated to {destPath}");
            File.WriteAllText(destPath, sContent);
        }

        static List<TransactionEntry> GetEntries() {
            var transactionEntries = new List<TransactionEntry>();
            
            transactionEntries.Add(new S_RegularSalesTrn
            {
                IdentificationNumber = "001495706",
                InvoiceDate = "20230811",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "101",
                TotalVatAmount = 17,
                CreditSymbol = "+",
                TotalInvoiceAmount = 100,
                FutureData = ""
            });
            transactionEntries.Add(new S_RegularSalesTrn
            {
                IdentificationNumber = "515219731",
                InvoiceDate = "20230811",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "201",
                TotalVatAmount = 34,
                CreditSymbol = "+",
                TotalInvoiceAmount = 200,
                FutureData = ""
            });
            transactionEntries.Add(new T_InputRegularTrn
            {
                IdentificationNumber = "515948321",
                InvoiceDate = "20230511",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "301",
                TotalVatAmount = 42.6F,
                CreditSymbol = "+",
                TotalInvoiceAmount = 250,
                FutureData = ""
            });

            /*
                transactionEntries.Add(new L_SalesUnidentifiedTrn
                {
                    IdentificationNumber = "001495706",
                    InvoiceDate = "20230811",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "101",
                    TotalVatAmount = 0,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 558505,
                    FutureData = ""
                });
                transactionEntries.Add(new S_RegularSalesTrn
                {
                    IdentificationNumber = "515219731",
                    InvoiceDate = "20230811",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "201",
                    TotalVatAmount = 20400,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 120000,
                    FutureData = ""
                });
                transactionEntries.Add(new T_InputRegularTrn
                {
                    IdentificationNumber = "515948321",
                    InvoiceDate = "20230511",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "301",
                    TotalVatAmount = 8500,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 50000,
                    FutureData = ""
                });
                transactionEntries.Add(new T_InputRegularTrn
                {
                    IdentificationNumber = "514486547",
                    InvoiceDate = "20230411",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "401",
                    TotalVatAmount = 6800,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 40000,
                    FutureData = ""
                });
                transactionEntries.Add(new T_InputRegularTrn
                {
                    IdentificationNumber = "013360714",
                    InvoiceDate = "20230311",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "501",
                    TotalVatAmount = 5100,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 30000,
                    FutureData = ""
                });
                transactionEntries.Add(new T_InputRegularTrn
                {
                    IdentificationNumber = "515948321",
                    InvoiceDate = "20230215",
                    ReferenceGroup = "0000",
                    InvoiceReferenceNumber = "701",
                    TotalVatAmount = 7650,
                    CreditSymbol = "+",
                    TotalInvoiceAmount = 45000,
                    FutureData = ""
                }); 
            */

            return transactionEntries;
        }
    }
}
