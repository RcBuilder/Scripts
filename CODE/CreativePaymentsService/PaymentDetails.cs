using System;
using System.Collections.Generic;

namespace PaymentsService
{
    public class PaymentDetails
    {
        public ePaymentProvider Provider { get; set; } = ePaymentProvider.Credit2000;

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

        public static explicit operator Credit2000PaymentData(PaymentDetails other) {
            return new Credit2000PaymentData
            {
                CardExpiry = other.CardExpiry,
                CardNumber = other.CardNumber ?? "",
                Comments = other.Comments,
                CVV = other.CVV,
                Price = other.Price,
                UserEmail = other.UserEmail,
                UserFax = other.UserFax,
                UserFullName = other.UserFullName,
                UserId = other.UserId,
                UserPhone = other.UserPhone,
                UserTZ = other.UserTZ
            };
        }

        public static explicit operator ZCreditMakeTransactionData(PaymentDetails other)
        {
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
                Track2 = ""
    };
        }
    }

    public enum ePaymentProvider { Credit2000, ZCreditS2S }
}