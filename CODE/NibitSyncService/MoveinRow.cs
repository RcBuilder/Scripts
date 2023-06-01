using Newtonsoft.Json;
using System;

namespace NibitSyncService
{
    public class MoveinRow
    {
        [JsonProperty(PropertyName = "Column2")]
        public string TypeId { get; set; }  // קוד סוג תנועה

        [JsonProperty(PropertyName = "Column3")]
        public string Asmac1 { get; set; } // אסמכתא 1

        [JsonProperty(PropertyName = "Column4")]
        public string Asmac2 { get; set; } // אסמכתא 2

        [JsonProperty(PropertyName = "Column5")]
        public string sAsmacDate { get; set; } // תאריך אסמכתא
        public DateTime AsmacDate {
            get {
                DateTime converted;
                return DateTime.TryParse(this.sAsmacDate.Insert(2, "-").Insert(5, "-"), out converted) ? converted : DateTime.Now;
            }
        }

        [JsonProperty(PropertyName = "Column6")]
        public string sValueDate { get; set; } // תעריך ערך
        public DateTime ValueDate {
            get {
                DateTime converted;
                return DateTime.TryParse(this.sValueDate.Insert(2, "-").Insert(5, "-"), out converted) ? converted : DateTime.Now;
            }
        }

        [JsonProperty(PropertyName = "Column7")]
        public string CurrencyCode { get; set; } // קוד מטבע

        [JsonProperty(PropertyName = "Column8")]
        public string Details { get; set; } // פרטים

        [JsonProperty(PropertyName = "Column9")]
        public string sAccountDebit1 { get; set; } // חשבון חובה 1
        public int AccountDebit1 {
            get {
                int converted;
                return int.TryParse(this.sAccountDebit1, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column10")]
        public string sAccountDebit2 { get; set; } // חשבון חובה 2
        public int AccountDebit2 {
            get {
                int converted;
                return int.TryParse(this.sAccountDebit2, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column11")]
        public string sAccountCrebit1 { get; set; } // חשבון זכות 1
        public int AccountCrebit1 {
            get {
                int converted;
                return int.TryParse(this.sAccountCrebit1, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column12")]
        public string sAccountCrebit2 { get; set; } // חשבון זכות 2
        public int AccountCrebit2 {
            get {
                int converted;
                return int.TryParse(this.sAccountCrebit2, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column13")]
        public string sSumDebit1 { get; set; } // סכום חובה 1
        public float SumDebit1 { 
            get {
                float converted;
                return float.TryParse(this.sSumDebit1, out converted) ? converted : 0;
            } 
        }

        [JsonProperty(PropertyName = "Column14")]
        public string sSumDebit2 { get; set; } // סכום חובה 2
        public float SumDebit2 {
            get {
                float converted;
                return float.TryParse(this.sSumDebit2, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column15")]
        public string sSumCrebit1 { get; set; } // סכום זכות 1
        public float SumCrebit1 {
            get {
                float converted;
                return float.TryParse(this.sSumCrebit1, out converted) ? converted : 0;
            }
        }

        [JsonProperty(PropertyName = "Column16")]
        public string sSumCrebit2 { get; set; } // סכום זכות 2
        public float SumCrebit2 {
            get {
                float converted;
                return float.TryParse(this.sSumCrebit2, out converted) ? converted : 0;
            }
        }
    }
}
