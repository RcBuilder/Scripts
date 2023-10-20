using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCNUtils.enums;

namespace PCNUtils
{
    public class HeaderEntry
    {
        public HeaderEntry(string identificationNumber)
        {
            this.IdentificationNumber = identificationNumber;
        }

        public string EntryType => "O";
        public string IdentificationNumber { get;}      // 9 digits
        string MonthReportDate { get; set; }         // 6 digits   , Yyyymm form

        public string ReportType => "1";

        string FileGenerationDate { get; set; }         // 8 digits ,Yyyymmdd form

        private string SymbolTotalTaxableSales { get; set; } = "+";          // 1 digits +/-

        string TotalAmountTaxableSales { get; set; }           // 11 digits

        private string SymbolTotalVatOnTaxableSales { get; set; } = "+";          // 1 digits +/-

        string TotalVatTaxableSales { get; set; }           // 9 digits

        private string SymbolDifferentRateTaxableSales { get; set; } = "+";         // 1 digits +/-  , Currently "+"

        string TotalSalesTaxableDifferentRate { get; set; } = "00000000000";           // 11 zeros / Currently zeros – for future use

        private string SymbolDifferentRateTaxableSalesVAT { get; set; } = "+";      // 1 digits +/-  , Currently "+"


        private string TotalVATonSalesTaxableDifferentRate { get; set; } = "000000000";     // 9 digits  , Currently zeros – for future use

        string TotalNumberOfRecordsForSales { get; set; }            // 9 digits , Number of sales records - both taxable and zero-rated/ exempt

        private string SymbolTotalOfZeroValueAndExemptSales { get; set; } = "+";      // 1 digits +/-  

        string TotalZeroValueExemptSales { get; set; }           // 11 digits

        private string SymbolTotalVATonOtherInputs { get; set; } = "+";      // 1 digits +/-  

        string TotalVATonOtherInputs { get; set; }              // 9 digits +/- 

        private string SymbolTotalVATOnEquipment { get; set; } = "+";       // 1 digits +/-  

        private string TotalVATonEquipment { get; set; } = "000000000";              // 9 digits +/- , zeros 
        string TotalNumberOfRecordsForInputs { get; set; }       // 9 digits +/-     , number of transactions of T type

        string SymbolTotalVATToPayReceive { get; set; }       // 1 digits +/-   ( can be + or -   , depend on total VAT of S   -   Total VAT of T)
                                                              // 
        string TotalVATtoToPayReceive { get; set; }           // 11 digits ( total VAT of S   -   Total VAT of T )


