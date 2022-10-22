using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// C:\Program Files (x86)\Reference Assemblies\Microsoft\WindowsPowerShell\3.0\System.Management.Automation.dll
using System.Management.Automation;  // powershell (32bit)
using Microsoft.Win32;
using DTOLib;  // dto.dll

namespace Helpers
{
    /*
        Usage
        -----                    

        // USING DTO & REGISTRY
        var success = ODBCManager.DTO.CreateDTODatabase(dbName, @"C:\Creative\Manager\Userdata\");
        if (success)
            ODBCManager.REGISTRY.CreatePervasiveDSN(dsnName, dbName);            
        
        -
        
        // USING POWERSHELL
        var success = ODBCManager.POWERSHELL.CreateDTODatabase(dbName, @"C:\Creative\Manager\Userdata\");
        if (success)
            ODBCManager.POWERSHELL.CreatePervasiveDSN(dsnName, dbName);
            
        
        -

        // USING ODBCCP32
        var success = ODBCManager.ODBCCP32.CreatePervasiveDSN(dsnName, @"C:\Creative\Manager\Userdata\");
        var success = ODBCManager.ODBCCP32.CreateAccessDSN(dsnName, @"D:\database.mdb");
        var success = ODBCManager.ODBCCP32.CreateExcelDSN(dsnName, @"D:\testDB.xlsx");

        -

        // CONNECTION STRINGS: 
        Driver={Pervasive ODBC Client Interface};serverName=RcBuilder-PC;dbq=Testdb
        Driver={SQL Server};ServerName=RCBUILDER-PC\RCBUILDERSQL2016;dbq=TEST
        Driver={Microsoft Text Driver (*.txt; *.csv)};dbq=D:\;Extensions=asc,csv,tab,txt;
        Driver={Microsoft Access Driver (*.MDB)};dbq=D:\database.mdb
        Driver={Microsoft Excel Driver (*.xls)};DriverId=790;dbq=D:\testDB.xls            

        -

        // CHECK CONNECTION (SYNTAX)
        var status = ODBCManager.CheckConnection(<connStr>);
        Console.WriteLine($"Connection Status = {status}");
        
        -

        // CHECK CONNECTION (USAGE)
        const string CONNETION_STRING_PERVASIVE_TPL = "Driver={{Pervasive ODBC Client Interface}};serverName={0};dbq={1}";
        const string CONNETION_STRING_SQL_TPL = "Driver={{SQL Server}};server={0};database={1};trusted_connection=YES";
        const string CONNETION_STRING_EXCEL_TPL = "Driver={{Microsoft Excel Driver (*.xls)}};DriverId=790;dbq={0}";  // DO NOT Support '.xlsx' files
        const string CONNETION_STRING_ACCESS_TPL = "Driver={{Microsoft Access Driver (*.MDB)}};dbq={0}";
        const string CONNETION_STRING_TXT_TPL = "Driver={{Microsoft Text Driver (*.txt; *.csv)}};dbq={0};Extensions=asc,csv,tab,txt;";
        const string CONNETION_STRING_CSV_TPL = "Driver={{Microsoft Text Driver (*.txt; *.csv)}};dbq={0};Extensions=asc,csv,tab,txt;";

        CheckConnection(string.Format(CONNETION_STRING_PERVASIVE_TPL, "RcBuilder-PC", "Testdb"));
	    CheckConnection(string.Format(CONNETION_STRING_SQL_TPL, @"RCBUILDER-PC\RCBUILDERSQL2016", "TEST"));			
	    CheckConnection(string.Format(CONNETION_STRING_TXT_TPL, @"D:\"));
	    CheckConnection(string.Format(CONNETION_STRING_CSV_TPL, @"D:\"));		
	    CheckConnection(string.Format(CONNETION_STRING_ACCESS_TPL, @"D:\database.mdb"));	
	    CheckConnection(string.Format(CONNETION_STRING_EXCEL_TPL, @"D:\testDB.xls"));        

        -
    
        // EXECUTE QUERY (USAGE) 
	    ExecuteQuery(string.Format(CONNETION_STRING_PERVASIVE_TPL, "RcBuilder-PC", "Testdb"), "SELECT TOP 3 * FROM Accounts");
	    ExecuteQuery(string.Format(CONNETION_STRING_SQL_TPL, @"RCBUILDER-PC\RCBUILDERSQL2016", "TEST"), "SELECT TOP 3 * FROM Products");
	    ExecuteQuery(string.Format(CONNETION_STRING_TXT_TPL, @"D:\"), "SELECT TOP 3 * FROM ttt.txt");
	    ExecuteQuery(string.Format(CONNETION_STRING_CSV_TPL, @"D:\"), "SELECT TOP 3 * FROM ttt.csv");
	    ExecuteQuery(string.Format(CONNETION_STRING_ACCESS_TPL, @"D:\database.mdb"), "SELECT TOP 3 * FROM Table1");
	    ExecuteQuery(string.Format(CONNETION_STRING_EXCEL_TPL, @"D:\testDB.xls"), "SELECT TOP 3 * FROM [Sheet1$]");

        -        

        REFERENCES
        see 'Scripts\ODBC' 
    */

