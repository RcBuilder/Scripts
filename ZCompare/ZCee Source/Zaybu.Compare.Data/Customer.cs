using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zaybu.Compare.Data.BaseClasses;
using Zaybu.Compare;
using System.ComponentModel;

namespace Zaybu.Compare.Data
{
    [Description("Customer Test Class")]
    public class Customer : EntityBase
    {
        #region Public Properties

        public string Name { get; set; }
        public List<RetailOutlet> RetailOutlets { get; set; } 
        [Description("Special Events")]       
        public Dictionary<DateTime, string> SpecialEvents { get; set; }
        public Single Latitude { get; set; }
        public Single Longitude { get; set; }

        #endregion

        public Customer()
        {
            RetailOutlets = new List<RetailOutlet>();
            SpecialEvents = new Dictionary<DateTime, string>();
        }

        public override string ToString()
        {
            return base.ToString();
            //return GetType().Description();
        }

    }
}
