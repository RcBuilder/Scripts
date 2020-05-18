using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LagojaMarketingServices
{
    public class CsvMediaTypeFormatter : MediaTypeFormatter
    {
        private const string SEPERATOR = ",";

        public CsvMediaTypeFormatter() {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/tsv"));

            this.SupportedEncodings.Add(Encoding.UTF8);
        }

	// validate that the API Response type is IEnumerable<T>
        public override bool CanWriteType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));                        
        }

        public override bool CanReadType(Type type) {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken) {
            var writer = new StringWriter();

            return Task.Factory.StartNew(() => {
                var collection = (IEnumerable<object>)value;

                var itemType = collection.First().GetType();                
                bool isPrimitiveType = itemType.IsPrimitive || itemType.IsValueType || (itemType == typeof(System.String));

                if (isPrimitiveType) RenderPrimitiveCollection(writer, collection, itemType);
                else RenderCollection(writer, collection, itemType);
                
                using(var sw = new StreamWriter(writeStream))
                    sw.Write(writer.ToString());
            });
        }

        protected void RenderPrimitiveCollection(StringWriter writer, IEnumerable<object> collection, Type itemType) {
            foreach (var obj in collection) {                
                var objValue = (string)obj;

                if (objValue != null) {
                    if (objValue.Contains(",")) objValue = string.Format("\"{0}\"", objValue);
                    objValue = objValue.Replace("\n", string.Empty).Replace("\r", string.Empty);
                }

                writer.WriteLine(objValue ?? string.Empty);
            }
        }

        protected void RenderCollection(StringWriter writer, IEnumerable<object> collection, Type itemType) {
            var titles = itemType.GetProperties().Select(x => x.Name);
            writer.WriteLine("{0}", string.Join(SEPERATOR, titles));

            foreach (var obj in collection) {
                var line = new List<string>();
                var objValues = obj.GetType().GetProperties().Select(x => new { Value = x.GetValue(obj, null) });

                foreach (var objValue in objValues) {
                    if (objValue.Value == null) {
                        line.Add(","); // empty
                        continue;
                    }

                    var tempValue = objValue.Value.ToString();
                    if (tempValue.Contains(",")) tempValue = string.Format("\"{0}\"", tempValue);
                    tempValue = tempValue.Replace("\n", string.Empty).Replace("\r", string.Empty);
                    line.Add(tempValue);
                }
                writer.WriteLine("{0}", string.Join(SEPERATOR, line));
            }
        }
    }
}