<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.CSharp.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\Xamarin.Mac\v2.0\Facades\Microsoft.Win32.Registry.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\WindowsPowerShell\3.0\System.Management.Automation.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.InteropServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>Microsoft.PowerShell</Namespace>
  <Namespace>Microsoft.Win32</Namespace>
  <Namespace>System.Data</Namespace>
  <Namespace>System.Data.Odbc</Namespace>
  <Namespace>System.Management.Automation</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

// Reference:
// https://docs.actian.com/psql/psqlv13/index.html#page/odbc%2Fodbcadm.htm%23

/*
	[Powershell]
	using System.Management.Automation;
	
	create powershell object:
	var ps = PowerShell.Create();
	
	commands:
	AddCommand(<command>)
	AddParameter(<name>, <value>)
	AddScript("<content>", <useLocalScope>)
	AddStatement()
	
	execute script (.ps1):
	var scriptContent = File.ReadAllText(@"D:\test-ps-script-2.ps1");			
	ps.AddScript(scriptContent, true);
	var res = ps.Invoke();		
*/	

// CONNETION_STRING
// Driver={Pervasive ODBC Client Interface};serverName=RcBuilder-PC;dbq=Testdb
// Driver={SQL Server};ServerName=RCBUILDER-PC\RCBUILDERSQL2016;dbq=TEST
// Driver={Microsoft Text Driver (*.txt; *.csv)};dbq=D:\;Extensions=asc,csv,tab,txt;
// Driver={Microsoft Access Driver (*.MDB)};dbq=D:\database.mdb
// Driver={Microsoft Excel Driver (*.xls)};DriverId=790;dbq=D:\testDB.xls
const string CONNETION_STRING_PERVASIVE_TPL = "Driver={{Pervasive ODBC Client Interface}};serverName={0};dbq={1}";
const string CONNETION_STRING_SQL_TPL = "Driver={{SQL Server}};server={0};database={1};trusted_connection=YES";
const string CONNETION_STRING_EXCEL_TPL = "Driver={{Microsoft Excel Driver (*.xls)}};DriverId=790;dbq={0}";  // DO NOT Support '.xlsx' files
const string CONNETION_STRING_ACCESS_TPL = "Driver={{Microsoft Access Driver (*.MDB)}};dbq={0}";
const string CONNETION_STRING_TXT_TPL = "Driver={{Microsoft Text Driver (*.txt; *.csv)}};dbq={0};Extensions=asc,csv,tab,txt;";
const string CONNETION_STRING_CSV_TPL = "Driver={{Microsoft Text Driver (*.txt; *.csv)}};dbq={0};Extensions=asc,csv,tab,txt;";

void Main()
{	
	/*
	var ps = PowerShell.Create();
	ps.AddCommand("Get-OdbcDsn");
	ps.AddParameter("Name", "DefaultDB");
	ps.AddParameter("DsnType", "System");
	ps.AddParameter("Platform", "32-bit");
	var res1 = ps.Invoke();
	Console.WriteLine(res1);
			
	var scriptContent = File.ReadAllText(@"D:\test-ps-script-2.ps1");			
	ps.AddScript(scriptContent, true);
	var res2 = ps.Invoke();
	Console.WriteLine(res2);
	
	ps.AddScript("Get-OdbcDsn -Name 'DefaultDB' -DsnType 'System' -Platform '32-bit'", true);
	var res3 = ps.Invoke();
	Console.WriteLine(res3);
	*/
	
	/// Build_SystemDSN1("Test201", @"D:\database.mdb");
	/// Build_SystemDSN2("Test400", @"D:\testDB.xlsx");
	/// Build_SystemDSN3("Test300", @"C:\Creative\Manager\Userdata\");	// TODO ->> Not Working 
	/// Build_SystemDSN4("Test600", @"D:\ttt.csv");	
	/// Build_SystemDSN5("Test700", @"TEST", @"RCBUILDER-PC\RCBUILDERSQL2016");	
	/// Build_SystemDSN4("Test800", @"D:\ttt.txt");	
	
	/// var success = SQLConfigDataSource(IntPtr.Zero, 4, "Pervasive ODBC Engine Interface", $@"DatabaseName=Test3db\0Description=General\0DictionaryLocation=C:\Creative\Manager\Userdata\\0DataFileLocation=C:\Creative\Manager\Userdata\");
	/// Console.WriteLine(success);

	/// ODBCManager.CreateDSN("Test1", "Bla bla bla", "Pervasive ODBC Engine Interface", "Testdb");
	/// ODBCManager.RemoveDSN("Test1");
		
	/// CheckConnection(string.Format(CONNETION_STRING_PERVASIVE_TPL, "RcBuilder-PC", "Testdb"));
	/// CheckConnection(string.Format(CONNETION_STRING_SQL_TPL, @"RCBUILDER-PC\RCBUILDERSQL2016", "TEST"));			
	/// CheckConnection(string.Format(CONNETION_STRING_TXT_TPL, @"D:\"));
	/// CheckConnection(string.Format(CONNETION_STRING_CSV_TPL, @"D:\"));		
	/// CheckConnection(string.Format(CONNETION_STRING_ACCESS_TPL, @"D:\database.mdb"));	
	/// CheckConnection(string.Format(CONNETION_STRING_EXCEL_TPL, @"D:\testDB.xls"));

	/// ExecuteQuery(string.Format(CONNETION_STRING_PERVASIVE_TPL, "RcBuilder-PC", "Testdb"), "SELECT TOP 3 * FROM Accounts");
	/// ExecuteQuery(string.Format(CONNETION_STRING_SQL_TPL, @"RCBUILDER-PC\RCBUILDERSQL2016", "TEST"), "SELECT TOP 3 * FROM Products");
	/// ExecuteQuery(string.Format(CONNETION_STRING_TXT_TPL, @"D:\"), "SELECT TOP 3 * FROM ttt.txt");
	/// ExecuteQuery(string.Format(CONNETION_STRING_CSV_TPL, @"D:\"), "SELECT TOP 3 * FROM ttt.csv");
	/// ExecuteQuery(string.Format(CONNETION_STRING_ACCESS_TPL, @"D:\database.mdb"), "SELECT TOP 3 * FROM Table1");
	/// ExecuteQuery(string.Format(CONNETION_STRING_EXCEL_TPL, @"D:\testDB.xls"), "SELECT TOP 3 * FROM [Sheet1$]");

	/// Console.WriteLine("");
}

