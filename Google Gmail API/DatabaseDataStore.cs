using Dapper;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Google.Apis.Util.Store;

namespace Website.Common
{
    /*
        // SQL
        SELECT * FROM GoogleAuthTokens

        CREATE TABLE GoogleAuthTokens (
            [Key] NVARCHAR(255) PRIMARY KEY,
            [Value] NVARCHAR(MAX)
        );

        CREATE PROCEDURE StoreGoogleAuthToken
            @Key NVARCHAR(256),
            @Value NVARCHAR(MAX)
        AS
        BEGIN
            IF EXISTS (SELECT 1 FROM GoogleAuthTokens WHERE [Key] = @Key)
            BEGIN
                UPDATE GoogleAuthTokens
                SET [Value] = @Value
                WHERE [Key] = @Key;
            END
            ELSE
            BEGIN
                INSERT INTO GoogleAuthTokens ([Key], [Value]) 
                VALUES (@Key, @Value);
            END
        END
        GO

        CREATE PROCEDURE GetGoogleAuthToken
            @Key NVARCHAR(256)
        AS
        BEGIN
            SELECT [Value] FROM GoogleAuthTokens WHERE [Key] = @Key;
        END
        GO

        CREATE PROCEDURE DeleteGoogleAuthToken
            @Key NVARCHAR(256)
        AS
        BEGIN
            DELETE FROM GoogleAuthTokens WHERE [Key] = @Key;
        END
        GO

        CREATE PROCEDURE ClearGoogleAuthTokens
        AS
        BEGIN
            DELETE FROM GoogleAuthTokens;
        END
        GO

        --

        [AllowAnonymous]
        public ActionResult GoogleLogin()
        {
            var ClientId = ConfigurationManager.AppSettings["GoogleClientId"].Trim();            
            var GoogleCallbackURL = ConfigurationManager.AppSettings["GoogleCallbackURL"].Trim();

            var redirectUrl = $"https://accounts.google.com/o/oauth2/auth?client_id={ClientId}&redirect_uri={Uri.EscapeDataString(GoogleCallbackURL)}&response_type=code&scope=email";
            return Redirect(redirectUrl);
        }
        
        --

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallback(string code)
        {            
            Debug.WriteLine("In ExternalLoginCallback...");
            var step = 1;
            
            try
            {
                var ClientId = ConfigSingleton.Instance.GoogleClientId;
                var ClientSecret = ConfigSingleton.Instance.GoogleClientSecret;
                var GoogleCallbackURL = ConfigSingleton.Instance.GoogleCallbackURL;

                step = 2;

                step = 3;                

                var tokenResponse = await new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = ClientId,
                            ClientSecret = ClientSecret
                        }                        
                    }).ExchangeCodeForTokenAsync("user", code, GoogleCallbackURL, CancellationToken.None);

                step = 4;

                var service = new Oauth2Service(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromAccessToken(tokenResponse.AccessToken)
                });

                step = 5;

                var userInfo = await service.Userinfo.Get().ExecuteAsync();

                step = 6;

                var DataStore = new DatabaseDataStore(ConfigSingleton.Instance.ConnStr);  // CUSTOM
                ///var DataStore = new FileDataStore($"{AppContext.BaseDirectory}", true);   // FILE-PATH
                ///var DataStore = new NullDataStore();    // NO STORAGE (In-Memory)
                await DataStore.StoreAsync(userInfo.Email, tokenResponse);               

                step = 7;

                return RedirectToAction("Index", "Home");
}
            catch (Exception ex) {
                FileIO.AppendAllText($"{AppContext.BaseDirectory}\\errors.txt", $"{ex.Message} (step {step})\n\r");
                throw;
            }
        } 
    */

    public class DatabaseDataStore : IDataStore
    {
        private readonly string _connectionString;

        public DatabaseDataStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            var data = JsonConvert.SerializeObject(value);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "StoreGoogleAuthToken";
                var parameters = new { Key = key, Value = data };
                await connection.ExecuteAsync(sql, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "GetGoogleAuthToken";
                var result = await connection.QuerySingleOrDefaultAsync<string>(sql, new { Key = key }, commandType: System.Data.CommandType.StoredProcedure);
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default;
            }
        }

        public async Task DeleteAsync<T>(string key)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "DeleteGoogleAuthToken";
                await connection.ExecuteAsync(sql, new { Key = key }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task ClearAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "ClearGoogleAuthTokens";
                await connection.ExecuteAsync(sql, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }

}