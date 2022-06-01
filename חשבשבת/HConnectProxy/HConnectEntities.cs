
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace HConnectEntities
{
    public class HConnectConfig
    {
        public string Station { get; set; }
        public string Company { get; set; } 
        public string Provider { get; set; }
        public string Token { get; set; }
    }

    // https://api.h-erp.co.il/docs/setupcust#3-%D7%9E%D7%96%D7%94%D7%94-%D7%A1%D7%95%D7%92-%D7%94%D7%9E%D7%9E%D7%A9%D7%A7---plugin
    public class Plugins {
        public const string IMOVEIN = "imovein"; // documents
        public const string MOVEIN = "movein";   // journal entries
        public const string HESHIN = "heshin";   // accounts
        public const string KUPAIN = "kupain";   // receipts
        public const string ITEMIN = "itemin";   // items  
    }

    /*
        {
            "station": "xxxxxxxxxxxxxxxxxxxxx",
            "plugin": "movein",
            "company": "demo",
            "message": {
                "netPassportID": "250030",
                "pluginData": [
                    {                         
                        "Reference":"9000919",
                        "transtype":"הוצ",
                        "accountkeydeb1":"30001",
                        "accountkeycred1":"30001",
                        "sufdeb1":"99.00",
                        "sufcred1":"99.00" 
                    }
                ]
            },
            "signature": "xxxxxxxxxxxxxxxxxxxx"
        } 
    */
    public class APIRequest<T>
    {
        [JsonProperty(PropertyName = "station")]
        public string Station { get; set; }

        [JsonProperty(PropertyName = "plugin")]
        public string Plugin { get; set; }

        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "message")]
        public APIRequestMessage<T> Message { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }
    }

    public class APIRequestMessage<T>
    { 
        [JsonProperty(PropertyName = "netPassportID")]
        public string Provider { get; set; }

        [JsonProperty(PropertyName = "pluginData")]
        public IEnumerable<T> Data { get; set; }
    }

    // https://api.h-erp.co.il/docs/imovein#%D7%A1%D7%95%D7%92%D7%99-%D7%94%D7%9E%D7%A1%D7%9E%D7%9B%D7%99%D7%9D-%D7%94%D7%A0%D7%99%D7%AA%D7%A0%D7%99%D7%9D-%D7%9C%D7%A7%D7%9C%D7%99%D7%98%D7%94-%D7%9C%D7%97%D7%A9%D7%91%D7%A9%D7%91%D7%AA--documentid
    public enum eDocumentType
    {
        None = 0,
        Invoice = 1,
        InvoiceReceipt = 2,
        InvoiceCredit = 3,
        DeliveryNote = 4,
        Return = 5,
        Order = 6,
        PriceOffer = 7,        
        ProformaInvoice = 78        
    }

    public enum eBalanceCode
    {
        Cash = 1,
        CheckToCollect = 2,
        ShotTermCredit = 3,
        TradingSecurities = 4,
        CheckToPay = 5,
        Customer = 6,
        Supplier = 7,
        StockOwner = 9,
        Inventory = 10,
        Expense = 14,
        Investment = 24,
        Revenue = 37,
        Export = 38,
        Import = 39,
        Shopping = 45,
        Contractor = 49,
        Salary = 58,
        LostDebt = 62,
        Taxes = 74
    }

    // https://api.h-erp.co.il/docs/movein
    public class JournalEntry
    {
        [Required]
        [JsonProperty(PropertyName = "AccountKeyDeb1")]
        public string DebitAccountKey { get; set; }

        [Required]
        [JsonProperty(PropertyName = "AccountKeyCred1")]
        public string CreditAccountKey { get; set; }

        public string Description { get; set; }
        public string Referance { get; set; }

        [JsonProperty(PropertyName = "Ref2")]
        public string Referance2 { get; set; }

        [Required]
        public string TransType { get; set; }   // קוד סוג תנועה        
        public string ValueDate { get; set; }   // dd/mm/yyyy
        public string DueDate { get; set; }     // dd/mm/yyyy        

        [Required]
        [JsonProperty(PropertyName = "SuFDeb1")] 
        public string TotalNIS { get; set; }  // סכום חובה

        [JsonProperty(PropertyName = "SuFCred1")]
        public string TotalNISCredit { get; set; }  // סכום זכות 

        [JsonProperty(PropertyName = "Quant")]
        public string Quantity { get; set; }

        [JsonProperty(PropertyName = "Branch")]
        public string BranchNo { get; set; }

        [JsonProperty(PropertyName = "Details")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "Osek874")]
        public string VATRegistrationNumber { get; set; }  // מספר עוסק מורשה	
    }

    // https://api.h-erp.co.il/docs/heshin
    public class Account
    {
        [Required]
        public string AccountKey { get; set; }
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "SortGroup")]
        public string Sort { get; set; }

        public string Address { get; set; }
        public string City { get; set; }

        [JsonProperty(PropertyName = "zipCode")]
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public eBalanceCode BalanceCode { get; set; } = eBalanceCode.Cash;  // קוד מאזן             

        [JsonProperty(PropertyName = "TFtalDiscount")]
        public string Discount { get; set; }
        public string VatExampt { get; set; }
        
        [JsonProperty(PropertyName = "Details")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "MaxCredit")]
        public string CreditLimit { get; set; }

        [JsonProperty(PropertyName = "MaxObligo")]
        public string ObligoLimit { get; set; }

        public string CustomerNote { get; set; }

        [JsonProperty(PropertyName = "Agent")]
        public string AgentId { get; set; }

        [JsonProperty(PropertyName = "DeductionPrc")]
        public string DeductionPercentage { get; set; }

        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string BankAccount { get; set; }

        [JsonProperty(PropertyName = "TaxFileNum")]
        public string VATRegistrationNumber { get; set; }  // מספר עוסק מורשה	

        [JsonProperty(PropertyName = "MainAccount")]
        public eAccountType AccountType { get; set; } = eAccountType.None;

        public string FixedOrderCost { get; set; }
        public string AverageSupplyPeriod { get; set; }

        public string CostCode { get; set; } = "";
        public string Email { get; set; }

        [JsonProperty(PropertyName = "DeductFile")]
        public string IRSRegistrationNumber { get; set; } // מספר תיק מס הכנסה	        
        public string WebSite { get; set; }
    }

    // https://api.h-erp.co.il/docs/imovein
    public class Document
    {        
        [Required]
        [JsonProperty(PropertyName = "DocumentID")]
        public eDocumentType DocumentType { get; set; } = eDocumentType.None;

        [Required]
        [JsonProperty(PropertyName = "Reference")]
        public string DocumentNumber { get; set; }

        [Required]
        public string AccountKey { get; set; }
        public string AccountName { get; set; }

        [JsonProperty(PropertyName = "Address1")]
        public string Address { get; set; }        
        public string Phone { get; set; }
        public string ValueDate { get; set; }   // dd-mm-yyyy
        public string Details { get; set; }

        [Required]
        public string ItemKey { get; set; }
        public string Quantity { get; set; }        
        public string Remarks { get; set; }        
        public string Price { get; set; }

        [JsonProperty(PropertyName = "DiscountPrc")]
        public string DiscountPercentage { get; set; }

        [JsonProperty(PropertyName = "Warehouse")]
        public string StoreId { get; set; }

        [JsonProperty(PropertyName = "Osek874")]
        public string VATRegistrationNumber { get; set; }  // מספר עוסק מורשה	

        /*
        public float Total
        {
            get { return this.Quantity * this.Price; }
        }
        */
    }

    // https://api.h-erp.co.il/docs/itemin
    public class Item {
        [Required]
        public string ItemKey { get; set; }
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "SortGroup")]
        public string Sort { get; set; }
        public string BarCode { get; set; }

        [Required]        
        public string Price { get; set; }

        [JsonProperty(PropertyName = "DiscountPrc")]
        public string DiscountPercentage { get; set; }
        public char VatExampt { get; set; } = 'ל';  // כ = פטור, ל = לא פטור

        [JsonProperty(PropertyName = "StockLevel")]
        public string Stock { get; set; }

        ///public string Currency { get; set; }
    }

    public class ResponseStatus
    {
        [JsonProperty(PropertyName = "apiRes")]
        public ResponseStatusBody Body { get; set; }

        public bool Success
        {
            get { return (this.Body?.Status?.ToLower() ?? "") == "ok"; }
        }
    }

    public class ResponseStatusBody
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = "error";

        [JsonProperty(PropertyName = "err")]
        public string Error { get; set; }        
        public string Message { get; set; }
    }













    // ---------------------------



    public interface IExportDataResult<T> {         
        IEnumerable<T> Data { get; set; }
    }

    public class ExportDataResult<T> : IExportDataResult<T>
    {
        [JsonProperty(PropertyName = "repdata")]
        public IEnumerable<T> Data { get; set; }
    }

    public class ExportAccount
    {
        [JsonProperty(PropertyName = "קוד סוג תנועה")]
        public string MonetaryCodeId { get; set; }

        [JsonProperty(PropertyName = "שם סוג התנועה")]
        public string MonetaryCodeName { get; set; }

        [JsonProperty(PropertyName = "קוד מיון")]
        public string SortCodeId { get; set; }

        [JsonProperty(PropertyName = "שם קוד מיון")]
        public string SortCodeName { get; set; }

        [JsonProperty(PropertyName = "שם חשבון")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "מפתח חשבון")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "עיר")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "כתובת")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "מיקוד")]
        public string ZipCode { get; set; }

        [JsonProperty(PropertyName = "טלפון")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "דואר אלקטרוני")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "חשבון ראשי")]
        public string MainAccount { get; set; }

        [JsonProperty(PropertyName = "מספר תיק מס הכנסה")]
        public string TaxDocId { get; set; }
    }

    // https://docs.wizcloud.co.il/docs/reportdata/
    public class ExportDataRequest {         
        [JsonProperty(PropertyName = "datafile")]
        public string DataFile { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public IEnumerable<ExportDataParameter> Parameters { get; set; } = Enumerable.Empty<ExportDataParameter>();

        public ExportDataRequest(string DataFile) : this(DataFile, null) { }
        public ExportDataRequest(string DataFile, ExportDataParameter Parameter) {
            this.DataFile = DataFile;
            if(Parameter != null) this.Parameters = new List<ExportDataParameter> { Parameter };
        }
    }

    // https://docs.wizcloud.co.il/docs/reportdata#json-example-with-client-changes
    public class ExportDataParameter
    {
        [JsonProperty(PropertyName = "p_name")]
        public string Name { get; set; } = "__MUSTACH_P0__";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "0";

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "long";

        [JsonProperty(PropertyName = "opName")]
        public string OperationName { get; set; } = "שווה";

        [JsonProperty(PropertyName = "opOrigin")]
        public string OperationOrigin { get; set; } = "from";

        [JsonProperty(PropertyName = "name")]
        public string FieldName { get; set; }

        [JsonProperty(PropertyName = "defVal")]
        public string FieldValue { get; set; }
    }

    public enum eAccountType
    {
        Customer = 1,
        Supplier = 2,
        Expense = 3,
        Bank = 4,
        Fund = 5,
        IncomeWithVAT = 7,
        IncomeWithoutVAT = 8,
        Interest = 9,
        VAT = 12,
        IncomeTaxAdvances = 23, // מקדמות למס הכנסה
        None = 26,
        BankInterest = 29,
        BlockedAccount = 30,
        Surpluses = 32  // עודפים
    }

    public enum eDeductionType
    {
        Services = 0,
        InsuranceFee = 2,
        IRSRegulations = 3,
        Interest = 4
    }
}
