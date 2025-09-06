using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using PuppeteerManager;
using System.Web;

// TODO ->> RESEARCH & IMPLEMETATION!
/*
    REFERENCE
    ---------    
    https://github.com/facebook-csharp-sdk/facebook-csharp-sdk
    https://developers.facebook.com/docs/marketing-api/reference/
    https://developers.facebook.com/tools/explorer/    
    https://developers.facebook.com/docs/facebook-login/guides/access-tokens/#apptokens   
    https://developers.facebook.com/docs/graph-api/overview

    https://developers.facebook.com/docs/marketing-api/reference/
    https://developers.facebook.com/docs/marketing-api/reference/ad-account/campaigns/
    https://developers.facebook.com/docs/marketing-api/reference/ad-account/adsets/
    https://developers.facebook.com/docs/marketing-api/reference/ad-account/ads/
    https://developers.facebook.com/docs/marketing-api/reference/ad-account/adcreatives/
    https://developers.facebook.com/docs/marketing-api/reference/ad-campaign-group/

    https://developers.facebook.com/docs/marketing-api/audiences/reference/advanced-targeting
    https://developers.facebook.com/docs/marketing-api/bidding/overview/bid-strategy/
    https://developers.facebook.com/docs/marketing-api/bidding/guides/cost-per-action-ads/
    https://developers.facebook.com/docs/marketing-api/audiences/reference/placement-targeting/

    https://www.facebook.com/groups/fbdevelopers/
    https://developers.facebook.com/docs/
    https://developers.facebook.com/blog/
    https://developers.facebook.com/social-technologies/products
    https://developers.facebook.com/quickstarts/

    https://www.youtube.com/watch?si=DSuQCMqgw1p0vdzT&v=0KTgD7QNGA0&feature=youtu.be

    // preview
    https://developers.facebook.com/docs/marketing-api/generatepreview/v22.0
    https://developers.facebook.com/docs/marketing-api/ad-preview-plugin/v22.0

    // targeting
    https://developers.facebook.com/docs/marketing-api/audiences/reference/basic-targeting/ 

    // advanced-targeting
    https://developers.facebook.com/docs/marketing-api/audiences/reference/advanced-targeting/

    // custom-audience
    https://developers.facebook.com/docs/marketing-api/reference/custom-audience

    // behaviors
    https://developers.facebook.com/docs/marketing-api/audiences/reference/basic-targeting/#behaviors 

    PROCESS
    -------
    (steps)
    1. Go to Developers Console 
       https://developers.facebook.com
    2. Create/Select your App
       https://developers.facebook.com/apps
    3. Add "Marketing API" product
    4. 
    5.

    https://elfsight.com/blog/how-to-get-facebook-access-token/
    https://developers.facebook.com/docs/facebook-login/guides/access-tokens/get-long-lived/
    ACCESS-TOKEN
    ------------
    Short-Lived access-token  (1–2 hours)
    Long-Lived access-token // generated from a Short-Lived access-token  (usually 60 days)

    generate a token via the api explorer:
    1. go to https://developers.facebook.com/tools/explorer
    2. choose permissions 
    3. click on 'Generate Access Token' 
       note that the generated token is of type 'Short-Lived access-token'
    4. convert it into 'Long-Lived access-token'

    IMPLEMENTATIONS
    ---------------    
    see 'FialkovDigital'

    SANDBOX
    -------
    // Graph API Explorer
    https://developers.facebook.com/tools/explorer/

    POSTMAN
    -------
    see 'Facebook API.postman_collection.json'

    NUGET
    -----
    https://www.nuget.org/packages/facebook 
    > Install-Package Facebook    

    LOCALE
    ------
    parameters:
    { "locale", "en_US" } // responses in english

    USING
    -----
    var fbManager = new FacebookManager(
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            "334712864258367"
        );
            
    var createdCampaignId = "";
    var campaignName = "TEST_API_CAMPAIGN_1";

    var campaign = await fbManager.FindCampaign(campaignName);            
    if (campaign != null)
        createdCampaignId = campaign.Id;
    else
        createdCampaignId = await fbManager.CreateCampaign(new FacebookEntities.Campaign
        {
            Name = campaignName,
            Status = FacebookEntities.eCampaignStatus.PAUSED,
            Budget = 1
        });
    Console.WriteLine($"Campaign #{createdCampaignId} CREATED");
    --
    var createdAdGroupId = await fbManager.CreateAdGroup(new FacebookEntities.AdGroup
    {
        Name = "ADGROUP_2",
        CampaignId = "6647585796200",
        BillingEvent = FacebookEntities.eBillingEvents.IMPRESSIONS,
        OptimizationGoal = FacebookEntities.eOptimizationGoals.REACH,
        Status = FacebookEntities.eAdGroupStatus.PAUSED,
        BidAmount = 10
    });
    Console.WriteLine($"AdGroup #{createdAdGroupId} CREATED");
    --      
    var createdAdCreativeId = await fbManager.CreateAdCreative(new FacebookEntities.AdCreative
    {
        Name = "AD_CREATIVE_1",
        PageId = "499409633258495",  
        Link = "https://example.com",
        ImageURL = "https://fastly.picsum.photos/id/21/3008/2008.jpg?hmac=T8DSVNvP-QldCew7WD4jj_S3mWwxZPqdF0CNPksSko4",
        Message = "Check out our latest products!",
        CallToActionType = "LEARN_MORE"
    });
    Console.WriteLine($"AdCreative #{createdAdCreativeId} CREATED");
    --
    var createdAdId = await fbManager.CreateAd(new FacebookEntities.Ad
    {
        AdGroupId = "6647958249800",
        AdCreativeId = createdAdCreativeId,
        Name = $"Ad_{DateTime.Now.Ticks}"
    });
    Console.WriteLine($"Ad #{createdAdId} CREATED");
    --
    var campaigns = await fbManager.GetCampaigns();
    foreach (var c in campaigns)
        Console.WriteLine(c.Name);
    --
    var adgroups = await fbManager.GetAdGroups("6647585796200");
    foreach (var a in adgroups)
        Console.WriteLine(a.Name);
    --
    var ads = await fbManager.GetAds("6647958249800");
    foreach (var a in ads)
        Console.WriteLine(a.Name);
    --
    var campaign = await fbManager.GetCampaign("6647585796200");            
    Console.WriteLine(campaign.Name);
    --
    var adGroup = await fbManager.GetAdGroup("6647958249800");
    Console.WriteLine(adGroup.Name);
    --
    var ad = await fbManager.GetAd("6648178705000");
    Console.WriteLine(ad.Name);
    --
    var ads1 = await fbManager.GetAds("6663477724800");  // By AdGroup/ AdSet
    foreach (var ad in ads1) Console.WriteLine($"#{ad.Id} {ad.Name}");

    var ads2 = await fbManager.GetAdsByCampaign("6663477722400");  // By Campaign
    foreach (var ad in ads2) Console.WriteLine($"#{ad.Id} {ad.Name}");

    var ads3 = await fbManager.GetAdsByAccount();  // By Account
    foreach (var ad in ads3) Console.WriteLine($"#{ad.Id} {ad.Name}");
    --

    -------
    
    [Sample Usage]

    var range = await sheetsServiceManager.GetRangeValues("Ads", "A", "T", 5);
    var campaignsData = range.ConvertToModel<FacebookCampaignMetaData>();

    ///foreach (var c in campaignsData) await CreateCampaign(c);
    await CreateCampaign(campaignsData.FirstOrDefault());

    async Task CreateCampaign(FacebookCampaignMetaData metadata) {
        if (metadata == null) return;

        Console.WriteLine(metadata);

        ////var createdCampaignId = "6659296565000";
        var createdCampaignId = await fbManager.CreateCampaign(new FacebookEntities.Campaign
        {
            Name = $"[RC-TEST-3] FD - Sales - Purchase - {metadata.Keyword} - {metadata.Publisher}FB",
            Status = FacebookEntities.eCampaignStatus.PAUSED,
            Objective = FacebookEntities.eObjective.OUTCOME_SALES,
            SpecialAdCategories = FacebookEntities.eSpecialAdCategories.NONE
        });
        Console.WriteLine($"Campaign #{createdCampaignId} CREATED");

        if (string.IsNullOrEmpty(createdCampaignId))
            throw new Exception("ERROR: CreateCampaign");

        var createdAdGroupId = await fbManager.CreateAdGroup(new FacebookEntities.AdGroup
        {
            Name = $"{metadata.Targeting} - {metadata.Country} - Age{metadata.MinAge} - {metadata.MaxAge} - {metadata.BidStrategy}{metadata.Value}%",
            CampaignId = createdCampaignId,                    
            BidStrategy = FacebookEntities.eBidStrategy.LOWEST_COST_WITH_MIN_ROAS,  // TODO ->> MAP metadata
            BillingEvent = FacebookEntities.eBillingEvents.IMPRESSIONS,
            Status = FacebookEntities.eAdGroupStatus.PAUSED,
            OptimizationGoal = FacebookEntities.eOptimizationGoals.VALUE,  // ROAS
            BudgetInCnt = 700,  // 7$
            BidValueInCnt = 200,  // 2$
            PixelId = "418988888234852", // TODO ->> PixelId
            TargetingData = new FacebookEntities.TargetingData { 
                Countries = new[] { metadata.Country },
                FromAge = metadata.MinAge.Value,
                ToAge = metadata.MaxAge.Value,                        
                Platforms = metadata.Platforms.Split(','),      // metadata.Platforms.Split(',')  | new[] { "facebook", "instagram" }
                Placements = metadata.Placements.Split(','),    // metadata.Placements.Split(',')  | new[] { "facebook feed", "instagram feed" }
                Genders = metadata.Gender.Split(','),           // metadata.Genders.Split(',') | new[] { "female", "male" }
                Devices = metadata.Devices.Split(',')           // metadata.Devices.Split(',') | new[] { "mobile" }
            }
        });
        Console.WriteLine($"AdGroup #{createdAdGroupId} CREATED");

        if (string.IsNullOrEmpty(createdAdGroupId))
            throw new Exception("ERROR: CreateAdGroup");

        var createdAdCreativeId = await fbManager.CreateAdCreative(new FacebookEntities.AdCreative
        {
            Name = $"Creative{metadata.No} - {metadata.Keyword}",
            PageId = "499409633258495", // TODO ->> Config? metadata? 
            Link = "https://example.com",
            ImageURL = "https://fastly.picsum.photos/id/21/3008/2008.jpg?hmac=T8DSVNvP-QldCew7WD4jj_S3mWwxZPqdF0CNPksSko4",
            Message = "Check out our latest products!",
            CallToActionType = "LEARN_MORE"
        });
        Console.WriteLine($"AdCreative #{createdAdCreativeId} CREATED");

        if (string.IsNullOrEmpty(createdAdCreativeId))
            throw new Exception("ERROR: CreateAdCreative");

        var createdAdId = await fbManager.CreateAd(new FacebookEntities.Ad
        {
            AdGroupId = createdAdGroupId,
            AdCreativeId = createdAdCreativeId,
            Name = $"AD{metadata.No.ToString("D2")} - {metadata.Keyword}"
        });
        Console.WriteLine($"Ad #{createdAdId} CREATED");

        if (string.IsNullOrEmpty(createdAdId))
            throw new Exception("ERROR: CreateAd");

        /// await fbManager.PauseCampaign(createdCampaignId);
    }
}
*/

