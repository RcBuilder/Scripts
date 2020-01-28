using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Web.Serivces.API.Models
{
    public class CrawlerVimeoVideoFilter : CrawlerRegexFilter
    {        
        public CrawlerVimeoVideoFilter(): base(@"vimeo\.com/(?:.*#|.*/)?([0-9]+)") {}
    }

    public class CrawlerYoutubeVideoFilter : CrawlerRegexFilter
    {
        public CrawlerYoutubeVideoFilter() : base(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)") { }
    }

    public class CrawlerWistiaVideoFilter : CrawlerRegexFilter
    {
        public CrawlerWistiaVideoFilter() : base(@"(.*)fast.(wistia.com|wistia.net)\/embed\/iframe\/(.*)") { }
    }

    public class CrawlerOtherVideoFilter : CrawlerRegexFilter
    {
        public CrawlerOtherVideoFilter() : base(@".*\.(mp4|webm|ogv)?$") { }
    }
}
