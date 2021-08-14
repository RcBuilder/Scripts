using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities
{
    public class SearchResult<T>
    {
        [JsonProperty(PropertyName = "rowCount")]
        public int RowCount {
            get {
                return this.Result?.Count() ?? 0;
            }
        }

        [JsonProperty(PropertyName = "result")]
        public IEnumerable<T> Result { get; protected set; }

        public SearchResult(IEnumerable<T> Result) {
            this.Result = Result;
        }
    }
}
