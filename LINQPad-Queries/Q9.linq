<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\Xamarin.Mac\v2.0\Facades\Microsoft.Win32.Registry.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Drawing.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

static Random rnd = new Random();
async Task Main()
{	
	Console.WriteLine("PaymentCreditCardNumOfInstallments".Substring(0, 20));
	
return;
	Console.WriteLine(new DirectoryInfo("C:").FullName);   // C:\ProgramData\LINQPad\Updates50\543
	Console.WriteLine(new DirectoryInfo("C:\\").FullName); // C:\

	bool? b1 = null;
	Console.WriteLine(b1 ?? false);
	
	Console.WriteLine(GetMACAddress());
	Console.WriteLine($"{AppDomain.CurrentDomain.BaseDirectory}Kill.txt");
	Console.WriteLine(KillProcess("Manager"));
	
		
	// read file 
	var folder = new DirectoryInfo(@"C:\Creative");
	var file = folder.GetFiles(@"Manager\Application.ini")?.FirstOrDefault();	
	Console.WriteLine($"{file.FullName}");
	var rows = File.ReadAllLines(file.FullName);
	var row = rows.FirstOrDefault(r => r.StartsWith("ApplicationName"));
	Console.WriteLine(row ?? "NULL");
	
	// ODBC DSN List 
	var dsnList = this.GetOdbcDsnList();
	foreach(var dsn in dsnList)
		Console.WriteLine($"{dsn}");
	
	// using Func to count data
	Func<long, bool> CountBy1HoursFilter = timestamp =>
    {
        var ts = DateTime.Now - new DateTime(timestamp);
        return ts.TotalHours <= 1;
    };

    Func<long, bool> CountBy24HoursFilter = timestamp =>
    {
        var ts = DateTime.Now - new DateTime(timestamp);
        return ts.TotalHours <= 24;
    };

	var latestFailures = new List<long>{
		DateTime.Now.Ticks,	
		DateTime.Now.AddMinutes(-30).Ticks,	
		DateTime.Now.AddHours(-1).Ticks,
		DateTime.Now.AddHours(-2).Ticks,
		DateTime.Now.AddHours(-3).Ticks,
		DateTime.Now.AddDays(-2).Ticks,
	};
    var countBy1Hours = latestFailures.Count(CountBy1HoursFilter);
    var countBy24Hours = latestFailures.Count(CountBy24HoursFilter);
	Console.WriteLine($"Last Hour: {countBy1Hours}");  // 3
	Console.WriteLine($"Last 24 Hours: {countBy24Hours}");	// 5
	

	// string Join
	IEnumerable<int> Ids1 = new List<int>{ 1,2,3 };
	IEnumerable<int> Ids2 = new List<int>();
	IEnumerable<int> Ids3 = null;
	Console.WriteLine("1: " + string.Join(",", Ids1 ?? Enumerable.Empty<int>()));  // 1,2,3
	Console.WriteLine("2: " + string.Join(",", Ids2 ?? Enumerable.Empty<int>()));  // Empty
	Console.WriteLine("3: " + string.Join(",", Ids3 ?? Enumerable.Empty<int>()));  // Empty		
	
	bool value = true;
	Console.WriteLine(value.GetType().Name);
	
	// MD5
	Console.WriteLine(CreateMD5(@"[{""AccountKeyDeb1"":""30001"",""AccountKeyCred1"":""30001"",""Description"":""Some description"",""Referance"":""9000919"",""Ref2"":"""",""TransType"":""הוצ"",""ValueDate"":"""",""DueDate"":"""",""SuFDeb1"":""99.00"",""SuFCred1"":""99.00"",""Quant"":""2"",""Branch"":""1"",""Details"":""Some notes"",""Osek874"":""""}]0387A078B117B645A69DD487D5DBCC8C"));  // A50C3629BD7ED81F2DC1E3379C46D314

	// Remove WhiteSpaces
	var input = " AB C D    EF   ";
	Console.WriteLine(string.Concat(input.Where(c => !Char.IsWhiteSpace(c))));  // ABCDEF
	Console.WriteLine(new Regex(@"\s+").Replace(input, string.Empty));  // ABCDEF	

	Console.WriteLine((float)Math.Round(951.6312, 2));

	Console.WriteLine("99".PadLeft(3, ' '));
	Console.WriteLine("9999".PadLeft(3, ' '));
	return;

	var sDateDB = "2021-12-07 07:27:03.433";
	var dateDB = Convert.ToDateTime(sDateDB);
	Console.WriteLine(dateDB.ToLocalTime());

	Console.WriteLine(((int)DateTime.Now.DayOfWeek) + 1);

	var messages = new string[]{ "message-1", "message-2", "message-3", "message-4" };
	Console.WriteLine(messages[rnd.Next(messages.Length)]);

	Console.WriteLine((float)Math.Round(Convert.ToSingle(3111.8993), 2));

	Console.WriteLine(FromUnixTime(DateTime.Now));
	Console.WriteLine(ToUnixTime(1636309195));  // 07/11/2021 18:19:55

	// encoding
	Console.WriteLine(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("\u05d0\u05e0\u05d0 \u05d1\u05d7\u05e8 \u05de\u05d5\u05e6\u05e8")));	

	// stream 2 string and vice-versa
	var str1 = "Hello World!";
	var stream1 = String2Stream(str1);
	var strCopy = Stream2String(stream1);
	Console.WriteLine(strCopy);
	
	// ---

	// stream copy
	var str2 = "Hello World!";
	var stream2 = String2Stream(str2);
	var ms = new MemoryStream();
	await CopyStream(stream2, ms);
	Console.WriteLine(Stream2String(stream2));
	Console.WriteLine(Stream2String(ms));
	
	// ---
	
	var utilityPath = @"E:\Scripts\Utilities\wk-Html-2-Pdf\wkhtmltopdf.exe";
	
	
	// url to pdf file
	ProcessManager.Execute(utilityPath, "https://google.com D:\\output1.pdf");	
	
	var sDocument = @"
        <style>
            .c1 { color:red; }
        </style>
        <h1 class=""c1"">Header<h1>
        <p>Content</p>
        <p>תוכן בעברית</p>
    ";
	
	// html to pdf stream
	var processResult1 = ProcessManager.InteractAsStream(utilityPath, "--encoding windows-1255 - -", sDocument);		
	await SaveStreamAsFile("D:\\output2.pdf", processResult1);
	
	// uri to pdf stream
	var processResult2 = ProcessManager.InteractAsStream(utilityPath, "https://google.com -", "");		
	await SaveStreamAsFile("D:\\output3.pdf", processResult2);

	var httpImgWrapper = @"
		<style>
			img { width: 100%; }
        </style>
		<img src='https://picsum.photos/id/1011/600/300' />
	";
	var processResult3 = ProcessManager.InteractAsStream(utilityPath, "- -", httpImgWrapper);	
	await SaveStreamAsFile("D:\\output4.pdf", processResult3);
	
	var localImgWrapper = @"
		<style>
			img { width: 100%; }
        </style>
		<img src='C:\Users\RcBuilder\Pictures\Sample Pictures\Koala.jpg' />
	";
	var processResult4 = ProcessManager.InteractAsStream(utilityPath, "--enable-local-file-access - -", localImgWrapper);	
	await SaveStreamAsFile("D:\\output5.pdf", processResult4);			
}

