using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Authorization
{
    /*
        USING
        -----
        return Request.CreateResponse(HttpStatusCode.OK, new {
            access_token = new JWTGenerator("my-secret-key").GenerateToken("1234567890", "John Doe")
        }); 

        --

        return Request.CreateResponse(HttpStatusCode.OK, new {
            access_token = new JWTGenerator("my-secret-key").GenerateToken(
                new {
                    id = "1234567890",
                    name = "John Doe",
                    timestamp = DateTime.Now.Ticks
                } 
            )
        }); 

        --

        var authorization = context.Request.Headers.Authorization;       
        var token = authorization.Parameter;        
        if (!new JWTGenerator("my-secret-key").VerifyToken(token))
            throw new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized Access");

        --

        var header = new JWTGenerator("my-secret-key").GetTokenHeader(token);
        var payload = new JWTGenerator("my-secret-key").GetTokenPayload(token);
    */

    public sealed class JWTGenerator
    {
        public sealed class Utilities {
            /*                
                [Query]
                ?token=xxxxxxxxxxxxxxxxxxxxxxxxxx
            */
            public static string GetTokenFromQuery(HttpRequestMessage Request)
            {
                var prmsMap = Request.GetQueryNameValuePairs();
                if (prmsMap == null) return null; // no parameters
                return prmsMap.FirstOrDefault(p => p.Key == "token").Value;
            }

            /*
                [Header]
                Authorization: Bearer xxxxxxxxxxxxxxxxxxxxxxxxxx
            */
            public static string GetTokenFromHeader(HttpRequestMessage Request)
            {
                var authorization = Request.Headers.Authorization;
                if (authorization == null)
                    throw new Exception("No Authorization Header");

                if (authorization.Scheme != "Bearer")
                    throw new Exception("Not a Bearer Authorization");

                return authorization.Parameter;
            }
        }


        private Encoding Encoding { get; set; } = Encoding.ASCII;
        private string SecretKey { get; set; }        

        public JWTGenerator(string SecretKey) {
            this.SecretKey = SecretKey;
        }

        public string GenerateToken(string UserId, string UserName) {
            return this.GenerateToken(new
            {
                id = UserId,
                name = UserName,
                timestamp = DateTime.Now.Ticks
            });
        }

        public string GenerateToken<T>(T PayloadData) {
            var header = JsonConvert.SerializeObject(new
            {
                alg = "HS256",  // algorithm 
                typ = "JWT"     // type
            });

            var payload = JsonConvert.SerializeObject(PayloadData);

            // Base64UrlEncoder -> Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').Replace("=", "")
            var message = $"{Base64UrlEncoder.Encode(header)}.{Base64UrlEncoder.Encode(payload)}";

            // SHA-256
            var hmac = new HMACSHA256(this.Encoding.GetBytes(this.SecretKey));
            var signature = Base64UrlEncoder.Encode(hmac.ComputeHash(this.Encoding.GetBytes(message))).Replace("+", "-").TrimEnd('=');

            // <header-toBase64>.<payload-toBase64>.<signature-toBase64>
            var token = $"{message}.{signature}";
            return token;
        }

        public bool VerifyToken(string Token) {
            var tokenData = new
            {
                headerEncoded = Token.Split('.')[0],
                payloadEncoded = Token.Split('.')[1],
                signature = Token.Split('.')[2],
            };

            var message = $"{tokenData.headerEncoded}.{tokenData.payloadEncoded}";

            var hmac = new HMACSHA256(this.Encoding.GetBytes(this.SecretKey));
            var signature = Base64UrlEncoder.Encode(hmac.ComputeHash(this.Encoding.GetBytes(message))).Replace("+", "-").TrimEnd('=');

            return signature == tokenData.signature;
        }

        public string GetTokenHeader(string Token) {
            return this.GetTokenData(Token).Item1;
        }

        public T GetTokenHeader<T>(string Token) {
            return JsonConvert.DeserializeObject<T>(this.GetTokenData(Token).Item1);
        }

        public string GetTokenPayload(string Token) {
            return this.GetTokenData(Token).Item2;
        }

        public T GetTokenPayload<T>(string Token) {
            return JsonConvert.DeserializeObject<T>(this.GetTokenData(Token).Item2);
        }

        
        // private (string sHeader, string sPayload) GetTokenData(string Token) 
        // return (sHeader, sPayload);
        private Tuple<string, string> GetTokenData(string Token) {
            var tokenData = new {
                headerEncoded = Token.Split('.')[0],
                payloadEncoded = Token.Split('.')[1],
                signature = Token.Split('.')[2],
            };

            var sHeader = Base64UrlEncoder.Decode(tokenData.headerEncoded);
            var sPayload = Base64UrlEncoder.Decode(tokenData.payloadEncoded);
            return Tuple.Create(sHeader, sPayload);
        }
    }
}