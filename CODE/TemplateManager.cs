using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    /*
        USING
        -----
        var templateManager = new HTMLTemplateManager("D:\\Templates\\");
        var sTemplateA = templateManager.Load("TemplateA.html");
        var sTemplateB = await templateManager.LoadAsync("TemplateB.html");

        -

        TemplateA.html:
        <div>
            <p>TEMPLATE-A</p>            
        </div>

        TemplateB.html:
        <div>
            <p>TEMPLATE-B</p>
            <p>hello {0}</p>
        </div>

        -

        note!
        to auto-copy a template file to the BIN directory, go to the file properties (F4) > set "Copy To Output" to "Copy Always"
    */

    public interface ITemplateManager {
        string Load(string templateName);
    }

    public interface ITemplateManagerAsync {
        Task<string> LoadAsync(string templateName);
    }

    public abstract class TemplateManager : ITemplateManager, ITemplateManagerAsync
    {
        protected string TemplatesFolder { get; set; }
        protected Encoding Encoding { get; set; } = Encoding.UTF8;
        
        public TemplateManager(string templatesFolder) {
            if (!Directory.Exists(templatesFolder)) 
                throw new Exception($"Folder '{templatesFolder}' Does Not Exist!");

            this.TemplatesFolder = templatesFolder;
        }

        public string Load(string templateName)
        {
            try
            {
                var templatePath = $"{this.TemplatesFolder}{templateName}";                
                using (var sr = new StreamReader(templatePath, this.Encoding))
                    return sr.ReadToEnd();                
            }
            catch(Exception ex) {
                Debug.WriteLine($"[ERROR] TemplateManager.Load, ex: {ex.Message}");
                return string.Empty; 
            }
        }

        public async Task<string> LoadAsync(string templateName)
        {
            try
            {
                var templatePath = $"{this.TemplatesFolder}{templateName}";
                using (var sr = new StreamReader(templatePath, this.Encoding))
                    return await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] TemplateManager.Load, ex: {ex.Message}");
                return string.Empty;
                /// return await Task.FromResult(string.Empty);                
            }
        }
    }

    public class HTMLTemplateManager : TemplateManager {
        public HTMLTemplateManager() : this($"{AppDomain.CurrentDomain.BaseDirectory}Templates\\") { }
        public HTMLTemplateManager(string templatesFolder) : base(templatesFolder) { }
    }
}
