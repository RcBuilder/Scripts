using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.Util.Reports;
using Google.Api.Ads.AdWords.v201708;
using Google.Api.Ads.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleAdwordsAPI
{
    public class GoogleAdwordsProxy
    {
        private AdWordsUser user;

        public GoogleAdwordsProxy()
        {
            this.user = new AdWordsUser();
        }

        public int CountCampaigns()
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201708.CampaignService);

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

        public List<long> GetALLCampaigns()
        {
            var service = (CampaignService)this.user.GetService(AdWordsService.v201708.CampaignService);

            var selector = new Selector
            {
                fields = new string[] {
                    Campaign.Fields.Id
                },
                paging = new Paging
                {
                    numberResults = this.CountCampaigns(),
                    startIndex = 0
                }
            };

            var campaignPage = service.get(selector);                        
            if (campaignPage.entries == null)
                return null;
            return campaignPage.entries.Select(x => x.id).ToList();
        }

        public List<ManagedAccount> GetManagedAccounts()
        {
            var service = (ManagedCustomerService)user.GetService(AdWordsService.v201708.ManagedCustomerService);

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
                    Name = x.name
                }).ToList();
        }

        public List<ChangeDetails> GetALLAccountsChanges(DateTime From, DateTime To)
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

                        var hasChanged = this.GetAccountHasChanged(From, To, account.Id.ToString());                        
                        changes.Add(new ChangeDetails(account.Id.ToString(), hasChanged));                        
                    }
                    catch(Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return changes;
        }

        public bool GetAccountHasChanged(DateTime From, DateTime To, string AccountId = null)
        {
            // save the current selected account (probably the mcc root)
            var originalAccountId = this.GetActiveAccount();

            // change active account 
            this.SetActiveAccount(AccountId);

            try
            {
                var service = (CustomerSyncService)this.user.GetService(AdWordsService.v201708.CustomerSyncService);

                // get all campaigns                 
                var campaignIds = this.GetALLCampaigns();
                if (campaignIds == null || campaignIds.Count == 0)
                    return false;

                var selector = new CustomerSyncSelector
                {
                    campaignIds = campaignIds.ToArray(),
                    dateTimeRange = new DateTimeRange
                    {
                        min = DateTimeParse(From),
                        max = DateTimeParse(To),
                    },
                };

                var changesDataValue = service.get(selector);
                if (changesDataValue == null || changesDataValue.changedCampaigns == null || changesDataValue.changedCampaigns.Length == 0)
                    return false;
                return true;
            }
            finally
            {
                // change-back to the original account 
                this.SetActiveAccount(originalAccountId);
            }            
        }

        private bool SetActiveAccount(string CustomerId)
        {
            try
            {
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

        private string DateTimeParse(DateTime Value)
        {
            return Value.ToString("yyyyMMdd HHmmss");
        }
    }
}