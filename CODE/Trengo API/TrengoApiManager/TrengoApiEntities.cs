using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TrengoApi
{   
    public class TrengoApiConfig
    {
        public string ApiUrl { get; set; }                
        public string ApiKey { get; set; }               
    }

    public abstract class TrengoApiBaseRequest
    {
        [JsonProperty(PropertyName = "api_key")]
        public string ApiKey { get; set; }
    }

    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public IEnumerable<string> Errors { get; set; }
    }


    public class CreateContactRequest {
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; } // phone or email (unique)

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "channel_id")]
        public int ChannelId { get; set; }
        
        public CreateContactRequest(string Identifier, string Name, int ChannelId)
        {
            this.Identifier = Identifier;
            this.Name = Name;
            this.ChannelId = ChannelId;
        }
    }

    /*
        {
            "id": 460022113,
            "name": "test-api",
            "full_name": "test-api",
            "email": null,
            "abbr": "t",
            "color": "#ff5722",
            "profile_image": null,
            "is_phone": true,
            "phone": "+972545614020",
            "formatted_phone": "+972 54-561-4020",
            "avatar": "https://assets.trengo.com/release/img/defaultpic.png",
            "identifier": "+972 54-561-4020",
            "custom_field_data": {
                "Customer number": "032828162",
                "phone": "972545614020",
                "contact_name": "Roby Cohen",
                "customer_123": "RcBuilder@walla.com",
                "api-custom-field-2": "RcBuilder@walla.com",
                "contact email": "RcBuilder@walla.com"
            },
            "pivot": null,
            "formatted_custom_field_data": {
                "contact email": "RcBuilder@walla.com",
                "api-custom-field-2": "RcBuilder@walla.com",
                "api-custom-field-1": null,
                "customer_123": "RcBuilder@walla.com",
                "123": null,
                "contact_name": "Roby Cohen",
                "phone": "972545614020",
                "Customer number": "032828162"
            },
            "display_name": "test-api (+972 54-561-4020)",
            "is_private": false,
            "custom_field_values": [
                {
                    "custom_field_id": 609825,
                    "field_name": "Customer number",
                    "value": "032828162"
                },
                {
                    "custom_field_id": 609835,
                    "field_name": "phone",
                    "value": "972545614020"
                },
                {
                    "custom_field_id": 609842,
                    "field_name": "contact_name",
                    "value": "Roby Cohen"
                },
                {
                    "custom_field_id": 609992,
                    "field_name": "customer_123",
                    "value": "RcBuilder@walla.com"
                },
                {
                    "custom_field_id": 614853,
                    "field_name": "api-custom-field-2",
                    "value": "RcBuilder@walla.com"
                },
                {
                    "custom_field_id": 614854,
                    "field_name": "contact email",
                    "value": "RcBuilder@walla.com"
                }
            ]
        }
    */
    public class Contact
    {        
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "is_phone")]
        public bool IsPhone { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "custom_field_data")]
        public Dictionary<string, string> CustomFieldsMap { get; set; }
    }

    public class SetContactCustomFieldRequest
    {
        [JsonProperty(PropertyName = "contact_id")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "custom_field_id")]
        public int CustomFieldId { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        
        public SetContactCustomFieldRequest(int ContactId, int CustomFieldId, string Value)
        {
            this.ContactId = ContactId;
            this.CustomFieldId = CustomFieldId;
            this.Value = Value;
        }
    }

    /*
        {
            "id": 613806,
            "title": "api-custom-field-1",
            "sort_order": -4,
            "type": "CONTACT",
            "identifier": "api-custom-field-1",
            "field_type": "TEXT",
            "placeholder": null,
            "items": null,
            "channels": [],
            "is_required": false
        }
    */
    public class CustomField
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "field_type")]
        public string fieldType { get; set; }                            
    }

    public class CreateContactNoteRequest
    {
        [JsonProperty(PropertyName = "contact_id")]
        public int ContactId { get; set; }
        
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        
        public CreateContactNoteRequest(int ContactId, string Note)
        {
            this.ContactId = ContactId;
            this.Note = Note;
        }
    }

    /*
        {
            "id": 6221328,
            "contact": {
                "id": 460022113,
                "name": "test-api",
                "full_name": "test-api",
                "email": null,
                "abbr": "t",
                "color": "#ff5722",
                "profile_image": null,
                "is_phone": true,
                "phone": "+972545614020",
                "formatted_phone": "+972 54-561-4020",
                "avatar": "https://assets.trengo.com/release/img/defaultpic.png",
                "identifier": "+972 54-561-4020",
                "custom_field_data": {
                    "Customer number": "032828162",
                    "phone": "972545614020",
                    "contact_name": "Roby Cohen",
                    "customer_123": "RcBuilder@walla.com",
                    "api-custom-field-2": "RcBuilder@walla.com",
                    "contact email": "RcBuilder@walla.com"
                },
                "pivot": null,
                "formatted_custom_field_data": {
                    "contact email": "RcBuilder@walla.com",
                    "api-custom-field-2": "RcBuilder@walla.com",
                    "api-custom-field-1": null,
                    "customer_123": "RcBuilder@walla.com",
                    "123": null,
                    "contact_name": "Roby Cohen",
                    "phone": "972545614020",
                    "Customer number": "032828162"
                },
                "display_name": "test-api (+972 54-561-4020)",
                "is_private": false,
                "custom_field_values": [
                    {
                        "custom_field_id": 609825,
                        "field_name": "Customer number",
                        "value": "032828162"
                    },
                    {
                        "custom_field_id": 609835,
                        "field_name": "phone",
                        "value": "972545614020"
                    },
                    {
                        "custom_field_id": 609842,
                        "field_name": "contact_name",
                        "value": "Roby Cohen"
                    },
                    {
                        "custom_field_id": 609992,
                        "field_name": "customer_123",
                        "value": "RcBuilder@walla.com"
                    },
                    {
                        "custom_field_id": 614853,
                        "field_name": "api-custom-field-2",
                        "value": "RcBuilder@walla.com"
                    },
                    {
                        "custom_field_id": 614854,
                        "field_name": "contact email",
                        "value": "RcBuilder@walla.com"
                    }
                ]
            },
            "user": {
                "id": 709618,
                "agency_id": 320554,
                "first_name": "מתן",
                "last_name": "צדקה",
                "name": "מתן צדקה",
                "full_name": "מתן צדקה",
                "email": "jackoca@gmail.com",
                "abbr": "מ",
                "phone": "+972544582610",
                "color": "#795548",
                "locale_code": "en-GB",
                "status": "ACTIVE",
                "text": "מתן צדקה",
                "is_online": 1,
                "user_status": "ONLINE",
                "chat_status": "1",
                "voip_status": "OFFLINE",
                "voip_device": "MOBILE",
                "profile_image": "https://s3.eu-central-1.amazonaws.com/trengo/media/user_232b1e84844bf69d8e3a1851f8270655.png",
                "authorization": "OWNER",
                "role": {
                    "id": 24,
                    "name": "administrator"
                },
                "is_primary": 1,
                "timezone": "Asia/Jerusalem",
                "created_at": "2023-10-13 15:32:42",
                "two_factor_authentication_enabled": false
            },
            "body": "some note 4",
            "created_at": "2024-01-02 10:00:34"
        } 
    */
    public class Note {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
    }

    public class CreateTicketRequest
    {        
        [JsonProperty(PropertyName = "contact_id")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "channel_id")]
        public int ChannelId { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        
        public CreateTicketRequest(int ContactId, int ChannelId, string Subject)
        {
            this.ContactId = ContactId;
            this.ChannelId = ChannelId;
            this.Subject = Subject;
        }
    }

    /*
        {
            "agency_id": 320554,
            "contact_id": 460022113,
            "status": "ASSIGNED",
            "user_id": 709618,
            "assigned_at": "2024-01-02T10:24:40.012331Z",
            "assigned_by": 709618,
            "channel_id": 1306497,
            "subject": "test-api-ticket",
            "updated_at": "2024-01-02T10:24:40.000000Z",
            "created_at": "2024-01-02T10:24:40.000000Z",
            "id": 731700402,
            "latest_message": "2024-01-02T10:24:40.000000Z",
            "channel": {
                "id": 1306497,
                "phone": "",
                "name": "Wa_business",
                "title": "Camel Mountain",
                "username": "+972 55-988-6383",
                "password": "134244633112334",
                "status_update": null,
                "status": "ACTIVE",
                "type": "WA_BUSINESS",
                "business_hour_id": 353489,
                "is_wa_connector": 0,
                "logo_path": null,
                "account_type": null,
                "last_login_at": null,
                "last_activity_at": null,
                "expires_at": null,
                "telegram_last_update_id": null,
                "notification_email": null,
                "is_running": 0,
                "agency_id": 320554,
                "auto_reply": "ENABLED",
                "wa_server_id": null,
                "connection_error_notification_email": null,
                "price": "15.00",
                "color": "#828216",
                "show_ticket_fields": 1,
                "show_contact_fields": 1,
                "can_be_deleted_at": null,
                "requested_by": 709618,
                "reopen_closed_ticket": 1,
                "deleted_at": null,
                "created_at": "2023-10-15T17:41:06.000000Z",
                "updated_at": "2023-10-18T17:04:35.000000Z",
                "notification_sound": "chat.mp3",
                "formatted_phone": "+",
                "password_is_null": false,
                "reassign_reopened_ticket": false,
                "reopen_closed_ticket_time_window_days": "30",
                "users": [],
                "agency": {
                    "id": 320554,
                    "name": "Camel Mountain",
                    "slug": "camel-mountain",
                    "status": "ACTIVE",
                    "trial_ends_at": null,
                    "channel_prefix": "kuh2mFNW5rxCcZUS3pQFMnreQg6ZzuMTQkZ1KW6aQCApyhVXYtP4Womzeq1enUwuQaVNFRIEbx9ii6HH6aE2VG5whUB3V5OlXbGSZq18zRLZF6oa1KYhstrkZxm2N",
                    "plan": null,
                    "subscription_started_at": null,
                    "moneybird_is_synced": 0,
                    "moneybird_contact_id": null,
                    "is_white_labelled": 0,
                    "agency_parent_id": null,
                    "price_package_a": null,
                    "price_package_b": null,
                    "price_package_c": null,
                    "locale_code": "en-GB",
                    "has_session_limit": 1,
                    "enable_whatsapp": 0,
                    "enable_bulk_sms": 0,
                    "enable_invoicing": false,
                    "add_wa_contacts": 0,
                    "deleted_at": null,
                    "created_at": "2023-10-13T13:32:42.000000Z",
                    "updated_at": "2023-10-26T13:29:51.000000Z",
                    "chargebee_customer_id": "320554",
                    "pricing_model": "seats",
                    "pricing": {
                        "A": null,
                        "B": null,
                        "C": null
                    }
                }
            },
            "agent": {
                "id": 709618,
                "agency_id": 320554,
                "parent_agency_id": null,
                "is_primary": 1,
                "title": "MR",
                "first_name": "מתן",
                "last_name": "צדקה",
                "email": "jackoca@gmail.com",
                "is_demo_user": 0,
                "is_online": 1,
                "user_status": "ONLINE",
                "authorization": null,
                "screen_lock_enabled": 0,
                "status": "ACTIVE",
                "activation_token": null,
                "requires_password_update": 0,
                "locale_code": "en-GB",
                "color": "#795548",
                "timezone": "Asia/Jerusalem",
                "profile_image": "https://s3.eu-central-1.amazonaws.com/trengo/media/user_232b1e84844bf69d8e3a1851f8270655.png",
                "voip_status": "OFFLINE",
                "voip_device": "MOBILE",
                "phone": "+972544582610",
                "created_at": "2023-10-13T13:32:42.000000Z",
                "updated_at": "2024-01-02T10:24:04.000000Z",
                "last_activity_at": "2024-01-02 10:24:04",
                "deleted_at": null,
                "rate_limit": 750,
                "role_id": 24,
                "name": "מתן צדקה",
                "abbr": "מ",
                "full_name": "מתן צדקה",
                "text": "מתן צדקה"
            },
            "team": null
        }
    */
    public class Ticket
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "contact_id")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "channel_id")]
        public int ChannelId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "assigned_by")]
        public int AssignedBy { get; set; }

        [JsonProperty(PropertyName = "assigned_at")]
        public DateTime AssignedDate { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedDate { get; set; }        
    }

    public class LabelATicketRequest {
        [JsonProperty(PropertyName = "ticket_id")]
        public int TicketId { get; set; }

        [JsonProperty(PropertyName = "label_id")]
        public int LabelId { get; set; }

        public LabelATicketRequest(int TicketId, int LabelId)
        {
            this.TicketId = TicketId;
            this.LabelId = LabelId;
        }
    }

    /*
        {
            "id": 1701915,
            "name": "Konimbo",
            "slug": "konimbo",
            "color": "#5D9CEC",
            "sort_order": null,
            "archived": null
        }
    */
    public class Label
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "slug")]
        public string Slug { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
    }

    public class CreateTicketMessageRequest
    {
        [JsonProperty(PropertyName = "ticket_id")]
        public int TicketId { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "internal_note")]
        public bool IsInternal { get; set; }

        public CreateTicketMessageRequest(int TicketId, string Message, bool IsInternal)
        {
            this.TicketId = TicketId;
            this.Message = Message;
            this.IsInternal = IsInternal;
        }
    }

    /*
       {
          "message": {
            "ticket_id": 734214490,
            "contact_id": 461518512,
            "agency_id": 320554,
            "user_id": 709618,
            "message": "NEW ORDER #14620700\n CLIENT: \u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2\n CART-TOTAL: 124",
            "type": "NOTE",
            "token": "xRhWGO5CdS",
            "body_type": "TEXT",
            "file_name": null,
            "read": true,
            "is_auto_reply": null,
            "updated_at": "2024-01-11 10:28:24",
            "created_at": "2024-01-11 10:28:24",
            "id": 3448655702,
            "message_class": "message-note",
            "email_message_class": "email_note",
            "created_at_time": 1704968904,
            "created_at_time_day": 1704931200,
            "ticket": {
              "id": 734214490,
              "agency_id": 320554,
              "channel_id": 1306497,
              "user_id": 709618,
              "team_id": null,
              "contact_id": 461518512,
              "assigned_by": 709618,
              "closed_by": null,
              "latest_message": "2024-01-11 10:28:07",
              "guid": null,
              "telegram_chat_id": null,
              "status": "ASSIGNED",
              "subject": "KONIMBO INCOMING ORDER #14620700",
              "closed_at": null,
              "assigned_at": "2024-01-11 10:28:07",
              "contact_cc": null,
              "custom_data": null,
              "created_at": "2024-01-11T10:28:07.000000Z",
              "updated_at": "2024-01-11T10:28:24.000000Z",
              "deleted_at": null,
              "channel": {
                "id": 1306497,
                "phone": "",
                "name": "Wa_business",
                "title": "Camel Mountain",
                "username": "+972 55-988-6383",
                "password": "134244633112334",
                "status_update": null,
                "status": "ACTIVE",
                "type": "WA_BUSINESS",
                "business_hour_id": 353489,
                "is_wa_connector": 0,
                "logo_path": null,
                "account_type": null,
                "last_login_at": null,
                "last_activity_at": null,
                "expires_at": null,
                "telegram_last_update_id": null,
                "notification_email": null,
                "is_running": 0,
                "agency_id": 320554,
                "auto_reply": "ENABLED",
                "wa_server_id": null,
                "connection_error_notification_email": null,
                "price": "15.00",
                "color": "#828216",
                "show_ticket_fields": 1,
                "show_contact_fields": 1,
                "can_be_deleted_at": null,
                "requested_by": 709618,
                "reopen_closed_ticket": 1,
                "deleted_at": null,
                "created_at": "2023-10-15T17:41:06.000000Z",
                "updated_at": "2023-10-18T17:04:35.000000Z",
                "notification_sound": "chat.mp3",
                "formatted_phone": "+",
                "password_is_null": false,
                "reassign_reopened_ticket": false,
                "reopen_closed_ticket_time_window_days": "30",
                "users": [
          
                ]
              },
              "custom_fields": [
        
              ]
            },
            "mentions": [
      
            ],
            "email_message": null,
            "attachments": [
      
            ],
            "contact": {
              "id": 461518512,
              "agency_id": 320554,
              "title": "MR",
              "first_name": null,
              "last_name": null,
              "full_name": "\u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2",
              "street_name": null,
              "house_no": null,
              "zip_code": null,
              "city": null,
              "whatsapp_phone_id": null,
              "phone": "+972542003091",
              "color": "#e91e63",
              "image_path": null,
              "email": null,
              "is_synced": 1,
              "custom_field_data": {
                "Customer number": "11421336",
                "phone": "972542003091",
                "contact_name": "\u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2",
                "contact email": "royva2712@icloud.com"
              },
              "created_at": "2024-01-11T10:28:05.000000Z",
              "updated_at": "2024-01-11T10:28:07.000000Z",
              "deleted_at": null,
              "deleted_by": null,
              "name": "\u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2",
              "avatar": "https:\/\/assets.trengo.com\/release\/img\/defaultpic.png",
              "profile_image": null,
              "formatted_custom_field_data": {
                "contact email": "royva2712@icloud.com",
                "api-custom-field-2": null,
                "api-custom-field-1": null,
                "customer_123": null,
                "123": null,
                "contact_name": "\u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2",
                "phone": "972542003091",
                "Customer number": "11421336"
              },
              "formatted_phone": "+972 54-200-3091",
              "abbr": "\u05e8",
              "is_phone": true,
              "identifier": "+972 54-200-3091",
              "is_private": false,
              "users": [
        
              ],
              "custom_fields": [
                {
                  "id": 609825,
                  "title": "Customer number",
                  "agency_id": 320554,
                  "sort_order": 1,
                  "type": "CONTACT",
                  "created_at": "2023-10-13T13:32:46.000000Z",
                  "updated_at": "2023-10-13T13:32:46.000000Z",
                  "identifier": "Customer-number",
                  "field_type": "TEXT",
                  "placeholder": null,
                  "meta": null,
                  "deleted_at": null,
                  "is_required": false,
                  "pivot": {
                    "contact_id": 461518512,
                    "custom_field_id": 609825,
                    "value": "11421336",
                    "created_at": "2024-01-11T10:28:06.000000Z",
                    "updated_at": "2024-01-11T10:28:06.000000Z"
                  }
                },
                {
                  "id": 609835,
                  "title": "phone",
                  "agency_id": 320554,
                  "sort_order": 0,
                  "type": "CONTACT",
                  "created_at": "2023-10-13T17:12:49.000000Z",
                  "updated_at": "2023-10-13T17:12:49.000000Z",
                  "identifier": "phone",
                  "field_type": "TEXT",
                  "placeholder": null,
                  "meta": null,
                  "deleted_at": null,
                  "is_required": false,
                  "pivot": {
                    "contact_id": 461518512,
                    "custom_field_id": 609835,
                    "value": "972542003091",
                    "created_at": "2024-01-11T10:28:06.000000Z",
                    "updated_at": "2024-01-11T10:28:06.000000Z"
                  }
                },
                {
                  "id": 609842,
                  "title": "contact_name",
                  "agency_id": 320554,
                  "sort_order": -1,
                  "type": "CONTACT",
                  "created_at": "2023-10-13T19:31:59.000000Z",
                  "updated_at": "2023-10-13T19:31:59.000000Z",
                  "identifier": "contact_name",
                  "field_type": "TEXT",
                  "placeholder": null,
                  "meta": null,
                  "deleted_at": null,
                  "is_required": false,
                  "pivot": {
                    "contact_id": 461518512,
                    "custom_field_id": 609842,
                    "value": "\u05e8\u05d5\u05d1\u05d9 \u05d1\u05d3\u05d9\u05e7\u05d5\u05ea 2",
                    "created_at": "2024-01-11T10:28:06.000000Z",
                    "updated_at": "2024-01-11T10:28:06.000000Z"
                  }
                },
                {
                  "id": 614854,
                  "title": "contact email",
                  "agency_id": 320554,
                  "sort_order": -6,
                  "type": "CONTACT",
                  "created_at": "2023-12-30T19:37:53.000000Z",
                  "updated_at": "2023-12-30T19:37:53.000000Z",
                  "identifier": "contact-email",
                  "field_type": "TEXT",
                  "placeholder": null,
                  "meta": null,
                  "deleted_at": null,
                  "is_required": false,
                  "pivot": {
                    "contact_id": 461518512,
                    "custom_field_id": 614854,
                    "value": "royva2712@icloud.com",
                    "created_at": "2024-01-11T10:28:07.000000Z",
                    "updated_at": "2024-01-11T10:28:07.000000Z"
                  }
                }
              ]
            },
            "agent": {
              "id": 709618,
              "agency_id": 320554,
              "parent_agency_id": null,
              "is_primary": 1,
              "title": "MR",
              "first_name": "\u05de\u05ea\u05df",
              "last_name": "\u05e6\u05d3\u05e7\u05d4",
              "email": "jackoca@gmail.com",
              "is_demo_user": 0,
              "is_online": 1,
              "user_status": "ONLINE",
              "authorization": null,
              "screen_lock_enabled": 0,
              "status": "ACTIVE",
              "activation_token": null,
              "requires_password_update": 0,
              "locale_code": "en-GB",
              "color": "#795548",
              "timezone": "Asia\/Jerusalem",
              "profile_image": "https:\/\/s3.eu-central-1.amazonaws.com\/trengo\/media\/user_232b1e84844bf69d8e3a1851f8270655.png",
              "voip_status": "OFFLINE",
              "voip_device": "MOBILE",
              "phone": "+972544582610",
              "created_at": "2023-10-13T13:32:42.000000Z",
              "updated_at": "2024-01-11T10:28:02.000000Z",
              "last_activity_at": "2024-01-11 10:28:02",
              "deleted_at": null,
              "rate_limit": 750,
              "role_id": 24,
              "name": "\u05de\u05ea\u05df \u05e6\u05d3\u05e7\u05d4",
              "abbr": "\u05de",
              "full_name": "\u05de\u05ea\u05df \u05e6\u05d3\u05e7\u05d4",
              "text": "\u05de\u05ea\u05df \u05e6\u05d3\u05e7\u05d4"
            },
            "contacts": [
      
            ]
          }
        }
    */
    public class TicketMessageResponse {
        [JsonProperty(PropertyName = "message")]
        public TicketMessage Message { get; set; }
    }

    public class TicketMessage
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "ticket_id")]
        public long TicketId { get; set; }
    }
}
