using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

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

    https://www.facebook.com/groups/fbdevelopers/
    https://developers.facebook.com/docs/
    https://developers.facebook.com/blog/
    https://developers.facebook.com/social-technologies/products
    https://developers.facebook.com/quickstarts/

    https://www.youtube.com/watch?si=DSuQCMqgw1p0vdzT&v=0KTgD7QNGA0&feature=youtu.be

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

    ACCESS-TOKEN
    ------------

    IMPLEMENTATIONS
    ---------------    

    SANDBOX
    -------
    // Graph API Explorer
    https://developers.facebook.com/tools/explorer/

    POSTMAN
    -------

    NUGET
    -----
    https://www.nuget.org/packages/facebook 
    > Install-Package Facebook    


    USING
    -----
    
*/

namespace FacebookAPI
{
    public class FacebookEntities
    {
        public enum eCampaignStatus{ ACTIVE, PAUSED, DELETED, ARCHIVED }
        public enum eAdGroupStatus { ACTIVE, PAUSED, DELETED, ARCHIVED }

        public enum eBillingEvents
        {
            IMPRESSIONS,
            CLICKS,
            LINK_CLICKS,
            OFFER_CLAIMS,
            PAGE_LIKES,
            PURCHASE,
            THRUPLAY
        }

        public enum eOptimizationGoals
        {
            REACH,
            APP_INSTALLS,
            BRAND_AWARENESS,
            CLICKS,
            ENGAGED_USERS,
            EVENT_RESPONSES,
            IMPRESSIONS,
            LEAD_GENERATION,
            LINK_CLICKS,
            OFFSITE_CONVERSIONS,
            PAGE_LIKES,
            POST_ENGAGEMENT,
            QUALITY_LEAD,
            STORE_VISITS,
            VIDEO_VIEWS
        }


        public class GoogleAdwordsConfig
        {
            // TODO ->> Implement
        }

        public class Campaign
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Budget { get; set; }
            public eCampaignStatus Status { get; set; } = eCampaignStatus.PAUSED;
        }

        public class AdGroup
        {
            public string Id { get; set; }
            public string CampaignId { get; set; }
            public string Name { get; set; }
            public eBillingEvents BillingEvent { get; set; } = eBillingEvents.IMPRESSIONS;
            public eOptimizationGoals OptimizationGoal { get; set; } = eOptimizationGoals.REACH;
            public string Targeting { get; set; }
            public eAdGroupStatus Status { get; set; } = eAdGroupStatus.PAUSED;
            public decimal BidAmount { get; set; }
        }

