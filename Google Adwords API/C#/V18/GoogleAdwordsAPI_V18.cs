using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Install-Package Google.Ads.GoogleAds
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.V18.Services;
using Google.Ads.GoogleAds.V18.Resources;
using Google.Ads.GoogleAds.V18.Common;
using Google.Ads.GoogleAds.V18.Enums;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.Util;
using static GoogleAdwords.IGoogleAdwordsEntities;
using Google.Ads.GoogleAds.V18.Errors;
using Google.Apis.Auth.OAuth2;
using Google.Ads.Gax.Config;
using System.Net;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Newtonsoft.Json;

// TODO ->> RESEARCH & IMPLEMETATION! + ASYNC

/*
    REFERENCE
    ---------
    https://developers.google.com/google-ads/api/docs/client-libs/dotnet/getting-started        
    https://developers.google.com/google-ads/api/docs/client-libs/dotnet/oauth-service    
    https://developers.google.com/google-ads/api/rest/auth    
    https://developers.google.com/identity/protocols/oauth2/native-app
    https://developers.google.com/identity/sign-in/web/sign-in
    https://github.com/googleapis/google-api-dotnet-client

    SCOPES
    ------        
    https://www.googleapis.com/auth/adwords
    note! SERVICE ACCOUNT DOES NOT REQUIRE SCOPE - JUST ADD IT AS USER TO THE SHEET PERMISSIONS

    PROCESS
    -------
    (steps)
    1. go to Google Cloud Platform
       https://console.cloud.google.com/
    2. Enable 'Google Ads API'
    3. Credentials > Create > OAuth Client ID > (app type) Desktop app // OAuth
       Credentials > Create > Service Account > Set Details > Keys > Add Key // App-Level
    4. Download json credentials > copy it to the app folder 
    5. OAuth consent screen > Publish app
       https://youtu.be/__03uyFWj0Y?si=lTJFxHcBs5ue7r1k
    6. Go to Ads Dashboard > Admin > Access and security > add 'Standard' access to the service-account email 
    7. Go to Ads Dashboard > (MCC) Manager Account > Admin > API Center > Developer token
       https://youtu.be/PodgQhJqW0M
       https://developers.google.com/google-ads/api/docs/get-started/dev-token
       (note! Only available for non-tests MCC accounts, for test-mcc use the "Customer ID" instead!)
    7. install library via nuget 
       > Install-Package Google.Ads.GoogleAds
    8. see sample code below

    CREDENTIALS
    -----------
    ServicePointManager.Expect100Continue = true;
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

    /// var keyPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{Config.CredentialsFileName}";
    /// var credential = GoogleCredential.FromFile(keyPath).CreateScoped(new[] { "https://www.googleapis.com/auth/adwords" });

    // OAuth Process
    if (string.IsNullOrEmpty(this.Config.RefreshToken))
    {
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets { ClientId = Config.ClientId, ClientSecret = Config.ClientSecret },
            new[] { "https://www.googleapis.com/auth/adwords" },
            "user",
            CancellationToken.None
        ).Result;

        Console.WriteLine(JsonConvert.SerializeObject(credential.Token));
        var refreshToken = credential.Token.RefreshToken;
        var accessToken = credential.Token.AccessToken;
        this.Config.RefreshToken = refreshToken;
    }

    var config = new GoogleAdsConfig
    {
        DeveloperToken = Config.DeveloperToken,
        OAuth2ClientId = Config.ClientId,
        OAuth2ClientSecret = Config.ClientSecret,
        OAuth2RefreshToken = this.Config.RefreshToken,
        LoginCustomerId = Config.ManagerId.ToString(),
    };

    this.Service = new GoogleAdsClient(config);

    REST
    ----
    // TODO ->> Postman + Code

    IMPLEMENTATIONS
    ---------------
    1.SDK 
    see 'RESEARCH > Google Adwords API\C#\V18'    

    2.REST
    see 'RESEARCH > Google Adwords API\REST\V18'

    NUGET
    -----
    
    USING
    -----
    /// https://console.cloud.google.com/apis/credentials?inv=1&invt=Abo4fw&project=fialkovdigitalsandbox
    var googleAdwordsManager = new GoogleAdwordsAPI_V18(new GoogleAdwordsConfig
    {
        ClientId = "346411181626-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.apps.googleusercontent.com",
        ClientSecret = "GOCSPX-xxxxxxxxxxxxxxxxxxxxxxxxxxx",
        DeveloperToken = "xxxxxxxxxxxxxxxxx",
        ManagerId = 8702916561,  
        CustomerId = 6948242805, 
        CredentialsFileName = "fialkovdigitalsandbox-4c892bf0fbeb.json",
        RefreshToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
    });

    var result = googleAdwordsManager.GetKeywordData("digital marketing");
    Console.WriteLine($"found {result.Count} KW ideas");

    var MinMonthlySearches = 1000;
    var LstCompetitions = new List<string> { "medium", "high"}; // ToLower
    var MinTopOfPageBid = 3;

    var filtered = result
        .Where(x => x.AverageMonthlySearches >= MinMonthlySearches)
        .Where(x => LstCompetitions.Contains(x.Competition.ToLower()))
        .Where(x => x.LowTopOfPageBid >= MinTopOfPageBid)
        .OrderByDescending(x => x.AverageMonthlySearches).ToList();
    Console.WriteLine($"found {filtered.Count} KW ideas (filtered)");



    ===================
    // TODO ->>

    // via Services.V18.<entity>Service - specific entity level (campaign, adgroup, ad and etc.)
    CampaignServiceClient campaignService = this.Service.GetService(Services.V18.CampaignService);

    // via Services.V18.GoogleAdsService - root level
    GoogleAdsServiceClient service = this.Service.GetService(Services.V18.GoogleAdsService);

    -

    Unverified Apps:
    Unverified Apps ONLY for sensitive scopes - see google console credentials - if has sensitive scopes, must be verified by google inc
        
    +App Test Users

    -

    סדר קוד + עדכון 
    RESEARCH (sheets+ads)

    -

    קונפיג מצבים 
    SERVICE/OAUTH + POSTMAN
*/