namespace FacebookAPI
{
    public class FacebookEntities
    {
        public enum eCampaignStatus { ACTIVE, PAUSED, DELETED, ARCHIVED }
        public enum eAdGroupStatus { ACTIVE, PAUSED, DELETED, ARCHIVED }
        public enum eAdStatus { ACTIVE, PAUSED, DELETED, ARCHIVED }

        public enum eBillingEvents
        {
            NONE,
            APP_INSTALLS, 
            CLICKS, 
            IMPRESSIONS, 
            LINK_CLICKS,             
            OFFER_CLAIMS, 
            PAGE_LIKES, 
            POST_ENGAGEMENT, 
            THRUPLAY, 
            PURCHASE, 
            LISTING_INTERACTION
        }

        public enum ePublisherPlatforms
        {
            FACEBOOK,
            INSTAGRAM,
            MESSENGER,
            AUDIENCE_NETWORK
        }

        public enum eOptimizationGoals
        {
            NONE,
            APP_INSTALLS,
            AD_RECALL_LIFT,
            ENGAGED_USERS,
            EVENT_RESPONSES,
            IMPRESSIONS,
            LEAD_GENERATION,
            QUALITY_LEAD,
            LINK_CLICKS,
            OFFSITE_CONVERSIONS,
            PAGE_LIKES,
            POST_ENGAGEMENT,
            QUALITY_CALL,
            REACH,
            LANDING_PAGE_VIEWS,
            VISIT_INSTAGRAM_PROFILE,
            VALUE,
            THRUPLAY,
            DERIVED_EVENTS,
            APP_INSTALLS_AND_OFFSITE_CONVERSIONS,
            CONVERSATIONS,
            IN_APP_VALUE,
            MESSAGING_PURCHASE_CONVERSION,
            SUBSCRIBERS,
            REMINDERS_SET,
            MEANINGFUL_CALL_ATTEMPT,
            PROFILE_VISIT,
            PROFILE_AND_PAGE_ENGAGEMENT,
            MESSAGING_APPOINTMENT_CONVERSION
        }

