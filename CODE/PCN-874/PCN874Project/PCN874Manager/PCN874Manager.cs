using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static PCN874.PCN874Entities;

namespace PCN874
{
    // TODO ->> Document (see RESEARCH\PCN-874)
    /*
        Reference
        ---------


        EntryType
        ---------
        תשומות - INPUT
        הכנסות - SALES


        Structure
        ---------
        [header]
        [entry]
        [entry]
        [entry]
        ...
        ...
        ...
        [footer]


        Header Notes
        ------------         
        * תשומות ציוד - 0 קבוע
        * כמות סך עסקאות T 
        * מספר תנועות - group-by סוג העסקה 
        * תשומות אחרות - סכום כלל העסקאות מע"מ T  
        * סכום להחזר - 'תשומות אחרות' פחות 'מע"מ עסקאות חייבות'

        
        USING
        ----- 
        // Export from CSV
        var manager = new PCN874Manager();

        var destPath = $"{AppContext.BaseDirectory}pcn784_{DateTime.Now.Ticks}.txt";
        var csvPath = $"{AppContext.BaseDirectory}pcn_csv_input1.txt";

        var config = new ExportConfig(destPath, "514457282", (2023, 8));
        manager.Export(csvPath, config);

        -

        // Export from Entries
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

    */
    public class PCN874Entities
    {
        public enum eTransactionType
        {
            S = 0,  // Sales - Regular
            L = 1,  // Sales - Unidentified
            M = 2,
            Y = 3,
            I = 4,
            T = 5,  // Input - Regular
            K = 6,
            R = 7,
            P = 8,
            H = 9,
            C = 10
        }

        public interface IBuildable {
            string Build();
        }
        public interface IHeaderBuildable {
            string BuildHeader(IEnumerable<TransactionEntry> transactionEntries, HeaderConfig Config);
        }

        public interface ITransactionEntry
        {
            eTransactionType TransactionType { get; }       // 1 digit
            string IdentificationNumber { get; set; }       // 9 digits         
            string InvoiceDate { get; set; }                // 8 digits
            string ReferenceGroup { get; set; }             // 4 digits
            string InvoiceReferenceNumber { get; set; }     // 9 digits
            string TotalVatAmount { get; set; }             // 9 digits
            string CreditSymbol { get; set; }               // 1 digits  +/-  Cancellation/credit from supplier or customer – always in minus
            string TotalInvoiceAmount { get; set; }         // 10 digits   
            string FutureData { get; set; }                 // 9 digits  (all zero)
        }

        public abstract class TransactionEntry : ITransactionEntry, IBuildable
        {
            public abstract eTransactionType TransactionType { get; }
            public string IdentificationNumber { get; set; }
            public string InvoiceDate { get; set; }
            public string ReferenceGroup { get; set; }
            public string InvoiceReferenceNumber { get; set; }
            public string TotalVatAmount { get; set; }
            public string CreditSymbol { get; set; }
            public string TotalInvoiceAmount { get; set; }
            public string FutureData { get; set; }

            public virtual string Build()
            {
                var sbRow = new StringBuilder();
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
        }

        public class L_SalesUnidentifiedTrn : TransactionEntry {
            public override eTransactionType TransactionType => eTransactionType.L;
        }

        public class S_RegularSalesTrn : TransactionEntry {
            public override eTransactionType TransactionType => eTransactionType.S;
        }

        public class T_InputRegularTrn : TransactionEntry {
            public override eTransactionType TransactionType => eTransactionType.T;
        }

        public class HeaderConfig {            
            public string IdentificationNumber { get; protected set; }
            public (int Year, int Month) ReportPeriod { get; protected set; }

            public HeaderConfig(string identificationNumber, (int Year, int Month) reportPeriod)
            {                
                this.IdentificationNumber = identificationNumber;
                this.ReportPeriod = reportPeriod;
            }
        }

        public class ExportConfig : HeaderConfig
        {
            public string FilePath { get; protected set; }           

            public ExportConfig(string filePath, string identificationNumber, (int Year, int Month) reportPeriod) : base(identificationNumber, reportPeriod)
            {
                this.FilePath = filePath;                
            }
        }

