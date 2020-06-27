using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHCommon
{
    public static class Serializer
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat // e.g: "\/Date(1356044400000+0100)\/"
        };

        public static string SerializeObject(object item){
            return JsonConvert.SerializeObject(item, settings);
        }

        public static T DeserializeObject<T>(string item){
            return JsonConvert.DeserializeObject<T>(item, settings);
        }

        public static object Parse(string item){
            return JsonConvert.DeserializeObject(item, settings);
        }
    }
}