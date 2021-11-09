using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    /*
        USING
        -----
        var result = new HttpServiceHelper().POST(<url>, <model>, null, new Dictionary<string, string> { 
            ["Content-Type"] = "application/json"
        });

        if (!result.Success) 
            throw new Exception(result.Content);
        return Request.CreateResponse(HttpStatusCode.OK, result);        

        -
        
        var result = new HttpServiceHelper().GET<SystemReportData<string>>(<url>, headers: <headers>);

        if (!result.Success) 
            throw new Exception(result.Content);
        return Request.CreateResponse(HttpStatusCode.OK, result);  

        -

        var result = await this.HttpServiceHelper.POST_ASYNC(<url>, <model>, null, new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json",
            ["Authorization"] = $"JWT {this.TokenData.Token}"
        });

        if (!result.Success) 
            throw new Exception(result.Content);
        return Request.CreateResponse(HttpStatusCode.OK, result);

        - 

        var result = await this.HttpServiceHelper.POST_DATA_ASYNC<T>(<url>, new List<string> { <param-1>, <param-2>...<param-N> }, null, new Dictionary<string, string>
        {
            ["Content-Type"] = "application/x-www-form-urlencoded"
        });

        if (!result.Success) 
            throw new Exception(result.Content);
        return Request.CreateResponse(HttpStatusCode.OK, result);

        ----

        // Sample: RefreshToken
        var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseAuthURL}oauth2/v1/tokens/bearer", new List<string> { 
            "grant_type=refresh_token", 
            $"refresh_token={this.Config.RefreshToken}" 
        }, null, new Dictionary<string, string> {
            ["Accept"] = "application/json",
            ["Content-Type"] = "application/x-www-form-urlencoded",
            ["Authorization"] = this.HttpService.GenerateBasicAuthorizationValue(this.Config.ClientId, this.Config.ClientSecret)
        });

        if (response.Success) {
            var modelSchema = new {
                x_refresh_token_expires_in = 0,
                refresh_token = string.Empty,
                access_token = string.Empty,
                token_type = string.Empty,
                expires_in = 0
            };

            var responseData = JsonConvert.DeserializeAnonymousType(response.Content, modelSchema);                
            this.Config.AccessToken = responseData.access_token;
            this.Config.RefreshToken = responseData.refresh_token;
        }

        -

        // Sample: handle Unauthorized
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
            return true;
        }

        public async Task<APICompanyInfo> GetCompanyInfo()
        {                                            
            var response = await this.HttpService.POST_DATA_ASYNC($"{this.Config.BaseURL}company/{this.Config.CompanyId}/query", new List<string> {
                "Select * from CompanyInfo"
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
                    "Select * from CompanyInfo"
                }, null, new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/text",
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                });
            }

            if (!response.Success)
                return null;

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

    */
    public class HttpServiceHelper : IHttpServiceHelper, IAsyncHttpServiceHelper, IHttpServiceUtilities
    {
        private const double TimeOutSec = 10;
        private const HttpStatusCode OK = HttpStatusCode.OK;
        private const HttpStatusCode ERROR = HttpStatusCode.BadRequest;

        private WebClient client { get; set; } = new WebClient {
            Proxy = null,
            Encoding = Encoding.UTF8            
        };

        public (bool Success, HttpStatusCode StatusCode, string Content) GET(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                client.Headers.Clear();
                if (headers != null)
                    foreach (var header in headers)
                        client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                return (true, OK, client.DownloadString(url));
            }
            catch (WebException ex)
            {
                var webRespEX = ((HttpWebResponse)ex.Response);
                var statusCode = webRespEX?.StatusCode ?? ERROR;

                var stream = ex?.Response?.GetResponseStream();
                if(stream == null) return (false, statusCode, $"{ex.Message}");

                using (var reader = new StreamReader(stream))
                    return (false, statusCode, $"{ex.Message} | {reader.ReadToEnd()}");
            }
            catch (Exception ex) {                
                return (false, ERROR, ex.Message);
            }
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) GET<TResult>(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var response = this.GET(url, querystring, headers);

                var model = default(TResult);
                if (response.Success)
                    model = JsonConvert.DeserializeObject<TResult>(response.Content);
                else if (response.Content.Contains("{"))
                {
                    try
                    {
                        var json = response.Content.Substring(response.Content.IndexOf("{"));
                        model = JsonConvert.DeserializeObject<TResult>(json);
                    }
                    catch { }
                }

                return (response.Success, response.StatusCode, response.Content, model);
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message, default(TResult));
            }
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) POST<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, "JSON", querystring, headers, "POST");
        }
        
        public (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) POST<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD<TPayload, TResult>(url, payload, "JSON", querystring, headers, "POST");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) PUT<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, "JSON", querystring, headers, "PUT");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) PUT<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return UPLOAD<TPayload, TResult>(url, payload, "JSON", querystring, headers, "PUT");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) DELETE<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD(url, payload, "JSON", querystring, headers, "DELETE");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) DELETE<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return UPLOAD<TPayload, TResult>(url, payload, "JSON", querystring, headers, "DELETE");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) POST_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null) 
        {
            return POST_DATA(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public (bool Success, HttpStatusCode StatusCode, string Content) POST_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {            
            return UPLOAD(url, string.Join("&", payload), "DATA", querystring, headers, "POST");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) POST_DATA<TResult>(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null) {
            return UPLOAD<string, TResult>(url, string.Join("&", payload), "DATA", querystring, headers, "POST");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) PUT_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return PUT_DATA(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public (bool Success, HttpStatusCode StatusCode, string Content) PUT_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return UPLOAD(url, string.Join("&", payload), "DATA", querystring, headers, "PUT");
        }

        public (bool Success, HttpStatusCode StatusCode, string Content) DELETE_DATA(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return DELETE_DATA(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public (bool Success, HttpStatusCode StatusCode, string Content) DELETE_DATA(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return UPLOAD(url, string.Join("&", payload), "DATA", querystring, headers, "DELETE");
        }

        protected (bool Success, HttpStatusCode StatusCode, string Content) UPLOAD<T>(string url, T payload, string payloadMode = "JSON" /*JSON|DATA*/, string querystring = null, Dictionary<string, string> headers = null, string method = "POST")
        {            
            try
            {
                client.Headers.Clear();
                if (headers != null)
                    foreach (var header in headers)
                        client.Headers.Add(header.Key, header.Value);
                    
                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                string response = null;

                // as form-variables (aka post-data)
                if (payloadMode == "DATA") {
                    response = Encoding.UTF8.GetString(client.UploadData(url, method, Encoding.UTF8.GetBytes(payload.ToString())));
                }
                else // as json payload                
                {
                    var payloadType = payload.GetType();
                    string sPayload;
                    if (payloadType.IsPrimitive || payloadType == typeof(System.String))
                        sPayload = payload.ToString();
                    else
                        sPayload = JsonConvert.SerializeObject(payload);

                    response = client.UploadString(url, method, sPayload);
                }

                return (true, OK, response);
            }
            catch (WebException ex)
            {
                var webRespEX = ((HttpWebResponse)ex.Response);
                var statusCode = webRespEX?.StatusCode ?? ERROR;

                var stream = ex?.Response?.GetResponseStream();
                if (stream == null) return (false, statusCode, $"{ex.Message}");

                using (var reader = new StreamReader(stream))
                    return (false, statusCode, $"{ex.Message} | {reader.ReadToEnd()}");                
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message);
            }
        }

        protected (bool Success, HttpStatusCode StatusCode, string Content, TResult Model) UPLOAD<TPayload, TResult>(string url, TPayload payload, string payloadMode = "JSON" /*JSON|DATA*/, string querystring = null, Dictionary<string, string> headers = null, string method = "POST") {
            try
            {
                var response = this.UPLOAD<TPayload>(url, payload, payloadMode, querystring, headers, method);

                var model = default(TResult);
                if (response.Success)
                    model = JsonConvert.DeserializeObject<TResult>(response.Content);
                else if (response.Content.Contains("{"))
                {
                    try
                    {
                        var json = response.Content.Substring(response.Content.IndexOf("{"));
                        model = JsonConvert.DeserializeObject<TResult>(json);
                    }
                    catch { }
                }

                return (response.Success, response.StatusCode, response.Content, model);
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message, default(TResult));
            }
        }

        // --------------------------------

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> GET_ASYNC(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                client.Headers.Clear();
                if (headers != null)
                    foreach (var header in headers)
                        client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                return (true, OK, await client.DownloadStringTaskAsync(url));
            }
            catch (WebException ex)
            {
                var webRespEX = ((HttpWebResponse)ex.Response);
                var statusCode = webRespEX?.StatusCode ?? ERROR;

                var stream = ex?.Response?.GetResponseStream();
                if (stream == null) return (false, statusCode, $"{ex.Message}");

                using (var reader = new StreamReader(stream))
                    return (false, statusCode, $"{ex.Message} | {reader.ReadToEnd()}");
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message);
            }
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content, T Model)> GET_ASYNC<T>(string url, string querystring = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var response = await this.GET_ASYNC(url, querystring, headers);
                return (response.Success, response.StatusCode, response.Content, response.Success ? JsonConvert.DeserializeObject<T>(response.Content) : default(T));
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message, default(T));
            }
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, payload, "JSON", querystring, headers, "POST");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> POST_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC<TPayload, TResult>(url, payload, "JSON", querystring, headers, "POST");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, payload, "JSON", querystring, headers, "PUT");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> PUT_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC<TPayload, TResult>(url, payload, "JSON", querystring, headers, "PUT");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_ASYNC<T>(string url, T payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, payload, "JSON", querystring, headers, "DELETE");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> DELETE_ASYNC<TPayload, TResult>(string url, TPayload payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC<TPayload, TResult>(url, payload, "JSON", querystring, headers, "DELETE");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await POST_DATA_ASYNC(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> POST_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, string.Join("&", payload), "DATA", querystring, headers, "POST");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> POST_DATA_ASYNC<TResult>(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC<string, TResult>(url, string.Join("&", payload), "DATA", querystring, headers, "POST");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await PUT_DATA_ASYNC(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> PUT_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, string.Join("&", payload), "DATA", querystring, headers, "PUT");
        }

        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_DATA_ASYNC(string url, Dictionary<string, string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await DELETE_DATA_ASYNC(url, payload.Select(p => $"{p.Key}={payload[p.Key]}"), querystring, headers);
        }
        public async Task<(bool Success, HttpStatusCode StatusCode, string Content)> DELETE_DATA_ASYNC(string url, IEnumerable<string> payload, string querystring = null, Dictionary<string, string> headers = null)
        {
            return await UPLOAD_ASYNC(url, string.Join("&", payload), "DATA", querystring, headers, "DELETE");
        }

        protected async Task<(bool Success, HttpStatusCode StatusCode, string Content)> UPLOAD_ASYNC<T>(string url, T payload, string payloadMode = "JSON" /*JSON|DATA*/, string querystring = null, Dictionary<string, string> headers = null, string method = "POST")
        {
            try
            {
                client.Headers.Clear();
                if (headers != null)
                    foreach (var header in headers)
                        client.Headers.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(querystring))
                    url = string.Concat(url, "?", querystring);

                string response = null;

                // as form-variables (aka post-data)
                if (payloadMode == "DATA")
                {
                    response = Encoding.UTF8.GetString(await client.UploadDataTaskAsync(url, method, Encoding.UTF8.GetBytes(payload.ToString())));
                }
                else // as json payload                
                {
                    var payloadType = payload.GetType();
                    string sPayload;
                    if (payloadType.IsPrimitive || payloadType == typeof(System.String))
                        sPayload = payload.ToString();
                    else
                        sPayload = JsonConvert.SerializeObject(payload);

                    response = await client.UploadStringTaskAsync(url, method, sPayload);                    
                }

                return (true, OK, response);
            }
            catch (WebException ex)
            {
                var webRespEX = ((HttpWebResponse)ex.Response);
                var statusCode = webRespEX?.StatusCode ?? ERROR;

                var stream = ex?.Response?.GetResponseStream();
                if (stream == null) return (false, statusCode, $"{ex.Message}");

                using (var reader = new StreamReader(stream))
                    return (false, statusCode, $"{ex.Message} | {reader.ReadToEnd()}");
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message);
            }
        }

        protected async Task<(bool Success, HttpStatusCode StatusCode, string Content, TResult Model)> UPLOAD_ASYNC<TPayload, TResult>(string url, TPayload payload, string payloadMode = "JSON" /*JSON|DATA*/, string querystring = null, Dictionary<string, string> headers = null, string method = "POST")
        {
            try
            {
                var response = await this.UPLOAD_ASYNC<TPayload>(url, payload, payloadMode, querystring, headers, method);

                var model = default(TResult);
                if (response.Success)
                    model = JsonConvert.DeserializeObject<TResult>(response.Content);
                else if (response.Content.Contains("{"))
                {
                    try
                    {
                        var json = response.Content.Substring(response.Content.IndexOf("{"));
                        model = JsonConvert.DeserializeObject<TResult>(json);
                    }
                    catch { }
                }

                return (response.Success, response.StatusCode, response.Content, model);
            }
            catch (Exception ex)
            {
                return (false, ERROR, ex.Message, default(TResult));
            }
        }

        public string Base64Encode(string Value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(Value);
            return Convert.ToBase64String(valueBytes);
        }

        public string Base64Decode(string Base64Value)
        {
            var base64ValueBytes = Convert.FromBase64String(Base64Value);
            return Encoding.UTF8.GetString(base64ValueBytes);
        }

        public string GenerateBasicAuthorizationValue(string UserName, string Password)
        {
            return $"Basic {this.Base64Encode($"{UserName}:{Password}")}";
        }

        // e.g: IsHttpFileExists("https://mml-videos-music.s3.eu-west-2.amazonaws.com/prefixMM_16LXmdeT15U=.wav")
        public bool IsHttpFileExists(string FilePath)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(FilePath);
                using (var response = (HttpWebResponse)request.GetResponse())
                    return response.StatusCode == HttpStatusCode.OK;
            }
            catch { return false; }
        }
    }
}
