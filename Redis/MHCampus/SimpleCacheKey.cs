using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonEntities.Cache
{
    public class SimpleCacheKey : ICacheKey
    {
        private string Value { set; get; }
        public SimpleCacheKey(string Value) {
            this.Value = Value;
        }

        public string CacheKey
        {
            get { return this.Value; }
        }
    }
}
