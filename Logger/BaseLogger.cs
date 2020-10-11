using Entities;
using System;
using System.Collections.Generic;

namespace Common
{
    public abstract class BaseLogger : ILogger
    {
        public void Info(string logName, string Format, params object[] Placeholders) {
            this.Info(logName, string.Format(Format, Placeholders));
        }

        public void Info(string logName, string Message) {
            this.Info(logName, Message, new List<string>());
        }

        public void Info(string Message){
            this.Info(string.Empty, Message);
        }

        public void Error(Exception Ex) {
            this.Error(string.Empty, Ex);
        }

        public abstract void Error(string logName, Exception Ex);
        public abstract void Info(string logName, string Message, List<string> Params);        
    }
}
