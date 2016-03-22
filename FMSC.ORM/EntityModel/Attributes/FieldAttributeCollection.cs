using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FMSC.ORM.EntityModel.Attributes
{
    public class FieldAttributeCollection : IEnumerable<FieldAttribute>
    {
        public Dictionary<String, FieldAttribute> _fields = new Dictionary<string,FieldAttribute>();

        public PrimaryKeyFieldAttribute PrimaryKeyField { get; set; }

        public FieldAttribute this[string value]
        {
            get
            {
                return GetField(value);
            }
        }

        public void AddField(FieldAttribute field)
        {
            Debug.Assert(field != null);
            Debug.Assert(field.Property != null);

            if (field.Property.CanRead == false
                || field.Property.CanWrite == false)
            {
                throw new FieldAccesabilityException(field.Name, !field.Property.CanRead, !field.Property.CanWrite);
            }

            if (field is PrimaryKeyFieldAttribute)
            {
                if(PrimaryKeyField != null) { throw new InvalidOperationException("Primary Key field already set"); }
                PrimaryKeyField = (PrimaryKeyFieldAttribute)field;
            }
            else
            {
                _fields.Add(field.NameOrAlias, field);
            }

                     
        }

        public IEnumerable<FieldAttribute> GetFields()
        {
            if(PrimaryKeyField != null)
            {
                yield return PrimaryKeyField;
            }

            foreach(var fa in _fields.Values)
            {
                yield return fa;
            }
            
        }

        public IEnumerable<FieldAttribute> GetPersistedFields(bool includeKeyField, PersistanceFlags filter)
        {            
            foreach(FieldAttribute fa in _fields.Values)
            {
                if((fa.PersistanceFlags & filter) == filter)
                {
                    yield return fa;
                }
            }
            
            if(includeKeyField && this.PrimaryKeyField != null)
            {
                yield return this.PrimaryKeyField;
            }
        }

        //public IEnumerable<InfrastructureFieldAttribute> GetInfrastructureFields()
        //{
        //    List<InfrastructureFieldAttribute> fields = new List<InfrastructureFieldAttribute>();
        //    foreach(InfrastructureFieldAttribute fa in _fields.Values)
        //    {
        //        if(fa is InfrastructureFieldAttribute)
        //        {
        //            fields.Add((InfrastructureFieldAttribute)fa);
        //        }
        //    }
        //    return fields;
        //}

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
            return GetFields().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetFields().GetEnumerator();
        }



    }
}