class ProcessManager{	
	public static void Execute(string Command, string Args = null) {            
		using (var p = new Process())
		{
		    SetCommonProperties(p);
		    p.StartInfo.FileName = Command;                
		    p.StartInfo.Arguments = Args ?? string.Empty;
		    p.Start();
		    p.WaitForExit();
		}
	}
	
	public static string ExecuteAsString(string Command, string Args = null) {            
		using (var p = new Process())
		{
		    SetCommonProperties(p);
		    p.StartInfo.FileName = Command;                
		    p.StartInfo.Arguments = Args ?? string.Empty;
		    p.Start();
		    p.WaitForExit();
			
		    return p.StandardOutput.ReadToEnd();
		}
	}
	
	public static Stream ExecuteAsStream(string Command, string Args = null) {            
		using (var p = new Process())
		{
		    SetCommonProperties(p);
		    p.StartInfo.FileName = Command;                
		    p.StartInfo.Arguments = Args ?? string.Empty;
		    p.Start();
		    p.WaitForExit();
			
		    return p.StandardOutput.BaseStream;
		}
	}
	
	public static string InteractAsString(string Command, string Args, string Input) {            
		using (var p = new Process())
		{
		    SetCommonProperties(p);
		    p.StartInfo.FileName = Command;                
		    p.StartInfo.Arguments = Args ?? string.Empty;
		    p.Start();		    
			
		    try
            {
                using(var stdin = p.StandardInput){
	                stdin.AutoFlush = true;
	                stdin.Write(Input);
				};                
				
				var output = p.StandardOutput.ReadToEnd();  
				p.StandardOutput.Close();				
				
				p.WaitForExit();
				return output;
            }
            catch { return null; }
		}
	}
	
