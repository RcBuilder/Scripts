using CommonEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;


namespace CommonEntities.BlackBoardPartnerCloudEntities
{
    #region PartnerCloudHeader
    public class PartnerCloudHeader
    {
        public const string BlackboardSite = "X-Blackboard-Site";

        //The Bb site ID for the LMS instance. The site ID is the consumer key for the LMS instance in the Partner Cloud. It is the same value which is passed to the partner application in the custom_source_tool_consumer_guid parameter with every LTI launch. This header is included in every web service request.
        public const string SourceToolConsumerGuid = "X-Partners-SourceToolConsumerGuid";

        public const string Lms = "X-Partners-Lms";

        //The user’s Bb profile ID. This is the same value which is passed to the partner application in the LTI parameter user_id. This header is present in any web service call which is executed on behalf of a particular user (i.e. any non-system web service call).
        public const string UserId = "X-Partners-UserId";

        //The user’s UUID. This same value which is passed to the partner application in the custom LTI parameter custom_user_uuid. This header is present in any web service call which is executed on behalf of a particular user (i.e. any non-system web service call).
        public const string UserUuid = "X-Partners-UserUuid";

        //The user’s normalized Bb course role. The user’s course role is normalized to the equivalent LIS context role based on the role mapping rules which have been configured for the partner and the institution. This is the same value which is passed to the partner application as part of the roles parameter in LTI launches. This header is present in any web service call which is executed on behalf of a particular user (i.e. any non-system web service call).
        public const string UserContextRole = "X-Partners-UserContextRole";

        //A comma delimited list of the user’s normalized Bb institution roles. The user’s institution role(s) are normalized to their equivalent LIS institution roles based on the role mapping rules which have been configured for the partner and the institution. These are the same role(s) which are passed to the partner application as part of the roles parameter in LTI launches. This header is present in any web service call which is executed on behalf of a particular user (i.e. any non-system web service call).
        public const string UserInstitutionRoles = "X-Partners-UserInstitutionRoles";

        //The tenant identifier for the current Bb course. This information can be used by the partner application to support an institution based licensing model. This is the same value which is passed to the partner application in the custom_tenant_identifier parameter for LTI launches. This parameter is only included if ALL of the following criteria are met:
        //•	The institution uses one of the Partner Cloud supported methods for segmenting their institution. The supported methods: domains, institutional hierarchy and data source keys.
        //•	The Bb administrator for the institution has configured the Partner Cloud B2 to enable multi-institution support.
        //•	The web service request was executed on behalf of a user and within a Bb course.
        public const string TenantIdentifier = "X-Partners-TenantIdentifier";

        //A brand parameter can be passed to the partner application to allow the partner to customize the end-user experience and web service results. It is the same value, which is passed to the partner application in the custom_brand parameter for LTI launches. The brand parameter is configured at partner level. If a brand is configured for this partner this header is included in every web service request.
        public const string Brand    = "X-Partners-Brand";

        //In order to support clients who want to migrate from one of our legacy integrations (Pearson, WileyPLUS, Cengage) we include the legacy userId.
        // example - Encrypted batchUid for the user
        public const string LegacyUserId = "X-Partners-LegacyUserId";

        //In order to support clients who want to migrate from using one of our legacy integrations (Pearson, WileyPLUS, Cengage) we include the legacy contextId. 
        //example - Encrypted batchUid for the course
        public const string LegacyContextId = "X-Partners-LegacyContextId";

        //This is an example of a custom parameter which could be configured by either the Bb Cloud administrator or the LMS administrator for a specific institution. Any custom parameter which applies to the specific association will be included as a header in every web service request.
        // NOTE – Since custom parameters are entered by an administrator and since the header name is created to match the custom parameter name (minus the “custom_”) there is no prefix which identifies or groups those headers. 
        public const string license_key = "license_key";
    }
    #endregion

    #region validateConfiguration

