using Entities;
using System;
using System.Collections.Generic;

namespace Common
{
    public class DBLogger : BaseLogger
    {
        public override void Error(string logName, Exception Ex) {
            throw new NotImplementedException();
        }

        public override void Info(string logName, string Message, List<string> Params) {
            throw new NotImplementedException();
        }
    }
}
