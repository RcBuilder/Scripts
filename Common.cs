using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Common {
        public static T ConvertToEnum<T>(string value) where T : struct {
            try {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T is not an ENUM!");
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch {
                return default(T);
            }
        }

        public static Dictionary<string, string> Query2Dictionary(string query)
        {
            try
            {
                query = query.Replace("?", string.Empty);

                return query.Split('&')
                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                    .Distinct()
                    .ToDictionary(x => x.key, x => x.value);
            }
            catch
            {
                return null;
            }
        }

        // for PURE VALUES lists (A.P)
        public static string ToRegexORGroup(IEnumerable<string> list) {
            var temp = new List<string>();
            foreach (var item in list)
                temp.Add(ToRegexValue(item));
            var result = string.Join("|", temp);            
            return result;
        }

        // for REGEX type lists (A\.Ps? etc)
        public static string JoinRegexAsGroup(IEnumerable<string> list) {            
            return string.Join("|", list.Select(x=>x.Replace(" ", @"\s+")));            
        }

        public static string ToRegexValue(string value)
        {
            string[] toEscapeCharacters = { "\\", "[", "]", "(", ")", "^", "$", ".", "|", "?", "*", "+" };
           
            foreach (var c in toEscapeCharacters)
                value = value.Replace(c, "\\" + c);
            value = value.Replace(" ", @"\s+");
            return value;
        }
    }
}
