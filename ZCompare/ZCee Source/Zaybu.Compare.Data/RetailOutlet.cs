using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zaybu.Compare.Data.BaseClasses;
using Zaybu.Compare;
using System.ComponentModel;

namespace Zaybu.Compare.Data
{
    public class RetailOutlet : EntityBase
    {
        #region Public Properties
        
        public int? Number { get; set; }
        public string Name { get; set; }
        public int Floors { get; set; }
        public OutletType OutletType { get; set; }        
        public SortedDictionary<Department, string> Departments { get; set;}
        public List<Promotion> Promotions { get; set; }

        #endregion

        public RetailOutlet()
        {
            Departments = new SortedDictionary<Department, string>();
            Promotions = new List<Promotion>();
        }    
        
    }

    public enum OutletType  
    {        
        Online, HighStreet, LeisurePark
    }

    public enum Department
    {
        Electrical, Home, Garden, Travel, Automotive
    }

}