    //Note the addition of JsonProperty attributes on some of the fields - explicitly states which fields are serialized by the newtonSoft json serializer, as oppose to the DataContractSerializer.
    //example - {"@type":"Configuration","@id":"blackboard","installationType":"Test","instanceGuid":"partner-cloud-service","ltiUrl":"https://login-aws-qa.mhcampus.com/SSO/DI/Blackboard/LtiHandler.aspx","override":true,"toolConsumerUrl":"https://api-beta02.cloud.bb/v1/cp/ws/"}
    [Serializable]
    [DataContract(Name = "ValidateConfiguration", Namespace = ServiceMetaData.NameSpace)]
    public class ValidateConfiguration
    {
        //[DataMember(Name = "context", IsRequired = false)]
        //public validateConfigurationContext context;

        [JsonProperty]
        [DataMember(Name = "@id")]
        public string id; // consumer key - isn't it redundant with the X headers/

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; } // should be Configuration

        [JsonProperty]
        [DataMember(Name = "instanceGuid")]
        public string instanceGuid; // is that the school identifier when the call is made from an LMS? - save the value for mapping to customer id? Is the cloud level value fixed?

        [JsonProperty]
        [DataMember(Name = "installationType")]
        public string installationType; // { Development, Test, Stage, Production } - save it in our DB - where?

        [JsonProperty]
        [DataMember(Name="lms")]
        public string lms;

        [JsonProperty]
        [DataMember(Name="locale")]
        public string locale;

        [JsonProperty]
        [DataMember(Name = "ltiUrl")]
        public string ltiUrl_ofMhcampus; // LTI end-point for the partner application - is that on a school/LMS level - save it in our DB - where?

        [JsonProperty]
        [DataMember(Name="market")]
        public List<string> market;

        [JsonProperty]
        [DataMember(Name = "publicUri")]
        public UriEntity publicUriOfSchool; // what is the differcne with ltiUrl?

        [DataMember(Name = "override")]
        public bool allowOverride; // if true override instanceGuid validation - BB - do we need to do something with it?

        [JsonProperty]
        [DataMember(Name = "clientId")]
        public string clientId; // hashed version of the Bb client ID - BB - do we need it for some reason?

        [JsonProperty]
        [DataMember(Name="clientLearnVersion")]
        public string clientLearnVersion;

        [JsonProperty]
        [DataMember(Name = "clientName")]
        public string clientName; // what is a client in BB? is that the school

        //[DataMember(Name = "TenantList")]
        //public TenantList TenantList; // do we care about it?

