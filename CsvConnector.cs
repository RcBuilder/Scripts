using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Data.Common;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace DanHotelsConnector
{
    /*
        USING
        -----        
        // get as json string
        var csvConnector = new CsvConnector(@"C:\test-stars_coupon.csv");
        var sJson = await csvConnector.GetAsJson();
            
        ---

        // get as data-table
        var csvConnector = new CsvConnector(@"C:\test-stars_coupon.csv");        
        var dt = await csvConnector.GetAsDataTable();

        ---

        // get as T object (generic) 
        var csvConnector = new CsvConnector(@"C:\test-stars_coupon.csv");
        var lst = await csvConnector.GetAsT<IEnumerable<SomeModel>>();
    */

    public interface ICsvConnector
    {
        string FilePath { get; set; }                
        Task<DataTable> GetAsDataTable();
        Task<string> GetAsJson();
        Task<T> GetAsT<T>();
    }

    public class CsvConnector : ICsvConnector
    {
        public string FilePath { get; set; }
        public string Delimiter { get; set; }
        public Encoding Encoding { get; set; } = Encoding.GetEncoding("Windows-1255");

        public CsvConnector(string FilePath, string Delimiter = ",") {
            this.FilePath = FilePath;
            this.Delimiter = Delimiter;
        }

        public async Task<DataTable> GetAsDataTable()
        {
            return await Task.Factory.StartNew(() => {
                var dt = new DataTable();
                using (var reader = new TextFieldParser(this.FilePath, this.Encoding))
                {
                    reader.TextFieldType = FieldType.Delimited;
                    reader.SetDelimiters(this.Delimiter);

                    var headerFields = reader.ReadFields();  // read 1st line - the header 
                    dt.Columns.AddRange(headerFields.Select(x => new DataColumn(x)).ToArray());

                    while (!reader.EndOfData) {                        
                        var rowFields = reader.ReadFields();
                        var row = dt.NewRow();
                        for (var i = 0; i < rowFields.Length; i++) // read row                                                 
                            row[i] = rowFields[i];
                        dt.Rows.Add(row);  // add row to collection                                                    
                    }
                }
                return dt;
            });
        }

        public async Task<string> GetAsJson()
        {
            return await Task.Factory.StartNew(() => {
                var results = new List<Dictionary<string, object>>();
                using (var reader = new TextFieldParser(this.FilePath, this.Encoding))
                {
                    reader.TextFieldType = FieldType.Delimited;
                    reader.SetDelimiters(this.Delimiter);

                    var headerFields = reader.ReadFields();  // read 1st line - the header 

                    while (!reader.EndOfData)
                    {
                        var result = new Dictionary<string, object>();
                        var rowFields = reader.ReadFields();
                        for (var i = 0; i < rowFields.Length; i++) // read row
                            result.Add(headerFields[i], rowFields[i]);  // add row to collection
                        results.Add(result);
                    }

                }
                return JsonConvert.SerializeObject(results);
            });            
        }

        public async Task<T> GetAsT<T>()
        {
            return JsonConvert.DeserializeObject<T>(await this.GetAsJson());
        }
    }
}
