using System;
using System.IO;

namespace GS1SyncService
{
    public class Logger
    {
        private static string PATH { get; set; }

        public Logger() : this($"{AppDomain.CurrentDomain.BaseDirectory}\\Logger.txt") { }
        public Logger(string LoggerPath) {
            PATH = LoggerPath;
        }

        private static bool WriteToFile(string Type, string Name, string Message) {
            try
            {
                File.AppendAllText(PATH, $"{Type} | {Name} | {Message}\n");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool WriteHR()
        {
            try
            {                
                File.AppendAllText(PATH, $"{new string('-', 50)}\n");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool Write(string Type, string Name, string Message)
        {
            try
            {
                WriteToFile(Type, Name, Message);
                return true;
            }
            catch(Exception ex)
            {
                WriteToFile("ERROR", "Logger", ex.Message);
                WriteToFile(Type, Name, Message);
                return false;
            }
        }

        public static bool WriteInfo(string Name, string Message) {
            return Write("INFO", Name, Message);
        }

        public static bool WriteError(string Name, string Message) {
            return Write("ERROR", Name, Message);
        }
    }
}
