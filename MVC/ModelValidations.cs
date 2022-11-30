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

    // [TagsValidation(3, 30, ErrorMessage = "תגיות לא תיקניות")]
    // e.g: tagA,tagB,tagC,tagD
    public class TagsValidation : ValidationAttribute
    {
        private int minLength { get; set; }
        private int maxLength { get; set; }

        public TagsValidation() : this(3, 30) { }
        public TagsValidation(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            var sTags = value.ToString()?.Split(',')?.Select(x => x.Trim());
            if (sTags != null)
            {
                foreach (var tag in sTags)
                {
                    if (tag.Trim().Length >= this.minLength && tag.Trim().Length <= this.maxLength) continue;
                    return false;
                }
            }
            return true;
        }
    }

    // [PasswordLengthValidation(ErrorMessage = "הסיסמה צריכה להכיל לפחות 6 ספרות")]
    // [PasswordLengthValidation(10, ErrorMessage = "הסיסמה צריכה להכיל לפחות 10 ספרות")]
    public class PasswordLengthValidation : ValidationAttribute
    {
        private int maxLength { get; set; }

        public PasswordLengthValidation() : this(6) { }
        public PasswordLengthValidation(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            string v = (string)value;
            return v.Length >= this.maxLength || v.Length == 0;
        }
    }
}
