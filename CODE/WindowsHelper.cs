using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Helpers
{
    /*
        // check service status (windows-services)	
	    var serviceName = "PervasiveEngineMonitorService";
	    Console.WriteLine($"'{serviceName}' is {GetServiceStatus(serviceName)}");
	
        ///StartService("PervasiveEngineMonitorService");
	    ///StopService("PervasiveEngineMonitorService");

        // ---

        // kill a process (all workers)
        var killCount = KillProcess("Manager");

	    // ---
	
	    // execute a process	
	    try{
		    var result = ExecuteProcess("D:\\1.bat", "");
		    Console.WriteLine(result);
	    }
	    catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
	
	    // ---
	
	    // execute a CMD commands
	    try{
		    var result = ExecuteCMDCommand("echo HELLO WORLD");
		    Console.WriteLine(result);
	    }
	    catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
	
	    // ---
	
	    // get internal ip address
	    try{
		    var result = GetInternalIP();
		    Console.WriteLine(result);
	    }
	    catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
	
	    // ---
	
	    // get external ip address
	    try{
		    var result = GetExternalIP();
		    Console.WriteLine(result);
	    }
	    catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
	
	    // ---
	
	    // write to event log
	    WriteEventLog("Test Log 1!");	
	    WriteEventLog("Test Log 2!");
	
	    // --
	
	    // check if file is locked 
	    Console.WriteLine(IsFileLocked(new FileInfo("D:\\tbl1.csv")) ? "LOCKED" : "OK");
	
	    // --
	
	    // copy a folder
	    CopyFolder(@"D:\TEMP\F1", @"D:\TEMP\F2");
	
	    // --
	
	    // get folder size and file count
	    var metrics = GetFolderMetrics(@"D:\TEMP\F1");
	    Console.WriteLine($"{metrics.Count} files, {metrics.SizeBytes} bytes");
	
	    // --
	
	    // check if process is running
	    Console.WriteLine(isProcessRunning("Chrome"));
	    Console.WriteLine(isProcessRunning("Postman"));
	    Console.WriteLine(isProcessRunning("Test"));
	
	    // --
	
	    // check is current time within the provided range
	    // usually used for checking working hours
	    Console.WriteLine(IsWithinTimeRange("13:00-01:00"));
	    Console.WriteLine(IsWithinTimeRange("13:00-16:00"));
	    Console.WriteLine(IsWithinTimeRange("23:00-01:00"));

        // --

        // computer name
	    Console.WriteLine(GetMachineName());

        // --

        var dsnList = this.GetOdbcDsnList();
	    foreach(var dsn in dsnList)
		    Console.WriteLine(dsn);

        var dsnCN1234 = this.FindOdbcDsnByPrefix("CN1234")?.FirstOrDefault();
        Console.WriteLine(dsnCN1234);
    */

    public class WindowsHelper
    {
        // HH:mm-HH:mm (e.g: 05:00-18:00)
        public static bool IsWithinTimeRange(string sTimeRange)
        {
            var partsStart = sTimeRange.Split('-')[0].Split(':');
            var partsEnd = sTimeRange.Split('-')[1].Split(':');

            var start = new TimeSpan(Convert.ToInt32(partsStart[0]), Convert.ToInt32(partsStart[1]), 0);
            var end = new TimeSpan(Convert.ToInt32(partsEnd[0]), Convert.ToInt32(partsEnd[1]), 0);
            var now = DateTime.Now.TimeOfDay;

            var isAlternateMode = Convert.ToInt32(partsEnd[0]) < Convert.ToInt32(partsStart[0]);

            return isAlternateMode ? (now >= start || now <= end) : (now >= start && now <= end);
        }

        public static bool isProcessRunning(string ProcessName)
        {
            var processes = Process.GetProcessesByName(ProcessName);
            return processes.Length > 0;
        }

        public static (int Count, long SizeBytes) GetFolderMetrics(string Source)
        {
            var size = 0L;
            var count = 0;

            var files = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                count++;
                size += new FileInfo(file).Length;
            }
            return (count, size);
        }

        public static void CopyFolder(string Source, string Dest, bool IncludeSubFolders = true)
        {
            var sourceFolder = new DirectoryInfo(Source);
            if (!sourceFolder.Exists)
                throw new DirectoryNotFoundException($"Source folder '{Source}' does not exist");

            if (!Directory.Exists(Dest))
                Directory.CreateDirectory(Dest);

            foreach (var file in sourceFolder.GetFiles())
                CopyFile(file.FullName, Path.Combine(Dest, file.Name), true);

            if (IncludeSubFolders)
            {
                var subFolders = sourceFolder.GetDirectories();
                foreach (var subFolder in subFolders)
                    CopyFolder(subFolder.FullName, Path.Combine(Dest, subFolder.Name), IncludeSubFolders);
            }
        }

        public static void CopyFile(string Source, string Dest, bool Override = true)
        {
            File.Copy(Source, Dest, Override);
            while (!File.Exists(Dest)) Thread.Sleep(10);
        }

        public static void MoveFile(string Source, string Dest, bool Override = true)
        {
            if (Override && File.Exists(Dest)) DeleteFile(Dest);

            File.Move(Source, Dest);
            while (!File.Exists(Dest)) Thread.Sleep(10);
        }

        public static void DeleteFile(string Source)
        {
            File.Delete(Source);            
        }

        public static bool IsFileLocked(FileInfo File)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (fileStream != null) fileStream.Close();
            }
        }

        /// EventViewer
        public static bool WriteEventLog(string Message, string Source = "DefaultSource")
        {
            try
            {
                var eventLog = new EventLog();
                eventLog.Source = Source;
                eventLog.WriteEntry(Message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ExecuteProcess(string FileName, string Arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    WorkingDirectory = new FileInfo(FileName).DirectoryName,
                    FileName = FileName,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Dispose();

            return result;
        }

        public static string ExecuteCMDCommand(string Command)
        {
            return ExecuteProcess("C:\\Windows\\system32\\cmd.exe", $"/C {Command}");
        }

        public static int KillProcess(string ProcessName)
        {
            try
            {
                var workers = Process.GetProcessesByName(ProcessName);
                foreach (var worker in workers)
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return workers.Length;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetServiceStatus(string ServiceName)
        {
            try
            {
                var sc = new ServiceController(ServiceName);

                // Running, Stopped, Paused      
                return sc.Status.ToString();
            }
            catch
            {
                return "NotExists";
            }
        }

        public static bool StartService(string ServiceName)
        {
            try
            {
                var sc = new ServiceController(ServiceName);                
                sc.Start();

                Thread.Sleep(500);
                return true;
            }
            catch { return false; }
        }

        public static bool StopService(string ServiceName)
        {
            try
            {
                var sc = new ServiceController(ServiceName);
                if(sc.CanStop) sc.Stop();

                Thread.Sleep(500);
                return true;
            }
            catch { return false; }
        }

        public static bool ReStartService(string ServiceName)
        {
            try
            {
                WindowsHelper.StopService(ServiceName);                
                Thread.Sleep(2000);
                WindowsHelper.StartService(ServiceName);
                return true;
            }
            catch { return false; }
        }

        public static string GetInternalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            /*
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP)) {
                socket.Connect("8.8.8.8", 65530);
                var iPEndPoint = socket.LocalEndPoint as IPEndPoint;
                return iPEndPoint.Address.ToString();
            }
            */

            return null;
        }
        
        public static string GetExternalIP()
        {
            var provider = "http://icanhazip.com";
            var strIP = new WebClient().DownloadString(provider).Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(strIP)?.ToString();
        }

        public static string GetMachineName() {
            try {
                return Environment.MachineName;
            }
            catch {
                return "UNKNOWN";
            }
        }

        public static void Restart(int DelayInSec = 0) {
            var args = DelayInSec == 0 ? "/r" : $"/r /t {DelayInSec}";
            var process = Process.Start("shutdown", args);            
        }

        public static void Shutdown(int DelayInSec = 0) {
            var args = DelayInSec == 0 ? "/s" : $"/s /t {DelayInSec}";
            var process = Process.Start("shutdown", args);
        }

        public static string GetAssemblyPath() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static IEnumerable<string> GetOdbcDsnList()
        {
            /*
                (powershell) 
                > Get-OdbcDsn
                > Get-OdbcDsn -Name "cn31179App*"  // e.g: cn31179Application
            */

            // using Microsoft.Win32
            var registryPath = @"Software\ODBC\ODBC.INI\ODBC Data Sources";
            var userKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPath);
            var machineKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPath);

            var result = new List<string>();
            if (userKey != null) result.AddRange(userKey.GetValueNames());
            if (machineKey != null) result.AddRange(machineKey.GetValueNames());

            return result?.Distinct();
        }

        public static IEnumerable<string> FindOdbcDsnByPrefix(string Prefix) {
            return GetOdbcDsnList()?.Where(x => x.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetMACAddress()
        {
            return (
                  from nic in NetworkInterface.GetAllNetworkInterfaces()
                  where nic.OperationalStatus == OperationalStatus.Up
                  select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
        }
    }
}
