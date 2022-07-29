using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{
    public class PervasiveDBHelper
    {
        public static string ClearQuery(string query) {
            try
            {
                return query?.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("  ", "");
            }
            catch {
                return query;
            }
        }

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
            
            var matches = Regex.Matches(value, "[0-9.,]{2,}|[a-zA-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

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
        public static string ChangeEncoding(string value, Encoding sourceEncoding, Encoding targetEncoding) {
            if (string.IsNullOrEmpty(value)) return string.Empty;

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
                try
                {
                    if (o == null) return 0;
                    if (string.IsNullOrEmpty(o.ToString()?.Trim())) return 0;
                    return Convert.ToInt32(o);
                }
                catch {
                    return 0;
                }
            }

            public static float ToSingle(object o, bool Round = true) {
                try
                {
                    if (o == null) return 0F;
                    if (string.IsNullOrEmpty(o.ToString()?.Trim())) return 0F;
                    var v = Convert.ToSingle(o);
                    if (Round) v = (float)Math.Round(v, 2);
                    return v;
                }
                catch { return 0F; }
            }

            public static bool ToBoolean(object o) {
                try
                {
                    if (o == null) return false;
                    if (string.IsNullOrEmpty(o.ToString()?.Trim())) return false;
                    return Convert.ToBoolean(o);
                }
                catch { return false; }
            }

            public static string ToString(object o)
            {
                if (o == null) return string.Empty;

                try
                {
                    return FixHebrewWithNumbers(ToStringWithEncoding(o, Encoding.GetEncoding("windows-1255"), Encoding.GetEncoding(862)));

                    //if (o == null) return string.Empty;
                    //return o.ToString()?.Trim();
                }
                catch { return string.Empty; }
            }

            public static char ToChar(object o)
            {
                try{
                    var str = FixHebrewWithNumbers(ToStringWithEncoding(o, Encoding.GetEncoding("windows-1255"), Encoding.GetEncoding(862)));
                    return str.Length == 0 ? '\0' : str[0];

                    //if (o == null) return string.Empty;
                    //return o.ToString()?.Trim();
                }
                catch { return '\0'; }
            }

            public static string ToStringWithEncoding(object o, Encoding sourceEncoding, Encoding targetEncoding)
            {
                if (o == null) return string.Empty;                
                return ChangeEncoding(o.ToString()?.Trim(), sourceEncoding, targetEncoding);
            }

            public static string ToPervasiveString(string value)
            {
                try
                {
                    value = value.Replace("'", "''");
                    return ToStringWithEncoding(FixHebrewWithNumbers(value), Encoding.GetEncoding(862), Encoding.GetEncoding("windows-1255"));
                }
                catch { return string.Empty; }
            }
        }

        public static string ReadAsJson(OdbcDataReader dr)
        {
            if (dr == null || !dr.HasRows) return "[]";

            var results = new List<Dictionary<string, string>>();
            while (dr.Read())
            {
                var result = new Dictionary<string, string>();
                for (var i = 0; i < dr.VisibleFieldCount; i++)
                    result.Add(dr.GetName(i), SafeConvert.ToString(dr[i]));
                results.Add(result);
            }

            return JsonConvert.SerializeObject(results);
        }

        public static T ReadAsT<T>(OdbcDataReader dr)
        {
            return JsonConvert.DeserializeObject<T>(ReadAsJson(dr));
        }

        public static bool IsDBStable(string Server, string Database) {
            try
            {
                var ConnetionString = $"Driver={{Pervasive ODBC Client Interface}};ServerName={Server};dbq={Database};Client_CSet=UTF-8;Server_CSet=CP850;";
                using (var connection = new OdbcConnection(ConnetionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
