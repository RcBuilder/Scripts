using Common;
using Newtonsoft.Json;
using System.Collections.Generic;

/*
    USING
    -----

    steps:
    1. Generate a Token using the UserName and Password received from inner-circle
    2. Register a User using the Token from chapter 1
    3. Map a Token to User (can use the user identity as it's token)

    // sample 
    var innerCircleManager = new InnerCircleManager(ConfigSingleton.Instance.InnerCircleUserName, ConfigSingleton.Instance.InnerCirclePassword);
    innerCircleManager.RegisterUser(new InnerCircleUser { 
        Email = subscription.Details.Email,
        Id = subscriptionId.ToString(),
        FirstName = subscription.Details.FirstName,
        LastName = subscription.Details.LastName
    });
    innerCircleManager.MapUserToken(subscriptionId.ToString());
*/
namespace WebsiteBLL
{
    #region Entities    
    public class InnerCircleUser
    {
        [JsonProperty(PropertyName = "fid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "addAllCircles")]
        public bool AddAllCircles { get; set; }
    }
    #endregion

    public class InnerCircleManager
    {
        #region Models
        private class getTokenRequest
        {
            [JsonProperty(PropertyName = "username")]
            public string UserName { get; set; }

            [JsonProperty(PropertyName = "password")]
            public string Password { get; set; }            
        }

        private class setAuthenticationRequest
        {
            [JsonProperty(PropertyName = "contributorId")]
            public string UserId { get; set; }

            [JsonProperty(PropertyName = "token")]
            public string Token { get; set; }            
        }
        #endregion

        private const string SERVER = "https://apq.inner-circle.io/";

        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected string Token { get; set; }

        public InnerCircleManager(string UserName, string Password) {
            this.UserName = UserName;
            this.Password = Password;

            this.Token = this.GenerateToken();
        }

        public string GenerateToken() {            
            return new HttpServiceHelper().POST($"{SERVER}getToken", 
                new getTokenRequest { 
                    UserName = this.UserName, 
                    Password = this.Password 
                }, 
                headers: new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                }).Content;
        }

        public bool MapUserToken(string UserId) { return this.MapUserToken(UserId, UserId); }
        public bool MapUserToken(string UserId, string Token) {
            return new HttpServiceHelper().POST($"{SERVER}setAuthentication",
                new setAuthenticationRequest
                {
                    UserId = UserId,
                    Token = Token
                },
                headers: new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = this.Token
                }).Success;
        }

        public bool RegisterUser(InnerCircleUser User) {
            return new HttpServiceHelper().POST($"{SERVER}addNewContributor",
                User,
                headers: new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = this.Token
                }).Success;
        }
    }
}
