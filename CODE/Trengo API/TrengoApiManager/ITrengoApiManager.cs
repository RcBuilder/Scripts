using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrengoApi
{
    public interface ITrengoApiManager
    {
        Task<int> CreateContact(CreateContactRequest Request);                
        Task<int> CreateContactNote(CreateContactNoteRequest Request);
        Task<int> CreateTicket(CreateTicketRequest Request);
        Task<int> CreateTicketMessage(CreateTicketMessageRequest Request);

        Task<int> SetContactCustomField(SetContactCustomFieldRequest Request);
        Task<int> LabelATicket(LabelATicketRequest Request);        

        Task<IEnumerable<CustomField>> GetCustomFields();
        Task<IEnumerable<Label>> GetLabels();
    }
}
