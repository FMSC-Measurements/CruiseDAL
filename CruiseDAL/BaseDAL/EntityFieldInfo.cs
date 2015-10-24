using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL
{
    public class EntityFieldInfo
    {
        public FieldAttribute _fieldAttr;

        public String FieldName { get { return _fieldAttr.FieldName; } }
        public String Source { get { return _fieldAttr.Source; } }

        public String SQLExpression { get { return _fieldAttr.SQLExpression; } }
        public bool IsPersisted { get { return _fieldAttr.IsPersisted; } }
        public int Ordnal { get { return _fieldAttr.Ordinal; } }

        public readonly Type DataType;
        public readonly MethodInfo Getter;
        public readonly MethodInfo Setter;
        public readonly bool IsGuid;

        public EntityFieldInfo(PropertyInfo prop) : this(prop, new FieldAttribute(prop.Name))
        { }        

        public EntityFieldInfo(PropertyInfo prop, FieldAttribute attr)
        {
            this._fieldAttr = attr;

            this.Getter = prop.GetGetMethod();
            this.Setter = prop.GetSetMethod();
            this.DataType = prop.PropertyType;
            this.IsGuid = this.DataType.Equals(typeof(Guid));
        }

        

        public object GetFieldValue(Object obj)
        {
            object value = Getter.Invoke(obj, null);
            if (DataType.IsEnum)
            {
                value = value.ToString();
            }
            else if (IsGuid)
            {
                value = value.ToString();
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

        public override string ToString()
        {
            return String.Format("EntityFieldInfo DBType({0}), FieldName({1}), Expression({2})",
                 DataType, FieldName ?? "null", SQLExpression ?? "null");
        }
    }
}
