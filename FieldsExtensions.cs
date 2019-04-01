using Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class FieldsExtensions
    {
        public static void LoadFields(this IEnumerable<EntityField> me, IEnumerable<EntityFieldMetaData> fieldsMetaData, ParserFactory parserFactory)
        {
            me.ToList().LoadFields(fieldsMetaData, parserFactory);
        }

        public static void LoadFields(this List<EntityField> me, IEnumerable<EntityFieldMetaData> fieldsMetaData, ParserFactory parserFactory) {
            // [Field + Parsers]
            foreach (var fieldMD in fieldsMetaData){
                var field = (EntityField)fieldMD;
                field.Parsers.LoadParsers(fieldMD.ParsersMetaData, parserFactory);
                me.Add(field);
            };
        }
    }
}
