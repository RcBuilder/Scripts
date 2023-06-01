using System;
using System.IO;

namespace AutosoftSyncService
{
    public class Logger
    {
        private static readonly string PATH = $"{AppDomain.CurrentDomain.BaseDirectory}\\AutosoftSyncService.txt";        

        private static bool WriteToFile(string Type, string Name, string Message) {
            try
            {
                File.AppendAllText(PATH, $"{Type} - {Name} - {Message}\n");
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
