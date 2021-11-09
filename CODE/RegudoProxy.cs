using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanHotelsConnector
{
    public interface IRegudoProxy
    {
        ///Task<RegudoToken> Login(string Email, string Password);
        ///Task<RegudoToken> RefreshToken();

        Task<RegudoEmployee> CreateEmployee(RegudoEmployee Employee);
        Task<RegudoEmployee> UpdateEmployee(RegudoEmployee Employee);
        Task<RegudoEmployee> SaveEmployee(RegudoEmployee Employee);
        Task<string> EmployeeExist(RegudoEmployee Employee);
    }
    
    public class RegudoProxy : IRegudoProxy
    {
        private RegudoConfig Config { get; set; }
        private RegudoToken TokenData { get; set; }
        protected readonly IAsyncHttpServiceHelper HttpServiceHelper;

        public RegudoProxy(RegudoConfig Config, IAsyncHttpServiceHelper HttpServiceHelper) {
            if (!Config.Server.EndsWith("/")) Config.Server = $"{Config.Server}/";  // fix server path

            this.Config = Config;                     
            this.HttpServiceHelper = HttpServiceHelper;
        }

        protected async Task<RegudoToken> Login(string Email, string Password)
        {
            var endpoint = $"{this.Config.Server}login";
            LoggerSingleton.Instance.Info(endpoint);

            var result = await this.HttpServiceHelper.POST_DATA_ASYNC<RegudoToken>(endpoint, new List<string> { $"email={Email}", $"password={Password}" }, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/x-www-form-urlencoded"
            });

            if (!result.Success)
                throw new Exception($"Login Failed With status code {result.StatusCode}");

            this.TokenData = result.Model;
            return result.Model;
        }

        protected async Task<RegudoToken> RefreshToken()
        {
            var endpoint = $"{this.Config.Server}refreshToken";
            LoggerSingleton.Instance.Info(endpoint);

            var result = await this.HttpServiceHelper.POST_DATA_ASYNC<RegudoToken>(endpoint, new List<string> { $"refreshToken={this.TokenData.RefreshToken}" }, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/x-www-form-urlencoded"
            });
            
            this.TokenData.Token = result.Model.Token;
            this.TokenData.RefreshToken = result.Model.RefreshToken;

            return result.Model;
        }

        protected int ParseErrorStatusCode(string Content) {
            try
            {
                var ErrorJson = Content.Split('|')[1].Trim();

                var ModelSchema = new
                {
                    error = new
                    {
                        type = "",
                        message = "",
                        data = new
                        {
                            name = "",
                            message = "",
                            status = 0
                        }
                    }
                };

                var model = JsonConvert.DeserializeAnonymousType(ErrorJson, ModelSchema);
                return model.error.data.status;   // http status code
            }
            catch(Exception ex) {
                Console.WriteLine($"[ERROR] ParseErrorStatusCode, ex: {ex.Message}");
                return -1;
            }
        }

        protected string ParseEmployeeId(string Content)
        {
            var ModelSchema = new
            {
                empId = ""
            };

            var model = JsonConvert.DeserializeAnonymousType(Content, ModelSchema);
            return model.empId;   // employee id
        }

        // ----

        public async Task<RegudoEmployee> CreateEmployee(RegudoEmployee Employee)
        {
            if (TokenData == null)
                await this.Login(this.Config.UserName, this.Config.Password);

            var endpoint = $"{this.Config.Server}{this.TokenData.ActiveUser.OrganizationId}/{this.TokenData.ActiveUser.Id}/createEmployee";
            LoggerSingleton.Instance.Info(endpoint);

            var result = await this.HttpServiceHelper.POST_ASYNC<RegudoEmployee, RegudoEmployee>(endpoint, Employee, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"JWT {this.TokenData.Token}"
            });
            
            // try to refresh token
            if (!result.Success) {
                /// var statusCode = this.ParseErrorStatusCode(result.Content);
                var statusCode = (int)result.StatusCode;
                if (statusCode == 403 || statusCode == 401) { 
                    await this.RefreshToken();

                    result = await this.HttpServiceHelper.POST_ASYNC<RegudoEmployee, RegudoEmployee>(endpoint, Employee, null, new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json",
                        ["Authorization"] = $"JWT {this.TokenData.Token}"
                    });
                }
            }

            if (!result.Success)
                throw new Exception(result.Content);
            return result.Model;
        }

        public async Task<RegudoEmployee> UpdateEmployee(RegudoEmployee Employee)
        {
            if (TokenData == null)
                await this.Login(this.Config.UserName, this.Config.Password);

            var endpoint = $"{this.Config.Server}{this.TokenData.ActiveUser.OrganizationId}/{this.TokenData.ActiveUser.Id}/updateEmployee";
            LoggerSingleton.Instance.Info(endpoint);

            var result = await this.HttpServiceHelper.PUT_ASYNC<RegudoEmployee, RegudoEmployee>(endpoint, Employee, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"JWT {this.TokenData.Token}"
            });

            // try to refresh token
            if (!result.Success)
            {
                /// var statusCode = this.ParseErrorStatusCode(result.Content);
                var statusCode = (int)result.StatusCode;
                if (statusCode == 403 || statusCode == 401)
                {
                    await this.RefreshToken();

                    result = await this.HttpServiceHelper.PUT_ASYNC<RegudoEmployee, RegudoEmployee>(endpoint, Employee, null, new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json",
                        ["Authorization"] = $"JWT {this.TokenData.Token}"
                    });
                }
            }

            if (!result.Success)
                throw new Exception(result.Content);
            return result.Model;
        }

        public async Task<RegudoEmployee> SaveEmployee(RegudoEmployee Employee) {
            if (TokenData == null)
                await this.Login(this.Config.UserName, this.Config.Password);

            var employeeId = await this.EmployeeExist(Employee);
            if (string.IsNullOrEmpty(employeeId)) return await this.CreateEmployee(Employee);

            Employee.Id = employeeId;
            return await this.UpdateEmployee(Employee);
        }

        public async Task<string> EmployeeExist(RegudoEmployee Employee)
        {
            if (TokenData == null)
                await this.Login(this.Config.UserName, this.Config.Password);

            var endpoint = $"{this.Config.Server}{this.TokenData.ActiveUser.OrganizationId}/{this.TokenData.ActiveUser.Id}/checkIfEmployeeExist";
            LoggerSingleton.Instance.Info(endpoint);

            var result = await this.HttpServiceHelper.POST_ASYNC(endpoint, Employee, null, new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = $"JWT {this.TokenData.Token}"
            });

            // try to refresh token
            if (!result.Success)
            {
                /// var statusCode = this.ParseErrorStatusCode(result.Content);
                var statusCode = (int)result.StatusCode;
                if (statusCode == 403 || statusCode == 401)
                {
                    await this.RefreshToken();

                    result = await this.HttpServiceHelper.POST_ASYNC(endpoint, Employee, null, new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json",
                        ["Authorization"] = $"JWT {this.TokenData.Token}"
                    });
                }
            }

            if (!result.Success)
                throw new Exception(result.Content);
            return this.ParseEmployeeId(result.Content);
        }
    }
}
