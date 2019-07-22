using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SearchResult
    {        
        public int RowId { get; set; }
        public eSearchEngine SearchEngineType { get; set; }
        public string Title { get; set; }
        public DateTime EnteredDate { get; set; }        
    }
}