	public static Stream InteractAsStream(string Command, string Args, string Input) {            
		using (var p = new Process())
		{
		    SetCommonProperties(p);
		    p.StartInfo.FileName = Command;                
		    p.StartInfo.Arguments = Args ?? string.Empty;
		    p.Start();		    
			
		    try
            {
                using(var stdin = p.StandardInput){
	                stdin.AutoFlush = true;
	                stdin.Write(Input);
				};                
				
				var ms = new MemoryStream();				
				p.StandardOutput.BaseStream.CopyTo(ms);
				p.StandardOutput.Close();
				
				p.WaitForExit();				
				
				ms.Position = 0;
				return ms;		
            }
            catch { return null; }
		}
	}
	
	// --- 
	
	private static void SetCommonProperties(Process p){		
		p.StartInfo.UseShellExecute = false;
	    p.StartInfo.CreateNoWindow = true;
	    p.StartInfo.RedirectStandardInput = true;
	    p.StartInfo.RedirectStandardOutput = true;
	    p.StartInfo.RedirectStandardError = true;	    
	}
}

IEnumerable<string> GetOdbcDsnList()
{
	// using Microsoft.Win32
	var registryPath = @"Software\ODBC\ODBC.INI\ODBC Data Sources";
    var userKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPath);
	var machineKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPath);
	
	var result = new List<string>();
    if (userKey != null)
	result.AddRange(userKey.GetValueNames());
    result.AddRange(machineKey.GetValueNames());
	
	return result;
}

async Task CopyStream(Stream Input, Stream Output) {
    /// Input.CopyTo(Output);            
    await Input.CopyToAsync(Output);

    /*
    var buffer = new byte[32768];
    int read;
    while ((read = await Input.ReadAsync(buffer, 0, buffer.Length)) > 0)            
        Output.Write(buffer, 0, read);
    */
	
	Input.Position = 0;
    Output.Position = 0;
}

async Task SaveStreamAsFile(string FilePath, Stream StreamSource) {
    using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
		await StreamSource.CopyToAsync(fs);
}

Stream String2Stream(string Value) { 
	byte[] bytes = Encoding.ASCII.GetBytes(Value);
	return new MemoryStream(bytes);
}

string Stream2String(Stream Value) { 
	using(var reader = new StreamReader(Value))
		return reader.ReadToEnd();
}


public DateTime ToUnixTime(int Date) {
    // unixTime (seconds count since 1970-01-01)
    return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Date);  
}

public int FromUnixTime(DateTime Date)
{
    // unixTime (seconds count since 1970-01-01)
    var DateUTC = Date.ToUniversalTime();
    var ts = DateUTC - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    return (int)ts.TotalSeconds;  
}

string CreateMD5(string Input)
{
    // using System.Security.Cryptography
    using (var md5 = System.Security.Cryptography.MD5.Create())
    {
        var inputBytes = Encoding.UTF8.GetBytes(Input);
        var hashBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)                    
            sb.Append(hashBytes[i].ToString("x2"));                    
        return sb.ToString();
    }
}

bool IsFileLocked(FileInfo file)
{
	FileStream fileStream = null;
	try {
		fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
	}
	catch{
		return true;
	}
	finally {	
		fileStream?.Close();
	}	
	return false;
}

int KillProcess(string name) {
	try{
		var workers = Process.GetProcessesByName(name);				
		foreach (var worker in workers) {
		     worker.Kill();
		     worker.WaitForExit();
		     worker.Dispose();
		}
		
		return workers.Length;
	}
	catch {		
		return 0;
	}
}

string GetMACAddress(){
	return (
	      from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
	      where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
	      select nic.GetPhysicalAddress().ToString()
	).FirstOrDefault();
}