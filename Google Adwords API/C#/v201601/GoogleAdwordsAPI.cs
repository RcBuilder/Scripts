using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Services;
using Google.Apis.Analytics.v3;
using Google.Apis.Util.Store;
using Google.Api.Ads.AdWords.v201601;
using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.Util.BatchJob.v201601;

namespace TESTConsole
{
    public class GoogleAdwordsAPI
    {
        public GoogleAdwordsAPI() { }

        // campaigns

        #region PrintCampaigns
        public void PrintCampaigns() {
            var user = new AdWordsUser(); // adwords account user
            var service = (CampaignService)user.GetService(AdWordsService.v201601.CampaignService);

            var selector = new Selector{
                fields = new string[] {
                    Campaign.Fields.Id, Campaign.Fields.Name, Campaign.Fields.Status
                },
                paging = Paging.Default
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var campaign in page.entries)
                Console.WriteLine(campaign.name);
        }
        #endregion

        #region PrintCampaignSettings
        public void PrintCampaignSettings()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CampaignCriterionService)user.GetService(AdWordsService.v201601.CampaignCriterionService);

            // where Campaign = '390383720'
            var predicate = new Predicate();
            predicate.field = "CampaignId";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "390383720" };

            var selector = new Selector
            {
                fields = new string[] {
                    CampaignCriterion.Fields.CampaignId, 
                    Language.Fields.LanguageName, 
                    Location.Fields.LocationName,
                    Platform.Fields.PlatformName
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var c in page.entries)
            {
                switch (c.criterion.type)
                {
                    case CriterionType.LANGUAGE: Console.WriteLine("Language {0}", ((Language)c.criterion).name);
                        break;
                    case CriterionType.LOCATION: Console.WriteLine("Location {0}", ((Location)c.criterion).locationName);
                        break;
                    case CriterionType.PLATFORM: Console.WriteLine("Platform {0}", ((Platform)c.criterion).platformName);
                        break;
                    case CriterionType.IP_BLOCK: Console.WriteLine("Blocked IP {0}", ((IpBlock)c.criterion).ipAddress);
                        break;

                    // cases here ...
                }
            }
        }
        #endregion

        #region CreateCampaign
        public void CreateCampaign()
        {
            var user = new AdWordsUser();
            var service = (CampaignService)user.GetService(AdWordsService.v201601.CampaignService);

            var campaign = new Campaign();
            campaign.name = "From API 2";
            campaign.status = CampaignStatus.PAUSED;
            campaign.advertisingChannelType = AdvertisingChannelType.SEARCH;

            campaign.biddingStrategyConfiguration = new BiddingStrategyConfiguration
            {
                biddingStrategyType = BiddingStrategyType.MANUAL_CPC
            };

            campaign.budget = GetBudgetByName("My Budget");

            var operation = new CampaignOperation();
            operation.@operator = Operator.ADD;
            operation.operand = campaign;

            var result = service.mutate(new CampaignOperation[] { operation });
            foreach (var c in result.value)
                Console.WriteLine("campaign #{0} Created!", c.id);
        }
        #endregion

        #region BlockIPForCampaign
        public void BlockIPForCampaign()
        {
            var user = new AdWordsUser();
            var service = (CampaignCriterionService)user.GetService(AdWordsService.v201601.CampaignCriterionService);

            // create ip to block
            var ipBlock = new IpBlock
            {
                ipAddress = "37.46.39.210"                
            };

            // create criterion (negative)
            var criterion = new NegativeCampaignCriterion();
            criterion.criterion = ipBlock;
            criterion.campaignId = 390387440; // associated campaign id                

            // create operation
            var operation = new CampaignCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = criterion;

            var result = service.mutate(new CampaignCriterionOperation[] { operation });
            foreach (var c in result.value.OfType<NegativeCampaignCriterion>())
                Console.WriteLine("ip {0} has blocked!", ((IpBlock)c.criterion).ipAddress);
        }
        #endregion

        #region AddCampaignLabel
        public void AddCampaignLabel()
        {
            var user = new AdWordsUser();
            var service = (CampaignService)user.GetService(AdWordsService.v201601.CampaignService);

            var label = new CampaignLabel();
            label.labelId = 451335680; // associated label id
            label.campaignId = 390387440; // associated campaign id

            // create operation
            var operation = new CampaignLabelOperation();
            operation.@operator = Operator.ADD;
            operation.operand = label;

            var result = service.mutateLabel(new CampaignLabelOperation[] { operation });
            foreach (var l in result.value)
                Console.WriteLine("label #{0} has added!", l.labelId);
        }
        #endregion

        #region PrintCampaignChanges
        public void PrintCampaignChanges() {
            var user = new AdWordsUser(); // adwords account user
            var service = (CustomerSyncService)user.GetService(AdWordsService.v201601.CustomerSyncService);

            DateTime dateFrom = new DateTime(2016, 4, 1), dateTo = new DateTime(2016, 5, 1);
            var selector = new CustomerSyncSelector
            {
                campaignIds = new long[] { 390387440 },
                dateTimeRange = new DateTimeRange { 
                    min = dateFrom.ToString("yyyyMMdd HHmmss"), 
                    max = dateTo.ToString("yyyyMMdd HHmmss") 
                },
            };

            var result = service.get(selector);
            var campaignChanges = result.changedCampaigns.FirstOrDefault(); // 390387440           

            /*  
                 CHANGES LISTS
                 -------------
                 AdGroupChangeData[] changedAdGroups 
                 long[] addedCampaignCriteria
                 long[] removedCampaignCriteria 
                 long[] addedAdExtensions
                 long[] removedAdExtensions
                 long[] changedFeeds
                 long[] removedFeeds
            */

            // status
            Console.WriteLine("status: {0}", campaignChanges.campaignChangeStatus);

            // adgroups changes 
            if (campaignChanges.changedAdGroups != null)
            {
                foreach (var adGroupChange in campaignChanges.changedAdGroups)
                    Console.WriteLine("adGroup #{0} changed -> status: {1}", adGroupChange.adGroupId, adGroupChange.adGroupChangeStatus);
            }
            else
                Console.WriteLine("No AdGroups Changes");

            // added criterias
            if (campaignChanges.addedCampaignCriteria != null)
            {
                foreach (var CriteriaId in campaignChanges.addedCampaignCriteria)
                    Console.WriteLine("Criteria #{0} has been added", CriteriaId);
            }
            else
                Console.WriteLine("No Criterias Changes (Add)");

            // added criterias
            if (campaignChanges.removedCampaignCriteria != null)
            {
                foreach (var CriteriaId in campaignChanges.removedCampaignCriteria)
                    Console.WriteLine("Criteria #{0} has been removed", CriteriaId);
            }
            else
                Console.WriteLine("No Criterias Changes (Remove)");
        }
        #endregion        

        #region CreateCampaignExperiment
        public void CreateCampaignExperiment()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ExperimentService)user.GetService(AdWordsService.v201601.ExperimentService);

            /*
                NOTE! 
                we still need to determine which keywords serve the experiment and which are not
                (set keyword as control, experiment or both)
            */

            // create experiment
            var experiment = new Experiment();
            experiment.name = "My First Experiment";
            experiment.campaignId = 390383720; // associated campaign id
            experiment.startDateTime = "20160501 010000"; // from the 1st of may
            experiment.endDateTime = "20160510 010000"; // till the 10th of may
            experiment.queryPercentage = 10; // 10% of the queries will get routed to this experiment            

            var operation = new ExperimentOperation();
            operation.@operator = Operator.ADD;
            operation.operand = experiment;

            var result = service.mutate(new ExperimentOperation[] { operation });

            foreach (var exp in result.value)
                Console.WriteLine("experiment #{0} on campaign #{1} created!", exp.id, exp.campaignId);
        }
        #endregion

        #region PrintCampaignExperiments
        public void PrintCampaignExperiments() {
            var user = new AdWordsUser(); // adwords account user
            var service = (ExperimentService)user.GetService(AdWordsService.v201601.ExperimentService);

            // where CampaignId = '390383720'
            var predicate = new Predicate();
            predicate.field = "CampaignId";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "390383720" };

            var selector = new Selector
            {
                fields = new string[] {
                    Experiment.Fields.Name,
                    Experiment.Fields.StartDateTime,
                    Experiment.Fields.EndDateTime,
                    Experiment.Fields.QueryPercentage,
                    Experiment.Fields.CampaignId, 
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var exp in page.entries)
            {
                Console.WriteLine("\"{0}\"", exp.name);
                Console.WriteLine("from {0} till {1}", exp.startDateTime.Substring(0, 8), exp.endDateTime.Substring(0, 8));
                Console.WriteLine("{0}% of the queries", exp.queryPercentage);
                Console.WriteLine("------------------");
            }
        }
        #endregion        

        #region PrintCampaignSharedLists
        public void PrintCampaignSharedLists()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CampaignSharedSetService)user.GetService(AdWordsService.v201601.CampaignSharedSetService);

            // where CampaignId = '390383480'
            var predicate = new Predicate();
            predicate.field = "CampaignId";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "390383480" };

            var selector = new Selector
            {
                fields = new string[] {
                    CampaignSharedSet.Fields.CampaignId,
                    CampaignSharedSet.Fields.SharedSetId,
                    CampaignSharedSet.Fields.SharedSetName,
                    CampaignSharedSet.Fields.SharedSetType
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var campaignSharedSet in page.entries)
                Console.WriteLine("#{0} {1} (type: {2})", campaignSharedSet.sharedSetId, campaignSharedSet.sharedSetName, campaignSharedSet.sharedSetType);
        }
        #endregion

        #region PrintCampaignSharedListsUsingQuery
        public void PrintCampaignSharedListsUsingQuery()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CampaignSharedSetService)user.GetService(AdWordsService.v201601.CampaignSharedSetService);
            var page = service.query("SELECT SharedSetName, SharedSetId WHERE CampaignId = '390383480'");

            foreach (var campaignSharedSet in page.entries)
                Console.WriteLine("#{0} \"{1}\"", campaignSharedSet.sharedSetId, campaignSharedSet.sharedSetName);
        }
        #endregion

        #region AddCampaignToSharedList
        public void AddCampaignToSharedList()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CampaignSharedSetService)user.GetService(AdWordsService.v201601.CampaignSharedSetService);

            // create list
            var negativeKeywordsList = new CampaignSharedSet();
            negativeKeywordsList.campaignId = 390383480; // associated campaign id
            negativeKeywordsList.sharedSetId = 1376909119; // associated list id
            negativeKeywordsList.sharedSetType = SharedSetType.NEGATIVE_KEYWORDS;

            var operation = new CampaignSharedSetOperation();
            operation.@operator = Operator.ADD;
            operation.operand = negativeKeywordsList;

            var result = service.mutate(new CampaignSharedSetOperation[] { operation });

            foreach (var campaignSharedSet in result.value)
                Console.WriteLine("campaign #{0} added to list {1}", campaignSharedSet.campaignId, campaignSharedSet.sharedSetName);
        }
        #endregion

        // adGroups

        #region PrintAdGroups
        public void PrintAdGroups()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (AdGroupService)user.GetService(AdWordsService.v201601.AdGroupService);

            var selector = new Selector
            {
                fields = new string[] {
                    AdGroup.Fields.Id, AdGroup.Fields.Name, AdGroup.Fields.Status
                }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var adGroup in page.entries)
                Console.WriteLine(adGroup.name);
        }
        #endregion

        #region PrintAdGroupsByCampaign
        public void PrintAdGroupsByCampaign()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (AdGroupService)user.GetService(AdWordsService.v201601.AdGroupService);

            // where CampaignName = 'Campaign #2'
            var predicate = new Predicate();
            predicate.field = "CampaignName"; 
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "Campaign #2" };

            var selector = new Selector
            {
                fields = new string[] {
                    AdGroup.Fields.Id, AdGroup.Fields.Name, AdGroup.Fields.Status
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var adGroup in page.entries)
                Console.WriteLine(adGroup.name);
        }
        #endregion

        #region PrintAdGroupNegativeSettings
        public void PrintAdGroupNegativeSettings()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // where AdGroupId = '29433199640'
            var predicate = new Predicate();
            predicate.field = "AdGroupId";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "29433199640" };

            var selector = new Selector
            {
                fields = new string[] {
                    AdGroupCriterion.Fields.AdGroupId, 
                    Placement.Fields.PlacementUrl,
                    Keyword.Fields.KeywordText
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var c in page.entries.OfType<NegativeAdGroupCriterion>())
            {
                switch (c.criterion.type)
                {
                    case CriterionType.PLACEMENT: Console.WriteLine("Negative Placement {0}", ((Placement)c.criterion).url);
                        break;
                    case CriterionType.KEYWORD: Console.WriteLine("Negative Keyword {0}", ((Keyword)c.criterion).text);
                        break;

                    // cases here ...
                }
            }
        }
        #endregion

        #region PrintAdGroupSettings
        public void PrintAdGroupSettings()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // where AdGroupId = '29433199640'
            var predicate = new Predicate();
            predicate.field = "AdGroupId";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "29433199640" };

            var selector = new Selector
            {
                fields = new string[] {
                    AdGroupCriterion.Fields.AdGroupId, 
                    Placement.Fields.PlacementUrl,
                    Keyword.Fields.KeywordText
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var c in page.entries.OfType<BiddableAdGroupCriterion>())
            {
                switch (c.criterion.type)
                {
                    case CriterionType.PLACEMENT: Console.WriteLine("Placement {0}", ((Placement)c.criterion).url);
                        break;
                    case CriterionType.KEYWORD: Console.WriteLine("Keyword {0}", ((Keyword)c.criterion).text);
                        break;
          
                    // cases here ...
                }
            }
        }
        #endregion

        #region CreateAdGroup
        public void CreateAdGroup()
        {
            var user = new AdWordsUser();
            var service = (AdGroupService)user.GetService(AdWordsService.v201601.AdGroupService);

            var adGroup = new AdGroup();
            adGroup.name = "From API";
            adGroup.status = AdGroupStatus.PAUSED;
            adGroup.campaignId = 390383720; // associated campaign id

            var operation = new AdGroupOperation();
            operation.@operator = Operator.ADD;
            operation.operand = adGroup;

            var result = service.mutate(new AdGroupOperation[] { operation });
            foreach (var a in result.value)
                Console.WriteLine("adGroup #{0} Created!", a.id);
        }
        #endregion

        #region CreateAdGroupKeyword
        public void CreateAdGroupKeyword()
        {
            var user = new AdWordsUser();
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // create keyword
            var keyword1 = new Keyword
            {
                text = "keyword1",
                matchType = KeywordMatchType.PHRASE
            };

            // create criterion (positive)
            var criterion = new BiddableAdGroupCriterion();
            criterion.criterion = keyword1;
            criterion.adGroupId = 29433199640; // associated adgroup id                

            // create operation
            var operation = new AdGroupCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = criterion;

            var result = service.mutate(new AdGroupCriterionOperation[] { operation });
            foreach (var c in result.value.OfType<BiddableAdGroupCriterion>())
                Console.WriteLine("keyword #{0} Created!", ((Keyword)c.criterion).id);
        }
        #endregion

        #region CreateAdGroupNegativeKeyword
        public void CreateAdGroupNegativeKeyword()
        {
            var user = new AdWordsUser();
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // create keyword
            var keyword1 = new Keyword
            {
                text = "negativeKeyword1",
                matchType = KeywordMatchType.EXACT
            };

            // create criterion (negative)
            var criterion = new NegativeAdGroupCriterion();
            criterion.criterion = keyword1;
            criterion.adGroupId = 29433199640; // associated adgroup id                

            // create operation
            var operation = new AdGroupCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = criterion;

            var result = service.mutate(new AdGroupCriterionOperation[] { operation });
            foreach (var c in result.value.OfType<NegativeAdGroupCriterion>())
                Console.WriteLine("negative keyword #{0} Created!", ((Keyword)c.criterion).id);
        }
        #endregion

        #region CreateAdGroupPlacement
        public void CreateAdGroupPlacement()
        {
            var user = new AdWordsUser();
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // create keyword
            var placement1 = new Placement
            {
                url = "rcb.co.il"
            };

            // create criterion (positive)
            var criterion = new BiddableAdGroupCriterion();
            criterion.criterion = placement1;
            criterion.adGroupId = 29433199640; // associated adgroup id                

            // create operation
            var operation = new AdGroupCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = criterion;

            var result = service.mutate(new AdGroupCriterionOperation[] { operation });
            foreach (var c in result.value.OfType<BiddableAdGroupCriterion>())
                Console.WriteLine("placement #{0} Created!", ((Placement)c.criterion).id);
        }
        #endregion

        #region CreateAdGroupNegativePlacement
        public void CreateAdGroupNegativePlacement()
        {
            var user = new AdWordsUser();
            var service = (AdGroupCriterionService)user.GetService(AdWordsService.v201601.AdGroupCriterionService);

            // create keyword
            var placement1 = new Placement
            {
                url = "ynet.co.il"
            };

            // create criterion (negative)
            var criterion = new NegativeAdGroupCriterion();
            criterion.criterion = placement1;
            criterion.adGroupId = 29433199640; // associated adgroup id                

            // create operation
            var operation = new AdGroupCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = criterion;

            var result = service.mutate(new AdGroupCriterionOperation[] { operation });
            foreach (var c in result.value.OfType<NegativeAdGroupCriterion>())
                Console.WriteLine("negative placement #{0} Created!", ((Placement)c.criterion).id);
        }
        #endregion

        #region AddAdGroupLabel
        public void AddAdGroupLabel()
        {
            var user = new AdWordsUser();
            var service = (AdGroupService)user.GetService(AdWordsService.v201601.AdGroupService);

            var label = new AdGroupLabel();
            label.labelId = 451335680; // associated label id
            label.adGroupId = 29032520720; // associated adGroup id

            // create operation
            var operation = new AdGroupLabelOperation();
            operation.@operator = Operator.ADD;
            operation.operand = label;

            var result = service.mutateLabel(new AdGroupLabelOperation[] { operation });
            foreach (var l in result.value)
                Console.WriteLine("label #{0} has added!", l.labelId);
        }
        #endregion

        // ads

        #region PrintAds
        public void PrintAds()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (AdGroupAdService)user.GetService(AdWordsService.v201601.AdGroupAdService);

            var selector = new Selector
            {
                fields = new string[] {
                    TextAd.Fields.Id, TextAd.Fields.Headline
                }
            };

            var page = service.get(selector);
            if (page.entries == null) return;
       
            foreach (TextAd ad in page.entries.Where(x => x.ad.type == AdType.TEXT_AD).Select(x => x.ad))
                Console.WriteLine(ad.headline);   
        }
        #endregion

        #region CreateAdInAdGroup
        public void CreateAdInAdGroup()
        {
            var user = new AdWordsUser();
            var service = (AdGroupAdService)user.GetService(AdWordsService.v201601.AdGroupAdService);

            // create ad
            var ad = new TextAd();
            ad.description1 = "bla bla bla";
            ad.description2 = "bla bla bla";
            ad.displayUrl = "mydomain.com";
            ad.headline = "from api";
            ad.finalUrls = new string[] { "http://mydomain.com" };

            // create adGroup ad
            var adGroupAd = new AdGroupAd();
            adGroupAd.ad = ad;
            adGroupAd.adGroupId = 29433199640; // associated adgroup id

            var operation = new AdGroupAdOperation();
            operation.@operator = Operator.ADD;
            operation.operand = adGroupAd;

            var result = service.mutate(new AdGroupAdOperation[] { operation });
            foreach (TextAd a in result.value.Select(x => x.ad))
                Console.WriteLine("ad #{0} Created!", a.id);
        }
        #endregion

        #region UpdateAdParam
        public void UpdateAdParam()
        {
            var user = new AdWordsUser();
            var service = (AdParamService)user.GetService(AdWordsService.v201601.AdParamService);

            // create adParam
            var adParam1 = new AdParam();
            adParam1.paramIndex = 1; // first parameter
            adParam1.insertionText = "11"; // new value
            adParam1.criterionId = 155867290; // associated keyword id
            adParam1.adGroupId = 29433199640; // associated adgroup id

            var operation = new AdParamOperation();
            operation.@operator = Operator.SET;
            operation.operand = adParam1;

            var result = service.mutate(new AdParamOperation[] { operation });
            foreach (var p in result)
                Console.WriteLine("param updated for keyword #{0}", p.criterionId);
        }
        #endregion

        // account labels

        #region PrintAccountLabels
        public void PrintAccountLabels()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (LabelService)user.GetService(AdWordsService.v201601.LabelService);

            var selector = new Selector
            {
                fields = new string[] {
                    Label.Fields.LabelId, Label.Fields.LabelName
                },
                paging = Paging.Default
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var label in page.entries)
                Console.WriteLine("#{0} {1}", label.id, label.name);
        }
        #endregion

        #region CreateAccountLabel
        public void CreateAccountLabel()
        {
            var user = new AdWordsUser();
            var service = (LabelService)user.GetService(AdWordsService.v201601.LabelService);

            // TODO add color and description
            var label = new TextLabel();
            label.name = "LabelAPI";            

            var operation = new LabelOperation();
            operation.@operator = Operator.ADD;
            operation.operand = label;

            var result = service.mutate(new LabelOperation[] { operation });
            foreach (var l in result.value)
                Console.WriteLine("label #{0} Created!", l.id);
        }
        #endregion

        #region GetAccountLabelByName
        public void GetAccountLabelByName(string Name)
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (LabelService)user.GetService(AdWordsService.v201601.LabelService);

            // where LabelName = Name
            var predicate = new Predicate();
            predicate.field = "LabelName";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { Name };

            var selector = new Selector
            {
                fields = new string[] {
                    Label.Fields.LabelId, Label.Fields.LabelName
                },
                paging = Paging.Default,
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var label in page.entries)
                Console.WriteLine("#{0} {1}", label.id, label.name);
        }
        #endregion

        // location

        #region PrintLocationByName
        public void PrintLocationByName()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (LocationCriterionService)user.GetService(AdWordsService.v201601.LocationCriterionService);

            // where CampaignId = '390383720'
            var predicate = new Predicate();
            predicate.field = "LocationName";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { "Israel" };

            var selector = new Selector
            {
                fields = new string[] {
                    Location.Fields.Id,
                    Location.Fields.LocationName,                                               
                    LocationCriterion.Fields.CountryCode,
                    LocationCriterion.Fields.CanonicalName
                },
                predicates = new Predicate[] { predicate }
            };

            var result = service.get(selector);
            if (result == null) return;

            var location = result.FirstOrDefault().location;
            Console.WriteLine("{0} {1}", location.id, location.locationName);
        }
        #endregion

        // constant data

        #region PrintAvailableLanguages
        public void PrintAvailableLanguages()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ConstantDataService)user.GetService(AdWordsService.v201601.ConstantDataService);

            var languages = service.getLanguageCriterion();
            foreach (var language in languages)
                Console.WriteLine(language.name);
        }
        #endregion

        #region PrintAvailableAges
        public void PrintAvailableAges()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ConstantDataService)user.GetService(AdWordsService.v201601.ConstantDataService);

            var ages = service.getAgeRangeCriterion();
            foreach (var age in ages)
                Console.WriteLine(age.ageRangeType);
        }
        #endregion

        #region PrintAvailableMobileDevices
        public void PrintAvailableMobileDevices()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ConstantDataService)user.GetService(AdWordsService.v201601.ConstantDataService);

            var devices = service.getMobileDeviceCriterion();
            foreach (var device in devices)
                Console.WriteLine("{0} {1}", device.manufacturerName, device.deviceName);
        }
        #endregion

        // Account

        #region PrintAccountData
        public void PrintAccountData()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CustomerService)user.GetService(AdWordsService.v201601.CustomerService);

            var customer = service.get();
            Console.WriteLine("#{0}", customer.customerId);
            Console.WriteLine("description: {0}", customer.descriptiveName);
            Console.WriteLine("test: {0}", customer.testAccount);
            Console.WriteLine("currency: {0}", customer.currencyCode);
        }
        #endregion

        // ManagedAccount (Sub Accounts on MCC)

        #region PrintManagedAccounts
        public void PrintManagedAccounts()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ManagedCustomerService)user.GetService(AdWordsService.v201601.ManagedCustomerService);

            var selector = new Selector
            {
                fields = new string[] {
                    ManagedCustomer.Fields.Name,
                    ManagedCustomer.Fields.TestAccount,
                    ManagedCustomer.Fields.CompanyName
                }
            };

            var result = service.get(selector);
            if (result == null) return;

            foreach (var customer in result.entries)
            {
                Console.WriteLine("{0}", customer.name);

                if (customer.accountLabels == null)
                    continue;

                foreach (var label in customer.accountLabels)
                    Console.WriteLine("label: {0}", label.name);
            }
        }
        #endregion

        #region CreateMccSubAccount
        public void CreateMccSubAccount()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (ManagedCustomerService)user.GetService(AdWordsService.v201601.ManagedCustomerService);

            // create account
            var account = new ManagedCustomer();
            account.name = "My Sub Account";

            // time zones: https://developers.google.com/adwords/api/docs/appendix/timezones#timezone-ids
            account.dateTimeZone = "Europe/Rome";

            // currency codes: https://developers.google.com/adwords/api/docs/appendix/currencycodes
            account.currencyCode = "EUR";

            var operation = new ManagedCustomerOperation();
            operation.@operator = Operator.ADD;
            operation.operand = account;

            var result = service.mutate(new ManagedCustomerOperation[] { operation });

            foreach (var a in result.value)
                Console.WriteLine("sub account #{0} created!", a.customerId);
        }
        #endregion

        // MCC account

        #region PrintMCCAccountData
        public void PrintMCCAccountData()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (CustomerService)user.GetService(AdWordsService.v201601.CustomerService);

            var customer = service.getCustomers().First();
            Console.WriteLine("#{0}", customer.customerId);
            Console.WriteLine("description: {0}", customer.descriptiveName);
            Console.WriteLine("test: {0}", customer.testAccount);
            Console.WriteLine("currency: {0}", customer.currencyCode);
            
        }
        #endregion

        // budgets

        #region PrintBudgets
        public void PrintBudgets()
        {
            var user = new AdWordsUser();
            var service = (BudgetService)user.GetService(AdWordsService.v201601.BudgetService);

            var selector = new Selector
            {
                fields = new string[] {
                    Budget.Fields.BudgetId, Budget.Fields.BudgetName
                },
                paging = Paging.Default
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var budget in page.entries)
                Console.WriteLine(budget.name);
        }
        #endregion

        #region CreateBudget
        public void CreateBudget()
        {
            var user = new AdWordsUser();
            var service = (BudgetService)user.GetService(AdWordsService.v201601.BudgetService);

            var budget = new Budget
            {
                name = "My budget",
                period = BudgetBudgetPeriod.DAILY,
                deliveryMethod = BudgetBudgetDeliveryMethod.STANDARD,
                amount = new Money { microAmount = 500000 }
            };

            var operation = new BudgetOperation();
            operation.@operator = Operator.ADD;
            operation.operand = budget;

            var budgetResult = service.mutate(new BudgetOperation[] { operation });
            var budgetId = budgetResult.value[0].budgetId;
            Console.WriteLine("budget #{0} Created!", budgetId);
        }
        #endregion

        #region GetBudgetByName
        public Budget GetBudgetByName(string Name)
        {
            var user = new AdWordsUser();
            var service = (BudgetService)user.GetService(AdWordsService.v201601.BudgetService);

            // where BudgetName = Name
            var predicate = new Predicate();
            predicate.field = "BudgetName";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { Name };

            var selector = new Selector
            {
                fields = new string[] {
                    Budget.Fields.BudgetId, Budget.Fields.BudgetName
                },
                paging = Paging.Default,
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return null;

            var budget = page.entries.FirstOrDefault();
            Console.WriteLine("#{0}", budget.budgetId);
            return budget;
        }
        #endregion

        // shared library

        #region PrintSharedLists
        public void PrintSharedLists()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (SharedSetService)user.GetService(AdWordsService.v201601.SharedSetService);

            var selector = new Selector
            {
                fields = new string[] {
                    SharedSet.Fields.Name,
                    SharedSet.Fields.SharedSetId,
                    SharedSet.Fields.Type 
                }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var list in page.entries)
                Console.WriteLine("List: #{0} \"{1}\"", list.sharedSetId, list.name);
        }
        #endregion

        #region PrintSharedListsUsingQuery
        public void PrintSharedListsUsingQuery()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (SharedSetService)user.GetService(AdWordsService.v201601.SharedSetService);
            var result = service.query("SELECT Name, SharedSetId WHERE Name = 'NegativeKeywordsList1'");

            var list = result.entries.FirstOrDefault();
            Console.WriteLine("#{0} \"{1}\"", list.sharedSetId, list.name);
        }
        #endregion

        #region CreateSharedNegativeKeywordsList
        public void CreateSharedNegativeKeywordsList()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (SharedSetService)user.GetService(AdWordsService.v201601.SharedSetService);

            // create list
            var negativeKeywordsList = new SharedSet();
            negativeKeywordsList.name = "NegativeKeywordsListFromAPI";
            negativeKeywordsList.type = SharedSetType.NEGATIVE_KEYWORDS;

            var operation = new SharedSetOperation();
            operation.@operator = Operator.ADD;
            operation.operand = negativeKeywordsList;

            var result = service.mutate(new SharedSetOperation[] { operation });

            foreach (var list in result.value)
                Console.WriteLine("list #{0} created", list.sharedSetId);
        }
        #endregion

        #region PrintSharedData
        public void PrintSharedData()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (SharedCriterionService)user.GetService(AdWordsService.v201601.SharedCriterionService);

            var selector = new Selector
            {
                fields = new string[] {
                    SharedCriterion.Fields.SharedSetId,
                    SharedCriterion.Fields.Negative,
                    Keyword.Fields.KeywordText,
                    Placement.Fields.PlacementUrl
                }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var c in page.entries)
            {
                Console.WriteLine("ListId: #{0}", c.sharedSetId);

                switch (c.criterion.type)
                {
                    case CriterionType.KEYWORD: Console.WriteLine("Negative Keyword: {0}", ((Keyword)c.criterion).text);
                        break;
                    case CriterionType.PLACEMENT: Console.WriteLine("Excluded Placement: {0}", ((Placement)c.criterion).url);
                        break;

                    // cases here ...
                }
            }
        }
        #endregion

        #region AddSharedNegativeKeyword
        public void AddSharedNegativeKeyword()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (SharedCriterionService)user.GetService(AdWordsService.v201601.SharedCriterionService);

            // create negative Keyword
            var negativeKeyword = new SharedCriterion();
            negativeKeyword.criterion = new Keyword
            {
                text = "negativeFromAPI",
                matchType = KeywordMatchType.EXACT
            };
            negativeKeyword.sharedSetId = 1376758789; // associated list id
            negativeKeyword.negative = true;

            var operation = new SharedCriterionOperation();
            operation.@operator = Operator.ADD;
            operation.operand = negativeKeyword;

            var result = service.mutate(new SharedCriterionOperation[] { operation });

            foreach (var c in result.value)
                Console.WriteLine("{0} #{1} added to the list #{2}", c.criterion.CriterionType, c.criterion.id, c.sharedSetId);
        }
        #endregion

        // keywords planner

        #region PrintIdeasByPhrase
        public void PrintIdeasByPhrase(string phrase)
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (TargetingIdeaService)user.GetService(AdWordsService.v201601.TargetingIdeaService);

            // keyword planner tool
            // get keywords ideas

            var selector = new TargetingIdeaSelector
            {
                requestType = RequestType.IDEAS,
                ideaType = IdeaType.KEYWORD,
                paging = new Paging
                {
                    startIndex = 1,
                    numberResults = 20
                },
                requestedAttributeTypes = new AttributeType[]{ // attributes to fetch
                        AttributeType.KEYWORD_TEXT,
                        AttributeType.SEARCH_VOLUME,
                        AttributeType.AVERAGE_CPC,
                        AttributeType.COMPETITION 
                },
                searchParameters = new SearchParameter[] // set the search parameters
                { 
                    new RelatedToQuerySearchParameter{ // set the queries which to get ideas based on
                        queries = new string[] { phrase }
                    }, 
                    new NetworkSearchParameter{  // set the network types (optional)
                        networkSetting = new NetworkSetting{ 
                            targetGoogleSearch = true,
                            targetSearchNetwork = false,
                            targetContentNetwork = false,
                            targetPartnerSearchNetwork = false                            
                        }
                    },
                    new LanguageSearchParameter{  // set the language (optional)
                        languages = new Language[]{ 
                            new Language{ id = 1000 } // english (EN)
                        }
                    }
                    // more parameters here .... 
                }
            };

            var page = service.get(selector);

            var MILLION = 1000000;

            foreach (var idea in page.entries)
            {
                Console.WriteLine("IDEA:");
                foreach (var attribute in idea.data)
                {
                    // https://developers.google.com/adwords/api/docs/reference/v201601/TargetingIdeaService.Type_AttributeMapEntry#field
                    switch (attribute.key)
                    {
                        case AttributeType.KEYWORD_TEXT: Console.WriteLine("\t keyword: \"{0}\"", ((StringAttribute)attribute.value).value);
                            break;
                        case AttributeType.SEARCH_VOLUME: Console.WriteLine("\t num of searches: \"{0:N}\"", ((LongAttribute)attribute.value).value);
                            break;
                        case AttributeType.AVERAGE_CPC: Console.WriteLine("\t cpc: \"{0:N}\"", ((MoneyAttribute)attribute.value).value.microAmount / MILLION);
                            break;
                        case AttributeType.COMPETITION: Console.WriteLine("\t competition: \"{0:N}\"", ((DoubleAttribute)attribute.value).value);
                            break;
                    }
                }
            }
        }
        #endregion

        #region PrintForecastByPhrase
        public void PrintForecastByPhrase(string phrase)
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (TrafficEstimatorService)user.GetService(AdWordsService.v201601.TrafficEstimatorService);

            // keyword planner tool
            // forecasts

            var keywordEstimateRequest = new KeywordEstimateRequest{
                keyword = new Keyword{ text = phrase, matchType = KeywordMatchType.EXACT },
                maxCpc = new Money{ microAmount = 1000000 } // 1M = 1 Unit
            }; 


            var selector = new TrafficEstimatorSelector
            {
                campaignEstimateRequests = new CampaignEstimateRequest[]{
                    new CampaignEstimateRequest{
                        adGroupEstimateRequests = new AdGroupEstimateRequest[]{ // set the phrase
                            new AdGroupEstimateRequest{
                                keywordEstimateRequests = new KeywordEstimateRequest[]{ keywordEstimateRequest }
                            }
                        },
                        criteria = new Criterion[] {                             
                            new Location{ id = 2840 }, // set locations -> US 
                            new Language{ id = 1000 }  // set languages -> english (EN)
                        },
                        networkSetting = new NetworkSetting{ // set the network types (optional)
                            targetGoogleSearch = true,
                            targetSearchNetwork = false,
                            targetContentNetwork = false,
                            targetPartnerSearchNetwork = false                            
                        }
                    }
                }
            };

            var result = service.get(selector);

            var MILLION = 1000000;

            // get the phrase estimation
            // note! can use multiple campaigns, adgroups and keywords
            var estimation = result.campaignEstimates.First().adGroupEstimates.First().keywordEstimates.First();
            Console.WriteLine("### \"{0}\" ###", keywordEstimateRequest.keyword.text);
            Console.WriteLine("cpc: {0:F} - {1:F}", estimation.min.averageCpc.microAmount / MILLION, estimation.max.averageCpc.microAmount / MILLION);
            Console.WriteLine("position: {0:F} - {1:F}", estimation.max.averagePosition, estimation.min.averagePosition);
            Console.WriteLine("daily clicks: {0:N} - {1:N}", estimation.min.clicksPerDay, estimation.max.clicksPerDay);
            Console.WriteLine("ctr: {0:N} - {1:N}", estimation.min.clickThroughRate, estimation.max.clickThroughRate);
            Console.WriteLine("impressions: {0:N} - {1:N}", estimation.min.impressionsPerDay, estimation.max.impressionsPerDay);
            Console.WriteLine("cost: {0:F} - {1:F}", estimation.min.totalCost.microAmount / MILLION, estimation.max.totalCost.microAmount / MILLION);
        }
        #endregion

        // Batch Jobs (async operations)

        #region PrintBatchJobs
        public void PrintBatchJobs()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (BatchJobService)user.GetService(AdWordsService.v201601.BatchJobService);

            var selector = new Selector
            {
                fields = new string[] {
                    BatchJob.Fields.Id,
                    BatchJob.Fields.Status                     
                }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            foreach (var job in page.entries)
                Console.WriteLine("JOB: #{0} {1}", job.id, job.status);
        }
        #endregion 

        #region PrintBatchJobById
        public void PrintBatchJobById(long JobId)
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (BatchJobService)user.GetService(AdWordsService.v201601.BatchJobService);

            var predicate = new Predicate();
            predicate.field = "Id";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { JobId.ToString() };

            var selector = new Selector
            {
                fields = new string[] {
                    BatchJob.Fields.Id,
                    BatchJob.Fields.Status                     
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            var job = page.entries.FirstOrDefault();

            if (job == null) return;
            Console.WriteLine("JOB: #{0} {1}", job.id, job.status);
        }
        #endregion 

        #region PrintBatchJobResults
        public void PrintBatchJobResults(long JobId)
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (BatchJobService)user.GetService(AdWordsService.v201601.BatchJobService);

            var predicate = new Predicate();
            predicate.field = "Id";
            predicate.@operator = PredicateOperator.EQUALS;
            predicate.values = new string[] { JobId.ToString() };

            var selector = new Selector
            {
                fields = new string[] {
                    BatchJob.Fields.Id,
                    BatchJob.Fields.Status                     
                },
                predicates = new Predicate[] { predicate }
            };

            var page = service.get(selector);
            if (page.entries == null) return;

            var job = page.entries.FirstOrDefault();

            if (job == null) return;
            Console.WriteLine("JOB: #{0} {1}", job.id, job.status);

            // print upload results
            if (job.status == BatchJobStatus.DONE)
            {
                var batchJobHelper = new BatchJobUtilities(user);
                var response = batchJobHelper.Download(job.downloadUrl.url);
                foreach (var item in response.rval)
                {
                    Console.WriteLine("item {0}:", item.index);

                    if (item.errorList == null || item.errorList.errors == null || item.errorList.errors.Length == 0)
                        Console.WriteLine("NO ERRORS!!");
                    else
                    {
                        Console.WriteLine("ERRORS:");
                        foreach (var error in item.errorList.errors)
                            Console.WriteLine("field: {0}, message: {1}", error.fieldPath, error.errorString);
                    }

                    Console.WriteLine("------");
                }
            }
        }
        #endregion       

        #region CreateBatchJob
        public void CreateBatchJob()
        {
            var user = new AdWordsUser(); // adwords account user
            var service = (BatchJobService)user.GetService(AdWordsService.v201601.BatchJobService);

            // -- (STEP 1) --
            // create the BatchJob 
            var operation = new BatchJobOperation();
            operation.@operator = Operator.ADD;
            operation.operand = new BatchJob();

            var result = service.mutate(new BatchJobOperation[] { operation });
            var job = result.value.FirstOrDefault();

            if (job == null) return;
            Console.WriteLine("JOB #{0} Created!", job.id);

            // -- (STEP 2) --
            // get the created job upload url 
            var jobUploadURL = job.uploadUrl.url;
            
            // -- (STEP 3) --
            // create operations LIST 
            var operations = new List<Operation>();

            var sharedBudget = GetBudgetByName("My Budget");
            operations.Add(new CampaignOperation
            {
                @operator = Operator.ADD,
                operand = new Campaign
                {
                    name = "campaignA_BatchJob", // required 
                    status = CampaignStatus.PAUSED,
                    advertisingChannelType = AdvertisingChannelType.SEARCH, // required
                    biddingStrategyConfiguration = new BiddingStrategyConfiguration
                    { // required
                        biddingStrategyType = BiddingStrategyType.MANUAL_CPC
                    },
                    budget = sharedBudget // required 
                }
            });

            operations.Add(new CampaignOperation
            {
                @operator = Operator.ADD,
                operand = new Campaign
                {
                    name = "campaignB_BatchJob",
                    status = CampaignStatus.PAUSED,
                    advertisingChannelType = AdvertisingChannelType.SEARCH,
                    biddingStrategyConfiguration = new BiddingStrategyConfiguration {
                        biddingStrategyType = BiddingStrategyType.MANUAL_CPC
                    },
                    budget = sharedBudget // required 
                }
            });

            // use the helper utility to upload the operations to the job upload url
            var batchJobHelper = new BatchJobUtilities(user);
            // exchange the bulk upload URL for a resumable upload URL!
            var resumableUploadUrl = batchJobHelper.GetResumableUploadUrl(jobUploadURL);
            batchJobHelper.Upload(resumableUploadUrl, operations.ToArray());
            Console.WriteLine("{0} operations uploaded to the job", operations.Count);
        }
        #endregion

        // reports

        #region ExecuteReport
        // using ExecuteReport(ReportDefinitionReportType.ACCOUNT_PERFORMANCE_REPORT)        
        public void ExecuteReport(ReportDefinitionReportType ReportType)
        {
            var user = new AdWordsUser();
            var service = (ReportDefinitionService)user.GetService(AdWordsService.v201601.ReportDefinitionService);

            var fields = service.getReportFields(ReportType);
            
            foreach (var field in fields) 
                Console.WriteLine("{0} ({1})", field.fieldName,field.fieldType);
        }
        #endregion


        public void Test() {
            
        }
    }
}