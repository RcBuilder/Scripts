using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Data.Common;
using Newtonsoft.Json;

namespace FilesProcessorBLL
{
    /*
        USING
        -----
        // get sheet-list
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sheetList = excelConnector.GetSheetList();                   
        sheetList.ToList().ForEach(x => Console.WriteLine(x));

        ---

        // set the default sheet name
        var excelConnector = new ExcelConnector(@"C:\test-transactions.xlsx");            
        excelConnector.SheetName = excelConnector.GetDefaultSheet();    

        ---

        // get as json string
        var excelConnector = new ExcelConnector(@"C:\test-transactions.xlsx");
        var sJson = await excelConnector.GetAsJson();

        ---

        // get as data-table
        var excelConnector = new ExcelConnector(@"C:\test-transactions.xlsx");
        var dt = await excelConnector.GetAsDataTable();

        ---

        // get as T object (generic) 
        var excelConnector = new ExcelConnector(@"C:\test-transactions.xlsx");
        var lst = await excelConnector.GetAsT<IEnumerable<SomeModel>>();
    */

    public interface IExcelConnector {
        string ConnStr { get; set; }
        IEnumerable<string> GetSheetList();
        string GetDefaultSheet();
        Task<DataTable> GetAsDataTable();
        Task<string> GetAsJson();
        Task<T> GetAsT<T>();
    }

    public abstract class ExcelConnector : IExcelConnector
    {
        public string ConnStr { get; set; }
        public string SheetName { get; set; }

        public ExcelConnector(string ConnStr, string SheetName)
        {
            this.ConnStr = ConnStr;
            this.SheetName = SheetName;

            // fix sheet name - must ends with $
            if (!this.SheetName.EndsWith("$"))
                this.SheetName = $"{this.SheetName}$";
        }

        public string GetDefaultSheet() {
            return this.GetSheetList()?.FirstOrDefault() ?? "";
        }

        public abstract IEnumerable<string> GetSheetList();
        public abstract Task<DataTable> GetAsDataTable();
        public abstract Task<string> GetAsJson();
        public abstract Task<T> GetAsT<T>();

        protected DataTable ReadAsDataTable(DbDataReader dr) {
            if (dr == null || !dr.HasRows) return null;

            var dt = new DataTable();
            dt.Load(dr);
            return dt;
        }

        protected string ReadAsJson(DbDataReader dr) {
            if (dr == null || !dr.HasRows) return "[]";

            var results = new List<Dictionary<string, string>>();
            while (dr.Read())
            {
                var result = new Dictionary<string, string>();
                for (var i = 0; i < dr.VisibleFieldCount; i++)
                    result.Add(dr.GetName(i), dr[i].ToString().Trim());
                results.Add(result);
            }

            return JsonConvert.SerializeObject(results);
        }

        protected T ReadAsT<T>(DbDataReader dr) {
            return JsonConvert.DeserializeObject<T>(this.ReadAsJson(dr));
        }
    }

    /*
        [OleDb]                  
        Engines:
        - Microsoft.Jet.OleDb.4.0    // xls 
        - Microsoft.ACE.OLEDB.12.0   // xlsx 
            file: AccessDatabaseEngine.exe (32bit or 64bit)
            download: https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255
            VS set IIS Version: Options > Projects And Solutions > Web Projects > General > check/uncheck 'use 64 bit version...'
            VS set Project Platform: Project Properties > Build > Platform target

        var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={this.FilePath};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'"; 
        var query = $"SELECT TOP 10 * FROM [{this.SheetName}]";
        using (var conn = new OleDbConnection(connectionString)) {
            conn.Open();
            var cmd = new OleDbCommand(query, conn);
            using (var dr = await cmd.ExecuteReaderAsync())
                while (dr.Read()) Console.WriteLine(dr[1]);
        }
    */
    public class ExcelOleDbConnector : ExcelConnector
    {        
        public ExcelOleDbConnector(string FilePath, string SheetName = "Sheet1") 
            : base($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={FilePath};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'", SheetName) { }                    

        public override IEnumerable<string> GetSheetList() {
            var sheetList = new List<string>();

            /*                  
                schema:
                https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ole-db-schema-collections                      
            */            
            using (var conn = new OleDbConnection(this.ConnStr)) {
                conn.Open();
                var dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow row in dbSchema.Rows)
                    sheetList.Add(row["TABLE_NAME"].ToString());
            }

            return sheetList;
        }

        public override async Task<DataTable> GetAsDataTable() {
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OleDbConnection(this.ConnStr)) {
                conn.Open();
                var cmd = new OleDbCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsDataTable(dr);
            }
        }

        public override async Task<string> GetAsJson() {
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OleDbConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OleDbCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsJson(dr);
            }
        }

        public override async Task<T> GetAsT<T>()
        {
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OleDbConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OleDbCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsT<T>(dr);
            }
        }
    }

    /*                
        [Odbc]
        Engines:
        - 32bit   // c:\Windows\System32\odbcad32.exe
        - 64bit   // C:\Windows\SysWOW64\odbcad32.exe

        var connectionString = $"Driver={{Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)}};Dbq={this.FilePath}";
        var query = $"SELECT TOP 10 * FROM [{this.SheetName}]";
        using (var conn = new OdbcConnection(connectionString)) {
            conn.Open();                    
            var cmd = new OdbcCommand(query, conn);                    
            using (var dr = await cmd.ExecuteReaderAsync())                    
                while (dr.Read()) Console.WriteLine(dr[1]);                     
        }
    */
    public class ExcelOdbcConnector : ExcelConnector
    {        
        public ExcelOdbcConnector(string FilePath, string SheetName = "Sheet1") 
            : base($"Driver={{Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)}};Dbq={FilePath}", SheetName) { }
       
        public override IEnumerable<string> GetSheetList()
        {
            var sheetList = new List<string>();

            /*                                 
                schema:
                https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/odbc-schema-collections                     
            */
            
            using (var conn = new OdbcConnection(this.ConnStr))
            {
                conn.Open();
                var dbSchema = conn.GetSchema("Tables");
                foreach (DataRow row in dbSchema.Rows)
                    sheetList.Add(row["TABLE_NAME"].ToString());
            }

            return sheetList;
        }
        
        public override async Task<DataTable> GetAsDataTable()
        {            
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OdbcConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OdbcCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsDataTable(dr);
            }
        }

        public override async Task<string> GetAsJson()
        {
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OdbcConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OdbcCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsJson(dr);
            }
        }

        public override async Task<T> GetAsT<T>()
        {
            var query = $"SELECT * FROM [{this.SheetName}]";
            using (var conn = new OdbcConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OdbcCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsT<T>(dr);
            }
        }
    }
}

public class TEST{
    public float BI_ID { get; set; }
    public string Pnr_No { get; set; }
    public string FirstName { get; set; }

    public override string ToString()
    {
        return $"#{this.BI_ID} | {this.Pnr_No} | {this.FirstName}";
    }
}