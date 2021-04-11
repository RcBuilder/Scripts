using System;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class NotZeroValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) > 0;
        }
    }

    public class FutureDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var date = Convert.ToDateTime(value);
            return date >= DateTime.Now;
        }
    }

    public class CheckedValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Convert.ToBoolean(value);
        }
    }
}
