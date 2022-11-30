using System;
using System.Collections.Generic;

namespace PaymentsService
{
    public class PaymentConfig
    {
        public string UserName { get; set; }
        public string UserKey { get; set; }
        public string Terminal { get; set; }
        public int Version { get; set; }
    }
}