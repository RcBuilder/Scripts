using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class Transaction
    {
        public int TrsNo { get; set; }
        public string accNo { get; set; }
        public string Asmac1 { get; set; }
        public double Sum { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}