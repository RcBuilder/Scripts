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
        https://www.youtube.com/watch?v=jeZV6vZj1IY
        https://help.shareasale.com/hc/en-us/articles/5375832636695-API-Building-Blocks
        https://help.shareasale.com/hc/en-us/articles/12442753460375-Merchant-API-Building-Blocks
        https://www.slideshare.net/ericnagel/best-ways-to-use-the-share-a-sale-api
        https://blog.shareasale.com/2013/07/16/merchant-api-building-blocks/

        -

        actions:
        1. keywords
        2. merchantStatus
        3. activity
        4. merchantApplication
        5. merchantTermsAndBio

        -

        using:

        var config = new ShareasaleAPIConfig {
            AffiliateId = "651616",
            ApiToken = "xxxxxxxxxxxxx",
            ApiSecret = "xxxxxxxxxxxxxxxxx"
        };
            
        var statusResult = new ShareasaleAPI(config).Invoke("merchantStatus");
        Console.WriteLine(statusResult);

        var keywordsResult = new ShareasaleAPI(config).Invoke("keywords");
        Console.WriteLine(keywordsResult);

        var tokenResult = new ShareasaleAPI(config).GenerateToken("merchantApplication");
        Console.WriteLine(tokenResult);

        Console.ReadKey();

        -

        (keywords) report-response:
        KEYWORDID|KEYWORD|MERCHANTID|NOTES|BID_LIMIT|ALLOWED|DATE_UPDATED
        105628|All-In-One kit|128383|||No|2023-02-21 01:50:30.133
        105627|lawnmower parts|128383|||No|2023-02-21 01:44:14.45
        105626|chainsaw parts|128383|||No|2023-02-21 01:44:07.607
        105623|small engine repair|128383|||No|2023-02-21 01:41:39.877
        105620|chainsaw repair|128383|||No|2023-02-21 01:40:01.52
        ....
        ....

        -

        (merchantStatus) report-response:
        Merchant Id|Merchant|WWW|Program Status|Program Category|Sale Comm|Lead Comm|Hit Comm|Approved|Link Url|Store Names|Store Ids|Store WWWs|Store Sale Comms|Store Link Urls|ruleCommissionDate|conversionLineDate|Ships To
        54060| Animal Trackers Club LLC|AnimalTrackersClub.com|TemporarilyOffline|Games/Toys|$6|||Declined|https://www.shareasale.com/r.cfm?b=620184&u=651616&m=54060||||||||        
        77719| EXPERIENCIAS XCARET WEB SAPI DE CV|hotelxcaret.com|Online|Travel|6%|||Declined|https://www.shareasale.com/r.cfm?b=1141199&u=651616&m=77719||||||2021-05-12 19:44:00||
        96959| Freethink Media, Inc.|bigthinkedge.com|Closed|Education|50%|||Yes|https://www.shareasale.com/r.cfm?b=1532337&u=651616&m=96959||||||||
        88344| Gpfilter(Glacialpurefilters)|gpfilter.com|Online|Home & Garden|10%|||Yes|https://www.shareasale.com/r.cfm?b=1346381&u=651616&m=88344||||||2019-04-14 00:05:21||US
        129530| Guangzhou sameway electric bicycle co.,ltd|www.samebike.com|TemporarilyOffline|Sports/Fitness|8%|||Yes|https://www.shareasale.com/r.cfm?b=2118257&u=651616&m=129530||||||2022-08-11 22:20:22||CA,EU,US                
        69944| Nectar Sleep|www.nectarsleep.com/|Closed|Home & Garden|$50|||Declined|https://www.shareasale.com/r.cfm?b=965472&u=651616&m=69944||||||2017-02-08 02:04:11|2017-11-08 15:45:48|
        124396| Samii Ryan|www.bysamiiryan.com|TemporarilyOffline|Clothing|10%|||Yes|https://www.shareasale.com/r.cfm?b=2007730&u=651616&m=124396||||||||
        124084|#Ai Computing Corporation|nocouchlock.com|TemporarilyOffline|Business|20%|||Yes|https://www.shareasale.com/r.cfm?b=2001830&u=651616&m=124084||||||||
        85928|&quot;GREYNUT&quot; Ltd|luxafor.com/|TemporarilyOffline|Business|20%|||Pending|https://www.shareasale.com/r.cfm?b=1315278&u=651616&m=85928||||||||
        83500|'47 Brand|www.47brand.com|Online|Clothing|4%|||Declined|https://www.shareasale.com/r.cfm?b=1258568&u=651616&m=83500||||||2018-10-24 10:43:01||
        94887|+ Lux Unfiltered, Inc.|www.luxunfiltered.com|Online|Health|10%|||Declined|https://www.shareasale.com/r.cfm?b=1492656&u=651616&m=94887||||||2023-12-21 16:19:28||
    */
    public class ShareasaleAPI
    {
        public ShareasaleAPIConfig Config { get; protected set; }
        public ShareasaleAPI(ShareasaleAPIConfig Config) {
            this.Config = Config;
        }

        // actionVerb - name of api to invoke        
        public string Invoke(string actionVerb) {
            return this.Invoke(actionVerb, DateTime.Now);
        }

        public string Invoke(string actionVerb, DateTime date)
        {            
            var resultToken = this.GenerateToken(actionVerb, date);
            var authHeader = resultToken.Token;
            var serviceURL = $"https://api.shareasale.com/x.cfm?affiliateID={this.Config.AffiliateId}&token={this.Config.ApiToken}&version={this.Config.ApiVersion}&action={actionVerb}";
            var request = WebRequest.Create(serviceURL);

            request.Method = "GET";
            request.Headers.Add("x-ShareASale-Date", resultToken.dateUTC);
            request.Headers.Add("x-ShareASale-Authentication", authHeader);

            var reader = new StreamReader(request.GetResponse().GetResponseStream());
            return reader.ReadToEnd();
        }

        public (string Token, string dateUTC) GenerateToken(string actionVerb){
            return this.GenerateToken(actionVerb, DateTime.Now);
        }
        public (string Token, string dateUTC) GenerateToken(string actionVerb, DateTime date) {
            return this.GenerateToken(actionVerb, date.ToUniversalTime().ToString("r"));
        }
        public (string Token, string dateUTC) GenerateToken(string actionVerb, string dateUTC) {
            var hasher = new SHA256Managed();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(this.Config.ApiToken + ':' + dateUTC + ':' + actionVerb + ':' + this.Config.ApiSecret));

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            var token = sb.ToString();
            return (token, dateUTC);
        }
    }

    public class ShareasaleAPIConfig {
        public string AffiliateId { get; set; } 
        public string ApiToken { get; set; }
        public string ApiSecret { get; set; }
        public string ApiVersion { get; set; } = "2.4";
    }
}
