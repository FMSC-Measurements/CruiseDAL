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
        //public PropertyInfo _propInfo;
        public readonly String FieldName;
        public FieldAttribute _fieldAttr;
        public readonly Type DataType;
        public int _ordinal = -1; //cached dataReader ordinal
        public readonly MethodInfo Getter;
        public readonly MethodInfo Setter;
        public readonly bool IsGuid;

        public EntityFieldInfo(PropertyInfo prop)
        {
            this.FieldName = prop.Name;
            this.Getter = prop.GetGetMethod();
            this.Setter = prop.GetSetMethod();
            this.DataType = prop.PropertyType;
            this.IsGuid = this.DataType.Equals(typeof(Guid));
        }

        public EntityFieldInfo(PropertyInfo prop, FieldAttribute attr) 
            : this(prop)
        {
            this._fieldAttr = attr;
            this.FieldName = attr.FieldName;

            this.Getter = prop.GetGetMethod();
            this.Setter = prop.GetSetMethod();
            this.DataType = prop.PropertyType;
            this.IsGuid = this.DataType.Equals(typeof(Guid));
        }

        public override string ToString()
        {
            return String.Format("PropType({0}); DBType({1}), FieldName({2}), Alias({3}), MapExpression({4})",
                DataType.Name, _fieldAttr.DataType, _fieldAttr.FieldName ?? "null", _fieldAttr.Alias ?? "null", _fieldAttr.MapExpression ?? "null");
        }

        public object GetFieldValue( Object obj)
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
    }
}
