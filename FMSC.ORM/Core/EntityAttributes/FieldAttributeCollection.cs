using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FMSC.ORM.Core.EntityModel;

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
                return GetField(value, false);
            }
        }

        public void AddField(PropertyAccessor p)
        {
            var field = new FieldAttribute(p.Name);
            field.Property = p;
            this.AddField(field);
        }

        public void AddField(FieldAttribute field)
        {
            Debug.Assert(field != null);
            Debug.Assert(field.Property != null);

            
            
            if (field is PrimaryKeyFieldAttribute)
            {
                if(PrimaryKeyField != null) { throw new InvalidOperationException("Primary Key field already set"); }
                PrimaryKeyField = (PrimaryKeyFieldAttribute)field;
            }


            if (field.Property.CanRead == false
                || field.Property.CanWrite == false)
            {
                throw new FieldAccesabilityException(field.FieldName, !field.Property.CanRead, !field.Property.CanWrite);
            }

            _fields.Add(field.FieldName, field);
            
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

        public FieldAttribute GetField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            { throw new ArgumentException("fieldName"); }

            if (_fields.ContainsKey(fieldName))
            {
                return _fields[fieldName];
            }
            else
            {
                return null;
            }
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