        public string buildHeaderRow(List<ITransactionEntry> transactionsData,
                                        int yearReportPeriod,int monthReportPeriod)
        {
            StringBuilder headerRow = new StringBuilder();

            headerRow.Append(EntryType);
            headerRow.Append(this.IdentificationNumber);

            string month = monthReportPeriod.ToString().PadLeft(2, '0');
            string monthPeriod = $"{yearReportPeriod.ToString()}{month}";
            this.MonthReportDate = monthPeriod;
            headerRow.Append(monthPeriod);
            headerRow.Append(this.ReportType);
            this.FileGenerationDate = DateTime.Now.ToString("yyyyMMdd");
            headerRow.Append(this.FileGenerationDate);

            headerRow.Append(this.SymbolTotalTaxableSales);

            this.TotalAmountTaxableSales = calcTotalAmountTaxableSales(transactionsData);
            headerRow.Append(this.TotalAmountTaxableSales);

            headerRow.Append(SymbolTotalVatOnTaxableSales);

            this.TotalVatTaxableSales = calcTotalAmountVATSales(transactionsData);
            headerRow.Append(this.TotalVatTaxableSales);

            headerRow.Append(this.SymbolDifferentRateTaxableSales);

            headerRow.Append(this.TotalSalesTaxableDifferentRate);

            headerRow.Append(SymbolDifferentRateTaxableSalesVAT);

            headerRow.Append(this.TotalVATonSalesTaxableDifferentRate);

            this.TotalNumberOfRecordsForSales = calcTotalNumberOfSales(transactionsData);

            headerRow.Append(TotalNumberOfRecordsForSales);

            headerRow.Append(SymbolTotalOfZeroValueAndExemptSales);


            this.TotalZeroValueExemptSales = calcTotalZeroValueExemptSales(transactionsData);

            headerRow.Append(this.TotalZeroValueExemptSales);

            headerRow.Append(SymbolTotalVATonOtherInputs);


            this.TotalVATonOtherInputs = calcTotalVATonOtherInputs(transactionsData);
            headerRow.Append(this.TotalVATonOtherInputs);


            headerRow.Append(SymbolTotalVATOnEquipment);

            headerRow.Append(this.TotalVATonEquipment);

            this.TotalNumberOfRecordsForInputs = calcTotalRecordsInputs(transactionsData);
            headerRow.Append(TotalNumberOfRecordsForInputs);



            long diffTotalVATtoToPayReceive = long.Parse(this.TotalVatTaxableSales) - long.Parse(this.TotalVATonOtherInputs);


            if (diffTotalVATtoToPayReceive <= 0)
            {
                this.SymbolTotalVATToPayReceive = "-";
                headerRow.Append(this.SymbolTotalVATToPayReceive);

                // cancel the - sign from the number
                diffTotalVATtoToPayReceive = diffTotalVATtoToPayReceive * -1;
            }
            else
            {
                this.SymbolTotalVATToPayReceive = "+";
                headerRow.Append(this.SymbolTotalVATToPayReceive);
            }

            this.TotalVATtoToPayReceive = diffTotalVATtoToPayReceive.ToString().PadLeft(11, '0');

            headerRow.Append(this.TotalVATtoToPayReceive.ToString().PadLeft(11, '0'));
            return headerRow.ToString();
        }



        private string calcTotalRecordsInputs(List<ITransactionEntry> transactionsData)
        {
            int numofRows = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.T)
                    {
                        numofRows += 1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalRecordsInputs : {e}");
            }

            return numofRows.ToString().PadLeft(9, '0');
        }



        private string calcTotalVATonOtherInputs(List<ITransactionEntry> transactionsData)
        {
            int sumTotal = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.T)
                    {
                        int totalVatAmount = Int32.Parse(transEntry.TotalVatAmount);
                        sumTotal += totalVatAmount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalVATonOtherInputs : {e}");
            }

            return sumTotal.ToString().PadLeft(9, '0');
        }


        private string calcTotalZeroValueExemptSales(List<ITransactionEntry> transactionsData)
        {
            int sumTotal = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.L)
                    {
                        int totalInvoiceAmount = Int32.Parse(transEntry.TotalInvoiceAmount);
                        sumTotal += totalInvoiceAmount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalZeroValueExemptSales : {e}");
            }

            return sumTotal.ToString().PadLeft(11, '0');
        }


        private string calcTotalNumberOfSales(List<ITransactionEntry> transactionsData)
        {
            int numofRows = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.S || 
                        transEntry.TransactionType == eTransactionType.L)
                    {
                        numofRows += 1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalNumberOfSales : {e}");
            }

            return numofRows.ToString().PadLeft(9, '0');
        }



        private string calcTotalAmountTaxableSales(List<ITransactionEntry> transactionsData)
        {
            int sumTotal = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.S)
                    {
                        int totalInvoiceAmount = Int32.Parse(transEntry.TotalInvoiceAmount);
                        sumTotal += totalInvoiceAmount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalAmountTaxableSales : {e}");
            }

            return sumTotal.ToString().PadLeft(11, '0');
        }

        private string calcTotalAmountVATSales(List<ITransactionEntry> transactionsData)
        {
            int sumTotal = 0;

            try
            {
                foreach (ITransactionEntry transEntry in transactionsData)
                {
                    if (transEntry.TransactionType == eTransactionType.S)
                    {
                        int totalVATAmount = Int32.Parse(transEntry.TotalVatAmount);
                        sumTotal += totalVATAmount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred in calcTotalAmountVATSales : {e}");
            }

            return sumTotal.ToString().PadLeft(9, '0');
        }
    }
}
