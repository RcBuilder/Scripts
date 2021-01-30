using System.Collections.Generic;

namespace BLL
{
    public interface IHttpServiceHelper
    {
        (bool Success, string Content) GET(string url, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content, T Model) GET<T>(string url, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, string Content) POST<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) PUT<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) DELETE<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, string Content) POST_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) POST_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) PUT_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) PUT_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) DELETE_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, string Content) DELETE_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
    }
}
