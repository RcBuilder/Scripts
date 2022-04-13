using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WizColudEntities;

namespace WizColudProxy
{
    /*
        USING
        -----
        var wizColudManager = new WizColudManager(new WizColudConfig {
            ServerName = "lb1.wizcloud.co.il",
            ApiToken = "xxxxxxxxxxxxxxxxxxx",
            DBName = "wizdb493n4"                
        });

        wizColudManager.TokensUpdated += (sender, eventArgs) => {
            Console.WriteLine($"Tokens Updated: {eventArgs.AccessToken}");
        };

        try
        {
            // [Companies]
            var companyList = await wizColudManager.GetCompanyList();
            foreach(var c in companyList.Companies)
                Console.WriteLine($"[Company] {c?.Name}");

            // [Accounts Report]
            var exportedAccounts = await wizColudManager.GetAccounts("0c61e29...d40017f");
            foreach (var a in exportedAccounts)
                Console.WriteLine($"[ExportedAccount] #{a?.Key}");

            // [JournalEntry]
            var batchNo = await wizColudManager.SaveJournalEntry(new JournalEntry
            {
                CreditAccountKey = "acc-1",
                DebitAccountKey = "acc-2",
                Description = "Some description",
                Referance = 1000,
                ValueDate = DateTime.Now.ToString("dd/MM/yyyy"),
                DueDate = DateTime.Now.ToString("dd/MM/yyyy"),
                TotalNIS = 50,
                Quantity = 2,
                BranchNo = 1,
                Remarks = "Some notes"
            });
            Console.WriteLine($"[JournalEntry] #{batchNo}");


            // [Account]
            var isAccountSaved = await wizColudManager.SaveAccount(new Account { 
                AccountKey = $"acc-{DateTime.Now.ToString("HHmm")}",
                FullName = "TEST ACCOUNT",
                Sort = 1,
                DeductionPercentage = 10,
                Email = "test@test.com",
                Country = "Israel",
                Address = "Somewhere over the rainbow",
                Phone = "03-5555555",
                Occupation = "Developer",
                Remarks = "Some notes",
                WebSite = "http://rcb.co.il",
                BankAccount = "123456",
                BankCode = "10",
                BranchCode = "943"
            });
            Console.WriteLine($"[Account] {isAccountSaved}");
        }            
        catch (Exception ex) {
            Console.WriteLine($"[EX] {ex.Message}");
        } 
    */

    public class WizColudManager : IWizColudManager
    {
        public EventHandler<TokensUpdatedEventArgs> TokensUpdated;

        protected WizColudConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public WizColudManager(WizColudConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        protected void OnTokensUpdated(TokensUpdatedEventArgs Args) {
            if (TokensUpdated == null) return;
            TokensUpdated(null, Args);
        }

        public async Task<bool> RefreshToken()
        {            
            var response = await this.HttpService.GET_ASYNC($"https://{this.Config.ServerName}/createSession/{this.Config.ApiToken}/{this.Config.DBName}");

            if (!response.Success)
                return false;

            this.Config.AccessToken = response.Content;  //  update instance config   
            this.OnTokensUpdated(new TokensUpdatedEventArgs(this.Config.AccessToken));  // raise an event
            return true;
        }

        public async Task<CompanyList> GetCompanyList()
        {            
            if(string.IsNullOrEmpty(this.Config.AccessToken))
                await this.RefreshToken();

            var response = await this.HttpService.POST_ASYNC<string, CompanyList>($"https://{this.Config.ServerName}/CompanyListToTokenApi/TokenCompanies", "", headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",                
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });
            var status = this.ParseResponse(response.Content);

            // Unauthorized - refresh token and try again            
            if (!status.Success && status.Error == "Token is not valid")
            {                
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<string, CompanyList>($"https://{this.Config.ServerName}/CompanyListToTokenApi/TokenCompanies", "", headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
                status = this.ParseResponse(response.Content);
            }

            if (!status.Success)
                throw new Exception(response.Content);
            return response.Model;
        }

        public async Task<bool> SaveAccount(Account Account)
        {
            if (string.IsNullOrEmpty(this.Config.AccessToken))
                await this.RefreshToken();

            var saveAccountRequest = new SaveAccountRequest(Account);
            var response = await this.HttpService.POST_ASYNC<SaveAccountRequest>($"https://{this.Config.ServerName}/IndexApi/importIndex", saveAccountRequest, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });
            var status = this.ParseResponse(response.Content);

            // Unauthorized - refresh token and try again            
            if (!status.Success && status.Error == "Token is not valid")
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<SaveAccountRequest>($"https://{this.Config.ServerName}/IndexApi/importIndex", saveAccountRequest, headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
                status = this.ParseResponse(response.Content);
            }

            if (!status.Success)
                throw new Exception(response.Content);
            return true;
        }

        public async Task<int> SaveJournalEntry(JournalEntry JournalEntry)
        {
            if (string.IsNullOrEmpty(this.Config.AccessToken))
                await this.RefreshToken();

            var saveJournalEntryRequest = new SaveJournalEntryRequest(JournalEntry);
            var response = await this.HttpService.POST_ASYNC<SaveJournalEntryRequest>($"https://{this.Config.ServerName}/jtransApi/tmpBatch", saveJournalEntryRequest, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });
            var status = this.ParseResponse(response.Content);

            // Unauthorized - refresh token and try again            
            if (!status.Success && status.Error == "Token is not valid")
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<SaveJournalEntryRequest>($"https://{this.Config.ServerName}/jtransApi/tmpBatch", saveJournalEntryRequest, headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
                status = this.ParseResponse(response.Content);
            }

            if (!status.Success)
                throw new Exception(response.Content);

            var modelSchema = new {
                batchno = 0
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return responseData.batchno;
        }

        public async Task<IEnumerable<ExportAccount>> GetAccounts(string DataFile) {
            var result = await this.ExportData<ExportDataResult<ExportAccount>>(new ExportDataRequest(DataFile, new ExportDataParameter
            {
                FieldName = "SortCode",
                FieldValue = "300"
            }));

            return result?.Data;
        }

        // --- 

        private async Task<T> ExportData<T>(ExportDataRequest ExportDataRequest)
        {
            if (string.IsNullOrEmpty(this.Config.AccessToken))
                await this.RefreshToken();
           
            var response = await this.HttpService.POST_ASYNC<ExportDataRequest, T>($"https://{this.Config.ServerName}/ExportDataApi/exportData", ExportDataRequest, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });
            var status = this.ParseResponse(response.Content);

            // Unauthorized - refresh token and try again            
            if (!status.Success && status.Error == "Token is not valid")
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<ExportDataRequest, T>($"https://{this.Config.ServerName}/ExportDataApi/exportData", ExportDataRequest, headers: new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
                status = this.ParseResponse(response.Content);
            }

            if (!status.Success)
                throw new Exception(response.Content);
            return response.Model;
        }

        private ResponseStatus ParseResponse(string ErrorRaw)
        {                        
            return JsonConvert.DeserializeObject<ResponseStatus>(ErrorRaw);            
        }
    }
}