        public class Ad
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string AdGroupId { get; set; }
            public string AdCreativeId { get; set; }
        }

        public class AdCreative
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string PageId { get; set; }
            public string ImageURL { get; set; }
            public string Link { get; set; }
            public string Message { get; set; }
            public string CallToActionType { get; set; }            
        }

        public class AdImage
        {
            public string Name { get; set; }
            /// public string URL { get; set; }  // ByURL [DEPRECATED]
            public string FILE { get; set; } // ByLocalFile
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
        Task<FacebookEntities.Ad> GetAd(string Id);
        Task<FacebookEntities.Targeting> GetTargeting(string AdGroupId);
        Task<string> CreateCampaign(FacebookEntities.Campaign Campaign);
        Task<string> CreateAdGroup(FacebookEntities.AdGroup AdGroup);
        Task<string> CreateAd(FacebookEntities.Ad Ad);
        Task<string> CreateAdCreative(FacebookEntities.AdCreative AdCreative);
        Task<string> CreateAdImage(FacebookEntities.AdImage AdImage);
        Task<bool> UpdateCampaign(string Id, FacebookEntities.Campaign Campaign);
        Task<bool> UpdateAdGroup(string Id, FacebookEntities.AdGroup AdGroup);
        Task<bool> UpdateAd(string Id, FacebookEntities.Ad Ad);
        Task<bool> UpdateTargeting(string AdGroupId, FacebookEntities.Targeting Targeting);
        Task<bool> DeleteCampaign(string Id);               
        Task<bool> DeleteAdGroup(string Id);               
        Task<bool> DeleteAd(string Id);
        Task<(bool Exists, string Id)> IsCampaignExists(string CampaignName);
        Task<FacebookEntities.Campaign> FindCampaign(string CampaignName);
    }

    public class FacebookManager : IFacebookManager
    {
        private readonly FacebookClient _Client;
        private readonly string _ApiVersion;
        private readonly string _AccountId;
        private const decimal UNIT_IN_CENTS = 100;

        public FacebookManager(string AccessToken, string AccountId, string ApiVersion = "v22.0")
        {
            _Client = new FacebookClient(AccessToken);
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

        public async Task<List<FacebookEntities.Ad>> GetAds(string AdGroupId)
        {
            var ads = new List<FacebookEntities.Ad>();

            try
            {
                /// id,campaign_id,ad_group_id,title,headline,description,body,image_url,video_url,landing_page_url,display_url,status,created_at,updated_at,start_date,end_date,budget,bid,bid_strategy,impressions,clicks,conversions,spend,ctr,cpc,cpm,quality_score,relevance_score,ad_type,creative_type,device_targeting,location_targeting,demographic_targeting,keywords,labels,notes,priority,tracking_url,final_urls,final_mobile_urls,final_app_urls,template_id
                var fields = "id,name,status,adset_id,campaign_id";
                var parameters = new Dictionary<string, object> {
                    { "fields", fields },
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
                        AdGroupId = item.adset_id                        
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
                    {"name", Campaign.Name},
                    {"objective", "OUTCOME_SALES"},
                    {"status", Campaign.Status.ToString()},
                    {"daily_budget", Campaign.Budget * UNIT_IN_CENTS},
                    {"special_ad_categories", "NONE"}
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
                var defaultTargeting = new
                {
                    facebook_positions = new[] { "feed" }, 
                    geo_locations = new {
                        countries = new[] { "US" }
                    },
                    publisher_platforms = new[] { "facebook", "audience_network" },
                    age_min = 18,
                    age_max = 55,
                };

                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/adsets/
                var parameters = new Dictionary<string, object>
                {
                    { "name", AdGroup.Name },
                    { "campaign_id", AdGroup.CampaignId },  // Campaign MUST be Active!
                    { "optimization_goal", AdGroup.OptimizationGoal.ToString() },
                    { "billing_event", AdGroup.BillingEvent.ToString() },
                    { "status", AdGroup.Status.ToString() },
                    { "bid_amount", AdGroup.BidAmount },
                    { "targeting", JsonConvert.SerializeObject(defaultTargeting) },                    
                };

                dynamic result = await _Client.PostTaskAsync($"/{_ApiVersion}/act_{_AccountId}/adsets", parameters);
                return result.id;
            }
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
                    {"status", "ACTIVE"}
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
                var object_story_spec = new
                {
                    page_id = AdCreative.PageId,
                    link_data = new
                    {
                        picture = AdCreative.ImageURL,
                        link = AdCreative.Link,
                        message = AdCreative.Message,
                        call_to_action = new
                        {
                            type = AdCreative.CallToActionType
                        }
                    }
                };                

                /// https://developers.facebook.com/docs/marketing-api/reference/ad-account/adcreatives/
                var parameters = new Dictionary<string, object>
                {
                    {"name", AdCreative.Name},
                    {"object_story_spec", ConvertSampleFormat(JsonConvert.SerializeObject(object_story_spec))}
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
        
        public async Task<string> CreateAdImage(FacebookEntities.AdImage AdImage) {
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
                return result.id;
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
                    {"daily_budget", Campaign.Budget}
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

        public async Task<FacebookEntities.Campaign> FindCampaign(string CampaignName) {
            var match = (await this.GetCampaigns())?.FirstOrDefault(x => string.Equals(x.Name, CampaignName, StringComparison.OrdinalIgnoreCase));
            return match;
        }

        public static string ConvertSampleFormat(string inputJson)
        {
            if (string.IsNullOrEmpty(inputJson))
            {
                return inputJson;
            }

            try
            {
                JObject jsonObject = JObject.Parse(inputJson);
                return jsonObject.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Newtonsoft.Json.JsonReaderException)
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
    }
}