        public class HeaderEntry : IHeaderBuildable
        {
            protected string EntryType => "O";
            protected string IdentificationNumber { get; set; }                               // 9 digits
            protected string MonthReportDate { get; set; }                                    // 6 digits   , Yyyymm form
            protected string ReportType => "1";
            protected string FileGenerationDate { get; set; }                                 // 8 digits ,Yyyymmdd form
            protected string SymbolTotalTaxableSales { get; set; } = "+";                     // 1 digits +/-
            protected string TotalAmountTaxableSales { get; set; }                            // 11 digits
            protected string SymbolTotalVatOnTaxableSales { get; set; } = "+";                // 1 digits +/-
            protected string TotalVatTaxableSales { get; set; }                               // 9 digits
            protected string SymbolDifferentRateTaxableSales { get; set; } = "+";             // 1 digits +/-  , Currently "+"
            protected string TotalSalesTaxableDifferentRate { get; set; } = "00000000000";    // 11 zeros / Currently zeros – for future use
            protected string SymbolDifferentRateTaxableSalesVAT { get; set; } = "+";          // 1 digits +/-  , Currently "+"
            protected string TotalVATonSalesTaxableDifferentRate { get; set; } = "000000000"; // 9 digits  , Currently zeros – for future use
            protected string TotalNumberOfRecordsForSales { get; set; }                       // 9 digits , Number of sales records - both taxable and zero-rated/ exempt
            protected string SymbolTotalOfZeroValueAndExemptSales { get; set; } = "+";        // 1 digits +/-  
            protected string TotalZeroValueExemptSales { get; set; }                          // 11 digits
            protected string SymbolTotalVATonOtherInputs { get; set; } = "+";                 // 1 digits +/-  
            protected string TotalVATonOtherInputs { get; set; }                              // 9 digits +/- 
            protected string SymbolTotalVATOnEquipment { get; set; } = "+";                   // 1 digits +/-  
            protected string TotalVATonEquipment { get; set; } = "000000000";                 // 9 digits +/- , zeros 
            protected string TotalNumberOfRecordsForInputs { get; set; }                      // 9 digits +/-     , number of transactions of T type
            protected string SymbolTotalVATToPayReceive { get; set; }                         // 1 digits +/-   ( can be + or -   , depend on total VAT of S   -   Total VAT of T) 
            protected string TotalVATtoToPayReceive { get; set; }                             // 11 digits ( total VAT of S   -   Total VAT of T )

            public virtual string BuildHeader(IEnumerable<TransactionEntry> transactionEntries, HeaderConfig Config) 
            {
                var sb = new StringBuilder();

                sb.Append(EntryType);
                this.IdentificationNumber = Config.IdentificationNumber;
                sb.Append(this.IdentificationNumber);

                var month = Config.ReportPeriod.Month.ToString().PadLeft(2, '0');
                var year = Config.ReportPeriod.Year.ToString();                
                this.MonthReportDate = $"{year}{month}";

                sb.Append(this.MonthReportDate);
                sb.Append(this.ReportType);

                this.FileGenerationDate = DateTime.Now.ToString("yyyyMMdd");
                sb.Append(this.FileGenerationDate);

                sb.Append(this.SymbolTotalTaxableSales);

                this.TotalAmountTaxableSales = CalcTotalAmountTaxableSales(transactionEntries);
                sb.Append(this.TotalAmountTaxableSales);

                sb.Append(SymbolTotalVatOnTaxableSales);

                this.TotalVatTaxableSales = CalcTotalAmountVATSales(transactionEntries);
                sb.Append(this.TotalVatTaxableSales);

                sb.Append(this.SymbolDifferentRateTaxableSales);
                sb.Append(this.TotalSalesTaxableDifferentRate);
                sb.Append(SymbolDifferentRateTaxableSalesVAT);
                sb.Append(this.TotalVATonSalesTaxableDifferentRate);

                this.TotalNumberOfRecordsForSales = CalcTotalNumberOfSales(transactionEntries);
                sb.Append(TotalNumberOfRecordsForSales);

                sb.Append(SymbolTotalOfZeroValueAndExemptSales);
                this.TotalZeroValueExemptSales = CalcTotalZeroValueExemptSales(transactionEntries);

                sb.Append(this.TotalZeroValueExemptSales);
                sb.Append(SymbolTotalVATonOtherInputs);

                this.TotalVATonOtherInputs = CalcTotalVATonOtherInputs(transactionEntries);
                sb.Append(this.TotalVATonOtherInputs);

                sb.Append(SymbolTotalVATOnEquipment);
                sb.Append(this.TotalVATonEquipment);

                this.TotalNumberOfRecordsForInputs = CalcTotalRecordsInputs(transactionEntries);
                sb.Append(TotalNumberOfRecordsForInputs);

                var diffTotalVATtoToPayReceive = long.Parse(this.TotalVatTaxableSales) - long.Parse(this.TotalVATonOtherInputs);
                if (diffTotalVATtoToPayReceive <= 0)
                {
                    this.SymbolTotalVATToPayReceive = "-";
                    sb.Append(this.SymbolTotalVATToPayReceive);

                    // cancel the - sign from the number
                    diffTotalVATtoToPayReceive = diffTotalVATtoToPayReceive * -1;
                }
                else
                {
                    this.SymbolTotalVATToPayReceive = "+";
                    sb.Append(this.SymbolTotalVATToPayReceive);
                }

                this.TotalVATtoToPayReceive = diffTotalVATtoToPayReceive.ToString().PadLeft(11, '0');
                sb.Append(this.TotalVATtoToPayReceive.ToString().PadLeft(11, '0'));

                return sb.ToString();
            }