        [JsonProperty]
        [DataMember(Name="toolConsumerUrl")]
        public string toolConsumerUrl;  //the url of the partner cloud servers (not the url of the school though - with blackboard there is a difference)
    }

    [Serializable]
    [DataContract]
    public class validateConfigurationContext
    {
        [DataMember(Name = "Configuration")]
        public string Configuration;

        [DataMember(Name = "TenantList")]
        public string TenantListUri;

        [DataMember(Name = "Tenant")]
        public string TenantUri;

        [DataMember(Name = "Uri")]
        public string Uri;
    }

    [Serializable]
    [DataContract(Name = "validateConfigurationResponse", Namespace = ServiceMetaData.NameSpace)]
    public class validateConfigurationResponse
    {
        [DataMember(Name = "context")]
        public validateConfigurationContext context;

        [DataMember(Name = "@id")]
        public string id;

        [DataMember(Name = "@type")]
        public string type;

        [DataMember(Name = "status")]
        public string __status
        {
            set
            {
                try
                {
                    this.status = (status)Enum.Parse(typeof(status), value, true);
                }
                catch
                {
                    this.status = default(status);
                }
            }
            get { return this.status.ToString(); }
        }
        
        public status status { get; set; }
        
        [DataMember(Name = "message")]
        public string message;

        [DataMember(Name = "allowOverride")]
        public bool allowOverride;
    }

    #endregion


        
    #region Provisioning
    [Serializable]
    [DataContract(Name = "ProvisioningInfoResponse", Namespace = ServiceMetaData.NameSpace)]
    public class ProvisioningInfoResponse
    {
        [DataMember(Name = "@context")]
        public ProvisioningInfoContext context;

        [DataMember(Name = "@id")]
        public string id; // TP enrollment ID; not used

        [DataMember(Name = "@type")]
        public string type; //"Binding" value

        [DataMember(Name = "contextId")]
        public string contextId; // "opaque context ID",

        [DataMember(Name = "userId")]
        public string userId; //"Bb profile ID",

        [DataMember(Name = "localUserId")]
        public string localUserId; //"local user identifier in the tool provider" // null if unknown to TP 

        [DataMember(Name = "localUserName")]
        public string localUserName; // "local display name for the user in the tool provider", // null if unknown to TP 

        [DataMember(Name = "localContextId")]
        public string localContextId; // "local course identifier in the tool provider", // null if the context ID is unknown to TP. If TP does not encapsulate the concept
                                                                // or a course/context they should respond with the string "NOT_APPLICABLE". This 
                                                                // indicates that there is nothing to provisioned in the TP.

        [DataMember(Name = "localContextName")]
        public string localContextName; // "local display name for the course in the tool provider", // null if unknown to TP

        [DataMember(Name = "localMembership")]
        public string localMembership; // "LIS normalized role for the user in the tool provider course/context", // { null, "Learner", "Instructor", "NOT_APPLICABLE" }

        //[DataMember(Name = "name")]
        //public List<ProvisionObject> localContextProductCodes;

    }
        
    [Serializable]
    [DataContract]
    public class InfoContext
    {
        [DataMember(Name = "ToolProviderProfileUri")]
        public string ToolProviderProfileUri;

        [DataMember(Name = "PremierProviderProfileUri")]
        public string PremierProviderProfileUri;

        [DataMember(Name = "Uri")]
        public string Uri;
    }
    [Serializable]
    [DataContract]
    public class ProvisioningInfoContext
    {
        [DataMember(Name = "Binding")]
        public string Binding;

        [DataMember(Name = "ProductCode")]
        public string ProductCode;
    }

    [Serializable]
    [DataContract]
    public class PremierProviderProfile
    {
        [DataMember(Name = "id")]
        public string id;

        [DataMember(Name = "type")]
        public string type;

        [DataMember(Name = "learnMoreUri")]
        public UriEntity learnMoreUri;

        [DataMember(Name = "selectContentImageUri")]
        public UriEntity selectContentImageUri;

        [DataMember(Name = "toolPanelImageUri")]
        public UriEntity toolPanelImageUri;
    }
    #endregion

    #region GetView
    [DataContract]
    public class GetViewResponse 
    {
        [DataMember(Name = "@context")]
        public GetViewContext Context { set; get; }

        [DataMember(Name = "@id")] 
        public string id;

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }

        [DataMember(Name = "title")]
        public string title;

        [DataMember(Name = "description")]
        public string description;

        [DataMember(Name = "providerName")]
        public string providerName;

        [DataMember(Name = "components")]
        public List<Component> components { get; set; }

        [DataMember(Name = "properties")]
        public ViewProperties properties;

        [DataMember(Name = "allowHtml")]
        public bool allowHtml;
    }

    [DataContract]
    public class GetViewContext
    {
        [DataMember(Name = "View")]
        public string view { set; get; }

        [DataMember(Name = "Component")]
        public string component { set; get; }

        [DataMember(Name = "Uri")]
        public string uri { set; get; }
    }

    [DataContract]
    public class ViewProperties
    {
        [DataMember(Name = "template")]
        public string __Template
        {
            set
            {
                try
                {
                    this.Template = (eTemplate)Enum.Parse(typeof(eTemplate), value, true);
                }
                catch
                {
                    this.Template = default(eTemplate);
                }
            }
            get { return this.Template.ToString(); }
        }
        public eTemplate Template { get; set; }
    }

    [DataContract]
    public enum eTemplate
    {
        NULL, 
        LIST_VIEW, 
        TABLE_VIEW, 
        TOC, 
        THUMBNAIL_TOC, 
        SEARCH, 
        DELEGATED_DISCOVERY 
    }

    /*
    [DataContract]
    public class GetViewItem
    {
        [DataMember(Name = "@id")]  
        public string id; 

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type = eResourceType.Component;

        [DataMember]
        public string title;

        [DataMember]
        public string description;

        [DataMember]
        public UriEntity targetUri;

        [DataMember(Name = "placement")]
        public string __placement
        {
            set
            {
                try
                {
                    this.placement = (ePlacement)Enum.Parse(typeof(ePlacement), value, true);
                }
                catch
                {
                    this.placement = default(ePlacement);
                }
            }
            get { return this.placement.ToString(); }
        }
        public ePlacement placement { set; get; }

        [DataMember]
        public bool allowHtml;
    }
    */

    public enum ePlacement
    {
        #region View
        [EnumMember]
        BUTTON,
        [EnumMember]
        LINK,
        [EnumMember]
        MENU_ITEM,
        [EnumMember]
        SEARCH_BAR,
        [EnumMember]
        TECH_SUPPORT_LINK,
        [EnumMember]
        TEST_LAUNCH_LINK,
        #endregion

        #region Tool
        [EnumMember]
        TOOL_ALL,
        [EnumMember]
        TOOL_ROLE
        #endregion
    }

    #endregion 

    #region GetTools

    [Serializable]
    [DataContract]
    public class GetToolsResponse
    {
        [DataMember(Name = "@context")]
        public GetToolsContext Context { set; get; }

        [DataMember(Name = "@id")] 
        public string componentListId;

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }

        [DataMember(Name="contextId")]
        public string contextId {get; set;}

        [DataMember(Name = "components")]
        public List<Component> components { get; set; }

    }

    [Serializable]
    [DataContract]
    public class GetToolsContext
    {
        [DataMember(Name = "ComponentList")]
        public string ComponentList { set; get; }

        [DataMember(Name = "Component")]
        public string Component { set; get; }

        [DataMember(Name = "Uri")]
        public string Uri { set; get; }
    }

   
    #endregion

    #region GetContent
    [DataContract]
    public class GetContentResponse
    {
        [DataMember]
        public GetContentContext context { get; set; }
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }
        [DataMember]
        public List<BlackboardAssignment> items { get; set; }
    }

    [DataContract]
    public class GetContentRequest
    {
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }
        [DataMember]
        public List<string> resourceIds { get; set; }
    }

    [DataContract]
    public class GetContentContext {
        [DataMember(Name = "ItemList")]
        public string ItemListURI { get; set; }
        [DataMember(Name = "Item")]
        public string ItemURI { get; set; }
        [DataMember]
        public string URI { get; set; }
    }

    ///public class GetContentResponseItem { }
    #endregion

    #region GetUpdates
    [DataContract]
    public class GetUpdatesResponse
    {
        [DataMember(Name = "@context")]
        public GetUpdatesContext context { get; set; }
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { get; set; }
        [DataMember(Name="updates")]
        public List<GetUpdatesItem> items { get; set; }

        public GetUpdatesResponse() {
            this.items = new List<GetUpdatesItem>();
        }
    }

    [DataContract]
    public class GetUpdatesContext
    {
        [DataMember(Name = "Updates")]
        public string UpdatesURI { get; set; }
        [DataMember(Name = "ItemList")]
        public string ItemListURI { get; set; }
        [DataMember(Name = "Item")]
        public string ItemURI { get; set; }
        [DataMember(Name = "Uri")]
        public string URI { get; set; }
        [DataMember(Name = "ResultList")]
        public string ResultListURI { get; set; }
        [DataMember(Name = "Result")]
        public string ResultURI { get; set; }
    }

    [DataContract]
    public class GetUpdatesItem
    {
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }
        [DataMember]
        public string contextId { get; set; }
        [DataMember]
        public List<GetUpdatesAssignmentItem> items { get; set; }

        public GetUpdatesItem() {
            this.items = new List<GetUpdatesAssignmentItem>();
        }
    }    

    [DataContract]
    public class GetUpdatesAssignmentItem : BlackboardAssignment
    {
        [DataMember(Name = "resultList")]
        public GetUpdatesResult results { get; set; }
    }

    [DataContract]
    public class GetUpdatesResult
    {
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type
        {
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }
        [DataMember(Name="results")]
        public List<BlackboardGrade> grades { get; set; }

        public GetUpdatesResult() {
            this.grades = new List<BlackboardGrade>();
        }
    } 
    #endregion

    #region DeploymentStatus

    public class ContentStatus
    {
        //"@context":{ "Content.Status": "http://www.blackboard.com/lms-integration/1.0/#Content.Status" }
        [DataMember(Name = "@context")]
        public ContentStatusContext Context;
            
        // "@id": "request ID",
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        // "@type": "Content.Status",
        [DataMember(Name = "@type")]
        public string type { get; set; } 

        // "resourceIds": [ "resource ID", "resource ID" ],
        [DataMember]
        public List<string> resourceIds { get; set; }

        // "status": "EXISTS", //
        // { "EXISTS", "DELETED" }
        public enum eStatus { EXISTS, DELETED }

        [DataMember(Name = "status")]
        public string __status
        {

            get
            {
                return Status.ToString();
            }
            set
            {
                try
                {
                    this.Status = (eStatus) Enum.Parse(typeof(eStatus), value);
                }
                catch { }
            }
        }
        public eStatus Status { get; set; }


        // "allItems": true // indicates whether this request contains all items deployed to this course; used to resynchronize course
        [DataMember(Name="allItems")]
        public bool AllItems { get; set; }

            
            
    }

    [Serializable]
    [DataContract]
    public class ContentStatusContext
    {
        [DataMember(Name = "Content.Status")]
        public const string contentStatusSchemaUri = "http://www.blackboard.com/lms-integration/1.0/#Content.Status";
    }
    #endregion

    #region UpdateConfirmation

    [Serializable]
    [DataContract]
    public class UpdateConfirmationContext
    {
        [DataMember(Name = "Updates")]
        public const string updates = "http://www.blackboard.com/lms-integration/1.0/#UpdateConfirmation";

        [DataMember(Name = "ItemList")]
        public const string itemList = "http://www.blackboard.com/lms-integration/1.0/#ItemList";

        [DataMember(Name = "Item")]
        public const string item = "http://www.blackboard.com/lms-integration/1.0/#Item";

        [DataMember(Name = "ResultList")]
        public const string resultList = "http://www.blackboard.com/lms-integration/1.0/#ResultList";

        [DataMember(Name = "Result")]
        public const string result = "http://www.blackboard.com/lms-integration/1.0/#Result";

    }

    [DataContract]
    public class ConfirmationUpdates
    {
        //"@id": "88888888", - customer id (instance Guid value)
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        //"@type": "ItemList",
        [DataMember(Name = "@type")]
        public const string type = "ItemList";

        [DataMember(Name = "contextId")]
        public string contextId; // Blackboard course Id

        [DataMember(Name = "items")]
        public List<ConfirmationUpdatesItem> items;
    }

    [DataContract]
    public class ConfirmationUpdatesItem
    {
            
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        [DataMember(Name = "@type")]
        public const string type = "Item";

        [DataMember(Name = "resultList")] 
        public GetUpdatesResult resultList;
    }

    [DataContract]
    public class UpdateConfirmationRequest
    {
        [DataMember(Name = "@context")]
        public UpdateConfirmationContext Context;

        // "@id": "1234567890", // consumer key
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        //"@type": "UpdateConfirmation",
        [DataMember(Name = "@type")] 
        public const string type = "UpdateConfirmation";

        [DataMember(Name = "updates")]
        public List<ConfirmationUpdates> updates { get; set; } 
        
    }