public void Build_SystemDSN1(string DSN_NAME, string Db_Path)
{
	// https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-access-driver?view=azure-sqldw-latest
    var Driver = "Microsoft Access Driver (*.MDB)" + '\0';
	var Attributes = "";
    Attributes = Attributes + "DSN=" + DSN_NAME + '\0';    
    Attributes = Attributes + "DBQ=" + Db_Path + '\0';

    var res = SQLConfigDataSource(IntPtr.Zero, 4, Driver, Attributes);
	
	Console.WriteLine(res);
}

public void Build_SystemDSN2(string DSN_NAME, string Db_Path)
{
	// https://learn.microsoft.com/nb-no/sql/odbc/microsoft/odbc-jet-sqlconfigdatasource-excel-driver?view=azure-sqldw-latest
    var Driver = "Microsoft Excel Driver (*.xls)" + '\0';
	var Attributes = "";
    Attributes = Attributes + "DSN=" + DSN_NAME + '\0';	  
    Attributes = Attributes + "DBQ=" + Db_Path + '\0';	

    var res = SQLConfigDataSource(IntPtr.Zero, 4, Driver, Attributes);
	
	Console.WriteLine(res);
}

// TODO ->> Not Working
public void Build_SystemDSN3(string DSN_NAME, string Db_Path)
{
    var Driver = "Pervasive ODBC Engine Interface" + '\0';
    var Attributes = "";
	Attributes = Attributes + "DSN=" + DSN_NAME + '\0';
	Attributes = Attributes + "DBQ=" + Db_Path + '\0';   	
	Attributes = Attributes + "DDFPATH=" + Db_Path + '\0';   
	Attributes = Attributes + "OpenMode=" + 0 + '\0';  	

    var res = SQLConfigDataSource(IntPtr.Zero, 4, Driver, Attributes);
	Console.WriteLine(res);
	
	/*
	int errorCode = 0, temp = 0; 
	var errorMsg = new StringBuilder();
	var retData = SQLInstallerError(1, ref errorCode, errorMsg, 52, ref temp);		 
	Console.WriteLine($"{errorMsg} ({errorCode})");
	*/
}

public void Build_SystemDSN4(string DSN_NAME, string Db_Path)
{
	// https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-text-file-driver?view=azure-sqldw-latest
    var Driver = "Microsoft Text Driver (*.txt; *.csv)" + '\0';
	var Attributes = "";
    Attributes = Attributes + "DSN=" + DSN_NAME + '\0';	
    Attributes = Attributes + "DefaultDir=" + Db_Path + '\0';
    Attributes = Attributes + "EXTENSIONS=txt" + '\0';
	Attributes = Attributes + "CHARACTERSET=ANSI" + '\0';
	Attributes = Attributes + "FORMAT=CSVDELIMITED" + '\0';
	Attributes = Attributes + "COLNAMEHEADER=FALSE" + '\0';
	Attributes = Attributes + "FIL=Text" + '\0';

    var res = SQLConfigDataSource(IntPtr.Zero, 4, Driver, Attributes);
	
	Console.WriteLine(res);
}

public void Build_SystemDSN5(string DSN_NAME, string Db_Name, string Server)
{
	// https://learn.microsoft.com/nb-no/sql/odbc/microsoft/sqlconfigdatasource-text-file-driver?view=azure-sqldw-latest
    var Driver = "SQL Server" + '\0';
	var Attributes = "";
    Attributes = Attributes + "DSN=" + DSN_NAME + '\0';	    
    Attributes = Attributes + "Trusted_Connection=Yes" + '\0';
	Attributes = Attributes + "Server=" + Server + '\0';
	Attributes = Attributes + "Database=" + Db_Name + '\0';
	
    var res = SQLConfigDataSource(IntPtr.Zero, 4, Driver, Attributes);
	
	Console.WriteLine(res);
}

