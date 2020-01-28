using CliClap.Services.WebScrapingService;
using CliClap.Web.Serivces.API.Models.WebScrapingService;
using CliClap.Web.Serivces.API.Common;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Collections.Generic;
using CliClap.Web.Serivces.API.Models;
using CliClap.Web.Serivces.API.CrawlerServices;

namespace CliClap.Web.Serivces.API.Controllers
{    
    [Authorize]
    [RoutePrefix("crawler")]
    public class CrawlerController : ApiController
    {
        // http://localhost:47084/crawler/links?URL=http://example.com
        [HttpGet]
        [Route("links")]
        public async Task<List<CrawlerPage>> CollectLinks(string URL)
        {
            var bot = new AbotProvider();
            return await bot.CollectLinks(URL);
        }
    }
}
