﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Helpers;
using Newtonsoft.Json;

namespace IntuitProxy
{
    /*        
        // QA
        var intuitAPIManager = new IntuitAPIManager(new IntuitAPIConfig { 
            BaseURL = "https://sandbox-quickbooks.api.intuit.com/v3/",
            BaseAuthURL = "https://oauth.platform.intuit.com/",
            ClientId = "xxxxxxxxxxxxxxxxxxxxxxxxx",
            ClientSecret = "xxxxxxxxxxxxxxxxxxxx",
            CompanyId = "4620816365179539720",
            AccessToken = "xxxxxxxxxxxxxxxxxxxx",
            RefreshToken = "xxxxxxxxxxxxxxxxxxx"
        });        

        // PROD
        var intuitAPIManager = new IntuitAPIManager(new IntuitAPIConfig
        {
            BaseURL = "https://quickbooks.api.intuit.com/v3/",
            BaseAuthURL = "https://oauth.platform.intuit.com/",
            ClientId = "xxxxxxxxxxxxxxxxxxxxxxxxx",
            ClientSecret = "xxxxxxxxxxxxxxxxxxxx",
            CompanyId = "9130351241967566",
            AccessToken = "xxxxxxxxxxxxxxxxxxxx",
            RefreshToken = "xxxxxxxxxxxxxxxxxxx"
        });

        intuitAPIManager.TokensUpdated += (sender, eventArgs) => {
            Console.WriteLine($"Tokens Updated #{eventArgs.CompanyId}");
        };    

        var companyInfo = await intuitAPIManager.GetCompanyInfo();
        Console.WriteLine($"[Company] {companyInfo?.Name}");
    */
    public class IntuitAPIManager : IAPIManager
    {
        public EventHandler<TokensUpdatedEventArgs> TokensUpdated;

        protected IntuitAPIConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }
        
