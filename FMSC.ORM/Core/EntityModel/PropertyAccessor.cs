using System;

using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace FMSC.ORM.Core.EntityModel
{
    public class PropertyAccessor
    {
        public string Name      { get; protected set; }

        public Type RuntimeType { get; protected set; }

        public MethodInfo Getter { get; protected set; }
        public MethodInfo Setter { get; protected set; }

        public bool CanRead     { get { return Getter != null; } }
        public bool CanWrite    { get { return Setter != null; } }

        public PropertyAccessor(PropertyInfo prop)
        {
            Name = prop.Name;

            var getMethod = prop.GetGetMethod(true);
            var setMethod = prop.GetSetMethod(true);

            Getter = getMethod;
            Setter = setMethod;

            RuntimeType = prop.PropertyType;
        }

        public object GetValue(object target)
        {
            object value = Getter.Invoke(target, null);
            return value;
        }

        public void SetValue(object target, object value)
        {
            Setter.Invoke(target, new Object[] { value, });
        }

    }
}
