using System;
using System.Collections.Generic;

namespace PaymentsService
{
    public class PaymentManager
    {
        public static PaymentResult Charge(Entities.Configuration Config, PaymentDetails Details) {            
            switch (Details.Provider) {
                case ePaymentProvider.Credit2000:
                    var credit2000Response = new Credit2000(Config.ApiUserName, Config.ApiUserKey, Config.ApiVersion).Charge((Credit2000PaymentData)Details);
                    return (PaymentResult)credit2000Response;
                case ePaymentProvider.ZCreditS2S:
                    var zCreditS2SResponse = new ZCreditS2S(Config.Terminal, Config.ApiPassword).MakeTransaction((ZCreditMakeTransactionData)Details);
                    return (PaymentResult)zCreditS2SResponse;
                default:
                    throw new Exception("No Provider!");
            }
        }
    }
}