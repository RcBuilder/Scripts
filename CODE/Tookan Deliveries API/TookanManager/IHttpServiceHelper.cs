using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Helpers
{
    public interface IHttpServiceHelper
    {
        (bool Success, HttpStatusCode StatusCode, string Content) GET(string url, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content, T Model) GET<T>(string url, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, HttpStatusCode StatusCode, string Content) POST<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) POST<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, HttpStatusCode StatusCode, string Content) PUT<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) PUT<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, HttpStatusCode StatusCode, string Content) DELETE<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) DELETE<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, HttpStatusCode StatusCode, string Content) POST_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content) POST_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) POST_DATA<TResult>(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);

        (bool Success, HttpStatusCode StatusCode, string Content) PUT_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content) PUT_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content) DELETE_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        (bool Success, HttpStatusCode StatusCode, string Content) DELETE_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
    }

    public interface IAsyncHttpServiceHelper
    {
        Task<(bool Success, HttpStatusCode StatusCode, string Content)> GET_ASYNC(string url, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content, T Model)> GET_ASYNC<T>(string url, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> POST_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> PUT_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> DELETE_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> POST_DATA_ASYNC<TResult>(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);

        Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null);
        Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null);
    }
}