    public class ODBCManager
    {
        /*  
            DTO
            ---
            Distributed Tuning Objects

            Files:
            - dto.dll
            - dto2.dll

            Platform Target 32bit:
            Project Properties > (tab) Build > Platform Target > Choose x86 (32bit)

            Location:
            C:\PVSW\Bin\

            Tech:
            VB6 DLL

            Register VB6 Dll:
            [CMD]
            > regsvr32 <path>                        
            > regsvr32 "C:\PVSW\Bin\dto.dll"
            > regsvr32 "C:\PVSW\Bin\dto2.dll"

            UnRegister VB6 Dll:
            [CMD]
            > regsvr32 <path> /u
            > regsvr32 "C:\PVSW\Bin\dto.dll" /u
            > regsvr32 "C:\PVSW\Bin\dto2.dll" /u

            Reference:
            Project > Add > Reference > (tab) COM > Browse > Choose Dlls > OK
        */
        public static class DTO
        {
            public static bool CreateDTODatabase(string dbName, string dbPath)
            {
                DtoSession mDtoSession = null;

                try
                {
                    mDtoSession = new DtoSession();

                    var result = mDtoSession.Connect("localhost", "", "");
                    if (result != 0) throw new Exception($"Error connecting to server [{result}]");

                    var db = new DtoDatabase();
                    db.Name = dbName;
                    db.DdfPath = dbPath;
                    db.DataPath = dbPath;
                    db.Flags = dtoDbFlags.dtoDbFlagNotApplicable;
                    result = mDtoSession.Databases.Add(db);

                    var success = result == dtoResult.Dto_Success || result == dtoResult.Dto_errDuplicateName;
                    if (!success) throw new Exception($"Error creating database {dbName} [{result}]");

                    Debug.WriteLine(string.Format("Database ({0}) created. ", dbName));
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {ex.Message}");
                    throw;
                }
                finally {
                    if(mDtoSession != null && mDtoSession.Connected)
                        mDtoSession.Disconnect();
                }
            }
        }

        public static class REGISTRY
        {
            private const string ODBC_INI_REG_PATH = "SOFTWARE\\ODBC\\ODBC.INI\\";
            private const string ODBCINST_INI_REG_PATH = "SOFTWARE\\ODBC\\ODBCINST.INI\\";

