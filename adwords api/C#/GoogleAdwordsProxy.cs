using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.Util.Reports;
using Google.Api.Ads.AdWords.v201809;
using Google.Api.Ads.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoogleAdwordsAPI
{
    public class GoogleAdwordsProxy
    { 
        private static long MICRO_UNIT = 1000000;
        private static int DEFAULT_ROWS_COUNT = 50;
        private AdWordsUser user;        

        public GoogleAdwordsProxy()
        {
            this.LoadConfigUser();
        }

        private bool LoadConfigUser()
        {
            try {
                this.user = new AdWordsUser();
                return true;
            }
            catch {
                return false;
            }
        }

        private List<CampaignDetails> SearchCampaigns(int RowsCount, Predicate Filter = null)
        {
            List<Predicate> filters = null;
            if (Filter != null)
                filters = new List<Predicate>() { Filter };         
            return this.SearchCampaigns(RowsCount, filters);
        }

        private List<CampaignDetails> SearchCampaigns(int RowsCount, List<Predicate> Filters)
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201809.CampaignService);
            
            var campaignFields = Campaign.Fields.All.Select(x => x.FieldName);
            var BudgetFields = new string[] {
                    Budget.Fields.BudgetId
            };

            var fields = new List<string>();
            fields.AddRange(campaignFields);
            fields.AddRange(BudgetFields);

            var selector = new Selector {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging {
                    numberResults = RowsCount,
                    startIndex = 0
                }
            };

            if (Filters != null && Filters.Count > 0)
                selector.predicates = Filters.ToArray();

            var campaignPage = service.get(selector);
            if (campaignPage.entries == null)
                return null;

            return campaignPage.entries.Select(x => 
                new CampaignDetails
                {
                    Id = x.id,
                    Name = x.name,
                    StartDate = new DateTime?(this.DateParse(x.startDate)),
                    EndDate = new DateTime?(this.DateParse(x.endDate)),
                    Status = (eEntityStatus)x.status,
                    Type = (eCampaignType)x.advertisingChannelType,
                    BudgetId = x.budget.budgetId                    
                }).ToList();
        }
        
        private List<AdGroupDetails> SearchAdGroups(Predicate Filter = null)
        {
            return this.SearchAdGroups(DEFAULT_ROWS_COUNT, Filter);
        }

        private List<AdGroupDetails> SearchAdGroups(int RowsCount, Predicate Filter)
        {
            List<Predicate> filters = null;
            if (Filter != null)
                filters = new List<Predicate>() { Filter };
            return this.SearchAdGroups(RowsCount, filters);
        }

        private List<AdGroupDetails> SearchAdGroups(int RowsCount, List<Predicate> Filters)
        {
            var service = (AdGroupService)this.user.GetService(AdWordsService.v201809.AdGroupService);

            var adGroupFields = AdGroup.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(adGroupFields);

            var selector = new Selector {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging {
                    numberResults = RowsCount,
                    startIndex = 0
                }
            };

            if (Filters != null && Filters.Count > 0)
                selector.predicates = Filters.ToArray();
         
            var adGroupPage = service.get(selector);
            if (adGroupPage.entries == null)
                return null;
            
            return adGroupPage.entries.Select(x => 
                new AdGroupDetails {
                    CampaignId = x.campaignId,
                    Id = x.id,
                    Name = x.name,
                    Status = (eEntityStatus)x.status
                }).ToList();
        }

        private List<BudgetDetails> SearchBudgets(Predicate Filter = null)
        {
            return this.SearchBudgets(DEFAULT_ROWS_COUNT, Filter);
        }

        private List<BudgetDetails> SearchBudgets(int RowsCount, Predicate Filter)
        {
            List<Predicate> filters = null;
            if (Filter != null)
                filters = new List<Predicate>() { Filter };
            return this.SearchBudgets(RowsCount, filters);
        }

        private List<BudgetDetails> SearchBudgets(int RowsCount, List<Predicate> Filters)
        {
            var service = (BudgetService)this.user.GetService(AdWordsService.v201809.BudgetService);

            var budgetFields = Budget.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(budgetFields);

            var selector = new Selector {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging{
                    numberResults = RowsCount,
                    startIndex = 0
                }
            };

            if (Filters != null && Filters.Count > 0)
                selector.predicates = Filters.ToArray();
      
            var budgetPage = service.get(selector);
            if (budgetPage.entries == null)
                return null;

            return budgetPage.entries.Select(x =>
                new BudgetDetails
                {
                    Id = x.budgetId,
                    BudgetAmount = (float)(x.amount.microAmount / MICRO_UNIT),
                    BudgetName = x.name
                }).ToList();
        }

        private List<string> SearchAds(Predicate Filter = null)
        {
            return this.SearchAds(DEFAULT_ROWS_COUNT, Filter);
        }

        private List<string> SearchAds(int RowsCount, Predicate Filter)
        {
            List<Predicate> filters = null;
            if (Filter != null)
                filters = new List<Predicate>() { Filter };
            return this.SearchAds(RowsCount, filters);
        }

        private List<string> SearchAds(int RowsCount, List<Predicate> Filters)
        {
            var service = (AdGroupAdService)this.user.GetService(AdWordsService.v201809.AdGroupAdService);

            var campaignFields = new string[] {
                    Campaign.Fields.Id,
                    Campaign.Fields.Name
            };

            var imageAdFields = ResponsiveDisplayAd.Fields.All.Select(x => x.FieldName);
            var textAdFields = ExpandedTextAd.Fields.All.Select(x => x.FieldName);
            
            var fields = new List<string>();
            fields.AddRange(campaignFields);
            fields.AddRange(imageAdFields);
            fields.AddRange(textAdFields);

            var selector = new Selector {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging{
                    numberResults = RowsCount,
                    startIndex = 0
                }
            };

            if (Filters != null && Filters.Count > 0)
                selector.predicates = Filters.ToArray();
            
            var adGroupAdPage = service.get(selector);
            if (adGroupAdPage.entries == null)  
                return null;

            return adGroupAdPage.entries.Select(x => x.ad.AdType).ToList();            
        }

        public int CountCampaigns()
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201809.CampaignService);


            var selector = new Selector
            {
                fields = new string[] { Campaign.Fields.Id },
                paging = new Paging
                {
                    numberResults = 0,
                    startIndex = 0
                }
            };

            var campaignPage = service.get(selector);
            return campaignPage == null ? 0 : campaignPage.totalNumEntries;
        }

        public List<CampaignDetails> GetTopXCampaigns()
        {
            return this.SearchCampaigns(DEFAULT_ROWS_COUNT);
        }

        public List<CampaignDetails> GetALLCampaigns()
        {
            var campaignsQuantity = this.CountCampaigns();
            return this.SearchCampaigns(campaignsQuantity);
        }

        public List<CampaignDetails> GetCampaignsByNamePattern(string Pattern)
        {
            var filter = new Predicate {
                field = "Name",
                @operator = PredicateOperator.CONTAINS,
                values = new string[] {
                    Pattern
                }
            };

            return this.SearchCampaigns(DEFAULT_ROWS_COUNT, filter);
        }

        public CampaignDetails GetCampaignByName(string CampaignName)
        {
            var filter = new Predicate {
                field = "Name",
                @operator = PredicateOperator.EQUALS,
                values = new string[] {
                    CampaignName
                }
            };

            var campaigns = this.SearchCampaigns(1, filter);
            if (campaigns == null)
                return null;            
            return campaigns.FirstOrDefault();
        }

        public AdGroupDetails GetAdGroupByName(string AdGroupName)
        {
            var filter = new Predicate {
                field = "Name",
                @operator = PredicateOperator.EQUALS,
                values = new string[]{
                    AdGroupName
                }
            };

            var adGroups = this.SearchAdGroups(1, filter);
            if (adGroups == null)            
                return null;         
            return adGroups.FirstOrDefault();
        }

        public List<string> GetCampaignAds(long CampaignId)
        {
            var filter = new Predicate {
                field = "CampaignId",
                @operator = PredicateOperator.EQUALS,
                values = new string[] {
                    CampaignId.ToString()
                }
            };

            return this.SearchAds(filter);
        }

        public BudgetDetails GetBudgetByName(string BudgetName)
        {
            var filter = new Predicate {
                field = "BudgetName",
                @operator = PredicateOperator.EQUALS,
                values = new string[] {
                    BudgetName
                }
            };

            var budgets = this.SearchBudgets(1, filter);
            if (budgets == null)            
                return null;
            return budgets.FirstOrDefault();
        }

        public bool IsCampaignExists(string CampaignName, out long Id)
        {
            Id = 0;
            var filter = new Predicate {
                field = "Name",
                @operator = PredicateOperator.EQUALS,
                values = new string[]{
                    CampaignName
                }
            };

            var campaigns = this.SearchCampaigns(1, filter);
            if (campaigns == null)
                return false;
            
            var campaignDetails = campaigns.FirstOrDefault();
            if (campaignDetails == null)
                return false;
            
            Id = campaignDetails.Id;
            return true;
        }

        public bool IsBudgetExists(string BudgetName, out long Id)
        {
            Id = 0;
            var filter = new Predicate {
                field = "BudgetName",
                @operator = PredicateOperator.EQUALS,
                values = new string[] {
                    BudgetName
                }
            };

            var budgets = this.SearchBudgets(1, filter);
            if (budgets == null)            
                return false;
            
            var budgetDetails = budgets.FirstOrDefault();
            if (budgetDetails == null)            
                return false;
            
            Id = budgetDetails.Id;
            return true;
        }

        public long SaveCampaign(CampaignDetails Details)
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201809.CampaignService);

            var campaign = new Campaign {
                name = Details.Name,
                status = (CampaignStatus)Details.Status,
                advertisingChannelType = (AdvertisingChannelType)Details.Type,
                biddingStrategyConfiguration = new BiddingStrategyConfiguration {
                    biddingStrategyType = BiddingStrategyType.MANUAL_CPC,                    
                },
                budget = new Budget {
                    budgetId = Details.BudgetId
                },                             
            };

            // NEW Campaign
            if (Details.Id == 0) {
                // Location Settings
                campaign.settings = new Setting[] {
                    new GeoTargetTypeSetting {
                        positiveGeoTargetType = GeoTargetTypeSettingPositiveGeoTargetType.LOCATION_OF_PRESENCE
                    }
                };
            }

            /*
                settings = new Setting[] { 
                    // dynamic ad settings
                    new DynamicSearchAdsSetting {
                        domainName = "example.com",
                        languageCode = "en",                        
                        pageFeed = new PageFeed {
                            feedIds = new long[] {
                                Details.CustomizerFeedId
                            }
                        }
                    }
                }, 
            */

            if (Details.StartDate.HasValue)
                campaign.startDate = this.DateParse(Details.StartDate.Value);
            
            if (Details.EndDate.HasValue)
                campaign.endDate = this.DateParse(Details.EndDate.Value);
            
            if (Details.Id > 0)
                campaign.id = Details.Id;

            var operations = new CampaignOperation[] {
                new CampaignOperation
                {
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = campaign
                }
            };

            var campaignReturnValue = service.mutate(operations);
            if (campaignReturnValue == null || campaignReturnValue.value == null || campaignReturnValue.value.Length == 0)            
                return 0;            
            return campaignReturnValue.value.First().id;
        }

        public long SaveBudget(BudgetDetails Details)
        {
            var service = (BudgetService)this.user.GetService(AdWordsService.v201809.BudgetService);

            var budget = new Budget
            {
                name = Details.BudgetName,
                deliveryMethod = BudgetBudgetDeliveryMethod.STANDARD,
                amount = new Money{
                    microAmount = (long)(Details.BudgetAmount * (float)MICRO_UNIT)
                }
            };

            if (Details.Id > 0)            
                budget.budgetId = Details.Id;

            var operations = new BudgetOperation[] {
                new BudgetOperation {
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = budget
                }
            };

            var budgetReturnValue = service.mutate(operations);
            if (budgetReturnValue == null || budgetReturnValue.value == null || budgetReturnValue.value.Length == 0)
                return 0;
            return budgetReturnValue.value.First().budgetId;
        }

        public long SaveAdGroup(AdGroupDetails Details)
        {
            var service = (AdGroupService)this.user.GetService(AdWordsService.v201809.AdGroupService);

            var adGroup = new AdGroup {
                name = Details.Name,
                status = AdGroupStatus.ENABLED,
                campaignId = Details.CampaignId
            };
            
            if (Details.Id > 0)
                adGroup.id = Details.Id;

            // NEW 
            if (Details.Id == 0) {
                var cpcBid = new CpcBid {
                    bid = new Money {
                        microAmount = (long)(Details.DefaultCpc * (float)MICRO_UNIT)
                    }
                };

                adGroup.biddingStrategyConfiguration = new BiddingStrategyConfiguration
                {
                    bids = new Bids[] { cpcBid }
                };
            }

            var operations = new AdGroupOperation[]{
                new AdGroupOperation{
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = adGroup
                }
            };

            var adGroupReturnValue = service.mutate(operations);
            if (adGroupReturnValue == null || adGroupReturnValue.value == null || adGroupReturnValue.value.Length == 0)            
                return 0;            
            return adGroupReturnValue.value.First().id;
        }

        public long SaveExpandedTextAd(ExpandedTextAdDetails Details)
        {
            var service = (AdGroupAdService)this.user.GetService(AdWordsService.v201809.AdGroupAdService);

            var expandedTextAd = new ExpandedTextAd {
                headlinePart1 = Details.Header1,
                headlinePart2 = Details.Header2,
                description = Details.Description,
                finalUrls = new string[] {
                        Details.URL
                }
            };

            if (Details.Id > 0)           
                expandedTextAd.id = Details.Id;
            
            var adGroupAd = new AdGroupAd();
            adGroupAd.ad = expandedTextAd;
            adGroupAd.adGroupId = Details.AdGroupId;

            var operations = new AdGroupAdOperation[] {
                new AdGroupAdOperation {
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = adGroupAd
                }
            };

            var adGroupAdReturnValue = service.mutate(operations);
            if (adGroupAdReturnValue == null || adGroupAdReturnValue.value == null || adGroupAdReturnValue.value.Length == 0)
                return 0;
            return adGroupAdReturnValue.value.First().ad.id;
        }

        public long SaveResponsiveImageAd(ResponsiveImageAdDetails Details)
        {
            var service = (AdGroupAdService)this.user.GetService(AdWordsService.v201809.AdGroupAdService);

            var responsiveDisplayAd = new ResponsiveDisplayAd {
                businessName = Details.BusinessName,
                shortHeadline = Details.Header1,
                longHeadline = Details.Header2,
                description = Details.Description,
                finalUrls = new string[]{
                    Details.URL
                },
                marketingImage = new Image {
                    mediaId = this.UploadImage(Details.ImageURL)
                },
                squareMarketingImage = new Image {
                    mediaId = this.UploadImage(Details.ImageSquareURL)
                }
            };

            if (Details.Id > 0)
                responsiveDisplayAd.id = Details.Id;
            
            var adGroupAd = new AdGroupAd();
            adGroupAd.ad = responsiveDisplayAd;
            adGroupAd.adGroupId = Details.AdGroupId;

            var operations = new AdGroupAdOperation[] {
                new AdGroupAdOperation {
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = adGroupAd
                }
            };

            var adGroupAdReturnValue = service.mutate(operations);
            if (adGroupAdReturnValue == null || adGroupAdReturnValue.value == null || adGroupAdReturnValue.value.Length == 0)            
                return 0;            
            return adGroupAdReturnValue.value.First().ad.id;
        }

        public long SaveKeyword(KeywordDetails Details)
        {
            var ids = this.SaveKeywords(new List<KeywordDetails> {
                Details
            });

            if (ids == null)            
                return 0;           
            return ids.FirstOrDefault();
        }

        public List<long> SaveKeywords(IEnumerable<KeywordDetails> KeywordsDetails)
        {
            if (KeywordsDetails == null || KeywordsDetails.Count<KeywordDetails>() == 0)            
                return null;
            
            var service = (AdGroupCriterionService)this.user.GetService(AdWordsService.v201809.AdGroupCriterionService);

            var operations = new List<AdGroupCriterionOperation>();
            foreach (var kd in KeywordsDetails)
            {
                var keyword = new Keyword {
                    text = kd.Value,
                    matchType = (KeywordMatchType)kd.MatchType
                };

                if (kd.Id > 0)
                    keyword.id = kd.Id;
                
                var operand = new BiddableAdGroupCriterion {
                    criterion = keyword,
                    adGroupId = kd.adGroupId
                };

                operations.Add(new AdGroupCriterionOperation {
                    @operator = (kd.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = operand
                });
            }

            var adGroupCriterionReturnValue = service.mutate(operations.ToArray());
            if (adGroupCriterionReturnValue == null || adGroupCriterionReturnValue.value == null || adGroupCriterionReturnValue.value.Length == 0)            
                return null;
            
            return adGroupCriterionReturnValue.value.Select(x => x.criterion.id).ToList();
        }

        public long SaveNegativeKeyword(KeywordDetails Details)
        {
            var service = (AdGroupCriterionService)this.user.GetService(AdWordsService.v201809.AdGroupCriterionService);

            var keyword = new Keyword {
                text = Details.Value,
                matchType = (KeywordMatchType)Details.MatchType
            };

            if (Details.Id > 0)
                keyword.id = Details.Id;
            
            var negativeAdGroupCriterion = new NegativeAdGroupCriterion();
            negativeAdGroupCriterion.criterion = keyword;
            negativeAdGroupCriterion.adGroupId = Details.adGroupId;

            var operations = new AdGroupCriterionOperation[] {
                new AdGroupCriterionOperation {
                    @operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = negativeAdGroupCriterion
                }
            };

            var adGroupCriterionReturnValue = service.mutate(operations);
            if (adGroupCriterionReturnValue == null || adGroupCriterionReturnValue.value == null || adGroupCriterionReturnValue.value.Length == 0)
                return 0;
            return adGroupCriterionReturnValue.value.First().criterion.id;
        }

        public bool AddLocations(long CampaignId, IEnumerable<int> LocationIds)
        {
            if (LocationIds == null || LocationIds.Count() == 0)
                return true;
            
            var criterions = new List<CampaignCriterion>();
            foreach (var locationId in LocationIds) {
                criterions.Add(new CampaignCriterion{
                    criterion = new Location {
                        id = (long)locationId
                    },
                    campaignId = CampaignId
                });
            }

            var responseIds = this.AddCampaignCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        /* 
            Content exclusions
            https://developers.google.com/adwords/api/docs/reference/v201809/CampaignCriterionService.ContentLabel#contentlabeltype
        */
        public bool AddContentExclusions(long CampaignId, IEnumerable<eContentExclusionType> ExclusionTypes)
        {
            if (ExclusionTypes == null || ExclusionTypes.Count() == 0)
                return true;
            
            var criterions = new List<NegativeCampaignCriterion>();            
            foreach (var exclusionType in ExclusionTypes) {
                criterions.Add(new NegativeCampaignCriterion {
                    campaignId = CampaignId,
                    criterion = new ContentLabel {
                        contentLabelType = (ContentLabelType)exclusionType
                    }
                });
            }

            var responseIds = this.AddCampaignNegativeCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddProximityToCity(long CampaignId, ProximityDetails Details)
        {
            var criterions = new List<CampaignCriterion>();
            criterions.Add(new CampaignCriterion
            {
                criterion = new Proximity
                {
                    radiusDistanceUnits = ProximityDistanceUnits.KILOMETERS,
                    radiusInUnits = Details.RadiusInKM,

                    // address or City                     
                    address = new Address {
                        countryCode = Details.CountryCode,
                        cityName = Details.CityName
                    }
                },
                campaignId = CampaignId
            });

            var responseIds = this.AddCampaignCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddProximityToPoint(long CampaignId, ProximityDetails Details)
        {
            if (Details == null || Details.Coordinates == null || Details.Coordinates.IsEmpty)
                return true;

            var criterions = new List<CampaignCriterion>();
            criterions.Add(new CampaignCriterion
            {
                criterion = new Proximity
                {
                    radiusDistanceUnits = ProximityDistanceUnits.KILOMETERS,
                    radiusInUnits = Details.RadiusInKM,

                    // address or geoPoint                     
                    geoPoint = new GeoPoint
                    {
                        latitudeInMicroDegrees = (int)(Details.Coordinates.Lat * MICRO_UNIT),
                        longitudeInMicroDegrees = (int)(Details.Coordinates.Lng * MICRO_UNIT)
                    }
                },
                campaignId = CampaignId
            });

            var responseIds = this.AddCampaignCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public List<ProximityDetails> GetProximities(long CampaignId)
        {
            var service = (CampaignCriterionService)this.user.GetService(AdWordsService.v201809.CampaignCriterionService);

            var selector = new Selector
            {
                fields = new string[] {
                    Criterion.Fields.Id,
                    Criterion.Fields.CriteriaType,
                    Proximity.Fields.RadiusInUnits,
                    Proximity.Fields.GeoPoint,
                    Proximity.Fields.Address,
                    CampaignCriterion.Fields.CampaignId
                },
                predicates = new Predicate[] {
                    // WHERE CampaignId = [CampaignId] AND CriteriaType = PROXIMITY
                    Predicate.Equals(CampaignCriterion.Fields.CampaignId, CampaignId.ToString()),
                    Predicate.Equals(Criterion.Fields.CriteriaType, CriterionType.PROXIMITY.ToString())
                }
            };

            var proximitiesPage = service.get(selector);
            if (proximitiesPage.entries == null)
                return null;

            return (from entry in proximitiesPage.entries
                    let proximity = (Proximity)entry.criterion
                    select new ProximityDetails
                    {
                        CityName = proximity.address == null ? null : proximity.address.cityName,
                        CountryCode = proximity.address == null ? null : proximity.address.countryCode,
                        Coordinates = proximity.geoPoint == null ? null : new CoordinatesDetails(
                            (float)proximity.geoPoint.latitudeInMicroDegrees / MICRO_UNIT,
                            (float)proximity.geoPoint.longitudeInMicroDegrees / MICRO_UNIT
                        ),
                        RadiusInKM = (int)proximity.radiusInUnits,
                        Id = proximity.id
                    }).ToList();
        }

        public int ClearProximities(long CampaignId)
        {
            var proximities = this.GetProximities(CampaignId);
            if (proximities == null || proximities.Count == 0)
                return 0;

            var campaignCriterions = proximities.Select(x => new CampaignCriterion
            {
                campaignId = CampaignId,
                criterion = new Proximity { id = x.Id }
            }).ToList();

            return DeleteCampaignCriterions(campaignCriterions);            
        }

        public bool AddLanguages(long CampaignId, IEnumerable<int> LanguageIds)
        {
            if (LanguageIds == null || LanguageIds.Count() == 0)            
                return true;
            
            List<CampaignCriterion> criterions = new List<CampaignCriterion>();
            foreach (var languageId in LanguageIds) {
                criterions.Add(new CampaignCriterion {
                    criterion = new Language {
                        id = (long)languageId
                    },
                    campaignId = CampaignId
                });
            }

            var responseIds = this.AddCampaignCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddGenders(long AdgroupId, IEnumerable<int> GenderIds)
        {
            if (GenderIds == null || GenderIds.Count() == 0)
                return true;
            
            var criterions = new List<AdGroupCriterion>();
            foreach (var genderId in GenderIds) {
                criterions.Add(new BiddableAdGroupCriterion {
                    criterion = new Gender {
                        id = (long)genderId
                    },
                    adGroupId = AdgroupId
                });
            }

            var responseIds = this.AddAdgroupCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddAgeRanges(long AdgroupId, IEnumerable<int> AgeRangeIds)
        {
            if (AgeRangeIds == null || AgeRangeIds.Count() == 0)
                return true;
            
            var criterions = new List<AdGroupCriterion>();
            foreach (var ageRangeId in AgeRangeIds) {
                criterions.Add(new BiddableAdGroupCriterion {
                    criterion = new AgeRange {
                        id = (long)ageRangeId
                    },
                    adGroupId = AdgroupId
                });
            }

            var responseIds = this.AddAdgroupCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddInterests(long AdgroupId, IEnumerable<int> InterestsIds)
        {
            if (InterestsIds == null || InterestsIds.Count() == 0)
                return true;
            
            var criterions = new List<AdGroupCriterion>();
            foreach (var interestsId in InterestsIds) {
                criterions.Add(new BiddableAdGroupCriterion {
                    criterion = new CriterionUserInterest {
                        userInterestId = (long)interestsId
                    },
                    adGroupId = AdgroupId
                });
            }

            var responseIds = this.AddAdgroupCriterions(criterions);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddCallouts(long CampaignId, IEnumerable<string> Callouts)
        {
            if (Callouts == null || Callouts.Count() == 0)            
                return true;
            
            var feedItems = new List<ExtensionFeedItem>();
            foreach (var callout in Callouts.Where(x => !string.IsNullOrWhiteSpace(x))) {
                feedItems.Add(new CalloutFeedItem {
                    calloutText = callout
                });
            }

            if (feedItems.Count == 0)
                return false;
            
            var extensionSetting = new CampaignExtensionSetting {
                campaignId = CampaignId,
                extensionType = FeedType.CALLOUT,
                extensionSetting = new ExtensionSetting {
                    extensions = feedItems.ToArray()
                }
            };

            var responseIds = this.AddCampaignExtensions(extensionSetting);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddSnippets(long CampaignId, IEnumerable<SnippetDetails> Snippets)
        {
            var validSnippets = new List<string> {
                "Brands",
                "Amenities",
                "Styles",
                "Types",
                "Destinations",
                "Services",
                "Courses",
                "Neighborhoods",
                "Shows",
                "Insurance coverage",
                "Degree programs",
                "Featured Hotels",
                "Models"
            };

            if (Snippets == null || Snippets.Count() == 0)
                return true;
            
            var feedItems = new List<ExtensionFeedItem>();
            foreach (var snippet in Snippets)
            {
                if (snippet.Values != null && snippet.Values.Count >= 3 && validSnippets.Contains(snippet.Header)) {
                    feedItems.Add(new StructuredSnippetFeedItem {
                        header = snippet.Header,
                        values = snippet.Values.ToArray()
                    });
                }
            }

            if (feedItems.Count == 0)
                return false;
            
            var extensionSetting = new CampaignExtensionSetting {
                campaignId = CampaignId,
                extensionType = FeedType.STRUCTURED_SNIPPET,
                extensionSetting = new ExtensionSetting {
                    extensions = feedItems.ToArray()
                }
            };

            var responseIds = this.AddCampaignExtensions(extensionSetting);
            return responseIds != null && responseIds.Count() > 0;
        }

        public bool AddSitelinks(long CampaignId, IEnumerable<SitelinkDetails> Sitelinks) 
        {
            if (Sitelinks == null || Sitelinks.Count() == 0)            
                return true;
            
            var feedItems = new List<ExtensionFeedItem>();
            foreach (var sitelink in Sitelinks)
            {
                feedItems.Add(new SitelinkFeedItem
                {
                    sitelinkText = sitelink.SitelinkText,
                    sitelinkFinalUrls = new UrlList {
                        urls = new string[] {
                            sitelink.SitelinkURL
                        }
                    }
                });
            }

            if (feedItems.Count == 0)            
                return false;
            
            var extensionSetting = new CampaignExtensionSetting
            {
                campaignId = CampaignId,
                extensionType = FeedType.SITELINK,
                extensionSetting = new ExtensionSetting
                {
                    extensions = feedItems.ToArray()
                }
            };
            var responseIds = this.AddCampaignExtensions(extensionSetting);
            return responseIds != null && responseIds.Count() > 0;
        }

        private long AddCampaignCriterions(CampaignCriterion Criterion)
        {
            var responseIds = this.AddCampaignCriterions(new List<CampaignCriterion> {
                Criterion
            });

            if (responseIds == null)           
                return 0;            
            return responseIds.FirstOrDefault();
        }

        private IEnumerable<long> AddCampaignCriterions(List<CampaignCriterion> Criterions)
        {
            var service = (CampaignCriterionService)this.user.GetService(AdWordsService.v201809.CampaignCriterionService);

            var operations = new List<CampaignCriterionOperation>();
            foreach (var criterion in Criterions){
                operations.Add(new CampaignCriterionOperation {
                    @operator = Operator.ADD,
                    operand = criterion
                });
            }

            var campaignCriterionReturnValue = service.mutate(operations.ToArray());
            if (campaignCriterionReturnValue == null || campaignCriterionReturnValue.value == null || campaignCriterionReturnValue.value.Length == 0)           
                return null;       
                
            return campaignCriterionReturnValue.value.Select(x => x.criterion.id).ToList();
        }

        private IEnumerable<long> AddCampaignNegativeCriterions(List<NegativeCampaignCriterion> Criterions)
        {
            var service = (CampaignCriterionService)this.user.GetService(AdWordsService.v201809.CampaignCriterionService);

            var operations = new List<CampaignCriterionOperation>();
            foreach (var criterion in Criterions) {
                operations.Add(new CampaignCriterionOperation {
                    @operator = Operator.ADD,
                    operand = criterion
                });
            }

            var campaignCriterionReturnValue = service.mutate(operations.ToArray());
            if (campaignCriterionReturnValue == null || campaignCriterionReturnValue.value == null || campaignCriterionReturnValue.value.Length == 0)
                return null;

            return campaignCriterionReturnValue.value.Select(x => x.criterion.id).ToList();
        }

        private int DeleteCampaignCriterions(List<CampaignCriterion> Criterions)
        {
            var service = (CampaignCriterionService)this.user.GetService(AdWordsService.v201809.CampaignCriterionService);

            var operations = new List<CampaignCriterionOperation>();
            foreach (var criterion in Criterions)
            {
                operations.Add(new CampaignCriterionOperation
                {
                    @operator = Operator.REMOVE,
                    operand = criterion
                });
            }

            var campaignCriterionReturnValue = service.mutate(operations.ToArray());
            if (campaignCriterionReturnValue == null || campaignCriterionReturnValue.value == null || campaignCriterionReturnValue.value.Length == 0)
                return 0;

            return campaignCriterionReturnValue.value.Length;
        }

        private long AddAdgroupCriterions(AdGroupCriterion Criterion)
        {
            var responseIds = this.AddAdgroupCriterions(new List<AdGroupCriterion>{
                Criterion
            });

            if (responseIds == null)            
                return 0;            
            return responseIds.FirstOrDefault();
        }

        private IEnumerable<long> AddAdgroupCriterions(List<AdGroupCriterion> Criterions)
        {
            var service = (AdGroupCriterionService)this.user.GetService(AdWordsService.v201809.AdGroupCriterionService);

            var operations = new List<AdGroupCriterionOperation>();
            foreach (var criterion in Criterions) {
                operations.Add(new AdGroupCriterionOperation {
                    @operator = Operator.ADD,
                    operand = criterion
                });
            }

            var adGroupCriterionReturnValue = service.mutate(operations.ToArray());
            if (adGroupCriterionReturnValue == null || adGroupCriterionReturnValue.value == null || adGroupCriterionReturnValue.value.Length == 0)            
                return null;
            
            return adGroupCriterionReturnValue.value.Select(x => x.criterion.id).ToList();
        }

        private IEnumerable<long> AddCampaignExtensions(CampaignExtensionSetting Extension)
        {
            var service = (CampaignExtensionSettingService)this.user.GetService(AdWordsService.v201809.CampaignExtensionSettingService);

            var operations = new CampaignExtensionSettingOperation[] {
                    new CampaignExtensionSettingOperation {
                        @operator = Operator.ADD,
                        operand = Extension
                    }
            };

            var campaignExtensionSettingReturnValue = service.mutate(operations);
            if (campaignExtensionSettingReturnValue == null || campaignExtensionSettingReturnValue.value == null || campaignExtensionSettingReturnValue.value.Length == 0)            
                return null;
            
            return campaignExtensionSettingReturnValue.value.First().extensionSetting.extensions.Select(x => x.feedId).ToList();
        }

        public long UpdateCampaignStatus(long CampaignId, eEntityStatus Status)
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201809.CampaignService);

            var operand = new Campaign {
                status = (CampaignStatus)Status,
                id = CampaignId
            };

            var operations = new CampaignOperation[] {
                new CampaignOperation {
                    @operator = Operator.SET,
                    operand = operand
                }
            };

            var campaignReturnValue = service.mutate(operations);
            if (campaignReturnValue == null || campaignReturnValue.value == null || campaignReturnValue.value.Length == 0)            
                return 0;           
            return campaignReturnValue.value.First().id;
        }

        /*  NOTE! 
            [GetManagedAccounts] use the ManagedCustomerService service to get the accounts under the MCC 
            [GetRelatedAccounts] use the CustomerService (getCustomers method) to get the related accounts (the accounts who gave you access)
        */

        #region MCC        
        public List<ManagedAccount> GetManagedAccounts()
        {             
            var service = (ManagedCustomerService)user.GetService(AdWordsService.v201809.ManagedCustomerService);
             
            var fields = new string[] {
                    ManagedCustomer.Fields.CustomerId,
                    ManagedCustomer.Fields.Name,
                    ManagedCustomer.Fields.CanManageClients
            };

            var selector = new Selector
            {
                fields = fields.ToArray()                
            };

            var accountsPage = service.get(selector);
            if (accountsPage.entries == null)
                return null;

            return accountsPage.entries.Select(x =>
                new ManagedAccount
                {
                    Id = x.customerId,
                    Name = x.name,                    
                    DateTimeZone = x.dateTimeZone,
                    CurrencyCode = x.currencyCode
                }).ToList();
        }

        public ManagedAccount CreateManagedAccount(ManagedAccount Details)
        {
            var service = (ManagedCustomerService)user.GetService(AdWordsService.v201809.ManagedCustomerService);

            var account = new ManagedCustomer {
                name = Details.Name,
                dateTimeZone = Details.DateTimeZone,
                currencyCode = Details.CurrencyCode
            };

            var operations = new ManagedCustomerOperation[] {
                new ManagedCustomerOperation {                    
                    @operator = Operator.ADD,
                    operand = account
                }
            };

            var returnValue = service.mutate(operations);
            if (returnValue == null || returnValue.value == null || returnValue.value.Length == 0)
                return null;

            var createdAccount = returnValue.value.First();
            return new ManagedAccount {
                Id = createdAccount.customerId,
                Name = createdAccount.name,
                DateTimeZone = createdAccount.dateTimeZone,
                CurrencyCode = createdAccount.currencyCode
            };
        }
        #endregion

        public bool SwitchToAccount(long AccountId) {
            var customerId = this.AccountId2CustomerId(AccountId);
            return this.SetActiveAccount(customerId);
        }

        public bool SwitchToConfigAccount() {
            return this.LoadConfigUser();
        }

        // TODO check
        public List<RelatedAccount> GetRelatedAccounts()
        {
            var service = (CustomerService)user.GetService(AdWordsService.v201809.CustomerService);

            var fields = new string[] {
                    ManagedCustomer.Fields.CustomerId,
                    ManagedCustomer.Fields.Name,
                    ManagedCustomer.Fields.CanManageClients
            };

            var selector = new Selector
            {
                fields = fields.ToArray()
            };

            var customers = service.getCustomers();
            if (customers == null)
                return null;

            return customers.Select(x =>
                new RelatedAccount
                {
                    Id = x.customerId,
                    Name = x.descriptiveName
                }).ToList();
        }

        #region AccountChanges
        public List<ChangeDetails> GetALLAccountsChangesAsync(DateTime From, DateTime To)
        {
            var changes = new List<ChangeDetails>();

            var accounts = this.GetManagedAccounts();
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    try
                    {
                        Console.WriteLine("\taccount #{0}", account.Id);

                        var accountChanges = this.GetAccountChangesAsync(From, To, account.Id.ToString());
                        if (accountChanges == null) continue;
                        changes.AddRange(accountChanges);
                    }
                    catch (Exception ex)
                    {
                        changes.Add(new ChangeDetails(account.Id.ToString(), 0, "EXCEPTION", ex.Message));
                    }
                }
            }

            return changes;
        }

        public List<ChangeDetails> GetAccountChangesAsync(DateTime From, DateTime To, string AccountId = null)
        {
            // save the current selected account (probably the mcc root)
            var originalAccountId = this.GetActiveAccount();

            // change active account 
            this.SetActiveAccount(AccountId);

            ConcurrentBag<ChangeDetails> changes = null; // Thread-Safe Generic List

            try
            {
                var service = (CustomerSyncService)this.user.GetService(AdWordsService.v201809.CustomerSyncService);

                // get all campaigns                 
                var campaigns = this.GetALLCampaigns();
                if (campaigns == null)
                    return changes.ToList();

                var campaignIds = campaigns.Select(x => x.Id).ToArray();

                var selector = new CustomerSyncSelector
                {
                    campaignIds = campaignIds,
                    dateTimeRange = new DateTimeRange
                    {
                        min = DateTimeParse(From),
                        max = DateTimeParse(To)
                    },
                };

                var changesDataValue = service.get(selector);
                if (changesDataValue == null || changesDataValue.changedCampaigns == null || changesDataValue.changedCampaigns.Length == 0)
                    return changes.ToList();

                changes = new ConcurrentBag<ChangeDetails>();

                /*  
                    CHANGES LISTS
                    -------------
                    ChangeStatus campaignChangeStatus
                    long[] addedCampaignCriteria
                    long[] removedCampaignCriteria 
                    long[] changedFeeds
                    long[] removedFeeds

                    AdGroupChangeData[] changedAdGroups 
                    --
                    ChangeStatus adGroupChangeStatus
                    long[] changedAdGroupBidModifierCriteria
                    long[] removedAdGroupBidModifierCriteria
                    long[] changedAds
                    long[] changedCriteria
                    long[] removedCriteria
                    long[] changedFeeds
                    long[] removedFeeds  
                */
                Parallel.ForEach(changesDataValue.changedCampaigns, (campaignChanges) =>
                {
                    // -- CAMPAIGN CHANGES -- 
                    CollectCampaignChanges(ref changes, campaignChanges, AccountId);

                    // -- ADGROUPS CHANGES -- 
                    if (campaignChanges.changedAdGroups != null)
                        Parallel.ForEach(campaignChanges.changedAdGroups, (adGroupChanges) =>
                        {
                            CollectAdgroupChanges(ref changes, adGroupChanges, AccountId);
                        });
                });
            }
            finally
            {
                // change-back to the original account 
                this.SetActiveAccount(originalAccountId);
            }

            return changes.ToList();
        }

        private Dictionary<long, string> GetAdgroupCriterias(IEnumerable<long> Ids)
        {
            if (Ids == null || Ids.Count() == 0)
                return null;

            var service = (AdGroupCriterionService)this.user.GetService(AdWordsService.v201809.AdGroupCriterionService);

            var criterionFields = AdGroupCriterion.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(criterionFields);

            var selector = new Selector
            {
                fields = this.FieldsDistinct(fields).ToArray(),
                predicates = new Predicate[] {
                    new Predicate {
                        field = "Id",
                        @operator = PredicateOperator.IN,
                        values = Ids.Select(x => x.ToString()).ToArray()
                    }
                }
            };

            var criterionPage = service.get(selector);
            if (criterionPage.entries == null)
                return null;

            var result = new Dictionary<long, string>();
            foreach (var item in criterionPage.entries)
                result[item.criterion.id] = item.criterion.CriterionType;

            // [{ CriterionId, CriterionTypeName } ...]
            return result;
        }

        private Dictionary<long, string> GetCampaignCriterias(IEnumerable<long> Ids)
        {
            if (Ids == null || Ids.Count() == 0)
                return null;

            var service = (CampaignCriterionService)this.user.GetService(AdWordsService.v201809.CampaignCriterionService);

            var criterionFields = CampaignCriterion.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(criterionFields);

            var selector = new Selector
            {
                fields = this.FieldsDistinct(fields).ToArray(),
                predicates = new Predicate[] {
                    new Predicate {
                        field = "Id",
                        @operator = PredicateOperator.IN,
                        values = Ids.Select(x => x.ToString()).ToArray()
                    }
                }
            };

            var criterionPage = service.get(selector);
            if (criterionPage.entries == null)
                return null;

            var result = new Dictionary<long, string>();
            foreach (var item in criterionPage.entries)
                result[item.criterion.id] = item.criterion.CriterionType;

            // [{ CriterionId, CriterionTypeName } ...]
            return result;
        }

        private void CollectCampaignChanges(ref ConcurrentBag<ChangeDetails> changes, CampaignChangeData campaignChanges, string accountId)
        {
            // status
            if (campaignChanges.campaignChangeStatus == ChangeStatus.NEW)
            {
                changes.Add(new ChangeDetails(accountId, campaignChanges.campaignId, "CAMPAIGN", "CREATED"));
                return; // note: no changes return for NEW status
            }

            if (campaignChanges.campaignChangeStatus == ChangeStatus.FIELDS_CHANGED)
                changes.Add(new ChangeDetails(accountId, campaignChanges.campaignId, "CAMPAIGN", "FIELDS_CHANGED"));

            // criteria (+)                
            if (campaignChanges.addedCampaignCriteria != null)
            {
                var criterias = GetCampaignCriterias(campaignChanges.addedCampaignCriteria);
                foreach (var criteriaId in campaignChanges.addedCampaignCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "ADDED"));
            }

            // criteria (-)                
            if (campaignChanges.removedCampaignCriteria != null)
            {
                var criterias = GetCampaignCriterias(campaignChanges.removedCampaignCriteria);
                foreach (var criteriaId in campaignChanges.removedCampaignCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "REMOVED"));
            }
        }

        private void CollectAdgroupChanges(ref ConcurrentBag<ChangeDetails> changes, AdGroupChangeData adGroupChanges, string accountId)
        {
            // status
            if (adGroupChanges.adGroupChangeStatus == ChangeStatus.NEW)
            {
                changes.Add(new ChangeDetails(accountId, adGroupChanges.adGroupId, "ADGROUP", "CREATED"));
                return; // note: no changes return for NEW status
            }

            if (adGroupChanges.adGroupChangeStatus == ChangeStatus.FIELDS_CHANGED)
                changes.Add(new ChangeDetails(accountId, adGroupChanges.adGroupId, "ADGROUP", "FIELDS_CHANGED"));

            // criteria (+)                        
            if (adGroupChanges.changedCriteria != null)
            {
                var criterias = GetAdgroupCriterias(adGroupChanges.changedCriteria);
                foreach (var criteriaId in adGroupChanges.changedCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "CHANGED"));
            }

            // criteria (-)                        
            if (adGroupChanges.removedCriteria != null)
            {
                var criterias = GetAdgroupCriterias(adGroupChanges.removedCriteria);
                foreach (var criteriaId in adGroupChanges.removedCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "REMOVED"));
            }

            // ads
            if (adGroupChanges.changedAds != null)
                foreach (var adId in adGroupChanges.changedAds)
                    changes.Add(new ChangeDetails(accountId, adId, "AD", "CHANGED"));

            // bids (+)
            if (adGroupChanges.changedAdGroupBidModifierCriteria != null)
            {
                var criterias = GetAdgroupCriterias(adGroupChanges.changedAdGroupBidModifierCriteria);
                foreach (var criteriaId in adGroupChanges.changedAdGroupBidModifierCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "CHANGED"));
            }

            // bids (-)
            if (adGroupChanges.removedAdGroupBidModifierCriteria != null)
            {
                var criterias = GetAdgroupCriterias(adGroupChanges.removedAdGroupBidModifierCriteria);
                foreach (var criteriaId in adGroupChanges.removedAdGroupBidModifierCriteria)
                    changes.Add(new ChangeDetails(accountId, criteriaId, criterias == null ? "CRITERIA" : criterias[criteriaId].ToUpper(), "REMOVED"));
            }
        }
        #endregion

        #region Feeds  
        public CustomizerFeedDetails CreateCustomizerFeed(CustomizerFeedDetails Details)
        {
            // binding: {=FEED_NAME.Property} to bind data to any ad text content 
            // e.g: Header1 = string.Format("{{={0}.Name}}", "TEST_FEED_201710052126")

            var service = (AdCustomizerFeedService)this.user.GetService(AdWordsService.v201809.AdCustomizerFeedService);

            var feedStructure = new AdCustomizerFeed()
            {
                feedName = Details.Name, 
                feedAttributes = Details.Attributes.Select(x => new AdCustomizerFeedAttribute
                {
                    // id = x.Id,
                    name = x.Name,
                    /* note! string max length: 38 characters */
                    type = (AdCustomizerFeedAttributeType)x.Type 
                }).ToArray()
            };

            if (Details.Id > 0)
                feedStructure.feedId = Details.Id;

            var operation = new AdCustomizerFeedOperation
            {
                //@operator = (Details.Id > 0) ? Operator.SET : Operator.ADD,
                @operator = Operator.ADD,
                operand = feedStructure,                
            };

            var addFeedReturnValue = service.mutate(new AdCustomizerFeedOperation[] { operation });
            if (addFeedReturnValue == null || addFeedReturnValue.value == null || addFeedReturnValue.value.Length == 0)
                return null;

            return addFeedReturnValue.value.Take(1).Select(x =>
                new CustomizerFeedDetails
                {
                    Id = x.feedId,
                    Name = x.feedName,
                    Attributes = x.feedAttributes.Select(a => new FeedAttributeDetails(a.name/*, (eFeedAttributeType)a.type*/)
                    {
                        Id = a.id
                    }).ToList()
                }).FirstOrDefault();
        }

        public CustomizerFeedDetails GetCustomizerFeedByName(string FeedName) {
            var service = (AdCustomizerFeedService)this.user.GetService(AdWordsService.v201809.AdCustomizerFeedService);

            var feedFields = AdCustomizerFeed.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(feedFields);
                        
            var selector = new Selector
            {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging
                {
                    numberResults = 1, // 1 item
                    startIndex = 0
                }
            };

            // WHERE FeedName = [FeedName]
            selector.predicates = new Predicate[] {
                new Predicate {
                    field = "FeedName",
                    @operator = PredicateOperator.EQUALS,
                    values = new string[] {
                        FeedName
                    }
                }
            };

            var feedPage = service.get(selector);
            if (feedPage.entries == null)
                return null;

            return feedPage.entries.Take(1).Select(x =>
                new CustomizerFeedDetails
                {
                    Id = x.feedId,
                    Name = x.feedName,
                    Attributes = x.feedAttributes.Select(a => new FeedAttributeDetails(a.name/*, (eFeedAttributeType)a.type*/) {
                        Id = a.id
                    }).ToList()
                }).FirstOrDefault();
        }

        public CustomizerFeedItemDetails GetCustomizerFeedItem(long FeedId)
        {
            var feedItems = this.GetCustomizerFeedItems(1, FeedId);
            if (feedItems == null || feedItems.Count == 0)
                return null;
            return feedItems.FirstOrDefault();
        }
        public List<CustomizerFeedItemDetails> GetCustomizerFeedItems(int RowsCount, long FeedId) {
            var service = (FeedItemService)this.user.GetService(AdWordsService.v201809.FeedItemService);

            var feedItemFields = FeedItem.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(feedItemFields);

            var selector = new Selector
            {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging
                {
                    numberResults = RowsCount,
                    startIndex = 0
                }
            };

            // WHERE FeedId = [FeedId]
            // AND Status IN (ENABLED) 
            selector.predicates = new Predicate[] {
                new Predicate {
                    field = "FeedId",
                    @operator = PredicateOperator.EQUALS,
                    values = new string[] {
                        FeedId.ToString()
                    }
                },
                Predicate.In("Status", new string[] { "ENABLED" }) 
            };

            var feedItemsPage = service.get(selector);
            if (feedItemsPage.entries == null)
                return null;

            return feedItemsPage.entries.Select(x =>
                new CustomizerFeedItemDetails
                {
                    Id = x.feedItemId,
                    FeedId = x.feedId,
                    AttributeBindings = x.attributeValues.Select(
                        a => new CustomizerFeedItemBinding(a.feedAttributeId, eFeedAttributeType.STRING, a.stringValue)
                    ).ToList()
                }).ToList();
        }

        public long SaveCustomizerFeedItem(CustomizerFeedItemDetails FeedItem) {
            var feedItemIds = SaveCustomizerFeedItems(new List<CustomizerFeedItemDetails> {
                FeedItem
            });

            if (feedItemIds == null)
                return 0;
            return feedItemIds.FirstOrDefault();
        }
        public IEnumerable<long> SaveCustomizerFeedItems(List<CustomizerFeedItemDetails> FeedItems) {            
            var service = (FeedItemService)this.user.GetService(AdWordsService.v201809.FeedItemService);

            var operations = new List<FeedItemOperation>();
            foreach (var feedItem in FeedItems)
            {
                var feedContent = new FeedItem
                {
                    feedId = feedItem.FeedId,
                    attributeValues = feedItem.AttributeBindings.Select(x => new FeedItemAttributeValue
                    {
                        feedAttributeId = x.ColumnId, // identity of the feed column
                        stringValue = x.Value ?? string.Empty

                        /*stringValue = x.ColumnType == eFeedAttributeType.STRING ? x.Value : string.Empty,
                        integerValue = x.ColumnType == eFeedAttributeType.INTEGER ? Convert.ToInt64(x.Value) : 0,
                        doubleValue = x.ColumnType == eFeedAttributeType.PRICE ? Convert.ToDouble(x.Value) : 0*/
                    }).ToArray()
                };

                if (feedItem.Id > 0)
                    feedContent.feedItemId = feedItem.Id;

                operations.Add(new FeedItemOperation
                {                   
                    @operator = (feedItem.Id > 0) ? Operator.SET : Operator.ADD,
                    operand = feedContent,
                });
            }

            var addFeedItemsReturnValue = service.mutate(operations.ToArray());
            if (addFeedItemsReturnValue == null || addFeedItemsReturnValue.value == null || addFeedItemsReturnValue.value.Length == 0)
                return null;
            return addFeedItemsReturnValue.value.Select(x => x.feedId).ToList();
        }
        #endregion

        #region HTML Template Ad
        public List<HTMLTemplateAdDetails> GetHTMLTemplateAds(string CampaignName)
        {
            var service = (AdGroupAdService)this.user.GetService(AdWordsService.v201809.AdGroupAdService);

            var adFields = TemplateAd.Fields.All.Select(x => x.FieldName);

            var fields = new List<string>();
            fields.AddRange(adFields);

            var selector = new Selector
            {
                fields = this.FieldsDistinct(fields).ToArray(),
                paging = new Paging
                {
                    numberResults = DEFAULT_ROWS_COUNT,
                    startIndex = 0
                }
            };

            // WHERE TemplateId = 419 (HTMLTemplateAd)
            // AND CampaignName = [CampaignName]
            selector.predicates = new Predicate[] {
                new Predicate {
                    field = "TemplateId",
                    @operator = PredicateOperator.EQUALS,
                    values = new string[] {
                        HTMLTemplateAdDetails.TEMPLATE_ID.ToString()
                    }
                },
                new Predicate {
                    field = "CampaignName",
                    @operator = PredicateOperator.EQUALS,
                    values = new string[] {
                        CampaignName
                    }
                }
            };

            var adsPage = service.get(selector);
            if (adsPage.entries == null)
                return null;

            return (from entry in adsPage.entries
                   let ad = entry.ad as TemplateAd
                   select new HTMLTemplateAdDetails
                   {
                       Id = ad.id,
                       Name = ad.name,
                       URL = ad.displayUrl,
                       Width = ad.dimensions.width,
                       Height = ad.dimensions.height
                   }).ToList();            
        }

        public long CreateHTMLTemplateAd(HTMLTemplateAdDetails Details)
        {
            var service = (AdGroupAdService)this.user.GetService(AdWordsService.v201809.AdGroupAdService);

            // TemplateAds 
            // types, fields names and more info can be found here:
            // https://developers.google.com/adwords/api/docs/guides/template-ads
            // note! beside each template type you can find its id, the supported dimensions and fields            

            var templateAd = new TemplateAd()
            {                
                name = Details.Name,
                templateId = Details.TemplateId,
                finalUrls = new string[] { Details.URL },
                displayUrl = Details.URL,
                dimensions = new Dimensions() {
                    width = Details.Width,
                    height = Details.Height
                }
            };

            var mediaBundle = new MediaBundle()
            {
                data = Details.ZipFileContent,
                entryPoint = "index.html", 
                type = MediaMediaType.MEDIA_BUNDLE
            };

            templateAd.templateElements = new TemplateElement[] {
                new TemplateElement() {
                    uniqueName = "adData",
                    fields = new TemplateElementField[] {
                        new TemplateElementField() {
                            name = "Custom_layout",
                            fieldMedia = mediaBundle,
                            type = TemplateElementFieldType.MEDIA_BUNDLE
                        },
                        new TemplateElementField() {
                            name = "layout",
                            fieldText = "Custom",
                            type = TemplateElementFieldType.ENUM
                        },
                    },
                }
            };

            var adGroupAd = new AdGroupAd
            {
                ad = templateAd,
                adGroupId = Details.AdGroupId
            };

            var operations = new AdGroupAdOperation[] {
                new AdGroupAdOperation {
                    @operator = Operator.ADD,
                    operand = adGroupAd                    
                }
            };

            // errors: 
            // https://support.google.com/adwords/answer/6335679?hl=en
            // https://developers.google.com/adwords/api/docs/reference/v201708/AdGroupAdService.MediaBundleError.Reason
            var adGroupAdReturnValue1 = service.mutate(operations);
            return adGroupAdReturnValue1.value.First().ad.id;           
        }
        #endregion

        #region Reporting  
        // https://developers.google.com/adwords/api/docs/appendix/reports

        public IEnumerable<ReportColumn> GetReportColumns(ReportDefinitionReportType ReportType)
        {         
            var service = (ReportDefinitionService)user.GetService(AdWordsService.v201809.ReportDefinitionService);

            var fields = service.getReportFields(ReportType);

            if (fields == null)
                return null;

            return fields.Select(x => new ReportColumn {
                Name = x.fieldName,
                Type = x.fieldType
            });
        }

        public IEnumerable<GenderReportRow> GetGenderReportRows(DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "AdGroupId", "Criteria", "Clicks", "Impressions", "Cost" };
            var filter = Predicate.In("CampaignStatus", new string[] { "ENABLED", "PAUSED" });

            var rows = GetReportRows(ReportDefinitionReportType.GENDER_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new GenderReportRow(x));
        }

        public IEnumerable<GenderReportRow> GetGenderReportRows(long CampaignId, DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "AdGroupId", "Criteria", "Clicks", "Impressions", "Cost" };
            var filter = new Predicate {
                field = "CampaignId",
                @operator = PredicateOperator.EQUALS,
                values = new string[]{
                    CampaignId.ToString()
                }
            };

            var rows = GetReportRows(ReportDefinitionReportType.GENDER_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new GenderReportRow(x));
        }

        public IEnumerable<AgeRangeReportRow> GetAgeRangeReportRows(DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "AdGroupId", "Criteria", "Clicks", "Impressions", "Cost" };
            var filter = Predicate.In("CampaignStatus", new string[] { "ENABLED", "PAUSED" });

            var rows = GetReportRows(ReportDefinitionReportType.AGE_RANGE_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new AgeRangeReportRow(x));
        }

        public IEnumerable<AgeRangeReportRow> GetAgeRangeReportRows(long CampaignId, DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "AdGroupId", "Criteria", "Clicks", "Impressions", "Cost" };
            var filter = new Predicate
            {
                field = "CampaignId",
                @operator = PredicateOperator.EQUALS,
                values = new string[]{
                    CampaignId.ToString()
                }
            };

            var rows = GetReportRows(ReportDefinitionReportType.AGE_RANGE_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new AgeRangeReportRow(x));
        }

        public IEnumerable<DeviceReportRow> GetDeviceReportRows(DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "Device", "Clicks", "Impressions", "Cost" };
            var filter = Predicate.In("CampaignStatus", new string[] { "ENABLED", "PAUSED" });

            var rows = GetReportRows(ReportDefinitionReportType.CAMPAIGN_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new DeviceReportRow(x));
        }

        public IEnumerable<DeviceReportRow> GetDeviceReportRows(long CampaignId, DateRangeFilter DateRange)
        {
            var fields = new List<string> { "CampaignId", "CampaignName", "Device", "Clicks", "Impressions", "Cost" };
            var filter = new Predicate
            {
                field = "CampaignId",
                @operator = PredicateOperator.EQUALS,
                values = new string[]{
                    CampaignId.ToString()
                }
            };

            var rows = GetReportRows(ReportDefinitionReportType.CAMPAIGN_PERFORMANCE_REPORT, DateRange, fields, new List<Predicate>() { filter });
            if (rows == null)
                return null;
            return rows.Select(x => new DeviceReportRow(x));
        }

        private IEnumerable<ReportRow> GetReportRows(ReportDefinitionReportType ReportType, DateRangeFilter DateRange, List<string> Fields, List<Predicate> Filters = null)
        {
            // get reports using Selector

            /* configuration */
            var config = user.Config as AdWordsAppConfig;
            config.IncludeZeroImpressions = true;
            config.SkipColumnHeader = true;
            config.SkipReportHeader = true;
            // config.SkipReportSummary = true;

            // create the SELECT clause
            var selector = new Selector() {
                fields = Fields.ToArray()
            };

            // for custom date - add date range
            if (DateRange.eSlice == eDateSlice.CUSTOM_DATE) {
                selector.dateRange = new DateRange
                {
                    min = DateParse(DateRange.From.Value),
                    max = DateParse(DateRange.To.Value)
                };
            }

            // create the WHERE clause
            if (Filters != null && Filters.Count > 0)
                selector.predicates = Filters.ToArray();
            
            var definition = new ReportDefinition()
            {
                reportName = "Dummy",
                reportType = ReportType, // FROM
                downloadFormat = DownloadFormat.XML,
                dateRangeType = (ReportDefinitionDateRangeType)DateRange.eSlice,
                selector = selector
            };

            var utilities = new ReportUtilities(this.user, "v201809", definition);
            using (var response = utilities.GetResponse())
            {
                using (var sr = new StreamReader(response.Stream))
                {
                    /*
                        <report>
	                        <table>
		                        <row cost="0" impressions="0" clicks="0" device="Computers" campaignID="914931574"/>
		                        <row cost="0" impressions="0" clicks="0" device="Computers" campaignID="915166228"/>
		                        <row cost="0" impressions="0" clicks="0" device="Mobile" campaignID="915166228"/>
	                        </table>
                        </report> 
                    */

                    var strXML = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(strXML))
                        return null;

                    var xDoc = XDocument.Parse(strXML);

                    var reportRows = new List<ReportRow>();

                    var xRows = xDoc.Root.Element("table").Elements("row");

                    foreach (var xRow in xRows)
                    {
                        // create a dynamic report columns mapping (using dictionary)
                        // each attribute is converted into a column-value item and added to the current report row  
                        // e.g: <row cost="0" device="Computers" /> will create a report row with 2 columns mappings - cost|0 and device|Computers
                        var reportRow = new ReportRow();
                        foreach (var attribute in xRow.Attributes())
                            reportRow[attribute.Name.LocalName] = attribute.Value;    
                         reportRows.Add(reportRow);
                    }

                    return reportRows;
                }
            }
        }

        // TODO Complete + check
        private IEnumerable<ReportRow> GetReportRowsAWQL(ReportDefinitionReportType ReportType, DateRangeFilter DateRange, List<string> Fields, List<Predicate> Filters = null)
        {
            // get reports using AWQL (Adwords Query Language)

            /* configuration */
            var config = user.Config as AdWordsAppConfig;
            config.IncludeZeroImpressions = true;
            config.SkipColumnHeader = true;
            config.SkipReportHeader = true;
            // config.SkipReportSummary = true;

            // TODO Fields (WHERE clause)
            string query = string.Format("SELECT {0} " +
            "FROM {1} " +
            "WHERE ?????? " +
            "DURING {2}", string.Join(",", Fields), ReportType, DateRange.eSlice);

            var utilities = new ReportUtilities(user, "v201809", query, DownloadFormat.XML.ToString());
            using (var response = utilities.GetResponse())
            {
                using (var sr = new StreamReader(response.Stream))
                {
                    var strXML = sr.ReadToEnd();
                    var xDoc = XDocument.Parse(strXML);

                    // TODO 
                }
            }

            return null;
        }

        #endregion        

        #region Helper
        // 2810645199 -> 281-064-5199
        private string AccountId2CustomerId(long AccountId) {
            var sAccountId = AccountId.ToString();
            if (sAccountId.Length != 10) return null;
            return sAccountId.Insert(3, "-").Insert(7, "-");
        }

        private bool SetActiveAccount(string CustomerId) {
            try {
                var appConfig = this.user.Config as AdWordsAppConfig;
                appConfig.ClientCustomerId = CustomerId;

                return true;
            }
            catch { return false; }
        }

        private string GetActiveAccount()
        {
            var appConfig = this.user.Config as AdWordsAppConfig;
            return appConfig.ClientCustomerId;
        }

        private long UploadImage(string ImageURL)
        {
            var service = (MediaService)this.user.GetService(AdWordsService.v201809.MediaService);

            var image = new Image {
                data = MediaUtilities.GetAssetDataFromUrl(ImageURL, this.user.Config as AdWordsAppConfig),
                type = MediaMediaType.IMAGE
            };

            var uploaded = service.upload(new Media[] {
                image
            });

            if (uploaded == null || uploaded.Length == 0)            
                return 0;            
            return uploaded.First().mediaId;
        }

        private string DateParse(DateTime Value)
        {
            return Value.ToString("yyyyMMdd");
        }

        private string DateTimeParse(DateTime Value)
        {
            return Value.ToString("yyyyMMdd HHmmss");
        }

        private DateTime DateParse(string Value)
        {
            return DateTime.ParseExact(Value, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        private IEnumerable<string> FieldsDistinct(IEnumerable<string> Fields)
        {
            return Fields.Distinct();
        }
        #endregion
    }
}