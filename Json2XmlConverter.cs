using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /*        
        JToken
            - JObject
            - JArray
            - JProperty
            - JValue 
    */

    /*
        using:
        var xmlStr = Json2XmlConverter.ConvertArrayAsHTML(strJson);    
        var xml = XDocument.Parse(xmlStr); 
        
        ---

        samples:
        var htmlStr = Json2XmlConverter.ConvertArrayAsHTML(
            @"[ 
                {id:1, name: 'a', p: [{ p1: 'a10', p2: 'a20' }]},
                {id:2, name: 'b', p: [{ p1: 'a10', p2: 'a20' }]} 
            ]");


        var xmlStr = Json2XmlConverter.ConvertArrayAsXML(
            @"[ 
                {id:1, name: 'a', p: [{ p1: 'a10', p2: 'a20' }]},
                {id:2, name: 'b', p: [{ p1: 'a10', p2: 'a20' }]} 
            ]", "Item");
    */
    public class Json2XmlConverter
    {
        /*
            parameters:
            strJson - the json content in string representation
            tagName - the tag name of each element

            sample:

            var strJson = "[
                { id: 1, name: 'itemA' }
                { id: 2, name: 'itemB' }
                { id: 3, name: 'itemC' }
            ]";

            ConvertArrayAsXML(strJson, "Item")

            <Items>
                <Item>
                    <id>1</id>
                    <name>itemA</name>
                </Item>
                <Item>
                    <id>2</id>
                    <name>itemB</name>
                </Item>
                <Item>
                    <id>3</id>
                    <name>itemC</name>
                </Item>
            </Items>
        */
        public static string ConvertArrayAsXML(string strJson, string tagName) {            
            var sb = new StringBuilder();
            var addTag = !string.IsNullOrEmpty(tagName);

            try {                
                if(addTag) sb.AppendFormat("<{0}s>", tagName);               
                var data = JsonConvert.DeserializeObject<JArray>(strJson);                
                foreach (JObject content in data.Children<JObject>())
                    sb.Append(ConvertAsXML(content.ToString(), tagName));                  
                if(addTag) sb.AppendFormat("</{0}s>", tagName);
            }
            catch { }

            return sb.ToString();
        }

        public static string ConvertAsXML(string strJson, string tagName)
        {
            var sb = new StringBuilder();
            var addTag = !string.IsNullOrEmpty(tagName);

            try
            {
                var content = JsonConvert.DeserializeObject<JObject>(strJson);

                if(addTag) sb.AppendFormat("<{0}>", tagName);
                foreach (JProperty prop in content.Properties()) {
                    /// Console.WriteLine(prop.Name);

                    sb.AppendFormat("<{0}>", prop.Name);
                    switch (prop.Value.Type)
                    {                        
                        case JTokenType.Array:
                            sb.Append(ConvertArrayAsXML(prop.Value.ToString(), null));
                            break;
                        
                        case JTokenType.Integer:
                        case JTokenType.Boolean:
                        case JTokenType.Float:
                            sb.AppendFormat("{0}", prop.Value);
                            break;
                        default:
                            sb.AppendFormat("<![CDATA[{0}]]>", prop.Value);
                            break;
                    }
                    sb.AppendFormat("</{0}>", prop.Name);
                }
                if(addTag) sb.AppendFormat("</{0}>", tagName);
            }
            catch { }

            return sb.ToString();
        }

        // -----------------------------------------------------------

        /*
            parameters:
            strJson - the json content in string representation            

            sample:

            var strJson = "[
                { id: 1, name: 'itemA' }
                { id: 2, name: 'itemB' }
                { id: 3, name: 'itemC' }
            ]";

            ConvertArrayAsHTML(strJson)
            
            <div>
                <p class="id">1</p>
                <p class="name">itemA</p>
            </div>
            <div>
                <p class="id">2</p>
                <p class="name">itemB</p>
            </div>
            <div>
                <p class="id">3</p>
                <p class="name">itemC</p>
            </div>            
        */
        public static string ConvertArrayAsHTML(string strJson)
        {
            var sb = new StringBuilder();

            try
            {                
                var data = JsonConvert.DeserializeObject<JArray>(strJson);
                foreach (JObject content in data.Children<JObject>())                
                    sb.Append(ConvertAsHTML(content.ToString()));                                
            }
            catch { }

            return sb.ToString();
        }

        public static string ConvertAsHTML(string strJson)
        {
            var sb = new StringBuilder();
            var decoder = new Html2TextParser();

            try
            {
                var content = JsonConvert.DeserializeObject<JObject>(strJson);                

                sb.Append("<div>");
                foreach (JProperty prop in content.Properties()) {
                    /// Console.WriteLine(prop.Name);

                    sb.AppendFormat("<p class='{0}'>", prop.Name);
                    switch (prop.Value.Type) {
                        case JTokenType.Array:
                            sb.Append(ConvertArrayAsHTML(prop.Value.ToString()));
                            break;
                        case JTokenType.Integer:
                        case JTokenType.Boolean:
                        case JTokenType.Float:                        
                            sb.AppendFormat("{0}", prop.Value);
                            break;
                        default:
                            decoder.Parse(prop.Value.ToString());
                            sb.AppendFormat("{0}", decoder.Value);
                            ///sb.AppendFormat("<![CDATA[{0}]]>", prop.Value);
                            break;
                    }                    
                    sb.AppendFormat("</p>", prop.Name);
                }
                sb.Append("</div>");                
            }
            catch { }

            return sb.ToString();
        }
    }
}
