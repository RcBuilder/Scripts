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

        // CHECK CONNECTION
        var status = ODBCManager.CheckConnection("Pervasive ODBC Client Interface", dbName, "RcBuilder-PC");
        Console.WriteLine($"Connection Status = {status}");

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
            public static bool CreateDTODatabase(string dbName, string dbPath) {

                try {
                    var mDtoSession = new DtoSession();

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

                    result = mDtoSession.Disconnect();                    

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {ex.Message}");
                    throw;
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

        public static class POWERSHELL {
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
            public static bool CreateDSN(string dsnName, string driverName, string dbName) {

                using (var ps = PowerShell.Create()) {
                    var scriptContent = $@"Add-OdbcDsn -Name '{dsnName}' -DriverName '{driverName}' -DsnType 'System' -Platform '32-bit' -SetPropertyValue @('DBQ={dbName}','OpenMode=0')";

                    ps.AddScript($"{scriptContent}|{OUTPUT_FILTER}", true);
                    var result = ps.Invoke();
                    Debug.WriteLine(result);

                    return result.Count > 0;
                }                
            }

            public static bool CreateDTODatabase(string dbName, string dbPath, string server = null) {

                try {
                    using (var ps = PowerShell.Create()) {
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
                        ps.AddScript($"{scriptContent} | {OUTPUT_FILTER}", true);
                        var result = ps.Invoke();
                        Debug.WriteLine(result);

                        if (result == null || result.Count == 0) throw new Exception($"Error creating database {dbName}");
                    }

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
                    ps.AddScript($"{scriptContent} | {OUTPUT_FILTER}", true);
                    var result = ps.Invoke();
                    Debug.WriteLine(result);

                    return true;
                }
                catch (Exception ex) {
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
            public static bool CreatePervasiveDSN(string dsnName, string dbPath) {
                return CreateDSN("Pervasive ODBC Engine Interface", new List<string> {
                    $"DSN={dsnName}", 
                    $"DBQ={dbPath}",
                    $"DDFPATH={dbPath}",
                    $"Description=",
                    $"OpenMode=0"
                });
            }
            public static bool CreateExcelDSN(string dsnName, string dbPath) {
                return CreateDSN("Microsoft Excel Driver (*.xls)", new List<string> {
                    $"DSN={dsnName}",
                    $"FileType=Excel",
                    $"Uid=",
                    $"pwd=",
                    $"Description=",
                    $"DataDirectory={dbPath}"
                });
            }
            public static bool CreateAccessDSN(string dsnName, string dbPath)
            {
                return CreateDSN("Microsoft Access Driver (*.MDB)", new List<string> {
                    $"DSN={dsnName}",
                    $"DBQ={dbPath}",
                    $"Uid=",
                    $"pwd=",
                    $"Description="
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

        public static bool CheckConnection(string driverName, string dbq, string server = null) {
            return CheckConnection($"Driver={{{driverName}}};ServerName={server ?? "localhost"};dbq={dbq}");
        }
        public static bool CheckConnection(string ConnStr)
        {
            try
            {
                using (var connection = new OdbcConnection(ConnStr))
                {
                    connection.Open();
                    Debug.WriteLine($"Connection Open! ({ConnStr})");

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
    }
}
