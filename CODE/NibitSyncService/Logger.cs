using System;
using System.IO;

namespace NibitSyncService
{
    public class Logger
    {
        private string PATH { get; set; }

        public Logger() : this($"{AppDomain.CurrentDomain.BaseDirectory}\\errors.txt") { }
        public Logger(string PATH) {
            this.PATH = PATH;
        }

        private bool WriteToFile(string Type, string Name, string Message) {
            try
            {
                File.AppendAllText(this.PATH, $"{Type} - {Name} - {Message}\n");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Write(string Type, string Name, string Message)
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

        public bool WriteInfo(string Name, string Message) {
            return Write("INFO", Name, Message);
        }

        public bool WriteError(string Name, string Message) {
            return Write("ERROR", Name, Message);
        }
    }
}
