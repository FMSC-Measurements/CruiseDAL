using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FMSC.ORM.Core.EntityAttributes
{
    public class FieldAttributeCollection : IEnumerable<FieldAttribute>
    {
        public Dictionary<String, FieldAttribute> _fields = new Dictionary<string,FieldAttribute>();

        public PrimaryKeyFieldAttribute PrimaryKeyField { get; set; }

        public FieldAttribute this[string value]
        {
            get
            {
                return _fields[value];
            }
        }

        public void AddField(PropertyInfo prop)
        {
            var field = new FieldAttribute(prop.Name);
            this.AddField(prop, field);
        }

        public void AddField(PropertyInfo prop, FieldAttribute field)
        {
            Debug.Assert(field != null);
            Debug.Assert(prop != null);

            field.Getter = prop.GetGetMethod();
            field.Setter = prop.GetSetMethod();
            field.RunTimeType = prop.PropertyType;

            if (field is PrimaryKeyFieldAttribute)
            {
                if(PrimaryKeyField != null) { throw new InvalidOperationException("Primary Key field already set"); }
                PrimaryKeyField = (PrimaryKeyFieldAttribute)field;
            }
            else
            {
                _fields.Add(field.FieldName, field);
            }
        }

        public IEnumerable<FieldAttribute> GetPersistedFields()
        {
            List<FieldAttribute> fields = new List<FieldAttribute>();
            foreach(FieldAttribute fa in _fields.Values)
            {
                if(fa.IsPersisted)
                {
                    fields.Add(fa);
                }
            }
            return fields;
        }

        public IEnumerable<InfrastructureFieldAttribute> GetInfrastructureFields()
        {
            List<InfrastructureFieldAttribute> fields = new List<InfrastructureFieldAttribute>();
            foreach(InfrastructureFieldAttribute fa in _fields.Values)
            {
                if(fa is InfrastructureFieldAttribute)
                {
                    fields.Add((InfrastructureFieldAttribute)fa);
                }
            }
            return fields;
        }

        public IEnumerator<FieldAttribute> GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }



    }
}
