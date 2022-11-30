using System;
using System.Collections.Generic;

namespace PaymentsService
{
    public class PaymentResult
    {        
        public string Provider { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
        public int ReferenceNumber { get; set; }
        public string VoucherNumber { get; set; }
        public string ApprovalNumber { get; set; }
  
        public static explicit operator PaymentResult(Credit2000ChargeResponse other) {
            return new PaymentResult
            {
                Provider = PaymentProviders.Credit2000,
                Code = other.Code,
                Message = other.Message                
            };
        }

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
}