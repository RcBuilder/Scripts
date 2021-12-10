using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Crawler.Rbot
{
    public class CrawlContext
    {        
        public int CrawledCount {
            get {
                return this._Links.Count;
            }            
        }

        // HashSet<string>
        protected ConcurrentBag<string> _Links { get; set; } = new ConcurrentBag<string>();

        public IEnumerable<string> Links
        {
            get
            {
                return this._Links.Distinct().ToList().AsReadOnly();
            }
        }

        public void AddLink(string Link) {
            this._Links.Add(Link);
        }

        protected ConcurrentBag<Exception> _Exceptions { get; set; } = new ConcurrentBag<Exception>();

        public IEnumerable<Exception> Exceptions
        {
            get {
                return this._Exceptions.Distinct().ToList().AsReadOnly();
            }
        }

        public void AddException(Exception Exception) {
            this._Exceptions.Add(Exception);
        }        
    }
}
