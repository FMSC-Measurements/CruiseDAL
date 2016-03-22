using FMSC.ORM.EntityModel.Support;
using System;
using System.Reflection;


namespace FMSC.ORM.EntityModel.Attributes
{
    public enum SepcialFieldType { None = 0, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RowVersion };

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldAttribute : BaseFieldAttribute
    {       
        public string SourceName { get; set; }
        public string Alias { get; set; }
        public string SQLExpression { get; set; }
        public virtual object DefaultValue { get; set; }
        //public bool IsDepreciated { get; set; }

        internal bool IsGuid;

        internal string SQLPramName { get { return "@" + Name.ToLower();  } }

        public string NameOrAlias { get { return Name ?? Alias; } }


        public FieldAttribute()
        {
            PersistanceFlags = PersistanceFlags.Always;
        }

        public FieldAttribute(string fieldName) : this()
        {
            this.Name = fieldName;
        }

        public override string GetResultColumnExpression()
        {
            if (!string.IsNullOrEmpty(SQLExpression))
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(Alias));
                return SQLExpression + " AS " + Alias;
            }
            else if (Alias != null)
            {
                return Alias;
            }
            else
            {
                if(string.IsNullOrEmpty(SourceName)) { return Name; }
                else { return SourceName + "." + Name; }
            }
        }

        //public object GetFieldValue(Object obj)
        //{
        //    object value = Property.GetValue(obj);
        //    if (RunTimeType.IsEnum)
        //    {
        //        value = value.ToString();
        //    }
        //    else if (IsGuid)
        //    {
        //        value = value.ToString();
        //    }
        //    return value;
        //}


        public object GetFieldValueOrDefault(Object obj)
        {
            return GetFieldValue(obj) ?? DefaultValue;
        }

        //public void SetFieldValue(Object dataObject, object value)
        //{
        //    try
        //    {
        //        Property.SetValue(dataObject, value);
        //    }
        //    catch(Exception e)
        //    {
        //        throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, this), e);
        //    }
        //}


        public void SetFieldValueOrDefault(Object dataObject, object value)
        {
            SetFieldValue(dataObject, value ?? DefaultValue);
        }

        public override string ToString()
        {
            return String.Format("EntityFieldInfo RunTimeType({0}), FieldName({1}), Expression({2})",
                 RunTimeType, Name ?? "null", SQLExpression ?? "null");
        }
    }
}
