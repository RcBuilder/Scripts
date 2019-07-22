using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RobyTest.Controllers
{
    public class SearchesController : ApiController
    {
        [HttpGet]
        [Route("api/searches/searchTopX/{Phrase}")]
        public IEnumerable<Entities.SearchResult> SearchTopX(string Phrase)
        {
            return BLL.Searches.DoSEOSearch(Phrase);
        }
    }
}
