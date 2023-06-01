using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibitSyncService
{
    public class Processor
    {
        protected static readonly string CONNECTION_STRING = ConfigurationManager.AppSettings["ConnStr"].Trim();
        protected static readonly string SRC_FOLDER = ConfigurationManager.AppSettings["SrcFolder"].Trim();
        protected static readonly string DONE_FOLDER = $@"{SRC_FOLDER}\DONE\";

        protected PervasiveDAL DAL { get; set; }
        protected Logger Logger { get; set; }

        // DI - NibitConfig
        public Processor() {
            this.DAL = new PervasiveDAL(CONNECTION_STRING);
            this.Logger = new Logger($"{DONE_FOLDER}{DateTime.Now.ToString("yyyyMMdd")}.txt");
        }

        public async Task Run()
        {
            var files = this.GetFilesToLoad(SRC_FOLDER);

            foreach (var file in files)
            {
                Console.WriteLine(file);

                var errorsCount = 0;
                var rows = await this.ParseFile(file);
                foreach (var row in rows) {
                    try {
                        this.DAL.CreateAccountingTransactions(row);                        
                    }
                    catch (Exception ex) {
                        errorsCount++;
                        Console.WriteLine(ex.Message);
                        Logger.WriteError("CreateAccountingTransaction", $"{ex.Message}\r\n{JsonConvert.SerializeObject(row)}\r\n-----");
                    }
                }

                File.Move(file, $"{DONE_FOLDER}{Path.GetFileName(file)}");  // move to DONE
            }
        }

        protected virtual IEnumerable<string> GetFilesToLoad(string SrcFolder)
        {
            return Directory.GetFiles(SrcFolder, "*.dat");
        }

        protected async Task<IEnumerable<MoveinRow>> ParseFile(string SrcFile)
        {
            var csvConnector = new BLL.CsvConnector(SrcFile, "|", true);
            return await csvConnector.GetAsT<IEnumerable<MoveinRow>>();
        }
    }
}
