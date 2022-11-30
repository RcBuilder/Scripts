using System;
using System.Collections.Generic;

namespace PaymentsService
{
    /*        
        var chargeResult = PaymentManager.Charge(compConfig, paymentData); 
        - 
        paymentData { 
            CardExpiry
            CardNumber
            Comments
            CVV
            Price
            UserFullName
            UserId
            UserPhone
            UserTZ
        };
        -
        compConfig {
            ChargeProvider  // Credit2000, ZCreditS2S
            ChargeTerminal
            ChargePassword
            CompanyName
            OsekNo
            LogoPath1
            LogoPath2
            LogoPath3
            LogoPath4
        }
    */

    public class PaymentManager
    {
        public static PaymentResult Charge(Entities.Configuration Config, PaymentDetails Details) {
            var providerName = Config.ChargeProvider.ToUpper().Trim();
            switch (providerName) {
                case PaymentProviders.Credit2000:
                    var credit2000Response = new Credit2000(Config.ChargeTerminal, Config.ChargePassword, 2).Charge((Credit2000PaymentData)Details);
                    return (PaymentResult)credit2000Response;
                case PaymentProviders.ZCreditS2S:
                    var zCreditS2SResponse = new ZCreditS2S(Config.ChargeTerminal, Config.ChargePassword).MakeTransaction((ZCreditMakeTransactionData)Details);
                    return (PaymentResult)zCreditS2SResponse;
                default:
                    throw new Exception($"No Provider {providerName}!");
            }
        }
    }
}