        public enum eSpecialAdCategories
        {
            NONE,
            EMPLOYMENT,
            HOUSING,
            CREDIT,
            ISSUES_ELECTIONS_POLITICS,
            ONLINE_GAMBLING_AND_GAMING,
            FINANCIAL_PRODUCTS_SERVICES
        }

        public enum eBidStrategy
        {
            LOWEST_COST_WITHOUT_CAP,
            LOWEST_COST_WITH_BID_CAP,
            COST_CAP,
            LOWEST_COST_WITH_MIN_ROAS
        }

        public enum eObjective
        {
            OUTCOME_LEADS,
            OUTCOME_SALES,
            OUTCOME_ENGAGEMENT,
            OUTCOME_AWARENESS,
            OUTCOME_TRAFFIC,
            OUTCOME_APP_PROMOTION
        }

        public class FacebookAPIConfig {
            
        }

        public class Campaign
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public eSpecialAdCategories SpecialAdCategories { get; set; } = eSpecialAdCategories.NONE;
            public eCampaignStatus Status { get; set; } = eCampaignStatus.PAUSED;
            public eObjective Objective { get; set; } = eObjective.OUTCOME_SALES;
        }

        public class AdGroup
        {
            public string Id { get; set; }
            public string CampaignId { get; set; }
            public string Name { get; set; }
            public int BudgetInCnt { get; set; }
            public int BidValueInCnt { get; set; }
            public eBillingEvents BillingEvent { get; set; } = eBillingEvents.IMPRESSIONS;
            public eOptimizationGoals OptimizationGoal { get; set; } = eOptimizationGoals.REACH;
            public TargetingData TargetingData { get; set; }
            public eAdGroupStatus Status { get; set; } = eAdGroupStatus.PAUSED;
            public eBidStrategy BidStrategy { get; set; } = eBidStrategy.LOWEST_COST_WITH_MIN_ROAS;
            public string PixelId { get; set; }
        }

