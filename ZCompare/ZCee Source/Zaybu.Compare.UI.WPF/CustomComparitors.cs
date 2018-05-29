using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaybu.Compare.Comparitors;
using Zaybu.Compare.Data;
using Zaybu.Compare.Types;

namespace Zaybu.Compare.UI.WPF
{
    public class SupplierCustomComparitor : ComparitorBase<Supplier>
    {
        public override void Compare(Supplier originalObject, Supplier compareToObject, ZCompareResults results)
        {            
            if (originalObject.ID != compareToObject.ID) results.AddResult("Supplier ID's are different", ResultStatus.Changed, originalObject.ID, compareToObject.ID);

            var productResults = ZCompare.Compare(originalObject.Products, compareToObject.Products);

            results.MergeResults(results.Root, productResults.Root);                     
        }
    }

    public class DateTimeComparitor : ComparitorBase<DateTime>
    {
        public override string GetStringValue(DateTime value)
        {
            return String.Format("{0:r}", value);
        }

        public override void Compare(DateTime baseProperty, DateTime compareToProperty, ZCompareResults results)
        {
            if (baseProperty.ToShortDateString() != compareToProperty.ToShortDateString()) results.CurrentResult.SetChanged(typeof(Supplier));
        }
    }
}
