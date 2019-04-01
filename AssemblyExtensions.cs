
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BLL
{
    public static class AssemblyExtensions
    {
        // for a given interface, find all the classes implement it!  
        /*
            var types = typeof(I1).FindInterfaceImplementers();
            foreach (var type in types)
                Console.WriteLine(type.Name);
        */
        public static IEnumerable<Type> FindInterfaceImplementers(this Type me)
        {
            if (!me.IsInterface)
                return null;

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            return types.Where(x => !x.IsInterface && me.IsAssignableFrom(x));
        }


        // for a given attribute, find all the classes uses it!  
        /*
            var types = typeof(A1).FindAttributeConsumers();
            foreach (var type in types)
                Console.WriteLine(type.Name);
        */
        public static IEnumerable<Type> FindAttributeConsumers(this Type me)
        {
            if (!typeof(Attribute).IsAssignableFrom(me))
                return null;

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            return types.Where(t => t.CustomAttributes.Any(a => a.AttributeType == me));
        }

        // for a given class, get it's properties!
        /*
            var c1 = new Class1();
            var result = c1.GetProperties();
        */
        public static Dictionary<string, object> GetProperties<T>(this T me) where T : class
        {
            try
            {
                var properties = new Dictionary<string, object>();
                var type = typeof(T);
                foreach (var p in type.GetProperties())
                    properties.Add(p.Name, p.GetValue(me));
                return properties;
            }
            catch
            {
                return null;
            }
        }

        public static void EmptyNullStrings<T>(this T me) where T : class
        {
            if (me == null) return;

            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.PropertyType == typeof(String));
            foreach (var p in properties) {
                if (p.GetValue(me) != null) continue;
                p.SetValue(me, "");                
            }
        }
    }
}