void CheckConnection(string ConnStr){
	try
	{   
		Console.WriteLine($"{ConnStr}");
		
		using(var connection = new OdbcConnection(ConnStr)){		
			connection.Open();		
	    	Console.WriteLine($"Connection Open!");	    	
			
			var command = new OdbcCommand("select 1");
			command.Connection = connection;
			command.ExecuteNonQuery();
		}    
	}
	catch (Exception ex){
	    Console.WriteLine(ex.Message);
	}
}

void ExecuteQuery(string ConnStr, string Query){
	try
	{   
		Console.WriteLine($"{ConnStr}");
		
		using(var connection = new OdbcConnection(ConnStr)){		
			connection.Open();		
	    	Console.WriteLine($"Connection Open!");	    	
			
			var command = new OdbcCommand(Query);
			command.Connection = connection;
			using(var reader = command.ExecuteReader()){
				while (reader.Read())        
            		Console.WriteLine(reader[0]); 			
			}
		}    
	}
	catch (Exception ex){
	    Console.WriteLine(ex.Message);
	}
}

public static class ODBCManager
{
    private const string ODBC_INI_REG_PATH = "SOFTWARE\\ODBC\\ODBC.INI\\";
    private const string ODBCINST_INI_REG_PATH = "SOFTWARE\\ODBC\\ODBCINST.INI\\";

    /// Creates a new DSN entry with the specified values. If the DSN exists, the values are updated.
    public static void CreateDSN(string dsnName, string description, string driverName, string database)
    {
        // Lookup driver path from driver name
        var driverKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + driverName);
        if (driverKey == null) throw new Exception(string.Format("ODBC Registry key for driver '{0}' does not exist", driverName));
        string driverPath = driverKey.GetValue("Driver").ToString();

        // Add value to odbc data sources
        var datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
        if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
        datasourcesKey.SetValue(dsnName, driverName);

        // Create new key in odbc.ini with dsn name and add values
        var dsnKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + dsnName);
        if (dsnKey == null) throw new Exception("ODBC Registry key for DSN was not created");
		dsnKey.SetValue("AutoDoubleQuote", "0");
        dsnKey.SetValue("DBQ", database.ToUpper());
        dsnKey.SetValue("Description", description);
        dsnKey.SetValue("Driver", driverPath);                               
		dsnKey.SetValue("OpenMode", "0");
		dsnKey.SetValue("LastUser", Environment.UserName);		
    }

    /// Removes a DSN entry
    public static void RemoveDSN(string dsnName)
    {
        // Remove DSN key
        Registry.LocalMachine.DeleteSubKeyTree(ODBC_INI_REG_PATH + dsnName);

        // Remove DSN name from values list in ODBC Data Sources key
        var datasourcesKey = Registry.LocalMachine.CreateSubKey(ODBC_INI_REG_PATH + "ODBC Data Sources");
        if (datasourcesKey == null) throw new Exception("ODBC Registry key for datasources does not exist");
        datasourcesKey.DeleteValue(dsnName);
    }

    /// Checks the registry to see if a DSN exists with the specified name
    public static bool DSNExists(string dsnName)
    {
        var driversKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + "ODBC Drivers");
        if (driversKey == null) throw new Exception("ODBC Registry key for drivers does not exist");

        return driversKey.GetValue(dsnName) != null;
    }

    /// Returns an array of driver names installed on the system
    public static string[] GetInstalledDrivers()
    {
        var driversKey = Registry.LocalMachine.CreateSubKey(ODBCINST_INI_REG_PATH + "ODBC Drivers");
        if (driversKey == null) throw new Exception("ODBC Registry key for drivers does not exist");

        var driverNames = driversKey.GetValueNames();

        var ret = new List<string>();

        foreach (var driverName in driverNames)
        {
            if (driverName != "(Default)")
            {
                ret.Add(driverName);
            }
        }

        return ret.ToArray();
    }
}

// Add-OdbcDsn -Name "TestDSN" -DriverName "Pervasive ODBC Engine Interface" -DsnType "System" -Platform "32-bit" -SetPropertyValue @("Name=TestDB","Description=","DataPath=C:\Creative\Manager\Userdata\","DdfPath=C:\Creative\Manager\Userdata\")
[DllImport("ODBCCP32.dll")]
private static extern bool SQLConfigDataSource(IntPtr hwndParent, int fRequest, string lpszDriver, string lpszAttributes);

[DllImport("ODBCCP32.dll")]
private static extern  bool SQLInstallerError(int iError, ref int pfErrorCode, StringBuilder lpszErrorMsg, int cbErrorMsgMax, ref int pcbErrorMsg);