            // --

            private string CalcTotalRecordsInputs(IEnumerable<TransactionEntry> transactionEntries)
            {                
                var rowCount = transactionEntries.Count(e => e.TransactionType == eTransactionType.T);
                return rowCount.ToString().PadLeft(9, '0');
            }

            private string CalcTotalVATonOtherInputs(IEnumerable<TransactionEntry> transactionEntries)
            {               
                var sumTotalVat = transactionEntries.Where(e => e.TransactionType == eTransactionType.T).Sum(e => Convert.ToInt32(e.TotalVatAmount));
                return sumTotalVat.ToString().PadLeft(9, '0');
            }

            private string CalcTotalZeroValueExemptSales(IEnumerable<TransactionEntry> transactionEntries)
            {           
                var sumTotal = transactionEntries.Where(e => e.TransactionType == eTransactionType.L).Sum(e => Convert.ToInt32(e.TotalInvoiceAmount));
                return sumTotal.ToString().PadLeft(11, '0');
            }

            private string CalcTotalNumberOfSales(IEnumerable<TransactionEntry> transactionEntries)
            {              
                var rowCount = transactionEntries.Count(e => e.TransactionType == eTransactionType.S || e.TransactionType == eTransactionType.L);
                return rowCount.ToString().PadLeft(9, '0');
            }

            private string CalcTotalAmountTaxableSales(IEnumerable<TransactionEntry> transactionEntries)
            {
                var sumTotal = transactionEntries.Where(e => e.TransactionType == eTransactionType.S).Sum(e => Convert.ToInt32(e.TotalInvoiceAmount));
                return sumTotal.ToString().PadLeft(11, '0');
            }

            private string CalcTotalAmountVATSales(IEnumerable<TransactionEntry> transactionEntries)
            {                
                var sumTotalVat = transactionEntries.Where(e => e.TransactionType == eTransactionType.S).Sum(e => Convert.ToInt32(e.TotalVatAmount));
                return sumTotalVat.ToString().PadLeft(9, '0');
            }
        }
    }

    public interface IPCN874Manager 
    {
        bool Export(IEnumerable<TransactionEntry> transactionEntries, ExportConfig Config);
        bool Export(string csvFilePath, ExportConfig Config);
    }

    public class PCN874Manager : IPCN874Manager
    {        
        public bool Export(IEnumerable<TransactionEntry> transactionEntries, ExportConfig Config)
        {
            try
            {
                var sb = new StringBuilder();
                var headerRow = new HeaderEntry().BuildHeader(transactionEntries, Config);
                var footerRow = $"X{Config.IdentificationNumber}";

                using (var sw = new StreamWriter(Config.FilePath))
                {
                    sw.WriteLine(headerRow);
                    foreach (var entry in transactionEntries)                                         
                        sw.WriteLine(entry.Build());
                    sw.WriteLine(footerRow);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PCN874Manager.Export] {ex.Message}");
                return false;
            }
        }

        public bool Export(string csvFilePath, ExportConfig Config)
        {
            var transactionEntries = new List<TransactionEntry>();

            try
            {
                using (var filestream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var file = new StreamReader(filestream, Encoding.UTF8, true, 128))
                    {
                        string line = "";

                        while ((line = file.ReadLine()) != null)
                        {
                            TransactionEntry entry = null;

                            var lineValues = line.Split(',').ToList();
                            var enumValue = (eTransactionType)Enum.Parse(typeof(eTransactionType), lineValues[0]);

                            switch (enumValue)
                            {
                                case eTransactionType.L:
                                    entry = new L_SalesUnidentifiedTrn();
                                    break;
                                case eTransactionType.S:
                                    entry = new S_RegularSalesTrn();
                                    break;
                                case eTransactionType.T:
                                    entry = new T_InputRegularTrn();
                                    break;
                                default:
                                    break;
                            }

                            entry.IdentificationNumber = lineValues[1].PadLeft(9, '0');
                            entry.InvoiceDate = lineValues[2];
                            entry.ReferenceGroup = lineValues[3];
                            entry.InvoiceReferenceNumber = lineValues[4].PadLeft(9, '0');
                            entry.TotalVatAmount = lineValues[5].PadLeft(9, '0');
                            entry.CreditSymbol = lineValues[6];
                            entry.TotalInvoiceAmount = lineValues[7].PadLeft(10, '0');
                            entry.FutureData = "".PadLeft(9, '0');

                            transactionEntries.Add(entry);
                        }
                    }

                    return this.Export(transactionEntries, Config);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in ExportPCN874FromCsv {e}");
                return false;
            }
        }
    }
}
