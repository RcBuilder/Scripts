using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHCommon
{
    public class Helper
    {
        public static T ConvertToEnum<T>(string value) where T : struct
        {
            try
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T is not an ENUM!");
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch{
                return default(T);
            }
        }

        public static Dictionary<string, string> Query2Dictionary(string query)
        {
            try
            {
                return query.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch {
                return null;
            }
        }
    }
}
