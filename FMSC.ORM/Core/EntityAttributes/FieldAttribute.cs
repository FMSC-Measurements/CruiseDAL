using System;
using System.Reflection;

namespace FMSC.ORM.Core.EntityAttributes
{
    public enum SepcialFieldType { None = 0, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RowVersion };

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        private int _ordinal = -1;
        public int Ordinal { get { return _ordinal; } set { _ordinal = value; } }

        public string FieldName { get; set; }
        public string SQLExpression { get; set; }
        public virtual bool IsPersisted { get; set; }

        public Type RunTimeType { get; set; }
        public MethodInfo Getter { get; set; }
        public MethodInfo Setter { get; set; }
        public bool IsGuid;

        public string SQLPramName { get { return "@" + FieldName.ToLower();  } }

        public virtual object DefaultValue { get; set; }
        //public bool IsDepreciated { get; set; }   
        
        public FieldAttribute()
        { }

        public FieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        

        public object GetFieldValue(Object obj)
        {
            object value = Getter.Invoke(obj, null);
            if (RunTimeType.IsEnum)
            {
                value = value.ToString();
            }
            else if (IsGuid)
            {
                value = value.ToString();
            }
            return value;
        }

        public object GetFieldValueOrDefault(Object obj)
        {
            object value = GetFieldValue(obj);
            if(value == null)
            {
                value = DefaultValue;
            }
            return value;
        }

        public void SetFieldValue(Object dataObject, object value)
        {
            try
            {
                Setter.Invoke(dataObject, new Object[] { value, });
            }
            catch
            {
                throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, this));
            }
        }

        public void SetFieldValueOrDefault(Object dataObject, object value)
        {
            if(value == null) { value = DefaultValue; }
            SetFieldValue(dataObject, value);
        }

        public override string ToString()
        {
            return String.Format("EntityFieldInfo RunTimeType({0}), FieldName({1}), Expression({2})",
                 RunTimeType, FieldName ?? "null", SQLExpression ?? "null");
        }
    }
}
