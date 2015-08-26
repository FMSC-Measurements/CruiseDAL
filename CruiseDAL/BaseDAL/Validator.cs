using System.Collections.Generic;
using System;
namespace CruiseDAL
{
    public enum ErrorLevel { Warning, Error }; 

    public class RowValidator
    {
        public RowValidator()
        {
            this.Constraints = new FieldValidatorCollection();
        }

        //public RowValidator(FieldValidatorCollection constraints)
        //{
        //    if (constraints == null) { throw new ArgumentNullException("constraints"); }
        //    this.Constraints = constraints;
        //}


        public FieldValidatorCollection Constraints { get; protected set; }

        public bool Validate(DataObject sender, string fieldName, object value)
        {
            IFieldValidator constraint = Constraints[fieldName];
            if (constraint == null) { return true; }//nothing to validate, pass
            string message;
            if (constraint.Level == ErrorLevel.Error)
            {
                message = constraint.ErrorMessage;
            }
            else if (constraint.Level == ErrorLevel.Warning)
            {
                message = "Warning::" + constraint.ErrorMessage;
            }
            else
            {
                message = String.Empty;
            }
            if (constraint.Validate(sender, value) == false)//fail validation
            {

                sender.AddError(fieldName, message);
                return false;
            }
            else //passes validation
            {
                sender.RemoveError(fieldName, message);
                return true;
            }

        }

        public void Add(IFieldValidator fv)
        {
            this.Constraints.Add( fv);
        }

        public void AddRange(IEnumerable<IFieldValidator> collection)
        {
            foreach (IFieldValidator fv in collection)
            {
                this.Add(fv);
            }
        }


    }

    public interface IFieldValidator
    {
        string Field { get; }

        string TableName { get; }

        string ErrorMessage { get; }

        ErrorLevel Level { get;}

        bool Validate(object sender, object value);
    }

    public class FieldValidatorCollection 
    {
        private Dictionary<string, IFieldValidator> lookup { get; set; }

        public FieldValidatorCollection()
        {
            lookup = new Dictionary<string, IFieldValidator>();
        }

        public void Add( IFieldValidator fv)
        {
            var fieldName = fv.Field;
            if (!HasConstraint(fieldName)) { lookup.Add(fieldName, fv); }
            if (object.ReferenceEquals(lookup[fieldName], fv)) { return; }

            lookup[fieldName] = fv;
        }


        public IFieldValidator this[string fieldName] 
        {
            get{
                if (HasConstraint(fieldName) == false) { return null; }
                return lookup[fieldName];
            }
            set{
                lookup.Add(fieldName, value);
            }
        }

        public bool HasConstraint(String fieldName)
        {
            return lookup.ContainsKey(fieldName);
        }


    }

    public class FieldValidator : IFieldValidator
    {
        public FieldValidator() { }

        public FieldValidator(string Field, string TableName, string ErrorMessage, 
            double Min, double Max, bool Required, string Values)
            : this()
        {
            this.Field = Field;
            this.TableName = TableName;
            this.ErrorMessage = ErrorMessage;
            this.Min = Min;
            this.Max = Max;
            this.Required = Required;
            this.Values = Values;
        }

        public ErrorLevel Level { get { return ErrorLevel.Error; } }
        

        public string Field
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public double Min { get; set; }

        public double Max { get; set; }

        public bool Required { get; set; }

        public string Values 
        {
            set 
            { 
                if(string.IsNullOrEmpty(value)) { return; }
                this.ValueSet = new List<string>(value.Split(' ')); 
            }            
        }

        public List<String> ValueSet { get; set; }

        public bool Validate(object sender, object value)
        {
            bool isValid = true;//valid until proven unvalid

            if (this.Required == true && value == null) { isValid = false; }

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

            if (isValid && value != null && ValueSet != null)
            {
                if (!ValueSet.Contains(value.ToString())) { isValid = false; }
            }
            return isValid;
        }
    }


    public class NotNullRule : IFieldValidator
    {

        public NotNullRule(String Field, string ErrorString) : this(Field, "", ErrorString) { }


        public NotNullRule(string Field, string TableName, string ErrorString)
        {
            this.Field = Field;
            this.TableName = TableName;
            this.ErrorMessage = ErrorString;
        }

        #region IFieldValidator Members

        public ErrorLevel Level { get { return ErrorLevel.Error; } }
 
        private string _field;
        public string Field
        {
            get { return _field; }
            internal set
            {
                if(_field != null && _field.GetHashCode() != value.GetHashCode())
                { throw new InvalidOperationException("Field can not be modified"); }
                _field = value;
            }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            internal set
            {
                if (_tableName != null && _tableName.GetHashCode() != value.GetHashCode()) 
                { throw new InvalidOperationException("Table Name can't be modified"); }
                _tableName = value;
            }
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool Validate(object value)
        {
            if (value is string)
            {
                return !String.IsNullOrEmpty(value as string);
            }
            return value != null;
        }

        public bool Validate(object sender, object value)
        {
            if (value is string)
            {
                return !String.IsNullOrEmpty(value as string);
            }
            return value != null;
        }

        #endregion
    }
}
