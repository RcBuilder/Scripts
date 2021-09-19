using System;
using System.Diagnostics;
using System.IO;

namespace Helpers
{
    /*
	    USING
	    -----
	    // TestApp1.exe
	    static void Main(string[] args) {
            Console.WriteLine($"Hello {args[0]} From Test Utility");        
        }
	
	    // TestApp2.exe
	    static void Main(string[] args) {        
            var input = Console.ReadLine();
            Console.WriteLine($"Your Input is '{input}'");
        }
	
	    -
		
	    ProcessManager.Execute($"D:\\TestApp1.exe", "Roby");  // No Output!!
	
	    var processResult1 = ProcessManager.ExecuteAsString($"D:\\TestApp1.exe", "Roby");  
	    Console.WriteLine(processResult1);  // Hello Roby From Test Utility
	
	    var processResult2 = ProcessManager.ExecuteAsStream($"D:\\TestApp1.exe", "Roby");  
	    Console.WriteLine(processResult2);  // Hello Roby From Test Utility
	
	    var processResult3 = ProcessManager.InteractAsString($"D:\\TestApp2.exe", "", "Its Working!");
	    Console.WriteLine(processResult3);  // Your Input is 'Its Working!'	
    */
    class ProcessManager
    {
        public static void Execute(string Command, string Args = null)
        {
            using (var p = new Process())
            {
                SetCommonProperties(p);
                p.StartInfo.FileName = Command;
                p.StartInfo.Arguments = Args ?? string.Empty;
                p.Start();
                p.WaitForExit();
            }
        }

        public static string ExecuteAsString(string Command, string Args = null)
        {
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

        public static Stream ExecuteAsStream(string Command, string Args = null)
        {
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

        public static string InteractAsString(string Command, string Args, string Input)
        {
            using (var p = new Process())
            {
                SetCommonProperties(p);
                p.StartInfo.FileName = Command;
                p.StartInfo.Arguments = Args ?? string.Empty;
                p.Start();

                try
                {
                    using (var stdin = p.StandardInput)
                    {
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

        public static Stream InteractAsStream(string Command, string Args, string Input)
        {
            using (var p = new Process())
            {
                SetCommonProperties(p);
                p.StartInfo.FileName = Command;
                p.StartInfo.Arguments = Args ?? string.Empty;
                p.Start();

                try
                {
                    using (var stdin = p.StandardInput)
                    {
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

        private static void SetCommonProperties(Process p)
        {
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
        }
    }
}