        public class Ad
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string CampaignId { get; set; }
            public string AdGroupId { get; set; }
            public string AdCreativeId { get; set; }
            public string AdCreativeImageURL { get; set; }
            public eAdStatus Status { get; set; } = eAdStatus.ACTIVE;
        }

        public class AdCreative
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string PageId { get; set; }
            public string ImageURL { get; set; }
            public string Link { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string CallToActionType { get; set; }
        }

        public class AdImage
        {
            public string Name { get; set; }
            /// public string URL { get; set; }  // ByURL [DEPRECATED]
            public string FILE { get; set; } // ByLocalFile
        }

        public class AdArchive
        {
            public string Id { get; set; }
            public string PageId { get; set; }
            public string PageName { get; set; }

            public List<string> BodyList { get; set; }
            public string Body { get { return this.BodyList?.FirstOrDefault(); } }
                       
            public string SnapshotURL { get; set; }
            public string SnapshotURLNoToken { 
                get {
                    if (string.IsNullOrEmpty(this.SnapshotURL)) return string.Empty;
                    return this.SnapshotURL.Split(new string[] { "&access_token" }, StringSplitOptions.RemoveEmptyEntries)[0];  
                } 
            }            

            public List<string> ButtonTextList { get; set; }
            public string ButtonText { get { return this.ButtonTextList?.FirstOrDefault(); } }

            public List<string> ButtonTitleList { get; set; }
            public string ButtonTitle { get { return this.ButtonTitleList?.FirstOrDefault(); } }

            public string CreatedDate { get; set; }
            public string StartDate { get; set; }
            public string StopDate { get; set; }

            public List<string> Languages { get; set; }
            public string SpendRange { get; set; }
            
            public string ImageURL { get; set; }
            public string LinkHref { get; set; }
        }

        public class TargetingData
        {
            public IEnumerable<string> Countries { get; set; } = new[] { "US" };
            public int FromAge { get; set; } = 18;
            public int ToAge { get; set; } = 65;
            public IEnumerable<(string Id, string Name)> Interests { get; set; }
            public IEnumerable<string> Platforms { get; set; } = new[] { "Facebook", "Instagram" };
            public IEnumerable<string> Placements { get; set; } = new[] { "Facebook Feed", "Instagram Feed" };
            public IEnumerable<string> Genders { get; set; } = new[] { "Female", "Male" };  // 1 = Female, 2 = Male -> new[] { 1, 2 }
            public IEnumerable<string> Devices { get; set; } = new[] { "mobile", "desktop" }; 
            public string CustomAudiencesId { get; set; } = ""; 
        }

        public class Targeting
        {
            public List<string> Locations { get; set; }
            public List<string> Interests { get; set; }
            public List<string> Keywords { get; set; }
            public string AgeRange { get; set; }
            public string Gender { get; set; }
        }
    }

    public interface IFacebookManager
    {
        Task<List<FacebookEntities.Campaign>> GetCampaigns();
        Task<FacebookEntities.Campaign> GetCampaign(string Id);
        Task<List<FacebookEntities.AdGroup>> GetAdGroups(string CampaignId);
        Task<FacebookEntities.AdGroup> GetAdGroup(string Id);
        Task<List<FacebookEntities.Ad>> GetAds(string AdGroupId);
        Task<List<FacebookEntities.Ad>> GetAdsByAdGroup(string AdGroupId, int? RowCount);
        Task<List<FacebookEntities.Ad>> GetAdsByCampaign(string CampaignId, int? RowCount);
        Task<List<FacebookEntities.Ad>> GetAdsByAccount(int? RowCount);        
        Task<FacebookEntities.Ad> GetAd(string Id);
        Task<List<FacebookEntities.AdArchive>> SearchAdsLibrary(string SearchTerms, int? RowCount, int PageSize);
        Task<FacebookEntities.Targeting> GetTargeting(string AdGroupId);
        Task<string> CreateCampaign(FacebookEntities.Campaign Campaign);
        Task<string> CreateAdGroup(FacebookEntities.AdGroup AdGroup);
        Task<string> CreateAd(FacebookEntities.Ad Ad);
        Task<string> CreateAdCreative(FacebookEntities.AdCreative AdCreative);
        //Task<string> CreateAdImage(FacebookEntities.AdImage AdImage);
        //Task<bool> UpdateCampaign(string Id, FacebookEntities.Campaign Campaign);
        //Task<bool> UpdateAdGroup(string Id, FacebookEntities.AdGroup AdGroup);
        //Task<bool> UpdateAd(string Id, FacebookEntities.Ad Ad);
        //Task<bool> UpdateTargeting(string AdGroupId, FacebookEntities.Targeting Targeting);
        //Task<bool> PauseCampaign(string CampaignId);
        //Task<bool> DeleteCampaign(string Id);               
        //Task<bool> DeleteAdGroup(string Id);               
        //Task<bool> DeleteAd(string Id);
        //Task<(bool Exists, string Id)> IsCampaignExists(string CampaignName);
        Task<FacebookEntities.Campaign> FindCampaign(string CampaignName);
        bool SetAccountId(string AccountId);
    }

    public class FacebookManager : IFacebookManager
    {
        public FacebookClient _Client { get; private set; }
        public FacebookClient _ClientLibrary { get; private set; }        
        public string _ApiVersion { get; private set; }
        public string _AccountId { get; private set; }

        // TODO ->> Switch to FacebookAPIConfig
        public FacebookManager(string AccessToken) : this(AccessToken, AccessToken, string.Empty) { }
        public FacebookManager(string AccessToken, string LibraryAccessToken) : this(AccessToken, LibraryAccessToken, string.Empty) { }
        public FacebookManager(string AccessToken, string LibraryAccessToken, string AccountId, string ApiVersion = "v22.0")
        {
            _Client = new FacebookClient(AccessToken);
            _ClientLibrary = new FacebookClient(LibraryAccessToken);
            _ApiVersion = ApiVersion;
            _AccountId = AccountId;
        }

        public async Task<List<FacebookEntities.Campaign>> GetCampaigns()
        {
            var campaigns = new List<FacebookEntities.Campaign>();

            try
            {
                /// id,name,title,description,status,start_date,end_date,budget,currency,objective,target_audience,location_targeting,demographic_targeting,keywords,ad_groups,ads,impressions,clicks,conversions,spend,cpc,cpm,ctr,created_at,updated_at,owner_id,campaign_type,platform,bid_strategy,schedule,frequency,conversion_window,tracking_url,labels,notes,priority,time_zone
                var fields = "id,name,status,objective";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                    /// {"effective_status", "ACTIVE,PAUSED"},
                    /// { "limit", "1000" } 
                };

                /// dynamic result = await _client.GetTaskAsync($"/{_apiVersion}/act_{_accountId}/campaigns?fields={fields}");
                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/act_{_AccountId}/campaigns", parameters);


                foreach (var item in result.data)
                {
                    FacebookEntities.eCampaignStatus campaignStatus;
                    Enum.TryParse(item.status, true, out campaignStatus);

                    campaigns.Add(new FacebookEntities.Campaign
                    {
                        Id = item.id,
                        Name = item.name,
                        Status = campaignStatus
                    });
                }

                return campaigns;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return campaigns;
            }
        }

        public async Task<FacebookEntities.Campaign> GetCampaign(string Id)
        {
            try
            {
                var fields = "id,name,status,objective";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{Id}", parameters);

                FacebookEntities.eCampaignStatus campaignStatus;
                Enum.TryParse(result.status, true, out campaignStatus);

                return new FacebookEntities.Campaign
                {
                    Id = result.id,
                    Name = result.name,
                    Status = campaignStatus
                };
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<FacebookEntities.AdGroup>> GetAdGroups(string CampaignId)
        {
            var adGroups = new List<FacebookEntities.AdGroup>();

            try
            {
                /// id,name,status,type,version,url,slug,user_id,username,email,first_name,last_name,profile_picture,avatar,latitude,longitude,address,city,state,province,country,zip_code,postal_code,date,time,timezone,count,items,parent_id,category_id,value,key,data,image,price,quantity
                var fields = "id,name,title,status,campaign_id";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{CampaignId}/adsets", parameters);

                foreach (var item in result.data)
                {
                    FacebookEntities.eAdGroupStatus adGroupStatus;
                    Enum.TryParse(item.status, true, out adGroupStatus);

                    adGroups.Add(new FacebookEntities.AdGroup
                    {
                        Id = item.id,
                        Name = item.name,
                        CampaignId = item.campaign_id,
                        Status = adGroupStatus
                    });
                }

                return adGroups;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return adGroups;
            }
        }

        public async Task<FacebookEntities.AdGroup> GetAdGroup(string Id)
        {
            try
            {
                var fields = "id,name,status,campaign_id";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{Id}", parameters);

                FacebookEntities.eAdGroupStatus adGroupStatus;
                Enum.TryParse(result.status, true, out adGroupStatus);

                return new FacebookEntities.AdGroup
                {
                    Id = result.id,
                    Name = result.name,
                    CampaignId = result.campaign_id,
                    Status = adGroupStatus
                };
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<FacebookEntities.Ad>> GetAds(string AdGroupId) {
            return await this.GetAdsByAdGroup(AdGroupId);
        }

        public async Task<List<FacebookEntities.Ad>> GetAdsByAdGroup(string AdGroupId, int? RowCount = null)
        {
            var ads = new List<FacebookEntities.Ad>();

            try
            {
                /// id,name,status,adset_id,campaign_id,creative{id,name,thumbnail_url,object_story_spec,image_url,video_id,body,title,link_url,call_to_action_type},bid_amount,configured_status,effective_status,account_id,bid_info,bid_strategy,bid_type,campaign,conversion_domain,conversion_specs,created_time,creative_sequence,daily_budget,demolished_at,demotion_app,demotion_cats,demotion_page,failed_delivery_checks,io_number,issues_info,last_updated_by_app_id,lifetime_budget,lifetime_impressions,optimization_goal,page_link,pacing_type,preview_shareable_link,priority,recommendations,source_ad,source_ad_id,status_transition_time,targeting,updated_time,adlabels,tracking_specs,ad_review_feedback,adset,budget_remaining,date_format,date_preset,display_sequence,execution_options,filename,time_range,tracking_and_conversion_with_defaults,attribution_spec,destination_set_id,payment_source
                var fields = "id,name,status,adset_id,campaign_id,creative{id,name,image_url}";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                    { "limit", RowCount }  // null = ALL
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{AdGroupId}/ads", parameters);

                foreach (var item in result.data)
                {
                    FacebookEntities.eAdGroupStatus adStatus;
                    Enum.TryParse(item.status, true, out adStatus);

                    ads.Add(new FacebookEntities.Ad
                    {
                        Id = item.id,
                        Name = item.name,
                        CampaignId = item.campaign_id,
                        AdGroupId = item.adset_id,
                        AdCreativeId = item.creative.id,
                        AdCreativeImageURL = item.creative.image_url
                    });
                }

                return ads;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return ads;
            }
        }

        public async Task<List<FacebookEntities.Ad>> GetAdsByCampaign(string CampaignId, int? RowCount = null) {
            var ads = new List<FacebookEntities.Ad>();

            try
            {
                /// id,name,status,adset_id,campaign_id,creative{id,name,thumbnail_url,object_story_spec,image_url,video_id,body,title,link_url,call_to_action_type},bid_amount,configured_status,effective_status,account_id,bid_info,bid_strategy,bid_type,campaign,conversion_domain,conversion_specs,created_time,creative_sequence,daily_budget,demolished_at,demotion_app,demotion_cats,demotion_page,failed_delivery_checks,io_number,issues_info,last_updated_by_app_id,lifetime_budget,lifetime_impressions,optimization_goal,page_link,pacing_type,preview_shareable_link,priority,recommendations,source_ad,source_ad_id,status_transition_time,targeting,updated_time,adlabels,tracking_specs,ad_review_feedback,adset,budget_remaining,date_format,date_preset,display_sequence,execution_options,filename,time_range,tracking_and_conversion_with_defaults,attribution_spec,destination_set_id,payment_source
                var fields = "id,name,status,adset_id,campaign_id,creative{id,name,image_url}";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                    { "limit", RowCount }  // null = ALL
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{CampaignId}/ads", parameters);

                foreach (var item in result.data)
                {
                    FacebookEntities.eAdGroupStatus adStatus;
                    Enum.TryParse(item.status, true, out adStatus);

                    ads.Add(new FacebookEntities.Ad
                    {
                        Id = item.id,
                        Name = item.name,
                        CampaignId = item.campaign_id,
                        AdGroupId = item.adset_id,
                        AdCreativeId = item.creative.id,
                        AdCreativeImageURL = item.creative.image_url
                    });
                }

                return ads;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return ads;
            }
        }

        public async Task<List<FacebookEntities.Ad>> GetAdsByAccount(int? RowCount = null) {
            var ads = new List<FacebookEntities.Ad>();

            try
            {
                /// id,name,status,adset_id,campaign_id,creative{id,name,thumbnail_url,object_story_spec,image_url,video_id,body,title,link_url,call_to_action_type},bid_amount,configured_status,effective_status,account_id,bid_info,bid_strategy,bid_type,campaign,conversion_domain,conversion_specs,created_time,creative_sequence,daily_budget,demolished_at,demotion_app,demotion_cats,demotion_page,failed_delivery_checks,io_number,issues_info,last_updated_by_app_id,lifetime_budget,lifetime_impressions,optimization_goal,page_link,pacing_type,preview_shareable_link,priority,recommendations,source_ad,source_ad_id,status_transition_time,targeting,updated_time,adlabels,tracking_specs,ad_review_feedback,adset,budget_remaining,date_format,date_preset,display_sequence,execution_options,filename,time_range,tracking_and_conversion_with_defaults,attribution_spec,destination_set_id,payment_source
                var fields = "id,name,status,adset_id,campaign_id,creative{id,name,image_url}";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                    { "limit", RowCount }  // null = ALL
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/act_{_AccountId}/ads", parameters);

                foreach (var item in result.data)
                {
                    FacebookEntities.eAdGroupStatus adStatus;
                    Enum.TryParse(item.status, true, out adStatus);

                    ads.Add(new FacebookEntities.Ad
                    {
                        Id = item.id,
                        Name = item.name,
                        CampaignId = item.campaign_id,
                        AdGroupId = item.adset_id,
                        AdCreativeId = item.creative.id,
                        AdCreativeImageURL = item.creative.image_url
                    });
                }

                return ads;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return ads;
            }
        }  

        public async Task<FacebookEntities.Ad> GetAd(string Id)
        {
            try
            {
                var fields = "id,name,status,adset_id,campaign_id";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
                };

                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return new FacebookEntities.Ad
                {
                    Id = result.id,
                    Name = result.name,
                    AdGroupId = result.adset_id
                };
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        
        public async Task<List<FacebookEntities.AdArchive>> SearchAdsLibrary(string SearchTerms, int? RowCount = null, int PageSize = 500)
        {
            var ads = new List<FacebookEntities.AdArchive>();
            string afterCursor = null;
            var totalFetched = 0;            
            var maxRows = RowCount.HasValue ? RowCount.Value : 10000;  // DEFAULT MAX ROWS

            try
            {
                /// Formal Ads Library Interface:
                /// https://www.facebook.com/ads/library/?active_status=active&ad_type=political_and_issue_ads&country=IL&is_targeted_country=false&media_type=all                
                /// https://www.facebook.com/ads/library/api/

                while (true)
                {
                    /// id,page_id,page_name,ad_creative_link_descriptions,ad_creative_link_titles,ad_delivery_start_time,ad_delivery_stop_time,ad_snapshot_url,languages,spend,ad_creative_bodies,ad_creative_link_captions
                    var fields = "id,page_id,page_name,ad_creative_link_descriptions,ad_creative_link_titles,ad_delivery_start_time,ad_delivery_stop_time,ad_snapshot_url,languages,spend,ad_creative_bodies,ad_creative_link_captions";
                    var parameters = new Dictionary<string, object> {
                        { "fields", fields },
                        { "search_terms", HttpUtility.UrlEncode(SearchTerms) },
                        { "ad_reached_countries", "['US']" },
                        { "ad_delivery_date_min", "" },
                        { "ad_delivery_date_max", "" },
                        { "ad_type", "" },
                        /// { "search_type", "KEYWORD_EXACT_PHRASE" },  // KEYWORD_UNORDERED, KEYWORD_EXACT_PHRASE
                        { "active_status", "ALL" },  // ACTIVE, ALL, INACTIVE                        
                        { "limit", Math.Min(PageSize, maxRows - totalFetched).ToString() } // (built-in default is 25)
                    };

                    if (!string.IsNullOrEmpty(afterCursor)) 
                        parameters["after"] = afterCursor;

                    dynamic result = await _ClientLibrary.GetTaskAsync($"/{_ApiVersion}/ads_archive", parameters);

                    if (result?.data == null) break;

                    foreach (var item in result.data)
                    {
                        if (totalFetched >= maxRows) break;

                        var bodyList = new List<string>();
                        if (item.ad_creative_bodies != null)
                            foreach (var body in item.ad_creative_bodies)
                                bodyList.Add(body.ToString());

                        var buttonTextList = new List<string>();
                        if (item.ad_creative_link_titles != null)
                            foreach (var title in item.ad_creative_link_titles)
                                buttonTextList.Add(title.ToString());

                        var buttonTitleList = new List<string>();
                        if (item.ad_creative_link_captions != null)
                            foreach (var caption in item.ad_creative_link_captions)
                                buttonTitleList.Add(caption.ToString());

                        var languagesList = new List<string>();
                        if (item.languages != null)
                            foreach (var lang in item.languages)
                                languagesList.Add(lang.ToString());

                        var spendRange = item.spend != null ? $"{item.spend?.lower_bound} - {item.spend.upper_bound}" : "";

                        ads.Add(new FacebookEntities.AdArchive
                        {
                            Id = item.id,
                            PageId = item.page_id,
                            PageName = item.page_name,

                            BodyList = bodyList,
                            ButtonTextList = buttonTextList,
                            ButtonTitleList = buttonTitleList,
                            SnapshotURL = item.ad_snapshot_url,

                            CreatedDate = item?.ad_creation_time ?? "",
                            StartDate = item.ad_delivery_start_time,
                            StopDate = item.ad_delivery_stop_time,

                            Languages = languagesList,
                            SpendRange = spendRange
                        });

                        totalFetched++;
                        if (totalFetched >= maxRows) break;
                    }

                    if (totalFetched >= maxRows) break;

                    // paging
                    /*
                    paging: {
                        "cursors": {
                            "after": "c2NyYXBpbmdfY3Vyc29yOk1UYzBNalUwTWpnMU1qb3lNemMyTURZANE5qWXlOelV3TkRNNQZDZD"
                        },
                    "next": "....&after=c2NyYXBpbmdfY3Vyc29yOk1UYzBNalUwTWpnMU1qb3lNemMyTURZANE5qWXlOelV3TkRNNQZDZD"
                    */

                    afterCursor = result.paging?.cursors?.after;
                    if (string.IsNullOrEmpty(afterCursor)) break;
                }          
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);               
            }

            return ads;
        }

        public async Task<FacebookEntities.Targeting> GetTargeting(string AdGroupId)
        {
            try
            {
                dynamic result = await _Client.GetTaskAsync($"/{_ApiVersion}/{AdGroupId}?fields=targeting");
                var targeting = result.targeting;

                return new FacebookEntities.Targeting
                {
                    Locations = targeting.geo_locations.countries,
                    AgeRange = $"{targeting.age_min}-{targeting.age_max}",
                    Gender = targeting.genders[0] == 1 ? "M" : "F",
                    Interests = targeting.interests,
                    Keywords = targeting.keywords
                };
            }
            catch (FacebookApiException)
            {
                return null;
            }
        }

        public async Task<string> CreateCampaign(FacebookEntities.Campaign Campaign)
        {
            try
            {
                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/campaigns/
                var parameters = new Dictionary<string, object>
                {
                    { "name", Campaign.Name},
                    { "objective", Campaign.Objective.ToString()},
                    { "status", Campaign.Status.ToString()},
                    { "special_ad_categories", Campaign.SpecialAdCategories.ToString() }
                };

                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/campaigns", parameters);
                return result.id;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task<string> CreateAdGroup(FacebookEntities.AdGroup AdGroup)
        {
            try
            {
                var targetingData = AdGroup.TargetingData ?? new FacebookEntities.TargetingData();

                /// https://developers.facebook.com/docs/marketing-api/audiences/reference/placement-targeting/
                dynamic defaultTargeting = new ExpandoObject();
                defaultTargeting.device_platforms = targetingData.Devices.Select(x => x.Trim().ToLower());
                defaultTargeting.geo_locations = new { countries = targetingData.Countries };
                defaultTargeting.publisher_platforms = targetingData.Platforms.Select(x => x.Trim().ToLower());
                defaultTargeting.interests = targetingData.Interests?.Select(x => new { id = x.Id, name = x.Name });
                defaultTargeting.age_min = targetingData.FromAge;
                defaultTargeting.age_max = targetingData.ToAge;

                if(!string.IsNullOrEmpty(targetingData.CustomAudiencesId))
                    defaultTargeting.custom_audiences = new [] { new { id = targetingData.CustomAudiencesId } };

                // Placements 
                // facebook_positions: feed, right_hand_column, marketplace, video_feeds, story, search, instream_video, facebook_reels, facebook_reels_overlay, profile_feed
                // instagram_positions: stream, story, explore, explore_home, reels, profile_feed, ig_search, profile_reels
                var placementsMap = targetingData.Placements
                    .Select(p => p.Trim().ToLower())
                    .GroupBy(p => p.Split(' ')[0])
                    .ToDictionary(g => g.Key, g => g.Select(v => v.Split(' ')[1].Trim().ToLower()));

                if (placementsMap.ContainsKey("facebook"))
                    defaultTargeting.facebook_positions = placementsMap["facebook"].ToArray();

                if (placementsMap.ContainsKey("instagram")) {
                    placementsMap["instagram"] = placementsMap["instagram"].Select(p => p == "feed" ? "stream" : p);  // fix: replace instagram "feed" value to "stream" as per api document                    
                    defaultTargeting.instagram_positions = placementsMap["instagram"];
                }

                // Genders
                var genderMap = new Dictionary<string, int> {
                    { "female", 1 },
                    { "male", 2 }
                };

                defaultTargeting.genders = targetingData.Genders
                    .Select(g => g.Trim().ToLower())
                    .Where(g => genderMap.ContainsKey(g))
                    .Select(g => genderMap[g]);   // new[] { "female", "male" } >> new[] { 1, 2 }

                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/adsets/
                /// https://developers.facebook.com/docs/marketing-api/bidding/overview/bid-strategy/
                /// https://developers.facebook.com/docs/marketing-api/bidding/guides/cost-per-action-ads/
                var parameters = new Dictionary<string, object>
                {
                    { "name", AdGroup.Name },
                    { "campaign_id", AdGroup.CampaignId },
                    { "daily_budget", AdGroup.BudgetInCnt },
                    { "billing_event", AdGroup.BillingEvent.ToString() },
                    { "optimization_goal", AdGroup.OptimizationGoal.ToString() },
                    { "bid_strategy", AdGroup.BidStrategy.ToString() },
                    { "status", AdGroup.Status.ToString() },                    
                    { "targeting", defaultTargeting },
                    { "locale", "en_US" } // responses in english
                };

                /// Sample Combinations (OptimizationGoal + BidStrategy)
                /// REACH + LOWEST_COST_WITH_BID_CAP + bid_amount
                if (AdGroup.OptimizationGoal == FacebookEntities.eOptimizationGoals.VALUE && AdGroup.BidStrategy == FacebookEntities.eBidStrategy.LOWEST_COST_WITH_MIN_ROAS)
                {
                    parameters.Add("promoted_object", new
                    {
                        pixel_id = AdGroup.PixelId,
                        custom_event_type = "PURCHASE"
                    });

                    parameters.Add("bid_constraints", new
                    {
                        roas_average_floor = AdGroup.BidValueInCnt
                    });
                }
                else
                    parameters.Add("bid_amount", AdGroup.BidValueInCnt);                

                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/adsets", parameters);
                return result.id;
            }
            ///catch (FacebookApiException ex)
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task<string> CreateAd(FacebookEntities.Ad Ad)
        {
            try
            {
                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/ads/
                var parameters = new Dictionary<string, object>
                {
                    {"name", Ad.Name},
                    {"adset_id", Ad.AdGroupId},
                    {"creative", new {
                        creative_id = Ad.AdCreativeId
                    }},
                    {"status", Ad.Status.ToString()}
                };

                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/ads", parameters);
                return result.id;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task<string> CreateAdCreative(FacebookEntities.AdCreative AdCreative)
        {
            try
            {
                /// https://developers.facebook.com/docs/marketing-api/reference/ad-creative-link-data/
                var object_story_spec = new
                {
                    page_id = AdCreative.PageId,
                    link_data = new
                    {
                        picture = AdCreative.ImageURL,
                        link = AdCreative.Link,
                        name = AdCreative.Title,
                        message = AdCreative.Message,
                        description = " ",

                        // BOOK_TRAVEL, CONTACT_US, DONATE, DONATE_NOW, DOWNLOAD, GET_DIRECTIONS, GO_LIVE, INTERESTED, LEARN_MORE, LIKE_PAGE, MESSAGE_PAGE, RAISE_MONEY, SAVE, SEND_TIP, SHOP_NOW, SIGN_UP, VIEW_INSTAGRAM_PROFILE, INSTAGRAM_MESSAGE, LOYALTY_LEARN_MORE, PURCHASE_GIFT_CARDS, PAY_TO_ACCESS, SEE_MORE, TRY_IN_CAMERA, WHATSAPP_LINK, GET_IN_TOUCH, BOOK_NOW, CHECK_AVAILABILITY, ORDER_NOW, WHATSAPP_MESSAGE, GET_MOBILE_APP, INSTALL_MOBILE_APP, USE_MOBILE_APP, INSTALL_APP, USE_APP, PLAY_GAME, WATCH_VIDEO, WATCH_MORE, OPEN_LINK, NO_BUTTON, LISTEN_MUSIC, MOBILE_DOWNLOAD, GET_OFFER, GET_OFFER_VIEW, BUY_NOW, BUY_TICKETS, UPDATE_APP, BET_NOW, ADD_TO_CART, SELL_NOW, GET_SHOWTIMES, LISTEN_NOW, GET_EVENT_TICKETS, REMIND_ME, SEARCH_MORE, PRE_REGISTER, SWIPE_UP_PRODUCT, SWIPE_UP_SHOP, PLAY_GAME_ON_FACEBOOK, VISIT_WORLD, OPEN_INSTANT_APP, JOIN_GROUP, GET_PROMOTIONS, SEND_UPDATES, INQUIRE_NOW, VISIT_PROFILE, CHAT_ON_WHATSAPP, EXPLORE_MORE, CONFIRM, JOIN_CHANNEL, MAKE_AN_APPOINTMENT, ASK_ABOUT_SERVICES, BOOK_A_CONSULTATION, GET_A_QUOTE, BUY_VIA_MESSAGE, ASK_FOR_MORE_INFO, CHAT_WITH_US, VIEW_PRODUCT, VIEW_CHANNEL, WATCH_LIVE_VIDEO, CALL, MISSED_CALL, CALL_NOW, CALL_ME, APPLY_NOW, BUY, GET_QUOTE, SUBSCRIBE, RECORD_NOW, VOTE_NOW, GIVE_FREE_RIDES, REGISTER_NOW, OPEN_MESSENGER_EXT, EVENT_RSVP, CIVIC_ACTION, SEND_INVITES, REFER_FRIENDS, REQUEST_TIME, SEE_MENU, SEARCH, TRY_IT, TRY_ON, LINK_CARD, DIAL_CODE, FIND_YOUR_GROUPS, START_ORDER
                        call_to_action = new
                        {
                            type = AdCreative.CallToActionType
                        }
                    }
                };

                var creative_features_spec = new
                {
                    description_automation = new
                    {
                        enroll_status = "OPT_OUT"
                    },
                    contextual_multi_ads = new
                    {
                        enroll_status = "OPT_OUT"
                    }
                };

                var enroll_status = new
                {
                    enroll_status = "OPT_OUT"
                };

                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/adcreatives/
                var parameters = new Dictionary<string, object>
                {
                    {"name", AdCreative.Name},
                    {"object_story_spec", ConvertSampleFormat(JsonConvert.SerializeObject(object_story_spec))},
                    {"creative_features_spec", creative_features_spec },
                    {"contextual_multi_ads", enroll_status },
                    {"description_automation", enroll_status }
                };

                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/adcreatives", parameters);
                return result.id;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);                
                return string.Empty;
            }
        }

        public async Task<string> CreateAdImage(FacebookEntities.AdImage AdImage)
        {
            try
            {
                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/ads/
                /// .jpg extension
                var parameters = new Dictionary<string, object>
                {
                    { "filename", new FacebookMediaObject
                        {
                            FileName = AdImage.Name,
                            ContentType = "image/jpeg"
                        }.SetValue(File.ReadAllBytes(AdImage.FILE))
                    }
                };

                // TODO ->> (OAuthException - #100) Invalid parameter
                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/adimages", parameters);
                return result.images[AdImage.Name].hash;
            }
            catch (FacebookApiException ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task<bool> UpdateCampaign(string Id, FacebookEntities.Campaign Campaign)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"name", Campaign.Name},
                    {"status", Campaign.Status},
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAdGroup(string Id, FacebookEntities.AdGroup AdGroup)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"name", AdGroup.Name},
                    {"status", "ACTIVE"}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAd(string Id, FacebookEntities.Ad Ad)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"creative", new {
                        creative_id = Ad.AdCreativeId
                    }},
                    {"status", "ACTIVE"}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTargeting(string AdGroupId, FacebookEntities.Targeting Targeting)
        {
            try
            {
                var targetingSpec = new Dictionary<string, object>
                {
                    {"geo_locations", new { countries = Targeting.Locations }},
                    {"age_min", Targeting.AgeRange.Split('-')[0]},
                    {"age_max", Targeting.AgeRange.Split('-')[1]},
                    {"genders", Targeting.Gender == "M" ? 1 : 2},
                    {"interests", Targeting.Interests},
                    {"keywords", Targeting.Keywords}
                };

                var parameters = new Dictionary<string, object>
                {
                    {"targeting", targetingSpec}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{AdGroupId}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> PauseCampaign(string CampaignId)
        {
            return await UpdateCampaign(CampaignId, new FacebookEntities.Campaign { Status = FacebookEntities.eCampaignStatus.PAUSED });
        }

        public async Task<bool> DeleteCampaign(string Id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"status", "DELETED"}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAdGroup(string Id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"status", "DELETED"}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAd(string Id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"status", "DELETED"}
                };

                await _Client.PostTaskAsync($"/{_ApiVersion}/{Id}", parameters);
                return true;
            }
            catch (FacebookApiException)
            {
                return false;
            }
        }

        public async Task<(bool Exists, string Id)> IsCampaignExists(string CampaignName)
        {
            var match = (await this.GetCampaigns())?.FirstOrDefault(x => string.Equals(x.Name, CampaignName, StringComparison.OrdinalIgnoreCase));
            return (match != null, match?.Id);
        }

        public async Task<FacebookEntities.Campaign> FindCampaign(string CampaignName)
        {
            var match = (await this.GetCampaigns())?.FirstOrDefault(x => string.Equals(x.Name, CampaignName, StringComparison.OrdinalIgnoreCase));
            return match;
        }

        public bool SetAccountId(string AccountId) {
            this._AccountId = AccountId;
            return true;
        }

        // -- PRIVATE -----

        protected string ValidateAccountId(string AccountId) {
            if (AccountId.StartsWith("act_", StringComparison.OrdinalIgnoreCase)) return AccountId;
            return $"act_{AccountId}";
        }

        // -- HELPERS -----

        public static string ConvertSampleFormat(string inputJson)
        {
            if (string.IsNullOrEmpty(inputJson))
            {
                return inputJson;
            }

            try
            {
                JObject jsonObject = JObject.Parse(inputJson);
                return jsonObject.ToString(Formatting.None);
            }
            catch (JsonReaderException)
            {
                // Handle invalid JSON input. Return the original string.
                return inputJson;
            }
            catch (Exception)
            {
                // Handle other exceptions. Return the original string.
                return inputJson;
            }
        }

        /// var text = "This is a long string with many words.";
        /// var result = TakeXWords(text, 5); // "This is a long string" 
        public static string TakeXWords(string input, int n)
        {
            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Take(n));
        }

        /// var result = TakeQueryParamValue("https://example.com/page?user=123&token=abc", "token");
        public static string TakeQueryParamValue(string URL, string paramKey)
        {            
            var uri = new Uri(URL);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            return queryParams[paramKey];
        }
    }
}