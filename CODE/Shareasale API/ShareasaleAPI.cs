using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole7
{
    /*
        sources:
        https://help.shareasale.com/hc/en-us/articles/5375832636695-API-Building-Blocks

        -

        using:
        var config = new ShareasaleAPIConfig {
            AffiliateId = "651616",
            ApiToken = "xxxxxxxx",
            ApiSecret = "xxxxxxxxxxxxx"
        };
        var result = new ShareasaleAPI(config).Invoke("keywords");
        Console.WriteLine(result);
        Console.ReadKey();

        -

        report-response:
        KEYWORDID|KEYWORD|MERCHANTID|NOTES|BID_LIMIT|ALLOWED|DATE_UPDATED
        105628|All-In-One kit|128383|||No|2023-02-21 01:50:30.133
        105627|lawnmower parts|128383|||No|2023-02-21 01:44:14.45
        105626|chainsaw parts|128383|||No|2023-02-21 01:44:07.607
        105623|small engine repair|128383|||No|2023-02-21 01:41:39.877
        105620|chainsaw repair|128383|||No|2023-02-21 01:40:01.52
        ....
        ....

        -

        // Ads Script (js-based)
        var CONFIG = {
          affiliateid: 651616,
          version: 2.4,
          token: 'xxxxxxxx',
          secret: 'xxxxxxxxxxxxx',
          action: 'keywords'
        };

        var HTTP_OPTIONS = {
          'method' : 'GET',
          'headers': {
            'x-ShareASale-Date': '',
            'x-ShareASale-Authentication': ''
          }
        };

        function main() {        
          var url = 'https://api.shareasale.com/x.cfm?affiliateID={0}&token={1}&version={2}&action={3}'
            .replace('{0}', CONFIG.affiliateid)
            .replace('{1}', CONFIG.token)
            .replace('{2}', CONFIG.version)
            .replace('{3}', CONFIG.action);      
          Logger.log(url);
    
          var utcNow = new Date().toISOString();
          console.log(utcNow);
          HTTP_OPTIONS.headers['x-ShareASale-Date'] = utcNow;
      
          var hash = Sha256Hash(CONFIG.token + ':' + utcNow + ':' + CONFIG.action + ':' + CONFIG.secret);
          console.log(hash);
          HTTP_OPTIONS.headers['x-ShareASale-Authentication'] = hash;
  
          var report = UrlFetchApp.fetch(url, HTTP_OPTIONS).getContentText();   
          Logger.log(report);   
        }

        function Sha256Hash(value) {
          return BytesToHex(
            Utilities.computeDigest(
              Utilities.DigestAlgorithm.SHA_256, value));
        }

        function BytesToHex(bytes) {
          let hex = [];
          for (let i = 0; i < bytes.length; i++) {
            let b = parseInt(bytes[i]);
            if (b < 0) {
              c = (256+b).toString(16);
            } else {
              c = b.toString(16);
            }
            if (c.length == 1) {
              hex.push("0" + c);
            } else {
              hex.push(c);
            }
          }
          return hex.join("");
        }
    */
    public class ShareasaleAPI
    {
        public ShareasaleAPIConfig Config { get; protected set; }
        public ShareasaleAPI(ShareasaleAPIConfig Config) {
            this.Config = Config;
        }

        // actionVerb - name of api to invoke        
        public string Invoke(string actionVerb = "activity")
        {
            var ut = DateTime.Now.ToUniversalTime().ToString("r");

            var hasher = new SHA256Managed();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(this.Config.ApiToken + ':' + ut + ':' + actionVerb + ':' + this.Config.ApiSecret));

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            var authHeader = sb.ToString();

            var serviceURL = $"https://api.shareasale.com/x.cfm?affiliateID={this.Config.AffiliateId}&token={this.Config.ApiToken}&version={this.Config.ApiVersion}&action={actionVerb}";
            var request = WebRequest.Create(serviceURL);

            request.Method = "GET";
            request.Headers.Add("x-ShareASale-Date", ut);
            request.Headers.Add("x-ShareASale-Authentication", authHeader);

            var reader = new StreamReader(request.GetResponse().GetResponseStream());
            return reader.ReadToEnd();
        }
    }

    public class ShareasaleAPIConfig {
        public string AffiliateId { get; set; } 
        public string ApiToken { get; set; }
        public string ApiSecret { get; set; }
        public string ApiVersion { get; set; } = "2.4";
    }
}
