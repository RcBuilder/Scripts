using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static string ToHTML(this string me) {
        return me.Replace("\n", "<br />"); 
    }

    public static bool Between(this float me, float min, float max) {
        return me >= min && me <= max;
    }

    // TODO DELETE AFTER UPDATING THE EDMX MODEL
    public static bool Between(this double me, float min, float max){
        return (float)me >= min && (float)me <= max;
    }
}
