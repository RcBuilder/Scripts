using Crawler.Rbot;
using System;
using System.Collections.Generic;

namespace Crawler
{
    public class CrawledPageComparer : IEqualityComparer<CrawledPage>
    {
        public bool Equals(CrawledPage x, CrawledPage y)
        {
            return String.Equals(x.URI.AbsoluteUri, y.URI.AbsoluteUri, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(CrawledPage obj)
        {
            return obj.URI.AbsoluteUri.GetHashCode();
        }
    }
}
