using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    public class PervasiveDBHelper
    {
        // the number of days since 01-01-0001 plus 381
        public static double ToPervasiveDate(DateTime? date)
        {
            if (!date.HasValue) return 0;
            TimeSpan ts = date.Value - DateTime.MinValue; // 01-01-0001
            return ts.TotalDays + 381;
        }

        public static DateTime? FromPervasiveDate(double datePervasive)
        {
            try
            {
                if (datePervasive <= 0) return null;
                var date = DateTime.MinValue; // 01-01-0001
                date = date.AddDays(datePervasive - 381);
                return date;
            }
            catch { return null; }
        }

        public static string FixHebrewWithNumbers(string value)
        {
            // hebrew with numeric are saved in pervasive as mirror numbers 
            // so "דן 14" becomes "דן 41"

            if (string.IsNullOrEmpty(value)) return value;

            var hasHebrew = Regex.IsMatch(value, "[א-ת]+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (!hasHebrew) return value;
            
            var matches = Regex.Matches(value, "\\d{2,}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            // collect numeric matches
            var hashSet = new HashSet<string>();
            foreach (var match in matches)
                hashSet.Add(match.ToString());

            // reverese numeric matches - mirror
            foreach (var num in hashSet)
                value = value.Replace(num, new string(num.Reverse().ToArray()));
            return value;
        }

        // NOTE! connStr should use the following encoding:
        // Client_CSet=UTF-8;Server_CSet=CP850;
        public static string FixEncoding(string value) {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var sourceEncoding = Encoding.GetEncoding("windows-1255");
            var targetEncoding = Encoding.GetEncoding(862);

            byte[] valueBytes = sourceEncoding.GetBytes(value);
            value = targetEncoding.GetString(valueBytes);
            return value;
        }

        public static int GetColumnMaxValue(OdbcCommand command, string tableName, string columnName) {
            string CreateColumnMaxValueQuery() {
                return $@"
                    select max({columnName}) from {tableName}
                ";
            }

            command.CommandText = CreateColumnMaxValueQuery();
            var oRes = command.ExecuteScalar();
            return oRes is DBNull ? 1 : ((int)oRes + 1);
        }

        public class SafeConvert {
            public static int ToInt32(object o) {
                if (o == null) return 0;
                if (string.IsNullOrEmpty(o.ToString()?.Trim())) return 0;
                return Convert.ToInt32(o);
            }

            public static float ToSingle(object o) {
                if (o == null) return 0;
                if (string.IsNullOrEmpty(o.ToString()?.Trim())) return 0F;
                return Convert.ToSingle(o);
            }

            public static string ToString(object o)
            {
                if (o == null) return string.Empty;                
                return FixEncoding(o.ToString()?.Trim());
            }
        }
    }
}