        public IntuitAPIManager(IntuitAPIConfig Config) {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        protected void OnTokensUpdated(TokensUpdatedEventArgs args)
        {
            if (TokensUpdated == null) return;
            TokensUpdated(null, args);
        }

        public async Task<bool> Authorize() {
            throw new NotImplementedException();
        }

        public async Task<bool> RefreshToken()
        {            
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseAuthURL}oauth2/v1/tokens/bearer", new List<string> { 
                "grant_type=refresh_token", 
                $"refresh_token={this.Config.RefreshToken}" 
            }, null, new Dictionary<string, string> {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/x-www-form-urlencoded",
                ["Authorization"] = this.HttpService.GenerateBasicAuthorizationValue(this.Config.ClientId, this.Config.ClientSecret)
            });

            if (!response.Success)
                return false;

            var modelSchema = new
            {
                x_refresh_token_expires_in = 0,
                refresh_token = string.Empty,
                access_token = string.Empty,
                token_type = string.Empty,
                expires_in = 0
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            this.Config.AccessToken = responseData.access_token;
            this.Config.RefreshToken = responseData.refresh_token;

            // raise an event
            this.OnTokensUpdated(new TokensUpdatedEventArgs(this.Config.CompanyId, this.Config.RefreshToken, this.Config.AccessToken));

            return true;
        }

        public async Task<APICompanyInfo> GetCompanyInfo()
        {
            var query = "Select * from CompanyInfo";
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                query
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/text",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                    query
                }, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/text",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            var companySchema = new {
                CompanyName = string.Empty
            };

            var modelSchema = new {
                QueryResponse = new {
                    CompanyInfo = new[] { companySchema }
                }
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return new APICompanyInfo {
                Name = responseData.QueryResponse.CompanyInfo.FirstOrDefault()?.CompanyName
            };
        }

        public async Task<IEnumerable<APIAccount>> GetAccounts()
        {
            var query = "Select * from Account";
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                query
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/text",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                    query
                }, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/text",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            var accountSchema = new
            {
                Id = string.Empty,
                Name = string.Empty
            };

            var modelSchema = new
            {
                QueryResponse = new
                {
                    Account = new[] { accountSchema }
                }
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return responseData?.QueryResponse?.Account?.Select(x => new APIAccount { 
                Id = x.Id,
                Name = x.Name
            });
        }

        public async Task<string> UploadJournalEntry(APIJournalEntry Entry)
        {
            // fix negative amount 
            Entry.Lines?.ToList()?.ForEach(line => {                
                line.Amount = Math.Abs(line.Amount);
            });

            var response = await this.HttpService.POST_ASYNC<APIJournalEntry>($"{this.Config.BaseURL}company/{this.Config.CompanyId}/journalentry", Entry, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_ASYNC<APIJournalEntry>($"{this.Config.BaseURL}company/{this.Config.CompanyId}/journalentry", Entry, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));
            
            var modelSchema = new {
                JournalEntry = new {
                    Id = string.Empty
                }
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return responseData?.JournalEntry?.Id;
        }

        public async Task<IEnumerable<string>> UploadJournalEntries(IEnumerable<APIJournalEntry> Entries) {
            var entryIds = new List<string>();
            foreach (var entry in Entries)
                entryIds.Add(await this.UploadJournalEntry(entry));
            return entryIds;
        }
        
        public async Task<IEnumerable<string>> UploadBankTransactions(IEnumerable<APIBankTransaction> Entries) {
            return await this.UploadJournalEntries(Entries.Select(e => (APIJournalEntry)e));
        }

        public async Task<IEnumerable<APIVendor>> GetVendors() {
            var query = "Select * from Vendor";
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                query
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/text",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                    query
                }, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/text",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            var vendorSchema = new
            {
                Id = string.Empty,
                DisplayName = string.Empty
            };

            var modelSchema = new
            {
                QueryResponse = new
                {
                    Vendor = new[] { vendorSchema }
                }
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return responseData?.QueryResponse?.Vendor?.Select(x => new APIVendor { 
                Id = x.Id,
                DisplayName = x.DisplayName
            });
        }

        public async Task<IEnumerable<APICustomer>> GetCustomers() {
            var query = "Select * from Customer";
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                query
            }, null, new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/text",
                ["Authorization"] = $"Bearer {this.Config.AccessToken}"
            });

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await this.RefreshToken();

                response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                    query
                }, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/text",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            var customerSchema = new
            {
                Id = string.Empty,
                DisplayName = string.Empty
            };

            var modelSchema = new
            {
                QueryResponse = new
                {
                    Customer = new[] { customerSchema }
                }
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);
            return responseData?.QueryResponse?.Customer?.Select(x => new APICustomer
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            });
        }

        // --- 

        private APIErrorResponse ParseError(string ErrorRaw) {
            /*
                The remote server returned an error: (401)Unauthorized.|
                {
                  "warnings": null,
                  "fault": {
                    "error": [
                      {
                        "message": "message=AuthenticationFailed; errorCode=003200; statusCode=401",
                        "detail": "Token expired: AB01629616862oj1SBIYYkD4qmTYCdcbhGWtOzizsMKpJNDXhU",
                        "code": "3200",
                        "element": null
                      }
                    ],
                    "type": "AUTHENTICATION"
                  },
                  "report": null,
                  "requestId": null,
                  "time": 1629905161081,
                  "status": null
                } 
            */

            var errorRawParts = ErrorRaw.Split('|');

            var errorSchema = new {
                message = string.Empty,
                detail = string.Empty
            };
            
            var exSchema = new {
                fault = new { 
                    error = new[] { errorSchema }
                },
                type = string.Empty
            };

            var exData = JsonConvert.DeserializeAnonymousType(errorRawParts[1], exSchema)?.fault?.error?.FirstOrDefault();
            return new APIErrorResponse
            {
                Message = errorRawParts[0].Trim(),
                InnerMessage = (
                    exData?.message?.Trim() ?? string.Empty,
                    exData?.detail?.Trim() ?? string.Empty
                )
            };            
        }
    }
}
