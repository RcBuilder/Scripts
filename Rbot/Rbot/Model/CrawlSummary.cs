using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Crawler.Rbot
{
    public class CrawlSummary
    {
        protected ConcurrentBag<CrawledPage> _Pages { get; set; } = new ConcurrentBag<CrawledPage>();

        public HashSet<CrawledPage> Pages
        {
            get
            {
                return this._Pages.ToList().AsReadOnly().ToHashSet();
            }
        }

        public int PagesCount
        {
            get
            {
                return this.Pages?.ToList().Count ?? 0;
            }
        }

        public void AddPages(CrawledPage Page)
        {
            if (this.PageExists(Page)) return;
            this._Pages.Add(Page);
        }

        public bool PageExists(CrawledPage Page)
        {
            return this.Pages.Contains(Page, new CrawledPageComparer());
        }

        public HashSet<string> Links
        {
            get
            {
                return this.Pages.Select(x => x.URI.AbsoluteUri).ToHashSet();
            }
        }

        public int LinksCount
        {
            get
            {
                return this.Links?.ToList().Count ?? 0;
            }
        }

    }
}
