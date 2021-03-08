using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WebsiteBLL
{
    /*
        SOURCES:
        http://kb.cardcom.co.il/article/AA-00402/0/
        http://kb.cardcom.co.il/article/AA-00240/0
        http://kb.cardcom.co.il/article/AA-00253/0/
        http://kb.cardcom.co.il/article/AA-00444/0/
        
        IPN:
        - terminalnumber
        - lowprofilecode
        - Operation       
        - DealResponse
        - TokenResponse
        - InvoiceResponseCode
        - OperationResponse
        - OperationResponseText
        - ReturnValue

        GENERATE-IFRAME-SOURCE:
        http://kb.cardcom.co.il/article/AA-00402/0/

        TRANSACTION-DETAILS:
        http://kb.cardcom.co.il/article/AA-00240/0/

        ACTIONS-USING-TOKEN:
        http://kb.cardcom.co.il/article/AA-00253/0/

        RECURRING-AUTO-PAYMENTS:
        http://kb.cardcom.co.il/article/AA-00444/0/
        
        PROCESS:
        (steps)
        1. create a collection of the required parameters (see 'GENERATE-IFRAME-SOURCE')
        2. generate url 

            POST https://secure.cardcom.co.il/Interface/LowProfile.aspx 
            H: ContentType: application/x-www-form-urlencoded
            B: <prms-from-chapter-1>   

        3. check the response 
            param 'ResponseCode' should be 0
        4. extract the iframe generated url 
            param 'url'
        5. set the returned url from the chapter-4 as the iframe 'src' attribute
        -
        6. IPN - parse the response and extract the code ('lowprofilecode' param) and store it (see 'IPN')
        7. DETAILS - get transaction details

           GET https://secure.cardcom.solutions/Interface/BillGoldGetLowProfileIndicator.aspx
           Q: ?terminalnumber=&username=&lowprofilecode=

        8. store the token (and other details) received from the reqeust in chapter-7
        -
        (optional)
        9. create a collection of the required parameters (see 'RECURRING-AUTO-PAYMENTS')
        10.RECURRING PAYMENTS - use the received 'code' from chapter 6 to set a recurring auto-payments (monthly, weekly, yearly and etc.)
           use the TimeIntervalId to schedule the payments mode, these values are custom values defined in cardcom dashboard (e.g: 1 = monthly, 2 = weekly, 3 = yearly)
           we can also set the starting date and the number of times to charge the user
           note! requires a terminal with No cvv. 

           POST https://secure.cardcom.solutions/interface/RecurringPayment.aspx 
            H: ContentType: application/x-www-form-urlencoded
            B: <prms-from-chapter-9> 

        11.parse the reponse and store the 'RecurringId' (in order to cancel the recurring payments process if needed)
        -
        (optional)
        12. create a collection of the required parameters (see 'RECURRING-AUTO-PAYMENTS')
        13.CANCEL-RECURRING PAYMENTS - to cancel recurring payments process use the following endpoint 
           this method requires the 'RecurringId' from chapter 11 and the 'code' from chapter 6
           note! requires a terminal with No cvv. 

           POST https://secure.cardcom.solutions/interface/RecurringPayment.aspx
           H: ContentType: application/x-www-form-urlencoded
           B: <prms-from-chapter-12> 

        PROCESS (Summary):
        1. generate an iframe source 
        2. create a listener for requests comming from cardcom servers after each transaction (aka IPN)
        3. extract both the code + token and store them
        4. use the code to get the transaction details (extended)
        5. store the transaction details.         

        (optional)
        6. use the code to create a recurring-auto-payments process 
        7. use the code + recurringId to cancel a recurring-auto-payments process
        8. use the token to make direct (no iframe) debit/ credit actions using a token

        PAYMENTS REPORT:
        1. go to the dashboard and login 
            https://secure.cardcom.solutions/
            note! can use the sandbox account 
        2. menu 'דוחות סליקת אשראי'
        3. option-7 'דוח עסקאות פרופיל נמוך' 
        4. find the transaction > click on 'מידע למפתח'

        RECURRING PAYMENTS REPORT:
        1. go to the dashboard and login 
            https://secure.cardcom.solutions/
            note! can use the sandbox account 
        2. menu 'הוראת קבע'
        3. option-2 'רשימת לקוחות' 
        4. find the user > click on '...' > 'הוראות לחיוב'

        SANDBOX:
        Terminal: 1000
        UserName: barak9611

        USING:
        var cardcomManager = new CardcomManager(
            "1000", 
            "barak9611", 
            "http://rcbuilder-003-site2.ctempurl.com/Purchase/ProcessCardcomTransaction",
            "http://localhost:5015/Purchase/Thanks",
            "http://localhost:5015/Purchase/Error"
        );
        var response = cardcomManager.GenerateIFrameSource(
            "ORDER 1", 
            new List<CardcomIFrameSourceItem> { 
                new CardcomIFrameSourceItem { 
                    Description = "Item A", 
                    Price = 0.5F,
                    Quantity = 1
                },
                new CardcomIFrameSourceItem {
                    Description = "Item B",
                    Price = 0.5F,
                    Quantity = 1
                }
            },
            "39.90|3|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7",  // PRICE|PACKAGE|COUPON (e.g: 39.90|3|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7)
            InvoiceDetails: new CardcomInvoiceDetails { 
                CustomerName = "Roby Cohen",
                Email = "RcBuilder@walla.com",
                SendEmail = true
            }
        );         
        if (response.StatusCode == 0) // OK                            
            ViewBag.FrameSource = response.URL;
        return View();
        -
        <iframe src="@ViewBag.FrameSource"></iframe>

        ---

        var cardcomManager = new CardcomManager("1000", "barak9611");
        var response = cardcomManager.GetTransactionDetails("cfcc6c69-8134-405d-8c61-f3f9779ac37e");        
        return View(response.Details);
        -
        @using System.Collections.Specialized;
        @model NameValueCollection

        @foreach (string key in Model.Keys) { 
            <div>@key = @Model[key]</div>
        }

        ---

        var cardcomManager = new CardcomManager("1000", "barak9611");
        var responseCharge = cardcomManager.ChargeWithToken(chargeDetails.Token, transaction.Price, chargeDetails.CardExpiry, chargeDetails.CardOwnerId);
        if(Convert.ToInt32(responseCharge.Details["ResponseCode"]) == 0) 
            Console.WriteLine("OK");
    */

    #region Entities
    public class CardcomIFrameSourceResponse {

        public int StatusCode { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string URL { get; set; }
    }

    public class CardcomIFrameSourceItem {
        public float Price { get; set; }
        public float Quantity { get; set; }
        public string Description { get; set; }
    }

    public class CardcomInvoiceDetails {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public bool SendEmail { get; set; }        
    }
    #endregion

    public class CardcomManager
    {
        private const string GENERATE_URL_ENDPOINT = "https://secure.cardcom.co.il/Interface/LowProfile.aspx";
        private const string TRANSACTION_DETAILS_ENDPOINT = "https://secure.cardcom.solutions/Interface/BillGoldGetLowProfileIndicator.aspx";
        private const string TOKEN_CHARGE_ENDPOINT = "https://secure.cardcom.solutions/interface/ChargeToken.aspx";
        private const string RECURRING_PAYMENT_ENDPOINT = "https://secure.cardcom.solutions/interface/RecurringPayment.aspx";

        private const string API_LEVEL = "10";
        private const string LANGUAGE_CODE = "he";  // he, en, ru, ar
        private const string UNICODE = "65001";

        protected string Terminal { get; set; }  
        protected string TerminalNoCvv { get; set; }  
        protected string UserName { get; set; }
        protected string NotifyURL { get; set; }
        protected string SuccessURL { get; set; }  
        protected string ErrorURL { get; set; }          
        
        public CardcomManager(string Terminal, string UserName, string NotifyURL = "", string SuccessURL = "", string ErrorURL = "", string TerminalNoCvv = "") {
            this.Terminal = Terminal;
            this.TerminalNoCvv = TerminalNoCvv;
            this.UserName = UserName;

            this.NotifyURL = NotifyURL;
            this.SuccessURL = SuccessURL;
            this.ErrorURL = ErrorURL;
        }

        #region GenerateIFrameSource
        /// <summary>
        /// Generate an IFrame Source
        /// </summary>
        /// <param name="ProductName">Name Of the paid service</param>
        /// <param name="Items">
        /// Items in this purchase
        /// { Price, Quantity, Description }
        /// </param>
        /// <param name="ReturnValue">Value to pass to the IPN</param>
        /// <param name="MaxNumOfPayments">Max number of payments to show to the user</param>
        /// <param name="Operation">
        /// 1 - Bill Only
        /// 2 - Bill And Create Token
        /// 3 - Token Only
        /// 4 - Suspended Deal (Order)</param>
        /// <param name="CreditType">
        /// 1 - Direct
        /// 2 - Payments
        /// </param>
        /// <param name="Currency">
        /// 1 - NIS 
        /// 2 - USD
        /// http://kb.cardcom.co.il/article/AA-00247/0
        /// </param>
        /// <param name="InvoiceDetails">Details about the invoce</param>
        /// <param name="CustomFields">Custom fields to pass to cardcom</param>
        /// <returns>Iframe src</returns>        
        public CardcomIFrameSourceResponse GenerateIFrameSource(
            string ProductName, 
            List<CardcomIFrameSourceItem> Items, 
            string ReturnValue, 
            int MaxNumOfPayments = 1,
            byte Operation = 2, 
            byte CreditType = 1, 
            byte Currency = 1,
            CardcomInvoiceDetails InvoiceDetails = null,
            List<string> CustomFields = null
        ) 
        {

            // http://kb.cardcom.co.il/article/AA-00402/0/
            var prms = new Dictionary<string, string>();

            prms["TerminalNumber"] = this.Terminal; 
            prms["UserName"] = this.UserName;  
            prms["APILevel"] = API_LEVEL;
            prms["codepage"] = UNICODE; 
            prms["Operation"] = Operation.ToString(); // 1 - Bill Only, 2- Bill And Create Token, 3 - Token Only, 4 - Suspended Deal (Order);
            prms["CreditType"] = CreditType.ToString(); // 1 - Direct, 2 - Payments 
            
            if(MaxNumOfPayments > 1)
                prms["MaxNumOfPayments"] = MaxNumOfPayments.ToString(); // max num of payments to show to the user
            
            // billing coin: 1 - NIS, 2 - USD...
            // list: http://kb.cardcom.co.il/article/AA-00247/0
            prms["CoinID"] = Currency.ToString();

            prms["Language"] = LANGUAGE_CODE; // he, en, ru, ar
            prms["SumToBill"] = Items.Sum(x => x.Price * x.Quantity).ToString(); // Sum To Bill 
            prms["ProductName"] = ProductName;

            // value that will be return with the IPN            
            prms["ReturnValue"] = ReturnValue;

            // (optional) can be defined in Cardcom dashboard
            if (!string.IsNullOrEmpty(this.SuccessURL))
                prms["SuccessRedirectUrl"] = this.SuccessURL;

            if (!string.IsNullOrEmpty(this.ErrorURL))
                prms["ErrorRedirectUrl"] = this.ErrorURL;

            if(!string.IsNullOrEmpty(this.NotifyURL))
                prms["IndicatorUrl"] = this.NotifyURL; // IPN Listener

            // prms["CancelType"] = "2"; // show Cancel button on start
            // prms["CancelUrl"] = "https://localhost:44373/Purchase/Cancel";

            // http://kb.cardcom.co.il/article/AA-00403/0            
            var cfIndex = 1;
            CustomFields?.ForEach(x =>
            {
                prms[$"CustomFields.Field{cfIndex}"] = x;
                cfIndex++;
            });
            
            if (InvoiceDetails != null && Operation < 3) // available only for bill operations (1 and 2)
            {
                // http://kb.cardcom.co.il/article/AA-00244/0                
                prms["InvoiceHead.CustName"] = InvoiceDetails.CustomerName;
                prms["InvoiceHead.SendByEmail"] = InvoiceDetails.SendEmail.ToString(); // will the invoice be send by email to the customer
                prms["InvoiceHead.Language"] = LANGUAGE_CODE; // he or en only
                prms["InvoiceHead.Email"] = InvoiceDetails.Email; // value that will be return and save in CardCom system

                /// NOTE! Sum of all Lines Price*Quantity  must be equals to SumToBil
                var itemIndex = 1;
                Items?.ForEach(x =>
                {
                    prms[$"InvoiceLines{itemIndex}.Description"] = x.Description ?? "";
                    prms[$"InvoiceLines{itemIndex}.Price"] = x.Price.ToString();
                    prms[$"InvoiceLines{itemIndex}.Quantity"] = x.Quantity.ToString();
                    itemIndex++;
                });                               
            }

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var response = client.UploadString(GENERATE_URL_ENDPOINT, string.Join("&", prms.Select(x => $"{x.Key}={x.Value}")));
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(response));

                var StatusCode = Convert.ToInt32(responseParsed["ResponseCode"]);
                if (StatusCode != 0) {
                    var responseText = HttpUtility.UrlDecode(response);
                    LoggerSingleton.Instance.Info("Cardcom", $"GenerateIFrameSource Failed With Code {StatusCode}", new List<string> { responseText });
                }

                return new CardcomIFrameSourceResponse
                {
                    StatusCode = StatusCode,
                    Description = responseParsed["Description"],
                    Code = responseParsed["LowProfileCode"],
                    URL = responseParsed["url"]
                };
            }
        }
        #endregion

        #region GetTransactionDetails
        /// <summary>
        /// Get Transaction Details
        /// </summary>
        /// <param name="Code">Unique Code (parameter: lowprofilecode)</param>
        /// <returns>
        /// Collection of parameters
        /// http://kb.cardcom.co.il/article/AA-00240/0
        /// </returns>
        public (string Raw, NameValueCollection Details) GetTransactionDetails(string Code) {
            using (var client = new WebClient())
            {
                #region ### Response Parameters ###
                /*
                    http://kb.cardcom.co.il/article/AA-00240/0
                    ResponseCode=0
                    Description=Low Profile Code Found
                    terminalnumber=1000
                    lowprofilecode=cfcc6c69-8134-405d-8c61-f3f9779ac37e
                    Operation=2
                    ProssesEndOK=0            
                    DealResponse=0
                    InternalDealNumber=77280553
                    TokenResponse=0
                    Token=4cf8e168-261e-4613-8d20-000332986b24
                    TokenExDate=20221101
                    CardValidityYear=2022
                    CardValidityMonth=10
                    CardOwnerID=040617649
                    TokenApprovalNumber=0012345   
                    NumOfPayments=1
                    InvoiceResponseCode=0
                    InvoiceNumber=264468
                    InvoiceType=1
                    ExtShvaParams.CardNumber5=0000
                    ExtShvaParams.Status1=0
                    ExtShvaParams.Sulac25=6
                    ExtShvaParams.JParameter29=0
                    ExtShvaParams.Tokef30=1022
                    ExtShvaParams.Sum36=100
                    ExtShvaParams.SumStars52=00000000
                    ExtShvaParams.ApprovalNumber71=0012345
                    ExtShvaParams.FirstPaymentSum78=00000000
                    ExtShvaParams.ConstPayment86=00000000
                    ExtShvaParams.NumberOfPayments94=00
                    ExtShvaParams.AbroadCard119=0
                    ExtShvaParams.FirstCardDigits=458000
                    ExtShvaParams.CardName=+++++(%d7%95%d7%99%d7%96%d7%94)+Cal
                    ExtShvaParams.CardTypeCode60=2
                    ExtShvaParams.Mutag24=2
                    ExtShvaParams.CardOwnerName=Card+Owner
                    ExtShvaParams.CardToken=4cf8e168-261e-4613-8d20-000332986b24
                    ExtShvaParams.CardHolderIdentityNumber=040617649
                    ExtShvaParams.CreditType63=1
                    ExtShvaParams.DealType61=02
                    ExtShvaParams.ChargType66=50
                    ExtShvaParams.TerminalNumber=1000
                    ExtShvaParams.BinId=0
                    ExtShvaParams.InternalDealNumber=77280553
                    ExtShvaParams.CouponNumber=03463750
                    ExtShvaParams.DealDate=20201103
                    ExtShvaParams.CardOwnerPhone=039436100
                    custom_field_01=Custom Value-1
                    custom_field_02=Custom Value-2
                    CardOwnerEmail=testsite%40test.co.il
                    CardOwnerName=Card+Owner
                    CardOwnerPhone=039436100
                    ReturnValue=39.90%7c3%7c98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7
                    CoinId=1
                    OperationResponse=0
                    OperationResponseText=OK 
                */
                #endregion

                client.Encoding = Encoding.UTF8;
                var query = $"terminalnumber={this.Terminal}&username={this.UserName}&lowprofilecode={Code}";
                var response = client.DownloadString($"{TRANSACTION_DETAILS_ENDPOINT}?{query}");
                var responseText = HttpUtility.UrlDecode(response);
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(response));

                var StatusCode = Convert.ToInt32(responseParsed["ResponseCode"]);
                if (StatusCode != 0)                
                    LoggerSingleton.Instance.Info("Cardcom", $"GetTransactionDetails #{Code} Failed With Code {StatusCode}", new List<string> { responseText });                

                return (responseText, responseParsed);
            }
        }
        #endregion
        
        #region ChargeWithToken
        public (string Raw, NameValueCollection Details) ChargeWithToken(
            string Token,            
            string CardExpiry /*YYYYMM*/, 
            string CardOwnerId,
            List<CardcomIFrameSourceItem> Items,
            string ReturnValue,
            int NumOfPayments = 1, 
            byte Currency = 1,
            CardcomInvoiceDetails InvoiceDetails = null,
            List<string> CustomFields = null
        )
        {
            // http://kb.cardcom.co.il/article/AA-00253/0
            var prms = new Dictionary<string, string>();

            prms["TerminalNumber"] = this.Terminal;
            prms["UserName"] = this.UserName;
            prms["APILevel"] = API_LEVEL;
            prms["codepage"] = UNICODE;
            
            prms["TokenToCharge.Token"] = Token;

            if (!string.IsNullOrEmpty(CardExpiry)) { 
                prms["TokenToCharge.CardValidityMonth"] = CardExpiry?.Substring(4, 2);
                prms["TokenToCharge.CardValidityYear"] = CardExpiry?.Substring(2, 2);
            }

            prms["TokenToCharge.SumToBill"] = Items.Sum(x => x.Price * x.Quantity).ToString(); // Sum To Bill;
            prms["TokenToCharge.IdentityNumber"] = CardOwnerId;            
            prms["TokenToCharge.NumOfPayments"] = NumOfPayments.ToString();

            // billing coin: 1 - NIS, 2 - USD...
            // list: http://kb.cardcom.co.il/article/AA-00247/0
            prms["TokenToCharge.CoinID"] = Currency.ToString();

            // value that will be return with the IPN            
            prms["ReturnValue"] = ReturnValue;
            
            if (!string.IsNullOrEmpty(this.NotifyURL))
                prms["IndicatorUrl"] = this.NotifyURL; // IPN Listener

            // http://kb.cardcom.co.il/article/AA-00403/0            
            var cfIndex = 1;
            CustomFields?.ForEach(x => {
                prms[$"CustomFields.Field{cfIndex}"] = x;
                cfIndex++;
            });

            // unique id for each transaction (to prevent duplicates)
            /// prms["TokenToCharge.UniqAsmachta"] = "..."

            if (InvoiceDetails != null)
            {
                // http://kb.cardcom.co.il/article/AA-00244/0                
                prms["InvoiceHead.CustName"] = InvoiceDetails.CustomerName;
                prms["InvoiceHead.SendByEmail"] = InvoiceDetails.SendEmail.ToString(); // will the invoice be send by email to the customer
                prms["InvoiceHead.Language"] = LANGUAGE_CODE; // he or en only
                prms["InvoiceHead.Email"] = InvoiceDetails.Email; // value that will be return and save in CardCom system

                /// NOTE! Sum of all Lines Price*Quantity  must be equals to SumToBil
                var itemIndex = 1;
                Items?.ForEach(x =>
                {
                    prms[$"InvoiceLines{itemIndex}.Description"] = x.Description ?? "";
                    prms[$"InvoiceLines{itemIndex}.Price"] = x.Price.ToString();
                    prms[$"InvoiceLines{itemIndex}.Quantity"] = x.Quantity.ToString();
                    itemIndex++;
                });
            }

            using (var client = new WebClient())
            {
                #region ### Response Parameters ###
                /*
                    http://kb.cardcom.co.il/article/AA-00253/0
                    ResponseCode
                    Description
                    InternalDealNumber
                    InvoiceResponse.ResponseCode
                    InvoiceResponse.Description
                    InvoiceResponse.InvoiceNumber
                    InvoiceResponse.InvoiceType
                    ApprovalNumber
                */
                #endregion

                client.Encoding = Encoding.UTF8;
                var response = client.UploadString(TOKEN_CHARGE_ENDPOINT, string.Join("&", prms.Select(x => $"{x.Key}={x.Value}")));
                var responseText = HttpUtility.UrlDecode(response);
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(response));

                var StatusCode = Convert.ToInt32(responseParsed["ResponseCode"]);
                if (StatusCode != 0)
                    LoggerSingleton.Instance.Info("Cardcom", $"ChargeWithToken #{Token} Failed With Code {StatusCode}", new List<string> { responseText });

                return (responseText, responseParsed);
            }
        }
        #endregion
        
        #region SetRecurringCharge
        public (string Raw, NameValueCollection Details) SetRecurringCharge(
            string Code,
            string ProductName,
            DateTime RecurringPaymentsStartDate,
            int TimeIntervalId,
            List<CardcomIFrameSourceItem> Items,
            string ReturnValue,                    
            CardcomInvoiceDetails InvoiceDetails,
            int NumOfPayments = 999999,
            byte Currency = 1            
        )
        {
            // http://kb.cardcom.co.il/article/AA-00444/0
            var prms = new Dictionary<string, string>();

            prms["TerminalNumber"] = this.Terminal;
            prms["UserName"] = this.UserName;
            prms["APILevel"] = API_LEVEL;
            prms["codepage"] = UNICODE;
             
            prms["RecurringPayments.ChargeInTerminal"] = this.TerminalNoCvv; 
            prms["Operation"] = "NewAndUpdate"; // NewAndUpdate, Update  
            prms["LowProfileDealGuid"] = Code;

            prms["Account.CompanyName"] = InvoiceDetails.CustomerName;
            prms["Account.RegisteredBusinessNumber"] = InvoiceDetails.CustomerId;
            prms["Account.SiteUniqueId"] = InvoiceDetails.CustomerId;

            prms["Account.Email"] = InvoiceDetails.Email;

            prms["RecurringPayments.TimeIntervalId"] = TimeIntervalId.ToString();  // 1 = monthly, 2 = weekly, 3 = yearly (note! custom values defined in cardcom dashboard)
            prms["RecurringPayments.FlexItem.Price"] = Items.Sum(x => x.Price * x.Quantity).ToString(); // Sum To Bill;            
            prms["RecurringPayments.TotalNumOfBills"] = NumOfPayments.ToString(); // number of payments, one per week/month/year (depends on the TimeIntervalId)
            prms["RecurringPayments.InternalDecription"] = ProductName;
            prms["RecurringPayments.FlexItem.InvoiceDescription"] = ProductName;
            prms["RecurringPayments.NextDateToBill"] = RecurringPaymentsStartDate.ToString("dd/MM/yyyy"); // format: dd/MM/yyyy

            // billing coin: 1 - NIS, 2 - USD...
            // list: http://kb.cardcom.co.il/article/AA-00247/0
            prms["RecurringPayments.FinalDebitCoinId"] = Currency.ToString();            

            // value that will be return with the IPN            
            prms["RecurringPayments.ReturnValue"] = ReturnValue;

            if (!string.IsNullOrEmpty(this.NotifyURL))
                prms["IndicatorUrl"] = this.NotifyURL; // IPN Listener
            
            using (var client = new WebClient())
            {
                #region ### Response Parameters ###
                /*
                    ResponseCode
                    Description
                    TotalRecurring
                    IsNewAccount
                    AccountId
                    Recurring0.RecurringId
                    Recurring0.ReturnValue
                    Recurring0.IsNewRecurring
                */
                #endregion

                client.Encoding = Encoding.UTF8;
                var response = client.UploadString(RECURRING_PAYMENT_ENDPOINT, string.Join("&", prms.Select(x => $"{x.Key}={x.Value}")));
                var responseText = HttpUtility.UrlDecode(response);
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(response));

                var StatusCode = Convert.ToInt32(responseParsed["ResponseCode"]);
                if (StatusCode != 0)
                    LoggerSingleton.Instance.Info("Cardcom", $"SetRecurringCharge #{Code} Failed With Code {StatusCode}", new List<string> { responseText });

                return (responseText, responseParsed);
            }
        }
        #endregion
        
        #region CancelRecurringCharge
        public (string Raw, NameValueCollection Details) CancelRecurringCharge(
            string Code,
            int RecurringId,
            string ProductName,                                           
            CardcomInvoiceDetails InvoiceDetails,            
            byte Currency = 1
        )
        {
            // http://kb.cardcom.co.il/article/AA-00444/0
            var prms = new Dictionary<string, string>();

            prms["TerminalNumber"] = this.Terminal;
            prms["UserName"] = this.UserName;
            prms["APILevel"] = API_LEVEL;
            prms["codepage"] = UNICODE;

            prms["RecurringPayments.ChargeInTerminal"] = this.TerminalNoCvv;
            prms["Operation"] = "Update"; // NewAndUpdate, Update  
            prms["LowProfileDealGuid"] = Code;
            prms["RecurringPayments.RecurringId"] = RecurringId.ToString();

            prms["Account.CompanyName"] = InvoiceDetails.CustomerName;
            prms["Account.RegisteredBusinessNumber"] = InvoiceDetails.CustomerId;
            prms["Account.SiteUniqueId"] = InvoiceDetails.CustomerId;
            prms["Account.Email"] = InvoiceDetails.Email;            

            prms["RecurringPayments.FlexItem.Price"] = "0"; // Sum To Bill;            
            prms["RecurringPayments.TotalNumOfBills"] = "1";
            prms["RecurringPayments.InternalDecription"] = ProductName;
            prms["RecurringPayments.FlexItem.InvoiceDescription"] = ProductName;           

            // CANCEL!
            prms["RecurringPayments.IsActive"] = "false";  // boolean: false = no charges, true = recurring charges

            // billing coin: 1 - NIS, 2 - USD...
            // list: http://kb.cardcom.co.il/article/AA-00247/0
            prms["RecurringPayments.FinalDebitCoinId"] = Currency.ToString();

            if (!string.IsNullOrEmpty(this.NotifyURL))
                prms["IndicatorUrl"] = this.NotifyURL; // IPN Listener

            using (var client = new WebClient())
            {
                #region ### Response Parameters ###
                /*
                    ResponseCode
                */
                #endregion

                client.Encoding = Encoding.UTF8;
                var response = client.UploadString(RECURRING_PAYMENT_ENDPOINT, string.Join("&", prms.Select(x => $"{x.Key}={x.Value}")));
                var responseText = HttpUtility.UrlDecode(response);
                var responseParsed = new NameValueCollection(HttpUtility.ParseQueryString(response));

                var StatusCode = Convert.ToInt32(responseParsed["ResponseCode"]);
                if (StatusCode != 0)
                    LoggerSingleton.Instance.Info("Cardcom", $"CancelRecurringCharge #{Code} Failed With Code {StatusCode}", new List<string> { 
                        responseText 
                    });

                return (responseText, responseParsed);
            }
        }
        #endregion
    }
}
