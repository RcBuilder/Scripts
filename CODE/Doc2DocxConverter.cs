using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;  // Install-Package Microsoft.Office.Interop.Word -Version 15.0.4797.1003

namespace Doc2DocxConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Application wordApp = null;

            var stats = new Stats();
            try
            {
                var deleteOriginal = ConfigurationManager.AppSettings["DeleteOriginal"].Trim() == "1";

                var docsFolder = ConfigurationManager.AppSettings["RootFolder"].Trim(); ///@"C:\TEMP\";                
                Console.WriteLine($"folder: {docsFolder}");

                var directoryInfo = new DirectoryInfo(docsFolder);
                var docsFiles = directoryInfo.GetFiles("*.doc", SearchOption.AllDirectories);
                docsFiles = docsFiles.Where(x => x.FullName.EndsWith("doc", StringComparison.OrdinalIgnoreCase)).ToArray();  // remove .docx suffix (contains doc)

                Console.WriteLine($"{docsFiles.Length} docs found.");
                stats.Total = docsFiles.Length;

                wordApp = new Application() { Visible = false };
                foreach (var docFile in docsFiles)
                {
                    Document doc = null;                    

                    try
                    {
                        var docFilePath = docFile.FullName;
                        Console.WriteLine($"processing {docFilePath}...");

                        // open file 
                        doc = wordApp.Documents.Open($"{docFilePath}");

                        //Save the document
                        object docxFilePath = $"{docFilePath.ToLower().Replace(".doc", ".docx")}";
                        object fileFormat = WdSaveFormat.wdFormatDocumentDefault;  // docx
                        
                        doc.SaveAs2(ref docxFilePath, ref fileFormat);
                        doc?.Close(); // close doc

                        // delete original .doc file                        
                        if (deleteOriginal) File.Delete(docFilePath);

                        // preserve creation and updated time                        
                        File.SetCreationTime(docxFilePath.ToString(), docFile.CreationTime);
                        File.SetLastWriteTime(docxFilePath.ToString(), docFile.LastWriteTime);

                        stats.Successes++;
                    }
                    catch(Exception Ex) {
                        Console.WriteLine($"[ERROR] File: {docFile.Name}, Ex: {Ex.Message}");
                        stats.Failures++;
                    }                    
                }
            }
            catch (Exception Ex) {
                Console.WriteLine($"[ERROR] {Ex.Message}");
            }
            finally {
                // release resource
                wordApp?.Quit();

                // write stats
                File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}{DateTime.Now.ToString("yyyyMMdd")}.info", stats.ToString());
            }

            Console.ReadKey();
        }
    }

    class Stats {
        public int Total { get; set; }
        public int Successes { get; set; }
        public int Failures { get; set; }

        public override string ToString()
        {
            return $"Total: {this.Total} Rows ({this.Successes} Successes, {this.Failures} Failures)";
        }
    }
}
