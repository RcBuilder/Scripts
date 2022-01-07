using WEB.ActionFilters;
using System.Web.Mvc;
using System.Threading.Tasks;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cardcom;
using System.Net;

namespace WEB.Controllers
{
    public class CartController : Controller
    {        
        public ActionResult Index()
        {            
            return View(new Models.CartDTO());
        }

        [HttpPost]
        public async Task<ActionResult> Index(Models.CartDTO Model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(Model);

                var ordersBLL = new OrdersBLL();
                var restaurantsBLL = new RestaurantsBLL();
     
                // create order + rows 
                var orderDetails = new Entities.OrderDetails { 
                    ClientEmail = Model.ClientEmail,
                    ClientName = Model.ClientName,
                    ClientPhone = Model.ClientPhone,
                    NameOnReceipt = Model.NameOnReceipt,
                    Notes = Model.Notes,
                    OrderDate = Model.OrderDate,
                    RestaurantId = Model.RestaurantId,
                    Type = Entities.eOrderType.TAKE_AWAY,
                    PaymentType = Model.PaymentType,
                    Status = Entities.eOrderStatus.NONE,
                    IsPaid = false,
                    TableNumber = Model.TableNumber
                };

                var menu = await restaurantsBLL.GetMenu(Model.RestaurantId);
                var menuIds = Model.CartItemIds;               
                var orderRows = menu.Items?.Join(menuIds, m => m.Id, idPair => idPair.ItemId, (m, idPair) => new { m, rowId = idPair.ItemRowId })?.Select(model =>
                {
                    var selectedProperties = Model.ItemPropertiesMap?.ContainsKey(model.rowId) ?? false ? Model.ItemPropertiesMap[model.rowId].Properties : "";
                    var isStarter = Model.IsStarterMap?.ContainsKey(model.rowId) ?? false ? Model.IsStarterMap[model.rowId].IsStarter : false;

                    return new Entities.OrderRow
                    {
                        IsDeal = false,
                        ItemName = model.m.Name,
                        ItemPrice = model.m.Price,
                        ItemId = model.m.Id,
                        Properties = selectedProperties,
                        IsStarter = isStarter
                    };
                })?.ToList();

                var sideDishesDB = new List<Entities.MenuItemSideDish>();
                if (Model.ItemSideDishMap != null) {
                    foreach (var sdm in Model.ItemSideDishMap) {
                        var sd = await new MenuBLL().GetItemSideDishes(sdm.ItemId);
                        sideDishesDB.Add(sd.FirstOrDefault(x => x.SideDishId == sdm.SidedishId));
                    };
                }

                var sideDishes = sideDishesDB?.Select(sd => {
                    return new Entities.OrderRow
                    {
                        IsDeal = false,
                        ItemName = sd.SideDishName,
                        ItemPrice = sd.Price,
                        ItemId = sd.SideDishId,
                        Notes = "תוספת למנה עיקרית"
                    };
                })?.ToList();

                if ((sideDishes?.Count ?? 0) > 0)
                    orderRows.AddRange(sideDishes);

                var orderId = await ordersBLL.Create(new Entities.Order { 
                    Details = orderDetails,
                    Rows = orderRows
                });

                // send overload alert if needed 
                await restaurantsBLL.NotifyOverload(Model.RestaurantId);

                if (Model.PaymentType == Entities.ePaymentType.CASH) {
                    // update order status
                    var orderUpdated = await ordersBLL.SaveStatus(orderId, Entities.eOrderStatus.APPROVED);
                        if (!orderUpdated) 
                            LoggerSingleton.Instance.Info("Cardcom", "Save Order Status Failed", new List<string> {
                                $"#{orderId}", "Status: APPROVED"
                            });

                    return RedirectToAction("PayThanks", new { Id = orderId });
                }
                else 
                    return RedirectToAction("Pay", new { Id = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("CustomError", ex.Message);
                return View(Model);
            }
        }

        public async Task<ActionResult> Pay(int Id)
        {            
            var ordersBLL = new OrdersBLL();
            var order = await ordersBLL.Get(Id);

            if(order == null)
                throw new Exception($"Order #{Id} Does Not Exist");
            if (order.Details.Status != Entities.eOrderStatus.NONE)
                throw new Exception($"Order #{Id} is already In-Progress");

            var cardcomManager = new CardcomManager(
                ConfigSingleton.Instance.CardcomTerminal,
                ConfigSingleton.Instance.CardcomUserName,
                ConfigSingleton.Instance.CardcomNotifyURL,
                ConfigSingleton.Instance.CardcomSuccessURL,
                ConfigSingleton.Instance.CardcomErrorURL                
            );

            var response = cardcomManager.GenerateIFrameSource(                
                "הזמנת אוכל",                
                    order.Rows?.Select(row => new CardcomIFrameSourceItem { 
                        Quantity = 1,
                        Price = row.ItemPrice,
                        Description = row.ItemName
                    })?.ToList(),
                    $"{Id}",
                    Operation: 1,
                    InvoiceDetails: new CardcomInvoiceDetails { 
                        CustomerName = order.Details.ClientName,
                        CustomerId = order.Details.ClientPhone,
                        Email = order.Details.ClientEmail,
                        SendEmail = true
                    }
            );

            return View(new Models.PayDTO { 
                FrameURL = response.URL
            });
        }

        public ActionResult PayThanks()
        {
            return View();
        }

        public ActionResult PayThanksFrame()
        {
            return View();
        }

        public ActionResult PayErrorFrame()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> ProcessCardcomTransaction()
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
            var transaction = new Entities.Transaction();
            var httpResult = "";

            var transactionsBLL = new TransactionsBLL();            
            var ordersBLL = new OrdersBLL();

            try
            {
                LoggerSingleton.Instance.Info("Cardcom", "Process-Transaction", new List<string> { $"QueryString: {Request.QueryString}" });
                
                step = 1;

                var cardcomManager = new CardcomManager(ConfigSingleton.Instance.CardcomTerminal, ConfigSingleton.Instance.CardcomUserName);

                step = 2;

                // Collect basic details                 
                transaction.Raw = Request.QueryString?.ToString();
                transaction.Code = Request.QueryString["lowprofilecode"];
                transaction.StatusCode = Convert.ToInt32(Request.QueryString["OperationResponse"]);
                transaction.StatusCodeCard = Convert.ToInt32(Request.QueryString["DealResponse"]);

                step = 3;

                if (transaction.StatusCode != 0)
                    throw new Exception($"[StatusCode Error] StatusCode {transaction.StatusCode} ({transaction.Code})");

                step = 4;

                // Collect extra details                 
                var detailsResponse = cardcomManager.GetTransactionDetails(transaction.Code);
                transaction.StatusCodeDetails = Convert.ToInt32(detailsResponse.Details["ResponseCode"]);
                transaction.RawDetails = detailsResponse.Raw;

                step = 5;
           
                // add invoice details 
                transaction.InvoiceStatusCode = Convert.ToInt32(detailsResponse.Details["InvoiceResponseCode"]);
                transaction.InvoiceNumber = detailsResponse.Details["InvoiceNumber"] ?? "";
                transaction.InvoiceType = Convert.ToInt32(detailsResponse.Details["InvoiceType"] ?? "0");

                step = 6;

                if (transaction.StatusCodeDetails != 0)
                    throw new Exception($"[StatusCode Error] StatusCodeDetails {transaction.StatusCodeDetails} ({transaction.Code})");

                step = 7;

                transaction.CardOwnerId = detailsResponse.Details["CardOwnerID"] ?? "";

                // fix format > MM (e.g: 7 to 07)
                var CardValidityMonth = detailsResponse.Details["CardValidityMonth"] ?? "";
                if (CardValidityMonth.Length == 1) CardValidityMonth = $"0{CardValidityMonth}";

                transaction.CardExpiry = $"{detailsResponse.Details["CardValidityYear"] ?? ""}{CardValidityMonth}";
                transaction.CardSuffix = detailsResponse.Details["ExtShvaParams.CardNumber5"] ?? "";
                transaction.NumOfPayments = Convert.ToInt32(detailsResponse.Details["NumOfPayments"] ?? "1");

                step = 8;

                // load ReturnValue field > extract order-id
                var returnValue = detailsResponse.Details["ReturnValue"].Trim();
                var orderId = Convert.ToInt32(returnValue);

                step = 9;

                // load related order
                var order = await ordersBLL.Get(orderId);

                step = 10;

                // Collect custom details                
                transaction.Price = order.Total;
                transaction.OrderId = order.Details.Id;

                step = 11;

                // update Order Status 
                order.Details.Status = Entities.eOrderStatus.APPROVED;
                order.Details.IsPaid = true;
                var orderUpdated = await ordersBLL.SaveDetails(order.Details) > 0;
                if (!orderUpdated) LoggerSingleton.Instance.Info("Cardcom", "Save Order Details Failed", new List<string> {
                    $"#{orderId}", "Status: APPROVED", $"Code: {transaction.Code}"
                });

                step = 12;

                // send sms
                if (orderUpdated)
                    SMSManager.OrderStatusChanged(order);

                step = 13;

                httpResult = "OK";
            }
            catch (Exception ex)
            {                
                ex.Data.Add("step", step);                
                ex.Data.Add("Method", "ProcessCardcomTransaction");
                LoggerSingleton.Instance.Error("Cardcom", ex);
                httpResult = ex.Message;
            }

            try {
                // add a transaction
                var transactionId = await transactionsBLL.Create(transaction);
                if (transactionId <= 0)
                    throw new Exception($"Error while trying to save an incoming transaction ({transaction.Code})");
            }
            catch (Exception ex)
            {                
                ex.Data.Add("Action", "CreateTransaction");
                ex.Data.Add("Method", "ProcessCardcomTransaction");
                LoggerSingleton.Instance.Error("Cardcom", ex);
                httpResult = ex.Message;
            }

            Response.StatusCode = httpResult == "OK" ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError;
            return httpResult;
        }
    }
}