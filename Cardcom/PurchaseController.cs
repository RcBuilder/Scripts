using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Website.Models;
using WebsiteBLL;

namespace Website.App_Start
{
    public class PurchaseController : Controller
    {
        public async Task<ActionResult> Thanks()
        {            
            #region ### Response Parameters ###
            /*
                terminalnumber=1000
                lowprofilecode=2467a885-1f97-40d5-8b5f-42222ce64c80
                Operation=2            
                ResponseCode=0
                Status=0                
            */
            #endregion

            var cardcomManager = new CardcomManager(ConfigSingleton.Instance.CardcomTerminal, ConfigSingleton.Instance.CardcomUserName, TerminalNoCvv: ConfigSingleton.Instance.CardcomTerminalNoCvv);
            var detailsResponse = cardcomManager.GetTransactionDetails(Request.QueryString["lowprofilecode"]);

            // Collect custom details
            // returnValue: PRICE|PACKAGE|USER_ID|COUPON (e.g: 39.90|3|1|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7)
            var returnValue = detailsResponse.Details["ReturnValue"];
            var returnValueSlices = returnValue.Split('|');
            
            var model = new PurchaseThanks();
            model.PackageDetails = PackagesSingleton.Instance.GetPackage(Convert.ToInt32(returnValueSlices[1]));
            model.SubscriptionDetails = (await new SubscriptionsBLL().Get(Convert.ToInt32(returnValueSlices[2])))?.Details;
            model.AmountPaid = Convert.ToSingle(returnValueSlices[0]);
            return View(model);
        }

