using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CliClap.Crawler.Models
{
    public class CrawlerImageFilter : CrawlerRegexFilter
    {        
        public CrawlerImageFilter() : base(@".*\.(jpg|jpeg|jpe|jif|jfif|jfi|png|gif|webp|tiff|tif|psd|raw|arw|cr2|nrw|k25|bmp|dib|heif|heic|ind|indd|indt|jp2|j2k|jpf|jpx|jpm|mj2|svg|svgz)$") {}
    }
}
