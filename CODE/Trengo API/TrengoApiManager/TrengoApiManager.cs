using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrengoApi
{
    /*
        Trengo API
        ----------
        customer-services app!
        all messages from all channels in one centralized location. 
        every message from any channel will create a trengo-ticket which we can then assign, schedule and share with a team.
        trengo also provides reports. 

        website:
        https://trengo.com
	
        reference:
        https://developers.trengo.com/
        https://developers.trengo.com/docs/reference
        https://developers.trengo.com/reference/list-all-tickets

        dashboard:
        https://app.trengo.com/tickets

        training:
        https://www.bigmarker.com/trengohq/Trengo-Training

        tutorials:
        https://www.youtube.com/watch?v=VFCQAWnWOmo
        https://www.youtube.com/playlist?list=PL3L1rEM9gh_1w3JX_wodUOPolWvm8kjUv
        https://www.youtube.com/watch?v=mZ6WT4S17u8&list=PL3L1rEM9gh_1w3JX_wodUOPolWvm8kjUv&index=17&t=5s
        https://www.youtube.com/@TrengoHQ/videos

        help center:
        https://help.trengo.com/en

        supported built-in channels:
        - email 
        - live chat
        - instegram
        - whatsapp business
        - voice
        - etc...

        note! 
        can also create mroe channels via the api. 

        --

        api-key:
        aka token.
        in order to generate an api key, go to your dashboard > Apps & Integrations 
        and click the 'Generate API token' button.

        note that the token is presented only once! copy and save it.   

        --

        base api url:
        https://app.trengo.com/api/v2/<service>

        authorization:
        H Authorization: Bearer <api-key>

        built-in api-client for testing purposes:
        'API Reference' > any endpoint > set header token > Try It!

        --

        paging & filters:

        QUERY PARAMS
        - page: int
        - status: string > OPEN, ASSIGNED,CLOSED,INVALID
        - contact_id: int
        - users: [int]
        - channels: [int]
        - labels: [int]
        - last_message_type: string > INBOUND,OUTBOUND
        - sort: string > use '-' to DESC (e.g: -date)
        - term: string > free text search

        note! 
        apply to GET lists. check-out the Reference for support per each service.
        e.g: https://developers.trengo.com/reference/list-all-tickets

        --

        SERVICES:
        https://developers.trengo.com/reference/
        - TICKETS
        - CONTACTS
        - PROFILES
        - TICKET RESULTS
        - CONTACT GROUPS
        - LABELS
        - TEAMS
        - USERS
        - MESSAGES
        - WEBHOOKS
        - WHATSAPP
        - SMS MESSAGES
        - CUSTOM FIELDS
        - TEAM CHAT
        - VOIP
        - HELP CENTER
        - QUICK REPLIES
        - QUICK ACTIONS

        --

        Locale-Codes:
        - ar-AR
        - de-DE
        - en-GB
        - es-ES
        - fr-FR
        - id-ID
        - it-IT
        - nl-NL
        - pt-BR
        - ru-RU

        User-Role:
        - observer
        - basic_agent
        - advanced_agent
        - supervisor
        - administrator

        --

        postman:
        see 'Trengo API.postman_collection.json'

        projects:
        - Camel-Mountain (ref:Konimbo)

        --

        usage:

        // https://developers.trengo.com/reference/list-all-tickets
        GET https://app.trengo.com/api/v2/tickets
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/list-all-tickets
        GET https://app.trengo.com/api/v2/tickets?page=1&status=OPEN&contact_id&last_message_type=INBOUND&sort=-date
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/list-all-messages
        // GET /api/v2/tickets/{ticket-id}/messages
        GET https://app.trengo.com/api/v2/tickets/713765077/messages
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/as
        GET https://app.trengo.com/api/v2/contacts
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/view-a-user
        // GET /api/v2/contacts/{id}
        GET https://app.trengo.com/api/v2/contacts/451347874
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/list-all-users
        GET https://app.trengo.com/api/v2/users
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/get-a-user
        // GET /api/v2/users/{id}
        GET https://app.trengo.com/api/v2/users/709618
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/get-a-user
        GET https://app.trengo.com/api/v2/users?page=1&sort=first_name
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/list-all-webhooks
        GET https://app.trengo.com/api/v2/webhooks
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/list-all-custom-fields
        GET https://app.trengo.com/api/v2/custom_fields
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/delete-a-profile
        // DELETE /api/v2/profiles/{id}
        DELETE https://app.trengo.com/api/v2/profiles/13571008
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/create-a-profile
        POST https://app.trengo.com/api/v2/profile
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
            "name": "api-profile-1"
        }

        // https://developers.trengo.com/reference/delete-a-user-1
        // DELETE /api/v2/users/{id}
        DELETE https://app.trengo.com/api/v2/users/711587
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/create-a-team
        POST https://app.trengo.com/api/v2/users
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
          "email": "api-test-1@gmail.com",
          "first_name": "api",
          "last_name": "test-1",
          "locale_code": "en-GB",
          "role": "basic_agent"
        }

        // https://developers.trengo.com/reference/delete-a-user
        // DELETE /api/v2/contacts/{id}
        DELETE https://app.trengo.com/api/v2/contacts/451515216
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json

        // https://developers.trengo.com/reference/create-update-a-user
        // POST /api/v2/channels/{id}/contacts
        POST https://app.trengo.com/api/v2/channels/1306497/contacts
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
          "identifier": "972545614020",          
          "name": "test-api-1"
        }

        // https://developers.trengo.com/reference/add-a-note
        // Add Note To Contact
        // POST /api/v2/contacts/{id}/notes
        POST https://app.trengo.com/api/v2/contacts/451515895/notes
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
          "id": 451515895,  
          "note": "some note"
        }

        // https://developers.trengo.com/reference/create-a-custom-field
        // Create Custom field
        // POST /api/v2/custom_fields
        POST https://app.trengo.com/api/v2/custom_fields
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
          "title": "api-custom-field-1",  
          "type": "CONTACT"     // types: PROFILE | CONTACT | TICKET
        }

        // https://developers.trengo.com/reference/set-custom-field
        // Set Contact Custom Field
        // POST /api/v2/contacts/{id}/custom_fields
        POST https://app.trengo.com/api/v2/contacts/451515895/custom_fields
        H Authorization: Bearer xxxxxxxxxx
        H Accept: application/json
        H Content-Type: application/json
        {
          "custom_field_id": 613806, 
          "value": "custom-value-A"
        }

        -- 

        sample-flow
        - create a contact with custome fields, add some notes  
        - create a ticket for this contact, attach a label and send some messages to the ticket

        1.	Create a Contact
	        - 1306497 = wa_business

	        POST channels/1306497/contacts
	        {
		        "identifier": "972545614020",		
		        "name": "test-api"
	        }

	        return { id } // 460022113

        2.	Set Custom Fields
	        GET custom_fields
	        - 609842 = contact_name
	        - 609835 = phone
	        - 609825 = customer number
	        - 609992 = customer_123
	        - 614854 = contact email
                    
	        POST contacts/460022113/custom_fields
	        {
		        "custom_field_id": 609842,  
		        "value": "Roby Cohen"
	        }
	        POST contacts/460022113/custom_fields
	        {
		        "custom_field_id": 609835,  
		        "value": "972545614020"
	        }
	        POST contacts/460022113/custom_fields
	        {
		        "custom_field_id": 609825,  
		        "value": "123456789"
	        }
	        POST contacts/460022113/custom_fields
	        {
		        "custom_field_id": 614854,  
		        "value": "RcBuilder@walla.com"
	        }

        3.	Add Some Notes 
	        POST contacts/460022113/notes
	        {  
		        "note": "some note 1"
	        }
	        POST contacts/460022113/notes
	        {  
		        "note": "some note 2"
	        }

        4.	Create a Ticket
	        POST tickets
	        {
		        "contact_id": "460022113",
		        "channel_id": 1306497,
		        "subject": "test-api-ticket"
	        }

	        return { id } // 731190204

        5.	Attach Label
	        GET labels
	        - 1683642 = Urgent
	        - 1701915 = Konimbo                    

	        POST tickets/731190204/labels
	        {  
		        "label_id": 1701915  
	        }

        6.	Add Message
	        POST tickets/731190204/messages
	        {  
		        "message": "test-api-ticket-message",
		        "internal_note": true
	        }

        -

        Using:        
        try
        {
            var manager = new TrengoApiManager(new TrengoApiConfig
            {
                ApiUrl = ConfigurationManager.AppSettings["TrengoApiUrl"].Trim(),
                ApiKey = ConfigurationManager.AppSettings["TrengoApiKey"].Trim()
            });

            /// 1306497 = wa_business
            const int WA_BUSINESS_CHANNEL = 1306497;
                
            var contactRequest = new CreateContactRequest(HookData.Order.ExtraInfo.Phone, HookData.Order.ExtraInfo.Name, WA_BUSINESS_CHANNEL);
            var contactId = await manager.CreateContact(contactRequest);

            /// 609842 = contact_name
            /// 609835 = phone
            /// 609825 = customer number
            /// 609992 = customer_123
            /// 614854 = contact email

            var customField1Request = new SetContactCustomFieldRequest(contactId, 609842, HookData.Order.ExtraInfo.Name);
            var customField1Id = await manager.SetContactCustomField(customField1Request);

            var customField2Request = new SetContactCustomFieldRequest(contactId, 609835, HookData.Order.ExtraInfo.Phone);
            var customField2Id = await manager.SetContactCustomField(customField2Request);
            
            var customField3Request = new SetContactCustomFieldRequest(contactId, 609825, HookData.Order.CustomerId);
            var customField3Id = await manager.SetContactCustomField(customField3Request);

            var customField4Request = new SetContactCustomFieldRequest(contactId, 614854, HookData.Order.Email);
            var customField4Id = await manager.SetContactCustomField(customField4Request);

            var contactNoteRequest = new CreateContactNoteRequest(contactId, HookData.ToString());
            var contactNoteId = await manager.CreateContactNote(contactNoteRequest);

            var ticketRequest = new CreateTicketRequest(contactId, WA_BUSINESS_CHANNEL, $"KONIMBO INCOMING ORDER #{orderId}");
            var ticketId = await manager.CreateTicket(ticketRequest);

            /// 1683642 = Urgent
            /// 1701915 = Konimbo

            var ticketLabelRequest = new LabelATicketRequest(ticketId, 1701915); 
            var ticketLabelId = await manager.LabelATicket(ticketLabelRequest);

            var ticketMessageRequest = new CreateTicketMessageRequest(ticketId, HookData.ToString(), true);
            var ticketMessageId = await manager.CreateTicketMessage(ticketMessageRequest);

            var result = new
            {
                contactRequest,
                contactId,
                customField1Request,
                customField1Id,
                customField2Request,
                customField2Id,
                customField3Request,
                customField3Id,
                customField4Request,
                customField4Id,
                contactNoteRequest,
                contactNoteId,
                ticketRequest,
                ticketId,
                ticketLabelRequest,
                ticketLabelId,
                ticketMessageRequest,
                ticketMessageId
            };

            try
            {
                File.WriteAllText($"{PAYLOADS_FOLDER}{orderId}_{logId}_model_OUT.txt", JsonConvert.SerializeObject(result));
            }
            catch { }
            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        catch (Exception ex)
        {
            var error = $"[#{orderId}] {ex.Message} [step={step}]";
            this.WriteLog(error);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
        }
    */

    public class TrengoApiManager : ITrengoApiManager
    {
        protected TrengoApiConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public TrengoApiManager(TrengoApiConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        public async Task<int> CreateContact(CreateContactRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<CreateContactRequest, Contact>(
                $"{this.Config.ApiUrl}channels/{Request.ChannelId}/contacts",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<int> CreateContactNote(CreateContactNoteRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<CreateContactNoteRequest, Note>(
                $"{this.Config.ApiUrl}contacts/{Request.ContactId}/notes",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<int> CreateTicket(CreateTicketRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<CreateTicketRequest, Ticket>(
                $"{this.Config.ApiUrl}contacts/{Request.ContactId}/notes",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<int> CreateTicketMessage(CreateTicketMessageRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<CreateTicketMessageRequest, TicketMessage>(
                $"{this.Config.ApiUrl}tickets/{Request.TicketId}/messages",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<int> SetContactCustomField(SetContactCustomFieldRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<SetContactCustomFieldRequest, CustomField>(
                $"{this.Config.ApiUrl}contacts/{Request.ContactId}/custom_fields",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<int> LabelATicket(LabelATicketRequest Request)
        {
            var response = await this.HttpService.POST_ASYNC<LabelATicketRequest, Label>(
                $"{this.Config.ApiUrl}tickets/{Request.TicketId}/labels",
                Request,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model.Id;
        }

        public async Task<IEnumerable<CustomField>> GetCustomFields()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<CustomField>>(
                $"{this.Config.ApiUrl}custom_fields",
                null,                
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model;
        }

        public async Task<IEnumerable<Label>> GetLabels()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Label>>(
                $"{this.Config.ApiUrl}labels",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {Config.ApiKey}"
                }
            );

            return response.Model;
        }
    }
}