            public static bool CreatePervasiveDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Pervasive ODBC Engine Interface", dbName);
            }
            public static bool CreateExcelDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Microsoft Excel Driver (*.xls)", dbName);
            }
            public static bool CreateAccessDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Microsoft Access Driver (*.MDB)", dbName);
            }

            public static bool CreateDSN(string dsnName, string driverName, string dbName)
            {
                var driverKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + driverName);
                if (driverKey == null) throw new Exception(string.Format("ODBC Registry key for driver '{0}' does not exist", driverName));
                var driverPath = driverKey.GetValue("Driver").ToString();

                var datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
                if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
                datasourcesKey.SetValue(dsnName, driverName);

                // Create new key in odbc.ini with dsn name and add values
                var dsnKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + dsnName);
                if (dsnKey == null) throw new Exception("ODBC Registry key for DSN was not created");
                dsnKey.SetValue("AutoDoubleQuote", "0");
                dsnKey.SetValue("DBQ", dbName.ToUpper());
                dsnKey.SetValue("Description", "");
                dsnKey.SetValue("Driver", driverPath);
                dsnKey.SetValue("OpenMode", "0");
                dsnKey.SetValue("LastUser", Environment.UserName);

                return true;
            }

            public static void RemoveDSN(string dsnName)
            {
                Registry.LocalMachine.DeleteSubKeyTree(ODBC_INI_REG_PATH + dsnName);

                var datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
                if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
                datasourcesKey.DeleteValue(dsnName);
            }

            public static bool DSNExists(string dsnName)
            {
                var driversKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + "ODBC Drivers");
                if (driversKey == null) throw new Exception("ODBC Registry key for drivers does not exist");

                return driversKey.GetValue(dsnName) != null;
            }

            public static string[] GetInstalledDrivers()
            {
                var driversKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + "ODBC Drivers");
                if (driversKey == null) throw new Exception("ODBC Registry key for drivers does not exist");

                var driverNames = driversKey.GetValueNames();

                var result = new List<string>();

                foreach (var driverName in driverNames)
                    if (driverName != "(Default)")
                        result.Add(driverName);

                return result.ToArray();
            }
        }

        public static class POWERSHELL
        {
            // Reference: see 'C# Powershell.txt'

            private const string OUTPUT_FILTER = "Out-String";

            public static bool CreatePervasiveDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Pervasive ODBC Engine Interface", dbName);
            }
            public static bool CreateExcelDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Microsoft Excel Driver (*.xls)", dbName);
            }
            public static bool CreateAccessDSN(string dsnName, string dbName)
            {
                return CreateDSN(dsnName, "Microsoft Access Driver (*.MDB)", dbName);
            }

            // TODO ->> TOFIX! - read output
            public static bool CreateDSN(string dsnName, string driverName, string dbName)
            {
                // TODO ->> Kill 'wmiprvse.exe' process if needed 

                var ps = PowerShell.Create();                
                var scriptContent = $@"Add-OdbcDsn -Name '{dsnName}' -DriverName '{driverName}' -DsnType 'System' -Platform '32-bit' -SetPropertyValue @('DBQ={dbName}','OpenMode=0')";

                /// throw new Exception(scriptContent);

                ps.AddScript($"{scriptContent}", true);
                var result = ps.Invoke();

                var hasError = ps.Streams.Error.Count > 0;
                if (hasError)
                {
                    var error = ps.Streams.Error[0];  // 1st error
                    Debug.WriteLine(error.ToString());  // The DSN exists already     
                }                

                return !hasError;
            }

            public static bool CreateDTODatabase(string dbName, string dbPath, string server = null)
            {
                try
                {
                    var ps = PowerShell.Create();
                    
                    var scriptContent = $@"
                        $mdtoSession = New-Object -ComObject DTO.DtoSession
                        $connected = $mdtoSession.connect('{server ?? "localhost"}', '', '')
                        $mdtoDatabase = New-Object -Com DTO.DtoDatabase
                        $mdtoDatabase.DataPath = '{dbPath}'
                        $mdtoDatabase.DdfPath = '{dbPath}'
                        $mdtoDatabase.Name = '{dbName}'
                        $mdtoDatabase.Flags = 0
                        $mdtoSession.Databases.Add($mdtoDatabase)
                    ";

                    // CREATE DB
                    ps.AddScript($"{scriptContent}", true);
                    var result = ps.Invoke();
                    Debug.WriteLine(result);

                    if (result == null || result.Count == 0) throw new Exception($"Error creating database {dbName}");
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {ex.Message}");
                    throw;
                }
            }

            public static bool DSNExists(string dsnName)
            {
                try
                {
                    var ps = PowerShell.Create();

                    var scriptContent = $"Get-OdbcDsn -Name '{dsnName}' -DsnType 'System' -Platform '32-bit'";
                    ps.AddScript($"{scriptContent}", true);
                    var result = ps.Invoke();
                    Debug.WriteLine(result);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {ex.Message}");
                    return false;
                }
            }
        }

        public static class ODBCCP32
        {
            // > Add-OdbcDsn
            [DllImport("ODBCCP32.dll")]
            private static extern bool SQLConfigDataSource(IntPtr hwndParent, int fRequest, string lpszDriver, string lpszAttributes);

            [DllImport("ODBCCP32.dll")]
            private static extern bool SQLInstallerError(int iError, ref int pfErrorCode, StringBuilder lpszErrorMsg, int cbErrorMsgMax, ref int pcbErrorMsg);

            // TODO ->> TOFIX! - ERROR (Pervasive)
            public static bool CreatePervasiveDSN(string dsnName, string dbPath)
            {
                return CreateDSN("Pervasive ODBC Engine Interface", new List<string> {
                    $"DSN={dsnName}",
                    $"DBQ={dbPath}",
                    $"DDFPATH={dbPath}",
                    $"Description=",
                    $"OpenMode=0"
                });
            }
            public static bool CreateExcelDSN(string dsnName, string dbPath)
            {
                // https://learn.microsoft.com/nb-no/sql/odbc/microsoft/odbc-jet-sqlconfigdatasource-excel-driver?view=azure-sqldw-latest

                return CreateDSN("Microsoft Excel Driver (*.xls)", new List<string> {
                    $"DSN={dsnName}",                                        
                    $"DBQ={dbPath}"
                });
            }
            public static bool CreateAccessDSN(string dsnName, string dbPath)
            {
                // https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-access-driver?view=azure-sqldw-latest

                return CreateDSN("Microsoft Access Driver (*.MDB)", new List<string> {
                    $"DSN={dsnName}",
                    $"DBQ={dbPath}",
                    /// $"Uid=",
                    /// $"pwd=",
                    /// $"Description="
                });
            }
            public static bool CreateTextDSN(string dsnName, string dbPath)
            {
                // https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-text-file-driver?view=azure-sqldw-latest

                return CreateDSN("Microsoft Text Driver (*.txt; *.csv)", new List<string> {
                    $"DSN={dsnName}",
                    $"DefaultDir={dbPath}",
                    $"EXTENSIONS=txt",
                    $"CHARACTERSET=ANSI",
                    $"FORMAT=CSVDELIMITED",
                    $"COLNAMEHEADER=FALSE",
                    $"FIL=Text"
                });
            }
            public static bool CreateCsvDSN(string dsnName, string dbPath) {
                return CreateTextDSN(dsnName, dbPath);
            }
            public static bool CreateSqlServerDSN(string dsnName, string dbName, string server)
            {
                // https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-text-file-driver?view=azure-sqldw-latest

                return CreateDSN("SQL Server", new List<string> {
                    $"DSN={dsnName}",
                    $"Server={server}",
                    $"Database={dbName}",                    
                    $"Trusted_Connection=Yes"
                });
            }

            public static bool CreateDSN(string driverName, List<string> attributesList)
            {
                var attributes = string.Join("\0", attributesList);
                var result = SQLConfigDataSource(IntPtr.Zero, 4, driverName, attributes);
                Debug.WriteLine(result);

                return result;
            }
        }
        
        public static bool CheckConnection(string ConnStr)
        {
            try
            {
                Debug.WriteLine($"{ConnStr}");

                using (var connection = new OdbcConnection(ConnStr))
                {
                    connection.Open();
                    Debug.WriteLine($"Connection Open!");

                    var command = new OdbcCommand("select 1");
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {ex.Message}");
                return false;
            }
        }

        public static bool ExecuteQuery(string ConnStr, string Query)
        {
            try
            {
                Debug.WriteLine($"{ConnStr}");

                using (var connection = new OdbcConnection(ConnStr))
                {
                    connection.Open();
                    Debug.WriteLine($"Connection Open!");

                    var command = new OdbcCommand(Query);
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            Debug.WriteLine(reader[0]);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {ex.Message}");
                return false;
            }
        }
    }
}
