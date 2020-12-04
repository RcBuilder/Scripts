
namespace Entities
{
    public class Transaction
    {             
        public int RowId { get; set; }     
        public int SubscriptionId { get; set; }     
        public string Raw { get; set; }
        public string RawDetails { get; set; }
        public string Code { get; set; }
        public int StatusCode { get; set; }
        public int CardStatusCode { get; set; }
        public string Token { get; set; }
        public string TokenExpiry { get; set; }
        public string TokenApprovalNumber { get; set; }
        public string CardOwnerId { get; set; }
        public string CardExpiry { get; set; }
        public string CardSuffix { get; set; }
        public int InvoiceStatusCode { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceType { get; set; }
        public int NumOfPayments { get; set; }
        public string Coupon { get; set; }
        public int PackageId { get; set; }    
        public float Price { get; set; }    
    }
}
