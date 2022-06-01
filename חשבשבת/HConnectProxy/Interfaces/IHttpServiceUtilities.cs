
namespace HConnectProxy
{
    public interface IHttpServiceUtilities
    {
        string Base64Encode(string Value);
        string Base64Decode(string Base64Value);
        string GenerateBasicAuthorizationValue(string UserName, string Password);
        bool IsHttpFileExists(string FilePath);
    }
}
