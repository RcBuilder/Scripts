using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zaybu.Compare.Data.BaseClasses;
using Zaybu.Compare;

namespace Zaybu.Compare.Data
{
    public class Simples : EntityBase
    {
        #region Public Properties

        public bool Bool { get; set; }  
        public char Char { get; set; }
        public byte Byte { get; set; }

        public bool? BoolNullable { get; set; }
        public char? CharNullable { get; set; }
        public byte? ByteNullable { get; set; }

        public Int16 Int16 { get; set; }
        public Int32 Int32 { get; set; }
        public Int64 Int64 { get; set; }

        public Int16? Int16Nullable { get; set; }
        public Int32? Int32Nullable { get; set; }
        public Int64? Int64Nullable { get; set; }

        public Single Single { get; set; }
        public Double Double { get; set; }
        public decimal Decimal { get; set; }

        public Single? SingleNullable { get; set; }
        public Double? DoubleNullable { get; set; }
        public decimal? DecimalNullable { get; set; }

        public DateTime Date { get; set; }
        public DateTime? DateNullable { get; set; }

        public string String { get; set; }
        public char[] CharArray { get; set; }
        public byte[] ByteArray { get; set; }

        #endregion

        public Simples()
        {
        }    
    }
}
