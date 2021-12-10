using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    // [NotZeroValidation(ErrorMessage = "שדה חובה")]
    public class NotZeroValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) > 0;
        }
    }

    //  [PositiveNumberValidation(ErrorMessage = "מספר תשובות מקסימלי לא תקני")]
    public class PositiveNumberValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) >= 0;
        }
    }

    // [FutureDateValidation(ErrorMessage = "תאריך הזמנה לא תקני")]
    public class FutureDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var date = Convert.ToDateTime(value);
            return date >= DateTime.Now.AddMinutes(-5);  // tolerance of 5 minute
        }
    }

    // [NotZeroValidation(ErrorMessage = "שדה חובה - מזהה מסעדה")]
    public class CheckedValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Convert.ToBoolean(value);
        }
    }
}