namespace GoogleAdwords
{
    public class IGoogleAdwordsEntities
    {
        public const int MICRO = 1000000;

        public enum eLocationId : long { USA = 2840 }
        public enum eLanguageId : long { English = 1000 }        

        // TODO ->> SERVICE ACCOUNT OR OAUTH KEY - options + Clean CreateClient function        
        public class GoogleAdwordsConfig
        {            
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }            
            public string DeveloperToken { get; set; }            
            public string CredentialsFileName { get; set; }
            public long CustomerId { get; set; }
            public long ManagerId { get; set; }
            public string RefreshToken { get; set; }
        }

        public class KeywordPlannerData
        {
            public string Keyword { get; set; }
            public long AverageMonthlySearches { get; set; }
            public string Competition { get; set; }
            public long CompetitionIndex { get; set; }
            public decimal LowTopOfPageBid { get; set; }
            public decimal HighTopOfPageBid { get; set; }

            /*
            public decimal LowTopOfPageBidMicros { get; set; }
            public decimal HighTopOfPageBidMicros { get; set; }

            public decimal LowTopOfPageBid { 
                get {
                    return this.LowTopOfPageBidMicros / MICRO;
                } 
            }
            public decimal HighTopOfPageBid { 
                get {
                    return this.HighTopOfPageBidMicros / MICRO;
                } 
            }
            */
        }
    }

    public interface IGoogleAdwordsAPI
    {
        List<KeywordPlannerData> GetKeywordData(string Keyword, eLanguageId Language, eLocationId Location);
        List<KeywordPlannerData> GetKeywordDataAndIdeas(IEnumerable<string> Keywords, eLanguageId Language, eLocationId Location);
        List<KeywordPlannerData> GetKeywordDataAndIdeas(IEnumerable<string> Keywords, eLanguageId Language, IEnumerable<eLocationId> Locations);
        void PrintCampaigns();
        void CreateCampaign();
        long GetBudgetIdByName(long CustomerId, string BudgetName);
    }

    public class GoogleAdwordsAPI_V18 : IGoogleAdwordsAPI
    {
        /// protected static readonly string ASSEMBLY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
        protected static readonly string ASSEMBLY_PATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)}".TrimStart("file:\\".ToCharArray());
        
        /// https://developers.google.com/sheets/api/scopes
        protected static readonly string[] SCOPES = new string[] {
            "https://www.googleapis.com/auth/adwords"
        };

        protected GoogleAdwordsConfig Config { get; set; }
        protected GoogleAdsClient Service { get; set; }
        
        public GoogleAdwordsAPI_V18(GoogleAdwordsConfig Config)
        {
            this.Config = Config;
            this.InitService();
        }

        public void InitService()
        {
            if (this.Service == null)
                this.CreateService();
        }

        /// https://developers.google.com/google-ads/api/docs/keyword-planning/overview
        /// https://developers.google.com/google-ads/api/docs/keyword-planning/generate-keyword-ideas
        public List<KeywordPlannerData> GetKeywordData(string Keyword, eLanguageId Language = eLanguageId.English, eLocationId Location = eLocationId.USA) {
            return this.GetKeywordDataAndIdeas(
                new string[] { Keyword },
                Language, 
                new eLocationId[] { Location }
            );
        }

        public List<KeywordPlannerData> GetKeywordDataAndIdeas(IEnumerable<string> Keywords, eLanguageId Language = eLanguageId.English, eLocationId Location = eLocationId.USA) {
            return this.GetKeywordDataAndIdeas(
                    Keywords,
                    Language,
                    new eLocationId[] { Location }
                );
        }

        public List<KeywordPlannerData> GetKeywordDataAndIdeas(IEnumerable<string> Keywords, eLanguageId Language, IEnumerable<eLocationId> Locations)
        {
            var service = this.Service.GetService(Services.V18.KeywordPlanIdeaService);

            var request = new GenerateKeywordIdeasRequest
            {
                CustomerId = this.Config.CustomerId.ToString(),
                KeywordSeed = new KeywordSeed { Keywords = { Keywords } },
                KeywordPlanNetwork = KeywordPlanNetworkEnum.Types.KeywordPlanNetwork.GoogleSearchAndPartners,
                Language = ResourceNames.LanguageConstant((long)Language),
                IncludeAdultKeywords = false
            };

            foreach (var locationId in Locations)            
                request.GeoTargetConstants.Add(ResourceNames.GeoTargetConstant((long)locationId));            

            var response = service.GenerateKeywordIdeas(request);
            var results = new List<KeywordPlannerData>();

            if (response == null) {
                Console.WriteLine("No response received.");
                return results;
            }

            foreach (var result in response)
            {
                var metrics = result.KeywordIdeaMetrics;
                results.Add(new KeywordPlannerData
                {
                    Keyword = result.Text,
                    AverageMonthlySearches = metrics?.AvgMonthlySearches ?? 0,
                    Competition = metrics?.Competition.ToString(),
                    CompetitionIndex = metrics?.CompetitionIndex ?? 0,
                    LowTopOfPageBid = Math.Round((decimal)(metrics?.LowTopOfPageBidMicros ?? 0)/ MICRO, 2),
                    HighTopOfPageBid = Math.Round((decimal)(metrics?.HighTopOfPageBidMicros ?? 0) / MICRO, 2)
                });
            }

            return results;
        }
        
        // TODO ->> CHECK
        public void PrintCampaigns()
        {
            try
            {
                GoogleAdsServiceClient service = this.Service.GetService(Services.V18.GoogleAdsService);

                string query = @"
                    SELECT campaign.id, campaign.name, campaign.status
                    FROM campaign
                    ORDER BY campaign.id";

                var request = new SearchGoogleAdsRequest
                {
                    CustomerId = this.Config.CustomerId.ToString(),
                    Query = query
                };

                var response = service.Search(request);
                if (response == null || !response.Any()) return;

                foreach (var row in response)
                    Console.WriteLine(row.Campaign.Name);
            }
            catch (GoogleAdsException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                foreach (var error in e.Failure.Errors)
                {
                    Console.WriteLine($"Error Code: {error.ErrorCode}");
                    Console.WriteLine($"Error Message: {error.Message}");
                }
            }
        }

        public void CreateCampaign()
        {      
            var campaignService = this.Service.GetService(Services.V18.CampaignService);

            var campaign = new Campaign
            {
                Name = "From API 3",
                Status = CampaignStatusEnum.Types.CampaignStatus.Paused,
                AdvertisingChannelType = AdvertisingChannelTypeEnum.Types.AdvertisingChannelType.Search,
                ManualCpc = new ManualCpc(),
                CampaignBudget = ResourceNames.CampaignBudget(this.Config.CustomerId, this.GetBudgetIdByName(this.Config.CustomerId, "My Budget"))
            };

            CampaignOperation operation = new CampaignOperation
            {
                Create = campaign
            };

            MutateCampaignsResponse response = campaignService.MutateCampaigns(this.Config.CustomerId.ToString(), new[] { operation });
            foreach (MutateCampaignResult result in response.Results)
            {
                Console.WriteLine($"Campaign created with resource name: {result.ResourceName}");
            }                        
        }

        public long GetBudgetIdByName(long CustomerId, string BudgetName)
        {
            var googleAdsService = this.Service.GetService(Services.V18.GoogleAdsService);
            string query = $"SELECT campaign_budget.id FROM campaign_budget WHERE campaign_budget.name = '{BudgetName}' LIMIT 1";
            var request = new SearchGoogleAdsRequest { CustomerId = CustomerId.ToString(), Query = query };
            var response = googleAdsService.Search(request);
            foreach (var row in response)            
                return row.CampaignBudget.Id;            
            throw new Exception("Budget not found");
        }

        // ---

        private void CreateService()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            ///RefreshToken();

            #region Tests
            /*
                using (var stream = new FileStream("client_secret_346411181626.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { "https://www.googleapis.com/auth/adwords" },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    ).Result;

                    Console.WriteLine(JsonConvert.SerializeObject(credential.Token));
                } 
            */

            /*
                var authUrl = $"https://accounts.google.com/o/oauth2/auth?client_id={Config.ClientId}&redirect_uri={"urn:ietf:wg:oauth:2.0:oob"}&response_type=code&scope={"https://www.googleapis.com/auth/adwords"}";            
                Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });           
                Console.Write("Enter the authorization code: ");
                string authCode = Console.ReadLine();
            */

            /*
                var success = credential.RefreshTokenAsync(CancellationToken.None).GetAwaiter().GetResult();
                var accessToken = credential.GetAccessTokenForRequestAsync().GetAwaiter().GetResult();
            */

            /*
                var refreshToken = "";
                if (credential.UnderlyingCredential is ITokenAccessWithHeaders tokenAccess) {
                    refreshToken = tokenAccess.GetAccessTokenForRequestAsync().Result;
                    Console.WriteLine("Token: " + refreshToken);
                } 
            */

            /*
                /// [DEPRECATED - OLD VERSION]
                UserCredential credential1;
                Console.WriteLine($"ASSEMBLY LOCATION: {ASSEMBLY_PATH}");
                using (var stream = new FileStream($"{CREDENTIALS_FILE_PATH}", FileMode.Open, FileAccess.Read))
                {
                    credential1 = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,                    
                        SCOPES,
                        "user",
                        CancellationToken.None
                    ).Result;
                }

                var Service1 = new AdWordsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential1,
                    ApplicationName = "FialkovDigitalSandbox"
                });
            */

            /*
                var config = new GoogleAdsConfig
                {
                    DeveloperToken = Config.DeveloperToken,
                
                    OAuth2ClientId = Config.ClientId,
                    OAuth2ClientSecret = Config.ClientSecret,
                    OAuth2RefreshToken = this.Config.RefreshToken,
                    LoginCustomerId = Config.ManagerId.ToString(),

                    ////OAuth2Mode = OAuth2Flow.SERVICE_ACCOUNT,
                    ////LoginCustomerId = Config.ManagerId.ToString(),
                    ////OAuth2SecretsJsonPath = keyPath,
                    ////OAuth2PrnEmail = "fialkovdigitalsandbox@fialkovdigitalsandbox.iam.gserviceaccount.com",                    

                    ////OAuth2Mode = OAuth2Flow.APPLICATION,
                    ////OAuth2ClientId = Config.ClientId,
                    ////OAuth2ClientSecret = Config.ClientSecret

                    ////OAuth2Mode = OAuth2Flow.SERVICE_ACCOUNT,
                    ////OAuth2SecretsJsonPath = keyPath,
                    ////OAuth2PrnEmail = "fialkovdigitalsandbox@tests-343908.iam.gserviceaccount.com",
                    ////OAuth2Scope = "https://www.googleapis.com/auth/adwords"

                }; 
            */
            #endregion

            var config = new GoogleAdsConfig
            {
                DeveloperToken = Config.DeveloperToken,                
                OAuth2ClientId = Config.ClientId,
                OAuth2ClientSecret = Config.ClientSecret,
                OAuth2RefreshToken = this.Config.RefreshToken,
                LoginCustomerId = Config.ManagerId.ToString(),
            };

            var keyPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{Config.CredentialsFileName}";
            /// var credential = GoogleCredential.FromFile(keyPath).CreateScoped(SCOPES);

            var config1 = new GoogleAdsConfig
            {
                DeveloperToken = Config.DeveloperToken,
                LoginCustomerId = Config.ManagerId.ToString(),
                OAuth2Mode = OAuth2Flow.SERVICE_ACCOUNT,
                OAuth2PrnEmail = "fialkovdigital@rok-n2s.iam.gserviceaccount.com",
                OAuth2SecretsJsonPath = keyPath
            };

            this.Service = new GoogleAdsClient(config1);
        }

        private void RefreshToken() {

            ///var keyPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{Config.CredentialsFileName}";
            ///var credential1 = GoogleCredential.FromFile(keyPath).CreateScoped(SCOPES);

            // OAuth Process
            if (string.IsNullOrEmpty(this.Config.RefreshToken))
            {
                ///var accessToken = credential.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;
                ///Console.WriteLine(accessToken);

                /**/
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { 
                        ClientId = Config.ClientId, 
                        ClientSecret = Config.ClientSecret 
                    },
                    new[] { 
                        "https://www.googleapis.com/auth/adwords" 
                    },
                    "user",
                    CancellationToken.None
                ).Result;
                
                Console.WriteLine(JsonConvert.SerializeObject(credential.Token));
                var refreshToken = credential.Token.RefreshToken;
                var accessToken = credential.Token.AccessToken;
                this.Config.RefreshToken = refreshToken;
                
            }
        }
    }
}
