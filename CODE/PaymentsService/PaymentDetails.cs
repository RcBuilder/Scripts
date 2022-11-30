using System;
using System.Collections.Generic;

namespace PaymentsService
{
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
}