using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class TemplateManager
    {
        protected string TemplatesFolder { get; set; }

        public TemplateManager() : this("Templates\\") { }
        public TemplateManager(string TemplatesFolder) {
            this.TemplatesFolder = $"{AppDomain.CurrentDomain.BaseDirectory}{TemplatesFolder}";
        }

        public string Load(string TemplateName)
        {
            try
            {                
                return this.Html2String(string.Concat(this.TemplatesFolder, TemplateName));
            }
            catch { return string.Empty; }
        }

        protected string Html2String(string filePath)
        {
            return Html2String(filePath, Encoding.UTF8);
        }
        protected string Html2String(string filePath, Encoding encoding)
        {
            if (filePath == string.Empty) return string.Empty;
            using (var sr = new StreamReader(filePath, encoding))
                return sr.ReadToEnd();
        }
    }
}
