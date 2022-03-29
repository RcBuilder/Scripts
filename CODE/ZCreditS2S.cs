using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PaymentsService
{
    /*        
        Sources:
        https://zcreditws.docs.apiary.io/#reference/0/make-transaction

        Projects:
        see 'Creative\PaymentsService'

        Using:
        var zCreditS2SResponse = new ZCreditS2S(Config.ChargeTerminal, Config.ChargePassword).MakeTransaction((ZCreditMakeTransactionData)Details);
        return (PaymentResult)zCreditS2SResponse;

        Entites:
        public class PaymentResult
        {        
            public string Provider { get; set; }
            public string Code { get; set; }
            public string Message { get; set; }

            public string Token { get; set; }
            public int ReferenceNumber { get; set; }
            public string VoucherNumber { get; set; }
            public string ApprovalNumber { get; set; }
              
            public static explicit operator PaymentResult(ZCreditMakeTransactionResponse other)
            {
                return new PaymentResult
                {
                    Provider = PaymentProviders.ZCreditS2S,
                    Code = other.ReturnCode.ToString(),
                    Message = other.ReturnCode == 0 ? "OK" : other.ReturnMessage,
                    Token = other.Token,
                    ReferenceNumber = other.ReferenceNumber,
                    VoucherNumber = other.VoucherNumber,
                    ApprovalNumber = other.ApprovalNumber
                };
            }
        }

        public class PaymentDetails
        {        
            public string CVV { get; set; }
            public string CardNumber { get; set; }
            public string CardExpiry { get; set; } // MMYY            
            public string UserId { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string UserPhone { get; set; }
            public string UserFax { get; set; }
            public string UserAddress { get; set; }        
            public string UserTZ { get; set; }
            public string Comments { get; set; }
            public float Price { get; set; }
            public string Description { get; set; }
            public string PinPad { get; set; }
            public int NumberOfPayments { get; set; } = 1;

            // depends on the provider!
            public byte TransactionType { get; set; }  // Regular, Refund 
            public byte CurrencyType { get; set; }  // ILS, USD, EUR etc.
            public byte CreditType { get; set; }  // Regular, Plus30, Immediate, Credit, Installments
            
            public static explicit operator ZCreditMakeTransactionData(PaymentDetails other)
            {
                float firstPayment = 0.0F, eachPayment = 0.0F;
                var creditType = (eZCreditCreditType)other.CreditType;
                if (creditType == eZCreditCreditType.Installments)
                    firstPayment = eachPayment = other.Price / other.NumberOfPayments;
            
                return new ZCreditMakeTransactionData
                {
                    ExpDate_MMYY = other.CardExpiry,
                    CardNumber = other.CardNumber,
                    ExtraData = other.Comments,
                    CVV = other.CVV,
                    TransactionSum = other.Price,
                    CustomerEmail = other.UserEmail,                
                    CustomerName = other.UserFullName,                
                    PhoneNumber = other.UserPhone,
                    HolderID = other.UserTZ,
                    CustomerAddress = other.UserAddress,
                    ItemDescription = other.Description,                
                    AuthNum = "",
                    Track2 = other.PinPad ?? "",
                    TransactionType = (eZCreditTransactionType)other.TransactionType,
                    CurrencyType = (eZCreditCurrency)other.CurrencyType,
                    CreditType = creditType,
                    NumberOfPayments = other.NumberOfPayments,
                    FirstPaymentSum = firstPayment,
                    OtherPaymentsSum = eachPayment
                };
            }
        }    
    */

    #region Entities
    public class ZCreditMakeTransactionData
    {        
        public string Track2 { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpDate_MMYY { get; set; }
        public float TransactionSum { get; set; }
        public int NumberOfPayments { get; set; } = 1;
        public float FirstPaymentSum { get; set; }
        public float OtherPaymentsSum { get; set; }
        public eZCreditTransactionType TransactionType { get; set; } = eZCreditTransactionType.Regular;
        public eZCreditCurrency CurrencyType { get; set; } = eZCreditCurrency.ILS;
        public eZCreditCreditType CreditType { get; set; } = eZCreditCreditType.Regular;
        public eZCreditBillingType J { get; set; } = eZCreditBillingType.Regular;
        public bool IsCustomerPresent { get; set; } = false;
        public string AuthNum { get; set; }
        public string HolderID { get; set; }
        public string ExtraData { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string ItemDescription { get; set; }                
    }

    public class ZCreditMakeTransactionRequest : ZCreditMakeTransactionData
    {
        public string TerminalNumber { get; set; }
        public string Password { get; set; }
        public eZCreditObeligoAction ObeligoAction { get; set; } = eZCreditObeligoAction.Regular;
        public int OriginalZCreditReferenceNumber { get; set; }
        public string TransactionUniqueIdForQuery { get; set; } = "";
        public string TransactionUniqueID { get; set; } = "";
        public bool UseAdvancedDuplicatesCheck { get; set; } = false;
        public bool AllowFullCardNumberInResponse { get; set; } = true;
        public bool ShowFullCardNumberInPrint { get; set; } = false;
        public ZCreditInvoiceReceipt InvoiceReceipt { get; set; }

        public ZCreditMakeTransactionRequest() { }
        public ZCreditMakeTransactionRequest(ZCreditMakeTransactionData TransactionData) {
            this.Track2 = TransactionData.Track2;
            this.CardNumber = TransactionData.CardNumber;
            this.CVV = TransactionData.CVV;
            this.ExpDate_MMYY = TransactionData.ExpDate_MMYY;
            this.TransactionSum = TransactionData.TransactionSum;
            this.NumberOfPayments = TransactionData.NumberOfPayments;
            this.FirstPaymentSum = TransactionData.FirstPaymentSum;
            this.OtherPaymentsSum = TransactionData.OtherPaymentsSum;
            this.TransactionType = TransactionData.TransactionType;
            this.CurrencyType = TransactionData.CurrencyType;
            this.CreditType = TransactionData.CreditType;
            this.J = TransactionData.J;
            this.IsCustomerPresent = TransactionData.IsCustomerPresent;
            this.AuthNum = TransactionData.AuthNum;
            this.HolderID = TransactionData.HolderID;
            this.ExtraData = TransactionData.ExtraData;
            this.CustomerName = TransactionData.CustomerName;
            this.CustomerAddress = TransactionData.CustomerAddress;
            this.CustomerEmail = TransactionData.CustomerEmail;
            this.PhoneNumber = TransactionData.PhoneNumber;
            this.ItemDescription = TransactionData.ItemDescription;            
        }
    }

    public class ZCreditInvoiceReceipt {
        public eZCreditInvoiceReceiptType Type { get; set; } = eZCreditInvoiceReceiptType.Invoice;
        public float TaxRate { get; set; }
        public string RecepientName { get; set; }
        public string RecepientCompanyID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNum { get; set; }
        public string FaxNum { get; set; }
        public string Comment { get; set; }
        public string ReceipientEmail { get; set; }
        public bool EmailDocumentToReceipient { get; set; } = true;
        public bool ReturnDocumentInResponse { get; set; } = false;
        public IEnumerable<ZCreditInvoiceReceiptItem> Items { get; set; } = new List<ZCreditInvoiceReceiptItem>();
    }

    public class ZCreditInvoiceReceiptItem {
        public string ItemDescription { get; set; }
        public int ItemQuantity { get; set; } = 1;
        public float ItemPrice { get; set; }
        public bool IsTaxFree { get; set; } = false;
    }

    public class ZCreditMakeTransactionResponse {
        public bool HasError { get; set; }     
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }               
        public int ReferenceNumber { get; set; }
        public string VoucherNumber { get; set; }
        public string ApprovalNumber { get; set; }        
        public string Token { get; set; }
        public string ClientReciept{ get; set; }
        public string SellerReciept { get; set; }
    }

    public enum eZCreditTransactionType { Regular = 1, Refund = 53 }
    public enum eZCreditCurrency { ILS = 1, USD = 2, EUR = 3 }
    public enum eZCreditCreditType { Regular = 1, Plus30 = 2, Immediate = 3, Credit = 6, Installments = 8 }
    public enum eZCreditBillingType { Regular = 0, NoCharge = 2, FutureCharge = 5 }
    public enum eZCreditObeligoAction { Regular, Capture, Release }
    public enum eZCreditInvoiceReceiptType { Auto, Invoice, Receipt }
    #endregion

    public class ZCreditS2S
    {
        private static readonly string DUMP_FOLDER = $"{AppContext.BaseDirectory}bin\\Logs\\";
        private const string SERVER = "https://pci.zcredit.co.il/ZCreditWS/api";
        private static readonly string MAKE_TRANSACTION_ENDPOINT = $"{SERVER}/Transaction/CommitFullTransaction/";
        
        protected string Terminal { get; set; }          
        protected string Password { get; set; }
        
        public ZCreditS2S(string Terminal, string Password) {                        
            this.Terminal = Terminal;
            this.Password = Password;           
        }

        #region MakeTransaction        
        /*
            POST {{ServerURL}}/Transaction/CommitFullTransaction
            H Content-Type: application/json
            {
              "TerminalNumber": "{{Terminal}}",
              "Password": "{{Password}}",
              "Track2": "",
              "CardNumber": "4580000000000000",
              "CVV": "123",
              "ExpDate_MMYY": "0129",
              "TransactionSum": "1.00",
              "NumberOfPayments": "1",
              "FirstPaymentSum": "0",
              "OtherPaymentsSum": "0",
              "TransactionType": "01",
              "CurrencyType": "1",
              "CreditType": "1",
              "J": "0",
              "IsCustomerPresent": "false",
              "AuthNum": "",
              "HolderID": "123456789",
              "ExtraData": "ref100",
              "CustomerName": "Roby",
              "CustomerAddress": "Tel Aviv",
              "CustomerEmail": "roby@rcb.co.il",
              "PhoneNumber": "",
              "ItemDescription": "Some Item",
              "ObeligoAction": "",
              "OriginalZCreditReferenceNumber": "",
              "TransactionUniqueIdForQuery": "",
              "TransactionUniqueID": "",
              "UseAdvancedDuplicatesCheck": "false",
              "AllowFullCardNumberInResponse": "true",
              "ShowFullCardNumberInPrint": "false"
            }
        */
        public ZCreditMakeTransactionResponse MakeTransaction(ZCreditMakeTransactionData TransactionData) 
        {
            try
            {                
                using (var client = new WebClient())
                {
                    client.Proxy = null;
                    client.Encoding = Encoding.UTF8;

                    client.Headers.Clear();
                    client.Headers.Add("Accept", "*/*");                    
                    client.Headers.Add("Content-Type", "application/json");

                    var request = new ZCreditMakeTransactionRequest(TransactionData);
                    request.TerminalNumber = this.Terminal;
                    request.Password = this.Password;

                    var payload = JsonConvert.SerializeObject(request);
                    this.WriteLog("ZCreditS2S_Payloads.txt", payload);
                    
                    var response = client.UploadString(MAKE_TRANSACTION_ENDPOINT, payload);
                    this.WriteLog("ZCreditS2S_Responses.txt", response);

                    return JsonConvert.DeserializeObject<ZCreditMakeTransactionResponse>(response);
                }
            }
            catch (Exception Ex) {
                return new ZCreditMakeTransactionResponse { };
            }
        }
        #endregion

        // --

        private void WriteLog(string FileName, string Content) {
            try
            {
                if (!Directory.Exists(DUMP_FOLDER)) Directory.CreateDirectory(DUMP_FOLDER);
                File.AppendAllText($"{DUMP_FOLDER}{FileName}", $"{Content}\n");
            }
            catch { }
        }
    }
}
