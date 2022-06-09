using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HConnectEntities;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;


namespace HConnectProxy
{
    /*
        USING
        -----
        var hConnectManager = new HConnectManager(new HConnectConfig
            {
                Station = "xxxxxxxxxxxxxxxxxxxxxxx",
                Company = "demo",
                Provider = "250030",
                Token = "xxxxxxxxxxxxxxxxxxxxxxx"
            });

        try
        {
            // [Accounts Report]
            var exportedAccounts = await hConnectManager.GetAccounts("0c61e29...d40017f");
            foreach (var a in exportedAccounts)
                Console.WriteLine($"[ExportedAccount] #{a?.Key}");

            // [JournalEntry]
            var batchNo = await hConnectManager.SaveJournalEntry(new JournalEntry
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
            var isAccountSaved = await hConnectManager.SaveAccount(new Account { 
                AccountKey = $"acc-{DateTime.Now.ToString("HHmm")}",
                FullName = "TEST ACCOUNT",
                Sort = 1,
                DeductionPercentage = 10,
                Email = "test@test.com",
                Country = "Israel",
                Address = "Somewhere over the rainbow",
                Phone = "03-5555555",                
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
    
    public class HConnectManager : IHConnectManager
    {
        protected const string Server = "https://ws.wizground.com/api";
        protected HConnectConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public HConnectManager(HConnectConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        /*
        public async Task<string> Test() {
            var testClass = new {
                Reference="9000919",
                transtype="הוצ",
                accountkeydeb1="30001",
                accountkeycred1="30001",
                sufdeb1="99.00",
                sufcred1="99.00"
            };

            var payload = this.GenerateAPIRequest(Plugins.MOVEIN, testClass);
            Console.WriteLine(JsonConvert.SerializeObject(payload));

            var response = await this.HttpService.POST_ASYNC($"{HConnectManager.Server}", payload, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            });

            return $"{payload.Signature} | {response.Content}";
        }
        */

        public virtual async Task<bool> SaveAccount(Account Account)
        {
            var status = await this.ImportData<Account, ResponseStatus>(Plugins.HESHIN, Account);
            return status.Success;
        }

        public virtual async Task<bool> SaveJournalEntry(JournalEntry JournalEntry)
        {
            var status = await this.ImportData<JournalEntry, ResponseStatus>(Plugins.MOVEIN, JournalEntry);
            return status.Success;
        }

        public virtual async Task<bool> SaveDocument(Document Document)
        {
            var status = await this.ImportData<Document, ResponseStatus>(Plugins.IMOVEIN, Document);
            return status.Success;
        }

        public virtual async Task<IEnumerable<ExportAccount>> GetAccounts(string DataFile) {
            var result = await this.ExportData<ExportAccount>(new ExportDataRequest(DataFile));
            return result?.Data;            
        }

        // --- 

        protected virtual async Task<TOut> ImportData<TIn, TOut>(string Plugin, TIn Model)
        {
            var Request = this.GenerateAPIRequest(Plugin, new List<TIn> { Model });
            var response = await this.HttpService.POST_ASYNC<APIRequest<List<TIn>>, TOut>($"{HConnectManager.Server}", Request, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            });
            var status = this.ParseResponse(response.Content);

            if (!status.Success)
                throw new Exception(response.Content);
            return response.Model;
        }

        protected virtual async Task<ExportDataResult<T>> ExportData<T>(ExportDataRequest ExportDataRequest)
        {
            var Request = this.GenerateAPIRequest(Plugins.REPORTS, ExportDataRequest);
            var response = await this.HttpService.POST_ASYNC<APIRequest<ExportDataRequest>, ExportDataResultWrapper<T>>($"{HConnectManager.Server}", Request, headers: new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"                
            });
            var status = this.ParseResponse(response.Content);

            if (!status.Success)
                throw new Exception(response.Content);
            return response.Model.Result;
        }

        protected string GenerateSignature<T>(T PluginData)
        {            
            string RemoveWhiteSpaces(string Input) {
                /// return string.Concat(Input.Where(c => !Char.IsWhiteSpace(c)));
                /// return new Regex(@"\s+").Replace(Input, string.Empty);
                return Regex.Replace(Input, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
            }

            // MD5([<model>]<token>)
            string CreateMD5(string Input)
            {
                // using System.Security.Cryptography
                using (var md5 = MD5.Create())
                {
                    var inputBytes = Encoding.UTF8.GetBytes(Input);
                    var hashBytes = md5.ComputeHash(inputBytes);

                    var sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)                    
                        sb.Append(hashBytes[i].ToString("x2"));                    
                    return sb.ToString();
                }
            }
            
            var sPluginData = JsonConvert.SerializeObject(PluginData);
            return CreateMD5($"{RemoveWhiteSpaces(sPluginData)}{this.Config.Token}");
        }

        protected APIRequest<T> GenerateAPIRequest<T>(string Plugin, T PluginData)
        {
            var result = new APIRequest<T> { 
                Company = this.Config.Company,
                Station = this.Config.Station,                
                Plugin = Plugin,
                Message = new APIRequestMessage<T> { 
                    Provider = this.Config.Provider,
                    Data = PluginData
                }
            };

            result.Signature = this.GenerateSignature(result.Message.Data);

            return result;
        }

        /*
            OK
            --
            {
                "apiRes": {
                    "status": "ok"
                },
                "actionType": "movein",
                "messType": "apiReplay"
            }

            EXPORT
            ------
            {
                "apiRes": {
                    "res": "ok",
                    "data": []
                },
                "actionType": "Reports",
                "messType": "apiReplay"
            }

            ERROR
            -----            
            {
                "apiRes": {
                    "err": "station is not connected"
                }
            }

            EXCEPTION
            ---------      
            Internal Server Error | {
                "apiRes": {
                    "err": "station is not connected"
                }
            }
            -
        */
        protected ResponseStatus ParseResponse(string ErrorRaw)
        {
            var result = new ResponseStatus();

            try
            {
                var parts = ErrorRaw.Split('|');
                if (parts != null && parts.Length > 1)
                {
                    result = JsonConvert.DeserializeObject<ResponseStatus>(parts[1].Trim());
                    result.Body.Message = parts[0];
                }
                else
                {
                    result = JsonConvert.DeserializeObject<ResponseStatus>(parts[0].Trim());
                }
            }
            catch (Exception Ex) {
                result.Body = new ResponseStatusBody
                {
                    Error = Ex.Message,
                    Status = "error"
                };
            }

            return result;
        }
    }
}
