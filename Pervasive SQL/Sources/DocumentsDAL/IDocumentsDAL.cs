
using Entities;
using System.Collections.Generic;

namespace DocumentsDAL
{
    public interface IDocumentsDAL
    {
        bool CreateOrder(Order order);
        bool CreateInvoice(Invoice invoice);
        bool CreateDeliveryNote(DeliveryNote deliveryNote);

        IEnumerable<Order> FindOrders(OrderSearchParams searchParams);        
        IEnumerable<Invoice> FindInvoices(InvoiceSearchParams searchParams);
        IEnumerable<DeliveryNote> FindDeliveryNotes(DeliveryNoteSearchParams searchParams);

        float GetVATRate();
        bool UsePriceRound();
        bool AccountExists(int accountId);
    }
}
