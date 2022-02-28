using System;
using System.Collections.Generic;

namespace PaymentsService
{
    public class PaymentResult
    {
        public ePaymentProvider Provider { get; set; } = ePaymentProvider.Credit2000;

        public string Code { get; set; }
        public string Message { get; set; }        

        public static explicit operator PaymentResult(Credit2000ChargeResponse other) {
            return new PaymentResult
            {
                Provider = ePaymentProvider.Credit2000,
                Code = other.Code,
                Message = other.Message
            };
        }

        public static explicit operator PaymentResult(ZCreditMakeTransactionResponse other)
        {
            return new PaymentResult
            {
                Provider = ePaymentProvider.ZCreditS2S,
                Code = other.ReturnCode.ToString(),
                Message = other.ReturnMessage
            };
        }
    }
}