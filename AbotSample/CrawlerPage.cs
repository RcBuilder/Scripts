using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CliClap.Web.Serivces.API.Models
{
    public class CrawlerPage
    {
        public string URL { get; set; }
        public int Depth { get; set; }
        public List<string> Links { get; set; }
        public int LinksCount {
            get { return this.Links == null ? 0 : this.Links.Count; }
        }
    }
}