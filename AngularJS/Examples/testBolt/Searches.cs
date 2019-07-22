using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Searches
    {
        public static IEnumerable<Entities.SearchResult> DoSEOSearch(string Phrase) {

            var googleTask = Task.Factory.StartNew(() =>
            {
                var google = new GoogleSearchEngine();
                return google.SearchTopX(Phrase, 5);
            });

            var bingTask = Task.Factory.StartNew(() =>
            {
                var bing = new BingSearchEngine();
                return bing.SearchTopX(Phrase, 5);
            });

            Task.WaitAll(googleTask, bingTask);
            var result = googleTask.Result.Concat(bingTask.Result);

            // TODO ->> Save To DB

            return result;
        }
    }
}
