using System;
using System.Reflection;
using FMSC.ORM.Core.EntityModel;

namespace FMSC.ORM.Core.EntityAttributes
{
    public enum SepcialFieldType { None = 0, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RowVersion };

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldAttribute : BaseFieldAttribute
    {
        private int _ordinal = -1;

        public int Ordinal { get { return _ordinal; } set { _ordinal = value; } }

        public string FieldName { get; set; }
        public string SQLExpression { get; set; }
        public PersistanceFlags PersistanceFlags { get; set; }
        public virtual object DefaultValue { get; set; }
        //public bool IsDepreciated { get; set; }

        internal PropertyAccessor Property { get; set; }
        internal Type RunTimeType { get { return Property.RuntimeType; } }       
        internal bool IsGuid;

        internal string SQLPramName { get { return "@" + FieldName.ToLower();  } }


        public FieldAttribute()
        {
            PersistanceFlags = PersistanceFlags.Always;
        }

        public FieldAttribute(string fieldName) : this()
        {
            this.FieldName = fieldName;
        }

        public string GetResultColumnExpression(string sourceName)
        {
            if (!string.IsNullOrEmpty(SQLExpression))
            {
                return SQLExpression + " AS " + sourceName + "." + FieldName;
            }
            else
            {
                return sourceName + "." + FieldName;
            }
        }
        
        
        public object GetFieldValue(Object obj)
        {
            object value = Property.GetValue(obj);
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
            return GetFieldValue(obj) ?? DefaultValue;
        }

        public void SetFieldValue(Object dataObject, object value)
        {
            try
            {
                Property.SetValue(dataObject, value);
            }
            catch(Exception e)
            {
                throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, this), e);
            }
        }


        public void SetFieldValueOrDefault(Object dataObject, object value)
        {
            SetFieldValue(dataObject, value ?? DefaultValue);
        }

        public override string ToString()
        {
            return String.Format("EntityFieldInfo RunTimeType({0}), FieldName({1}), Expression({2})",
                 RunTimeType, FieldName ?? "null", SQLExpression ?? "null");
        }
    }
}
