
using Entities;
using System.Collections.Generic;

namespace DocumentsDAL
{
    public interface IDocumentsDAL
    {
        bool CreateOrder(Order order);
        bool CreateOrderMas(Order order);
        bool CreatePriceOffer(PriceOffer priceOffer);
        bool CreateInvoice(Invoice invoice);        
        bool CreateDeliveryNote(DeliveryNote deliveryNote);
        bool CreateReceipt(Receipt receipt);
        bool CreateInvoiceReceipt(InvoiceReceipt invoiceReceipt);
        bool CreateAccountingTransaction(AccountingTransaction transaction);

        IEnumerable<Order> FindOrders(OrderSearchParams searchParams);
        IEnumerable<Order> FindOrdersMas(OrderSearchParams searchParams);
        IEnumerable<PriceOffer> FindPriceOffers(PriceOfferSearchParams searchParams);
        IEnumerable<Invoice> FindInvoices(InvoiceSearchParams searchParams);
        IEnumerable<InvoiceReceipt> FindInvoiceReceipts(InvoiceReceiptSearchParams searchParams);
        IEnumerable<Receipt> FindReceipts(ReceiptSearchParams searchParams);
        IEnumerable<DeliveryNote> FindDeliveryNotes(DeliveryNoteSearchParams searchParams);
        IEnumerable<MismRowData> FindInMISM(MISMSearchParams searchParams);

        bool DeleteOrder(int Id);
        bool DeleteOrderMas(int Id);
        bool IsExistsOrder(int Id, string TableName);        
        int UpdateOrderCollected(OrderCollected orderCollected);
        bool UpdateMISMRowURL(int MismNo, string MismCode, string DocumentURL);

        float GetVATRate();
        int GetOpAcc(int recNo);
        bool UsePriceRound();
        bool AccountExists(int accountId);
        int GetNextDocCounter(eCounterNo counterNo, bool CreateIfNotExists = true);
        bool SetNextDocCounter(eCounterNo counterNo);
        int GetNextTransactionId(eTransactionProvider provider);
        bool CreateDocCounter(eCounterNo counterNo, int Value);
        Configuration GetConfiguration();
        CompanyInfo GetCompanyInfo();
        HashSet<string> GetTablesInMISM(string MismCode);
        KupaMappingData GetKUPA(int PaymentType);
        string PaymentType2KupaName(int PaymentType);
    }
}
