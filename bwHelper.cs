using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class bwHelper
    {
        /*
            &	AND
            |	OR
            ^	XOR
            <<	left shift
            >>	right shift
            ~	NOT
        */

        public class bwHelper
        {
            private static void Validate<TSource>()
            {
                if (!typeof(TSource).IsEnum)
                    throw new ArgumentException("NOT AN ENUM TYPE");
            }

            // e.g: var result = IsOn(eColor.Red, 37)
            public static bool IsOn<TSource>(TSource source, int value) where TSource : struct
            {
                try
                {
                    Validate<TSource>();
                    var sourceInt = (int)(object)source;
                    return (value & sourceInt) > 0;
                }
                catch
                {
                    return false;
                }
            }

            // e.g: var result = IsEquals(eColor.Red, 37)
            public static bool IsEquals<TSource>(TSource source, int value) where TSource : struct
            {
                try
                {
                    Validate<TSource>();
                    var sourceInt = (int)(object)source;
                    return (value & sourceInt) == sourceInt;
                }
                catch
                {
                    return false;
                }
            }

            // e.g: var result = On(eColor.Red, 37)
            public static TSource On<TSource>(TSource source, int value) where TSource : struct
            {
                try
                {
                    Validate<TSource>();
                    var sourceInt = Convert.ToInt32(source);
                    return (TSource)(object)(value | sourceInt);
                }
                catch
                {
                    return default(TSource);
                }
            }

            // e.g: var result = Off(eColor.Red, 37)
            public static TSource Off<TSource>(TSource source, int value) where TSource : struct
            {
                try
                {
                    Validate<TSource>();
                    var sourceInt = Convert.ToInt32(source);
                    return (TSource)(object)(value & ~sourceInt);
                }
                catch
                {
                    return default(TSource);
                }
            }

            // e.g: var result = bwHelper.GetNames<eColor>(37)
            public static List<string> GetNames<TSource>(int value) where TSource : struct
            {
                try
                {
                    Validate<TSource>();

                    var names = new List<string>();
                    foreach (TSource e in Enum.GetValues(typeof(TSource)))
                        if (IsOn<TSource>(e, value))
                            names.Add(e.ToString());
                    return names;
                }
                catch
                {
                    return null;
                }
            }

            // e.g: var result = Int2Bits(37)
            // also see 'BitArray Reverse.txt'
            public static string Int2Bits(int value)
            {
                try
                {
                    var sb = new StringBuilder();
                    var temp = value;
                    /* 
                        i = 31
                        int bits length =  32, sign = 1 (the left bit define the sign -/+)
                    
                        note! 
                        use i = 32 for uint!! 
                     */
                    for (int i = 31; i > 0; i--, temp >>= 1)
                        sb.Append((temp & 1) > 0 ? "1" : "0"); // check the right bit (1 = ...0000001)

                    var arrChars = sb.ToString().ToCharArray();
                    Array.Reverse(arrChars); // reverse
                    return new string(arrChars);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
