
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace WizColudEntities
{
    public class WizColudConfig
    {
        public string ServerName { get; set; }
        public string ApiToken { get; set; }  // used to generate an access-token (createSession method) 
        public string DBName { get; set; }
        public string AccessToken { get; set; }  // Bearer token - used as an Authorization header
    }

    public class TokensUpdatedEventArgs : EventArgs
    {        
        public string AccessToken { get; set; }

        public TokensUpdatedEventArgs(string AccessToken) {          
            this.AccessToken = AccessToken;
        }
    }

    /*
        {
            "repdata": [
                {
                    "Company_File_Name": "wizdb493n4",
                    "Company_Name": "חברה לבדיקות api",
                    "Comp_Vatnum": "514680909"
                }
            ]
        } 
    */
    public class CompanyList {
        [JsonProperty(PropertyName = "repdata")]
        public IEnumerable<Company> Companies { get; set; }
    }

    /*
        {
            "status": "error",
            "statusCode": 500,
            "message": "Token is not valid"
        }
    */
    public class ResponseStatus
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = "ok";

        [JsonProperty(PropertyName = "statusCode")]
        public HttpStatusCode HttpStatus { get; set; } = HttpStatusCode.OK;

        [JsonProperty(PropertyName = "message")]
        public string Error { get; set; }

        public bool Success {
            get { return this.Status.ToLower() == "ok"; }
        }
    }

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
        [JsonProperty(PropertyName = "שם חשבון")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "מפתח")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "עיר")]
        public string City { get; set; }        
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

    public abstract class SaveIndexRequest<T> where T: IIndex
    {
        [JsonProperty(PropertyName = "myindex")]
        public abstract string IndexType { get; protected set; }

        [JsonProperty(PropertyName = "insertnew")]
        public abstract string InsertNew { get; set; }

        [JsonProperty(PropertyName = "rows")]
        public IEnumerable<T> Rows { get; set; }
    }

    public class SaveAccountRequest : SaveIndexRequest<Account>
    {        
        public override string IndexType { get; protected set; } = "acc";

        [JsonProperty(PropertyName = "insertnew")]
        public override string InsertNew { get; set; } = "true";

        public SaveAccountRequest(Account Account) {
            this.Rows = new List<Account> { Account };
        }
    }

    public class SaveItemRequest : SaveIndexRequest<Item>
    {
        public override string IndexType { get; protected set; } = "itm";

        [JsonProperty(PropertyName = "insertnew")]
        public override string InsertNew { get; set; } = "true";

        public SaveItemRequest(Item Item)
        {
            this.Rows = new List<Item> { Item };
        }
    }

    public interface IIndex { }

    // https://docs.wizcloud.co.il/docs/indexes/#items-data
    public class Item : IIndex { }

    // https://docs.wizcloud.co.il/docs/indexes/#accounts-data
    public class Account : IIndex
    {
        [Required]
        public string AccountKey { get; set; } 
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "SortGroup")]
        public int Sort { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public eBalanceCode BalanceCode { get; set; } = eBalanceCode.Cash;  // קוד מאזן             

        [JsonProperty(PropertyName = "TFtalDiscount")]
        public float Discount { get; set; }
        public byte VatExampt { get; set; }
        
        [JsonProperty(PropertyName = "WorF")]
        public string Occupation { get; set; }

        [JsonProperty(PropertyName = "Details")]
        public string Remarks { get; set; }
        
        [JsonProperty(PropertyName = "MaxCredit")]
        public float CreditLimit { get; set; }

        [JsonProperty(PropertyName = "MaxObligo")]
        public float ObligoLimit { get; set; }

        public string CustomerNote { get; set; }

        [JsonProperty(PropertyName = "Agent")]
        public int AgentId { get; set; }

        [JsonProperty(PropertyName = "DeductionPrc")]
        public float DeductionPercentage { get; set; }
        
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string BankAccount { get; set; }

        [JsonProperty(PropertyName = "TaxFileNum")]
        public string VATRegistrationNumber { get; set; }  // מספר עוסק מורשה	

        [JsonProperty(PropertyName = "MainAccount")]
        public eAccountType AccountType { get; set; } = eAccountType.None;

        public float FixedOrderCost { get; set; }
        public float AverageSupplyPeriod { get; set; }

        public string CostCode { get; set; } = "";        
        public string Email { get; set; }

        [JsonProperty(PropertyName = "DeductFile")]
        public string IRSRegistrationNumber { get; set; } // מספר תיק מס הכנסה	        
        public string WebSite { get; set; }

        [JsonProperty(PropertyName = "DeductType")]
        public eDeductionType DeductionType { get; set; } = eDeductionType.Services;     
    }

    public class Company
    {
        [JsonProperty(PropertyName = "Comp_Vatnum")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Company_File_Name")]
        public string DBName { get; set; }

        [JsonProperty(PropertyName = "Company_Name")]
        public string Name { get; set; }
    }

    public class SaveJournalEntryRequest
    {
        [JsonProperty(PropertyName = "insertolastb")]
        public string AppendToLastBatch { get; protected set; } = "false";  // NEW = false 

        [JsonProperty(PropertyName = "batchNo")]
        public int BatchNo { get; set; }

        [JsonProperty(PropertyName = "rows")]
        public IEnumerable<JournalEntry> Rows { get; set; }

        public SaveJournalEntryRequest(JournalEntry JournalEntry) {
            this.Rows = new List<JournalEntry> { JournalEntry };
        }
    }

    public class JournalEntry
    {
        [Required]
        [JsonProperty(PropertyName = "TransDebID")]
        public string DebitAccountKey { get; set; }

        [Required]
        [JsonProperty(PropertyName = "TransCredID")]
        public string CreditAccountKey { get; set; }

        [JsonProperty(PropertyName = "DebName")]
        public string DebitAccountName { get; set; }

        [JsonProperty(PropertyName = "CredName")]
        public string CreditAccountName { get; set; }

        public string Description { get; set; }
        public int Referance { get; set; }

        [JsonProperty(PropertyName = "Ref2")]
        public int Referance2 { get; set; }

        public string TransType { get; set; }   // קוד סוג תנועה        
        public string ValueDate { get; set; }   // dd/mm/yyyy
        public string DueDate { get; set; }     // dd/mm/yyyy        

        [Required]
        [JsonProperty(PropertyName = "SuF")]
        public float TotalNIS { get; set; }

        [JsonProperty(PropertyName = "SuFDlr")]
        public float TotalOther { get; set; }

        [JsonProperty(PropertyName = "Quant")]
        public float Quantity { get; set; }

        [JsonProperty(PropertyName = "Branch")]
        public int BranchNo { get; set; }

        [JsonProperty(PropertyName = "Details")]
        public string Remarks { get; set; }

        [JsonProperty(PropertyName = "Osek874")]        
        public string VATRegistrationNumber { get; set; }  // מספר עוסק מורשה	
    }
}
