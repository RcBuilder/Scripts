using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class Helper
    {
        #region DateFormat:
        public static string DateFormat(DateTime dt)
        {
            return DateFormat(dt, "dd/MM/yyyy");
        }

        public static string DateFormat(DateTime dt, string format)
        {
            return dt.ToString(format);
        }
        #endregion

        #region InnerText:
        public static string InnerText(string val)
        {
            return Regex.Replace(val, "<.*?>", string.Empty);
        }
        #endregion

        #region SubString:
        public static string SubString(string val, int Length)
        {
            val = InnerText(val); // strip all HTML elements from the string - innerText

            if (val.Length <= Length)
                return val.Trim();
            return val.Trim().Substring(0, Length - 2) + "..";
        }
        #endregion

        #region Marker:
        public static string Marker(string HTML, string word, string color)
        {
            return Regex.Replace(HTML, word, string.Format("<span style=\"color:{0}\">{1}</span>", color, word), RegexOptions.IgnoreCase);
        }
        #endregion

        #region ToFix:
        public static string ToFix(double price)
        {
            return string.Format("{0:N} ₪", price);
        }

        public static string ToFixGrade(double grade)
        {
            return string.Format("{0:N}", grade);
        }
        #endregion

        #region GenerateHushCode:
        public static string GenerateHushCode()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public static string GenerateHushCode(int Length)
        {
            return GenerateHushCode().Substring(0, Length);
        }
        #endregion

        #region Time2Seconds:
        /// <summary>
        /// convert time to seconds
        /// </summary>
        /// <param name="time">FORMAT HH:mm:ss</param>
        /// <returns>seconds</returns>
        public static int Time2Seconds(string time) // FORMAT HH:mm:ss
        {
            var pattern = @"^(?<H>\d{2})\:(?<M>\d{2})\:(?<S>\d{2})$"; // e.g: 00:02:05
            var match = Regex.Match(time, pattern);
            if (match == null) return 0;
            return Convert.ToByte(match.Groups["S"].Value) + (Convert.ToByte(match.Groups["M"].Value) * 60 /*MINUTE*/) + (Convert.ToByte(match.Groups["H"].Value) * 3600 /*HOUR*/);
        }
        #endregion

        #region Seconds2Time:
        /// <summary>
        /// convert seconds to time
        /// </summary>
        /// <param name="seconds">seconds</param>
        /// <returns>FORMAT HH:mm:ss</returns>
        public static string Seconds2Time(int seconds)
        {            
            var ts = TimeSpan.FromSeconds(seconds);
            //return ts.ToString();             
            return string.Format("{0:00}:{1:00}:{2:00}", (int)(ts.Days * 24 + ts.Hours), (int)ts.Minutes, (int)ts.Seconds);
        }
        #endregion
    }
}