#endregion UpdateConfirmation

    #region GetAvailableContent
    [DataContract]
    public class GetAvailableContentResponse
    {
        [DataMember(Name = "@context")]
        public GetAvailableContentContext context { get; set; }
        [DataMember(Name = "@id")]
        public string Id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { get; set; }

        [DataMember(Name = "contextId")]
        public string ContextId { get; set; }        

        [DataMember(Name = "items")]
        public List<AvailableContentItem> Items { get; set; }

        public GetAvailableContentResponse() {
            this.Items = new List<AvailableContentItem>();
        }
    }

    [DataContract]
    public class GetAvailableContentContext
    {
        [DataMember(Name = "ItemList")]
        public string ItemListURI { get; set; }
        [DataMember(Name = "Item")]
        public string ItemURI { get; set; }
        [DataMember(Name = "Uri")]
        public string URI { get; set; }
    }

    [DataContract]
    public class AvailableContentItem {
        [DataMember(Name = "@id")]
        public string Id;

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type = eResourceType.Item;

        [DataMember(Name = "title")]
        public string Title;

        [DataMember(Name = "description")]
        public string Description;

        [DataMember(Name = "targetUri")]
        public UriEntity TargetUri;

        [DataMember(Name = "additionalInfo")]
        public string AdditionalInfo { set; get; }

        [DataMember(Name = "category")]
        public string Category { set; get; }

        [DataMember(Name = "dueDate")]
        public string __dueDate
        {
            get
            {
                var strDueDate = DueDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
                return strDueDate.Remove(strDueDate.LastIndexOf(':'), 1);
            }
            set
            {
                try
                {
                    this.DueDate = DateTime.Parse(value, CultureInfo.GetCultureInfo("en-US"));
                }
                catch { }
            }
        }
        public DateTime DueDate { get; set; }

        [DataMember(Name = "pointsPossible")]
        public float PointsPossible { get; set; }

        [DataMember(Name = "custom")]
        public customParameterMap Properties { set; get; }

        [DataMember(Name = "children")]
        public List<AvailableContentItem> Children { set; get; }

        public AvailableContentItem() {
            Children = new List<AvailableContentItem>();
        }
    }
    #endregion



    #region SHARED
    [DataContract]
    public class UriEntity
    {
        [DataMember(Name = "@id")]
        public string id;
        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                try
                {
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch
                {
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set; get; }
        [DataMember(Name = "scheme")]
        public string __scheme
        {
            set
            {
                try
                {
                    this.scheme = (eUriScheme)Enum.Parse(typeof(eUriScheme), value, true);
                }
                catch
                {
                    this.scheme = default(eUriScheme);
                }
            }
            get { return this.scheme.ToString(); }
        }
        public eUriScheme scheme { set; get; }
        [DataMember]
        public string uri;
    }
 
    [Serializable]
    [DataContract]
    public class Component
    {
        [DataMember(Name = "@id")]  
        public string id;       

        [DataMember(Name = "title")]
        public string title;    

        [DataMember(Name = "description")]
        public string description;

        [DataMember(Name = "@type")]
        public string __type
        {
            set
            {
                this.type = eResourceType.Component;
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { set { } get { return eResourceType.Component; } }

        [DataMember(Name = "documentTarget")]
        public string __documentTarget
        {
            set
            {
                try
                {
                    this.documentTarget = string.IsNullOrWhiteSpace(value) ? (eDocumentTarget?) null : (eDocumentTarget?)Enum.Parse(typeof(eDocumentTarget), value, true);
                }
                catch
                {
                    this.documentTarget = null;
                }
            }
            get { return this.documentTarget.ToString(); }
        }
        public eDocumentTarget? documentTarget;   

        [DataMember(Name = "targetUri")]
        public UriEntity targetUri;
    
        [DataMember(Name = "iconUri")]
        public UriEntity iconUri;

        [DataMember(Name = "placement")]
        public string __placement
        {
            set
            {
                try
                {
                    this.placement = (ePlacement)Enum.Parse(typeof(ePlacement), value, true);
                }
                catch
                {
                    this.placement = default(ePlacement);
                }
            }
            get { return this.placement.ToString(); }
        }
        public ePlacement placement;

        [DataMember(Name = "allowHtml")]
        public bool allowHtml;

        [DataMember(Name = "custom")]
        public customParameterMap customPropertiesCollection; //Optional Optional Property	map
    }
        
    [DataContract]
    public class customParameterMap
    {
        [DataMember(Name = "tool_id")]
        public string ToolId;
    }

    [DataContract(Name = "status")]
    public enum status
    {
        [EnumMember]
        OK,
        [EnumMember]
        ERROR,
    }

    public enum eResourceType { NULL, Request, ItemList, Item, Uri, Updates, ResultList, Result, View, Component, ComponentList, Configuration }
    public enum eUriScheme { NULL, ACTION, B2, URL, HTTP, HTTPS }
    public enum eDocumentTarget { FRAME, WINDOW, IFRAME }
    public enum eGradingSchema { SCORE, PERCENT }
    #endregion
}
