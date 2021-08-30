using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace BLL
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
        public bool HasNoHeader { get; set; }
        public Encoding Encoding { get; set; } = Encoding.GetEncoding("Windows-1255");

        public CsvConnector(string FilePath, string Delimiter = ",", bool HasNoHeader = false) {
            this.FilePath = FilePath;
            this.Delimiter = Delimiter;
            this.HasNoHeader = HasNoHeader;
        }

        public async Task<DataTable> GetAsDataTable()
        {
            return await Task.Factory.StartNew(() => {
                var dt = new DataTable();
                using (var reader = new TextFieldParser(this.FilePath, this.Encoding))
                {
                    reader.TextFieldType = FieldType.Delimited;
                    reader.SetDelimiters(this.Delimiter);

                    var firstRow = reader.ReadFields();
                    string[] headerFields = firstRow;  // read 1st line - the header 

                    // no-header mode
                    if (this.HasNoHeader) {
                        headerFields = this.GenerateDefaultHeader(headerFields.Length); // override with default header
                        dt.Columns.AddRange(headerFields.Select(x => new DataColumn(x)).ToArray());
                        dt.Rows.Add(this.ReadDataRow(firstRow, dt));  // add the 1st row to the Rows collection
                    }
                    else
                        dt.Columns.AddRange(headerFields.Select(x => new DataColumn(x)).ToArray());                    

                    while (!reader.EndOfData)                                               
                        dt.Rows.Add(this.ReadDataRow(reader, dt));  // add row to collection                                                    
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

                    var firstRow = reader.ReadFields();
                    string[] headerFields = firstRow;  // read 1st line - the header 

                    // no-header mode
                    if (this.HasNoHeader) {
                        headerFields = this.GenerateDefaultHeader(headerFields.Length); // override with default header
                        results.Add(this.ReadJsonRow(firstRow, headerFields));  // add the 1st row to the Rows collection
                    }
                    
                    while (!reader.EndOfData)                       
                        results.Add(this.ReadJsonRow(reader, headerFields));
                }
                return JsonConvert.SerializeObject(results);
            });            
        }

        public async Task<T> GetAsT<T>()
        {
            return JsonConvert.DeserializeObject<T>(await this.GetAsJson());
        }

        // -- private --

        private Dictionary<string, object> ReadJsonRow(TextFieldParser reader, string[] headerFields) {
            return this.ReadJsonRow(reader.ReadFields(), headerFields);            
        }
        private Dictionary<string, object> ReadJsonRow(string[] rowFields, string[] headerFields)
        {
            var result = new Dictionary<string, object>();            
            for (var i = 0; i < headerFields.Length; i++) // read row
                result.Add(headerFields[i], rowFields[i]);  // add row to collection
            return result;
        }

        private DataRow ReadDataRow(TextFieldParser reader, DataTable dt){
            return this.ReadDataRow(reader.ReadFields(), dt);
        }
        private DataRow ReadDataRow(string[] rowFields, DataTable dt)
        {            
            var row = dt.NewRow();
            for (var i = 0; i < rowFields.Length; i++) // read row                                                 
                row[i] = rowFields[i];
            return row;
        }

        private string[] GenerateDefaultHeader(int ColumnsCount) {
            return Enumerable.Range(1, ColumnsCount).Select(i => $"Column-{i}").ToArray();
        }
    }
}
