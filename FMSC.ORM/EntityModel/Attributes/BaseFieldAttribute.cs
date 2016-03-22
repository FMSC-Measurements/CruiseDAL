using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FMSC.ORM.EntityModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public abstract class BaseFieldAttribute : Attribute
    {
        private int _ordinal = -1;

        public string Name { get; set; }
        public int Ordinal { get { return _ordinal; } set { _ordinal = value; } }
        public PersistanceFlags PersistanceFlags { get; set; }
        internal PropertyAccessor Property { get; set; }
        internal Type RunTimeType { get { return Property.RuntimeType; } }

        public abstract string GetResultColumnExpression();

        public object GetFieldValue(Object obj)
        {
            object value = Property.GetValue(obj);
            return value;
        }

        public void SetFieldValue(Object dataObject, object value)
        {
            try
            {
                Property.SetValue(dataObject, value);
            }
            catch (Exception e)
            {
                throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, this), e);
            }
        }
    }
}
