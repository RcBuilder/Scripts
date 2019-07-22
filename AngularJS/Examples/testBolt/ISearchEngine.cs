using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface ISearchEngine<T>
    {
        IEnumerable<T> SearchTopX(string Phrase, int rowcount);
    }

    public interface ISEOSearchEngine : ISearchEngine<SearchResult> {}
}
