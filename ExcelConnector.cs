using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Data.Common;
using Newtonsoft.Json;
using System.IO;

/// Install-Package DocumentFormat.OpenXml -Version 2.18.0
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BLL
{
    /*
        INSTANCES
        ---------
        * ExcelOdbcConnector    // using Odbc
        * ExcelOleDbConnector   // using OleDB

        DEPENDENCY
        ----------
        * AccessDatabaseEngine_X32.exe
        * AccessDatabaseEngine_X64.exe

        ATTRIBUTES
        ----------
        * ExcelColumn        

        USING
        -----        
        // get sheet-list
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sheetList = excelConnector.GetSheetList();                   
        sheetList.ToList().ForEach(x => Console.WriteLine(x));

        ---

        // set the default sheet name
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");            
        excelConnector.SheetName = excelConnector.GetDefaultSheet();    

        ---

        // get as json string
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sJson = await excelConnector.GetAsJson();

        ---

        // get as data-table
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var dt = await excelConnector.GetAsDataTable();

        ---

        // get as T object (generic) 
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var lst = await excelConnector.GetAsT<IEnumerable<SomeModel>>();

        ---

        // create a sheet from List<T>
        var items = new List<Item>();
        items.Add(new Item("1000", "Item-A", 399));
        items.Add(new Item("1001", "Item-B", 250));
        items.Add(new Item("1002", "Item-C", 209));
        
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sheetPath = await excelConnector.Create(items);

        ---

        // create a sheet from DataTable
        var dt = new DataTable();
        dt.Columns.Add(new DataColumn("COLUMN-1"));
        dt.Columns.Add(new DataColumn("COLUMN-2"));
        dt.Columns.Add(new DataColumn("COLUMN-3"));        

        foreach (var item in items) {
            DataRow row = dt.NewRow();
            row["COLUMN-1"] = item.Id;
            row["COLUMN-2"] = item.Name;
            row["COLUMN-3"] = item.Price;            
            dt.Rows.Add(row);
        }

        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sheetPath = await excelConnector.Create(dt);

        --- 

        // using custom attributes
        // note! only Entities support this feature
        class Item
        {
            [ExcelColumn("Identity")]
            public string Id { get; set; }
            public string Name { get; set; }
            public float Price { get; set; }

            public Item(string Id, string Name, float Price) {
                this.Id = Id;
                this.Name = Name;
                this.Price = Price;
            }
        }        
        ...
        ...
        var excelConnector = new ExcelConnector(@"C:\test.xlsx");
        var sheetPath = await excelConnector.Create(items);

        ---
    */

    public interface IExcelConnector {
        string ConnStr { get; set; }
        IEnumerable<string> GetSheetList();
        string GetDefaultSheet();
        Task<DataTable> GetAsDataTable();
        Task<string> GetAsJson();
        Task<T> GetAsT<T>();
        Task<string> Create(DataTable Source);
        Task<string> Create<T>(IEnumerable<T> Source);
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : Attribute {
        public string Name { get; protected set; }
        public ExcelColumnAttribute(string Name) {
            this.Name = Name;            
        }
    }

    public abstract class ExcelConnector : IExcelConnector
    {
        public const int HEADER_MAX_LENGTH = 31;

        public string ConnStr { get; set; }
        public string FilePath { get; set; }
        public string SheetName { get; set; }

        public ExcelConnector(string ConnStr) : this(ConnStr, ConnStr) { }
        public ExcelConnector(string ConnStr, string FilePath) : this(ConnStr, FilePath, "Sheet1") { }
        public ExcelConnector(string ConnStr, string FilePath, string SheetName)
        {
            this.ConnStr = ConnStr;
            this.FilePath = FilePath;
            this.SheetName = this.FixSheetName(SheetName);
        }

        public virtual string GetDefaultSheet() {
            return this.GetSheetList()?.FirstOrDefault() ?? "";
        }

        public abstract IEnumerable<string> GetSheetList();
        public abstract Task<DataTable> GetAsDataTable();
        public abstract Task<string> GetAsJson();
        public abstract Task<T> GetAsT<T>();
        public abstract Task<string> Create(DataTable Source);
        public abstract Task<string> Create<T>(IEnumerable<T> Source);

        protected DataTable ReadAsDataTable(DbDataReader dr) {
            if (dr == null || !dr.HasRows) return null;

            var dt = new DataTable();
            dt.Load(dr);
            return dt;
        }

        /*  
            note! 
            use [JsonProperty] attribute to map fields (also supports hebrew)

            e.g: 
            [JsonProperty(PropertyName = "מספר לקוח")]
            public string ClientName { get; set; }
        */
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

        protected string FixSheetName(string Value) {
            // empty name
            if (string.IsNullOrWhiteSpace(Value))
                Value = $"{DateTime.Now.ToString("yyyyMMddHHmm")}";

            // too long
            if (Value.Length > HEADER_MAX_LENGTH)
                Value = Value.Substring(0, HEADER_MAX_LENGTH);

            // must ends with $
            ////if (!Value.EndsWith("$"))
            ////    Value = $"{Value}$";

            return Value;
        }

        protected async Task<bool> SaveAsync(byte[] data) {
            using (var fs = new FileStream(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                await fs.WriteAsync(data, 0, data.Length);
            return true;
        }

        protected bool Save(byte[] data) {
            File.WriteAllBytes(this.FilePath, data);
            return true;
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
            : base($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={FilePath};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'", FilePath, SheetName) { }                    

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
            var query = $"SELECT * FROM [{this.SheetName}$]";
            using (var conn = new OleDbConnection(this.ConnStr)) {
                conn.Open();
                var cmd = new OleDbCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsDataTable(dr);
            }
        }

        public override async Task<string> GetAsJson() {
            var query = $"SELECT * FROM [{this.SheetName}$]";
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
            var query = $"SELECT * FROM [{this.SheetName}$]";
            using (var conn = new OleDbConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OleDbCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsT<T>(dr);
            }
        }

        // TODO ->> Implement!
        public override Task<string> Create(DataTable Source) {
            throw new NotImplementedException();
        }

        public override Task<string> Create<T>(IEnumerable<T> Source) {
            throw new NotImplementedException();
        }
    }

    /*                
        [Odbc]
        Engines:
        - 32bit   // C:\Windows\System32\odbcad32.exe
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
            : base($"Driver={{Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)}};Dbq={FilePath}", FilePath, SheetName) { }
       
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
            var query = $"SELECT * FROM [{this.SheetName}$]";
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
            var query = $"SELECT * FROM [{this.SheetName}$]";
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
            var query = $"SELECT * FROM [{this.SheetName}$]";
            using (var conn = new OdbcConnection(this.ConnStr))
            {
                conn.Open();
                var cmd = new OdbcCommand(query, conn);
                using (var dr = await cmd.ExecuteReaderAsync())
                    return this.ReadAsT<T>(dr);
            }
        }

        // TODO ->> Implement!
        public override Task<string> Create(DataTable Source) {
            throw new NotImplementedException();
        }

        public override Task<string> Create<T>(IEnumerable<T> Source) {
            throw new NotImplementedException();
        }
    }

    // TODO ->> Implement! (ALL but Create)
    /*                
        [OpenXml]
        Nuget:
        > Install-Package DocumentFormat.OpenXml -Version 2.18.0

        Source:
        https://github.com/OfficeDev/Open-XML-SDK

        Namespaces:
        using DocumentFormat.OpenXml;
        using DocumentFormat.OpenXml.Packaging;
        using DocumentFormat.OpenXml.Spreadsheet;

        Aliases: (optional)
        using nsOpenXml = DocumentFormat.OpenXml;        
    */
    public class ExcelOpenXmlConnector : ExcelConnector
    {
        public ExcelOpenXmlConnector(string FilePath, string SheetName = "Sheet1")
            : base(FilePath, FilePath, SheetName) { }

        public override IEnumerable<string> GetSheetList()
        {
            throw new NotImplementedException();
        }

        public override async Task<DataTable> GetAsDataTable()
        {
            throw new NotImplementedException();
        }

        public override async Task<string> GetAsJson()
        {
            throw new NotImplementedException();
        }

        public override async Task<T> GetAsT<T>()
        {
            throw new NotImplementedException();
        }

        public async override Task<string> Create(DataTable Source)
        {
            using (var ms = new MemoryStream())
            {
                using (var spreadsheet = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheet.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());
                    var sheetsCount = sheets?.Elements<Sheet>()?.Count() ?? 0;

                    var sheetId = sheetsCount > 0 ? sheets.Elements<Sheet>()?.Max(x => x.SheetId.Value) : 1;
                    var sheet = new Sheet()
                    {
                        Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = sheetId,
                        Name = this.SheetName
                    };

                    // -- DATA --

                    var columns = new List<(string Name, string DisplayName, Type Type)>();
                    foreach (DataColumn column in Source.Columns)
                        columns.Add((column.ColumnName, column.ColumnName, column.DataType));                    

                    // add header row
                    var rowHeader = new Row();
                    rowHeader.Append(columns.Select(c => new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(c.DisplayName)
                    }));
                    sheetData.Append(rowHeader);

                    // add items rows
                    foreach (DataRow item in Source.Rows)
                    {
                        var row = new Row();
                        foreach (var c in columns)
                            row.Append(new Cell
                            {
                                DataType = this.PropertyType2CellType(c.Type),
                                CellValue = new CellValue(item[c.Name].ToString())
                            });
                        sheetData.Append(row);
                    }

                    // -- END OF DATA --

                    sheets.Append(sheet);
                }

                this.Save(ms.ToArray());
            }

            return this.FilePath;
        }

        public async override Task<string> Create<T>(IEnumerable<T> Source)
        {
            using (var ms = new MemoryStream())
            {
                using (var spreadsheet = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheet.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());
                    var sheetsCount = sheets?.Elements<Sheet>()?.Count() ?? 0;

                    var sheetId = sheetsCount > 0 ? sheets.Elements<Sheet>()?.Max(x => x.SheetId.Value) : 1;
                    var sheet = new Sheet()
                    {
                        Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = sheetId,
                        Name = this.SheetName
                    };

                    // -- DATA --

                    var type = typeof(T);
                    var columns = new List<(string Name, string DisplayName, Type Type)>();
                    foreach (var p in type.GetProperties()) {
                        var attr = (p.GetCustomAttributes(typeof(ExcelColumnAttribute), false).FirstOrDefault() as ExcelColumnAttribute);
                        columns.Add((p.Name, attr != null ? attr.Name : p.Name, p.PropertyType));
                    }

                    // add header row
                    var rowHeader = new Row();
                    rowHeader.Append(columns.Select(c => new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(c.DisplayName)
                    }));
                    sheetData.Append(rowHeader);

                    // add items rows
                    foreach (var item in Source)
                    {
                        var row = new Row();
                        foreach (var c in columns)                            
                            row.Append(new Cell
                            {
                                DataType = this.PropertyType2CellType(c.Type),
                                CellValue = DynamicCellValue(type.GetProperty(c.Name).GetValue(item, null), c.Type)
                            });                        
                        sheetData.Append(row);
                    }

                    // -- END OF DATA --

                    sheets.Append(sheet);                    
                }

                this.Save(ms.ToArray());               
            }

            return this.FilePath;
        }

        protected CellValues PropertyType2CellType(Type PropertyType)
        {
            switch (Type.GetTypeCode(PropertyType))
            {
                default:
                case TypeCode.String: return CellValues.String;
                case TypeCode.Boolean: return CellValues.Boolean;
                case TypeCode.DateTime: return CellValues.Date;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64: return CellValues.Number;
            }
        }

        protected CellValue DynamicCellValue(object Value, Type PropertyType) {            
            switch (Type.GetTypeCode(PropertyType))
            {
                default:
                case TypeCode.String:   return new CellValue((string)Convert.ChangeType(Value, PropertyType));
                case TypeCode.Boolean:  return new CellValue((bool)Convert.ChangeType(Value, PropertyType));
                case TypeCode.DateTime: return new CellValue((DateTime)Convert.ChangeType(Value, PropertyType));
                case TypeCode.Decimal:  return new CellValue((decimal)Convert.ChangeType(Value, PropertyType));
                case TypeCode.Double:   return new CellValue((double)Convert.ChangeType(Value, PropertyType));
                case TypeCode.UInt16:
                case TypeCode.Int16:    return new CellValue((byte)Convert.ChangeType(Value, PropertyType));
                case TypeCode.UInt32:
                case TypeCode.Int32:    return new CellValue((int)Convert.ChangeType(Value, PropertyType));
                case TypeCode.UInt64:
                case TypeCode.Int64:    return new CellValue((double)(long)Convert.ChangeType(Value, PropertyType));
                case TypeCode.Byte:     return new CellValue((byte)Convert.ChangeType(Value, PropertyType));
                case TypeCode.SByte:    return new CellValue((sbyte)Convert.ChangeType(Value, PropertyType));
                case TypeCode.Single:   return new CellValue((float)Convert.ChangeType(Value, PropertyType));                
            }            
        }
    }
}