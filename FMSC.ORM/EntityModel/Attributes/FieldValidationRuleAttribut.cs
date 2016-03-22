using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldValidationRuleAttribut : Attribute, ORM.EntityModel.IFieldValidator
    {
        int _min = int.MinValue;
        int _max = int.MinValue;

        public string Field { get; set; }
        public string TableName { get; set; }
        public string ErrorMessage { get; set; }
        public ORM.EntityModel.ErrorLevel Level { get; set; }

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }
        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }
        public string[] Values { get; set; }
        public bool NotNull { get; set; }

        

        public bool Validate(object sender, object value)
        {
            bool isValid = true;//valid until proven invalid

            if (this.NotNull == true && value == null) { isValid = false; }

            if (isValid && this.Min != int.MinValue)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num < this.Min) { isValid = false; }
                }
            }
            if (isValid && this.Max != int.MinValue)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num > this.Max) { isValid = false; }
                }
            }

            if (isValid && value != null && Values != null)
            {
                //values doesn't contain value
                if (Array.IndexOf(Values, value.ToString()) < 0 ) { isValid = false; }
            }
            return isValid;
        }
    }
}
