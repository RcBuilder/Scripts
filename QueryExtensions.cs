
using System;

namespace BLL
{
    public static class QueryExtensions
    {
        public static string AddAsQuery(this string me, string paramName, int paramVlaue) {
            return me.AddAsQuery(paramName, paramVlaue.ToString());
        }

        public static string AddAsQuery(this string me, string paramName, string paramVlaue) {
            return string.Concat(me, me.Contains("?") ? "&" : "?", paramName, "=", paramVlaue);
        }
    }
}
