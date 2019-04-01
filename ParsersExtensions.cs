using Entities;
using System.Collections.Generic;

namespace BLL
{
    public static class ParsersExtensions
    {
        public static void LoadParsers(this List<IParser> me, IEnumerable<ParserMetaData> parsersMetaData, ParserFactory parserFactory) {
            foreach (var parserMetaData in parsersMetaData)
            {
                var parser = parserFactory.Produce(parserMetaData);
                if (parser == null) continue;
                me.Add(parser);
            }
        }

        public static void Reset(this List<IParser> me)
        {
            me.ForEach(x => { x.Value = null; });
        }
    }
}
