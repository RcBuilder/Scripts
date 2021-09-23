using System;

namespace IntuitProxy
{
    public class TokensUpdatedEventArgs : EventArgs
    {
        public string CompanyId { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }

        public TokensUpdatedEventArgs(string CompanyId, string RefreshToken, string AccessToken)
        {
            this.CompanyId = CompanyId;
            this.RefreshToken = RefreshToken;
            this.AccessToken = AccessToken;
        }
    }

    public class IntuitAPIConfig
    {
        public string BaseURL { get; set; }
        public string BaseAuthURL { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string CompanyId { get; set; }
    }
}
