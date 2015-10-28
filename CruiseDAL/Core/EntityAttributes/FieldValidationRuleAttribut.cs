using CruiseDAL.Core.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldValidationRuleAttribut : Attribute, IFieldValidator
    {
        public string Field { get; set; }
        public string TableName { get; set; }
        public string ErrorMessage { get; set; }
        public ErrorLevel Level { get; set; }

        public int Min { get; set; }
        public int Max { get; set; }
        public string[] Values { get; set; }
        public bool NotNull { get; set; }

        

        public bool Validate(object sender, object value)
        {
            bool isValid = true;//valid until proven invalid

            if (this.NotNull == true && value == null) { isValid = false; }

            if (isValid && this.Min != double.NaN)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num < this.Min) { isValid = false; }
                }
            }
            if (isValid && this.Max != double.NaN)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num > this.Max) { isValid = false; }
                }
            }

            if (isValid && value != null && Values != null)
            {
                if (!Values.Contains(value.ToString())) { isValid = false; }
            }
            return isValid;
        }
    }
}
