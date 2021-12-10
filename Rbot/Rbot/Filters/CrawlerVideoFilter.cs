
namespace Crawler.Models
{
    public abstract class CrawlerVideoFilter : CrawlerRegexFilter
    {
        public CrawlerVideoFilter(string Expression) : base(Expression) { }
    }

    public class CrawlerVimeoVideoFilter : CrawlerVideoFilter
    {        
        public CrawlerVimeoVideoFilter(): base(@"vimeo\.com/(?:.*#|.*/)?([0-9]+)") {}
    }

    public class CrawlerYoutubeVideoFilter : CrawlerVideoFilter
    {
        public CrawlerYoutubeVideoFilter() : base(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)") { }
    }

    public class CrawlerWistiaVideoFilter : CrawlerVideoFilter
    {
        public CrawlerWistiaVideoFilter() : base(@"(.*)fast.(wistia.com|wistia.net)\/embed\/iframe\/(.*)") { }
    }

    public class CrawlerOtherVideoFilter : CrawlerVideoFilter
    {
        public CrawlerOtherVideoFilter() : base(@".*\.(mp4|webm|ogv)$") { }
    }
}
