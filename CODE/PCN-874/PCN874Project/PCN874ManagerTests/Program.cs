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
            TestExport();return;
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
                TotalVatAmount = "17",
                CreditSymbol = "+",
                TotalInvoiceAmount = "100",
                FutureData = ""
            });
            transactionEntries.Add(new S_RegularSalesTrn
            {
                IdentificationNumber = "515219731",
                InvoiceDate = "20230811",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "201",
                TotalVatAmount = "34",
                CreditSymbol = "+",
                TotalInvoiceAmount = "200",
                FutureData = ""
            });
            transactionEntries.Add(new T_InputRegularTrn
            {
                IdentificationNumber = "515948321",
                InvoiceDate = "20230511",
                ReferenceGroup = "0000",
                InvoiceReferenceNumber = "301",
                TotalVatAmount = "42",
                CreditSymbol = "+",
                TotalInvoiceAmount = "250",
                FutureData = ""
            });

            return transactionEntries;
        }
    }
}
