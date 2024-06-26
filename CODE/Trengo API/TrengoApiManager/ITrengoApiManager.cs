﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrengoApi
{
    public interface ITrengoApiManager
    {
        Task<int> CreateContact(CreateContactRequest Request);                
        Task<int> CreateContactNote(CreateContactNoteRequest Request);
        Task<int> CreateTicket(CreateTicketRequest Request);
        Task<long> CreateTicketMessage(CreateTicketMessageRequest Request);

        Task<int> SetContactCustomField(SetContactCustomFieldRequest Request);
        Task<bool> LabelATicket(LabelATicketRequest Request);
        Task<bool> SetContactProfile(SetContactProfileRequest Request);

        Task<IEnumerable<CustomField>> GetCustomFields();
        Task<IEnumerable<Label>> GetLabels();
    }
}