        public async Task<ActionResult> Thanks0()
        {
            #region ### Response Parameters ###
            /*
                ReturnValue                
            */
            #endregion
            
            // Collect custom details
            // returnValue: PRICE|PACKAGE|USER_ID|COUPON (e.g: 39.90|3|1|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7)
            var returnValue = Request.QueryString["ReturnValue"].Trim();
            var returnValueSlices = returnValue.Split('|');

            var model = new PurchaseThanks();
            model.PackageDetails = PackagesSingleton.Instance.GetPackage(Convert.ToInt32(returnValueSlices[1]));
            model.SubscriptionDetails = (await new SubscriptionsBLL().Get(Convert.ToInt32(returnValueSlices[2])))?.Details;
            model.AmountPaid = Convert.ToSingle(returnValueSlices[0]);
            return View(model);
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]                                  
        public async Task<HttpStatusCodeResult> ProcessCardcomTransaction()
        {
            #region ### IPN Parameters ###
            /*             
                GET https://localhost:44373/Purchase/ProcessCardcomTransaction?terminalnumber=1000&lowprofilecode=2467a885-1f97-40d5-8b5f-42222ce64c80&Operation=2&DealRespone=0&DealResponse=0&TokenResponse=0&InvoiceResponseCode=0&OperationResponse=0&OperationResponseText=OK&ReturnValue=39.90%7c3%7c1%7c98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7 

                ---

                terminalnumber=1000
                lowprofilecode=2467a885-1f97-40d5-8b5f-42222ce64c80
                Operation=2            
                DealResponse=0
                TokenResponse=0
                InvoiceResponseCode=0
                OperationResponse=0
                OperationResponseText=OK
                ReturnValue=39.90|3|1|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7
            */
            #endregion

            var step = 0;
            try
            {                
                LoggerSingleton.Instance.Info("Cardcom", "Process-Transaction", new List<string> { $"QueryString: {Request.QueryString}" });

                var transactionsBLL = new TransactionsBLL();
                var couponsBLL = new CouponsBLL();
                var subscriptionsBLL = new SubscriptionsBLL();
                
                var cardcomManager = new CardcomManager(ConfigSingleton.Instance.CardcomTerminal, ConfigSingleton.Instance.CardcomUserName, TerminalNoCvv: ConfigSingleton.Instance.CardcomTerminalNoCvv);
                step = 1;

                // Collect basic details 
                var transaction = new Transaction();
                transaction.Raw = Request.QueryString?.ToString();
                transaction.Code = Request.QueryString["lowprofilecode"];                
                transaction.StatusCode = Convert.ToInt32(Request.QueryString["OperationResponse"]);
                transaction.CardStatusCode = Convert.ToInt32(Request.QueryString["DealResponse"]);
                step = 2;

                // Collect extra details                 
                var detailsResponse = cardcomManager.GetTransactionDetails(transaction.Code);                
                step = 3;

                transaction.RawDetails = detailsResponse.Raw;
                transaction.Token = detailsResponse.Details["Token"] ?? "";
                transaction.TokenExpiry = detailsResponse.Details["TokenExDate"] ?? "";
                transaction.TokenApprovalNumber = detailsResponse.Details["TokenApprovalNumber"] ?? "";
                transaction.CardOwnerId = detailsResponse.Details["CardOwnerID"] ?? "";

                // fix format > MM (e.g: 7 to 07)
                var CardValidityMonth = detailsResponse.Details["CardValidityMonth"] ?? "";
                if (CardValidityMonth.Length == 1) CardValidityMonth = $"0{CardValidityMonth}";

                transaction.CardExpiry = $"{detailsResponse.Details["CardValidityYear"] ?? ""}{CardValidityMonth}";
                transaction.CardSuffix = detailsResponse.Details["ExtShvaParams.CardNumber5"] ?? "";
                transaction.InvoiceStatusCode = Convert.ToInt32(detailsResponse.Details["InvoiceResponseCode"]);
                transaction.InvoiceNumber = detailsResponse.Details["InvoiceNumber"] ?? "";
                transaction.InvoiceType = Convert.ToInt32(detailsResponse.Details["InvoiceType"] ?? "0");
                transaction.NumOfPayments = Convert.ToInt32(detailsResponse.Details["NumOfPayments"] ?? "1");
                step = 4;

                // Collect custom details
                // returnValue: PRICE|PACKAGE|USER_ID|COUPON (e.g: 39.90|3|1|98F41F8F-6F6C-4A0A-A7D4-D719A41A6BD7)                
                var returnValue = detailsResponse.Details["ReturnValue"].Trim();                
                var returnValueSlices = returnValue.Split('|');
                transaction.Price = Convert.ToSingle(returnValueSlices[0]);                
                transaction.PackageId = Convert.ToInt32(returnValueSlices[1]);
                transaction.SubscriptionId = Convert.ToInt32(returnValueSlices[2]);

                var coupon = await couponsBLL.Find(returnValueSlices[3]);
                transaction.Coupon = coupon == null ? "" : coupon.Code.ToString();
                step = 5;

                // Create Transaction Row 
                var transactionId = await transactionsBLL.Save(transaction);
                if (transactionId <= 0)
                    throw new Exception($"Error while trying to process an incoming transaction ({transaction.Code})");
                transaction.RowId = transactionId;
                step = 6;

                // Update Coupon Capacity 
                if (!string.IsNullOrEmpty(transaction.Coupon)) {
                    var successCapacity = await couponsBLL.IncreaseCapacityInUse(Guid.Parse(transaction.Coupon));
                    if(!successCapacity) LoggerSingleton.Instance.Info("Cardcom", "IncreaseCapacity Failed", new List<string> {
                        $"#{transaction.SubscriptionId}",
                        transaction.Coupon 
                    });
                }
                step = 7;

                var packageDetails = PackagesSingleton.Instance.GetPackage(transaction.PackageId);
                step = 8;

                // calculate the valid-until, the Recurring Payments Start Date and the corresponding interval id
                DateTime? validUntil = packageDetails.ValidUntil;
                var timeIntervalId = packageDetails.CardcomIntervalId;                
                step = 9;

                // update ValidUntil 
                var successValidUntil = await subscriptionsBLL.SaveValidUntil(transaction.SubscriptionId, validUntil);
                if (!successValidUntil) LoggerSingleton.Instance.Info("Cardcom", "SaveValidUntil Failed", new List<string> {
                    $"#{transaction.SubscriptionId}",
                }); 
                step = 10;

                var chargeDetails = new Entities.ChargeDetails
                {
                    SubscriptionId = transaction.SubscriptionId,
                    Token = transaction.Token,
                    IsAutoRenewal = true,
                    CardSuffix = transaction.CardSuffix,
                    CardExpiry = transaction.CardExpiry,
                    CardOwnerId = transaction.CardOwnerId,
                    Code = transaction.Code
                };

                // save charge details for this subscription
                var successChargeDetails = await subscriptionsBLL.SaveChargeDetails(chargeDetails);
                if (!successChargeDetails) LoggerSingleton.Instance.Info("Cardcom", "SaveChargeDetails Failed", new List<string> {
                    $"#{chargeDetails.SubscriptionId}",
                    chargeDetails.Token
                });                
                step = 11;

                // load subscription 
                var subscription = await subscriptionsBLL.Get(transaction.SubscriptionId);
                step = 12;
                
                var responseCharge = cardcomManager.SetRecurringCharge(
                    chargeDetails.Code, 
                    "My Morning Love", 
                    validUntil.Value, 
                    timeIntervalId,
                    new List<CardcomIFrameSourceItem> {
                        new CardcomIFrameSourceItem {
                            Description = packageDetails.Name,
                            Price = packageDetails.Price,
                            Quantity = 1
                        }
                    },
                    returnValue,
                    new CardcomInvoiceDetails
                    {
                        CustomerId = subscription.Details.Id.ToString(),
                        CustomerName = subscription.Details.FullName,
                        Email = subscription.Details.Email,
                        SendEmail = true
                    }
                );
                step = 13;
                if (responseCharge.Details["ResponseCode"] != "0") {            
                    LoggerSingleton.Instance.Info("Cardcom", $"SetRecurringCharge Failed With Code {responseCharge.Details["ResponseCode"]}");                       

                    await subscriptionsBLL.SaveAutoRenewal(transaction.SubscriptionId, false); // recurring charge has failed > mark user as AutoRenewal false
                }
                step = 14;

                chargeDetails.RecurringId = Convert.ToInt32(responseCharge.Details["Recurring0.RecurringId"]);
                successChargeDetails = await subscriptionsBLL.SaveChargeDetails(chargeDetails);
                if (!successChargeDetails) LoggerSingleton.Instance.Info("Cardcom", "SaveChargeDetails (with RecurringId) Failed", new List<string> {
                    $"#{chargeDetails.SubscriptionId}",
                    chargeDetails.Token,
                    responseCharge.Details["Recurring0.RecurringId"]
                });
                step = 15;

                var success = await subscriptionsBLL.SaveRegistrationStep(subscription.Details.Id, "Completed");
                if (!success) LoggerSingleton.Instance.Info("Cardcom", $"SaveRegistrationStep Failed For User #{subscription.Details.Id}");

                return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
            }
            catch (Exception ex) {                
                ex.Data.Add("step", step);
                ex.Data.Add("Method", "ProcessCardcomTransaction");
                LoggerSingleton.Instance.Error("Cardcom", ex);                
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}