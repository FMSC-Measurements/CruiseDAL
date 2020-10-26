using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FMSC.ORM.EntityModel.Support
{
    public class FieldInfoCollection : IFieldInfoCollection
    {
        private List<FieldInfo> _fields = new List<FieldInfo>();

        public FieldInfo PrimaryKeyField { get; set; }

        public IEnumerable<FieldInfo> DataFields => _fields;

        public FieldInfoCollection()
        {
        }

        public FieldInfoCollection(Type type)
        {
            try
            {
                foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (p.CanWrite == false) { continue; }
                    if (p.GetIndexParameters().Any())
                    { continue; } // if property is index property ignore

                    var propType = p.PropertyType;
                    propType = Nullable.GetUnderlyingType(propType) ?? propType;

                    var typeCode = Type.GetTypeCode(propType);
                    if (typeCode == TypeCode.Object && propType != typeof(Guid)) { continue; }

                    var attr = Attribute.GetCustomAttribute(p, typeof(BaseFieldAttribute))
                        as BaseFieldAttribute;

                    if (attr == null)
                    {
                        var fieldInfo = new FieldInfo(new PropertyAccessor(p));
                        AddField(fieldInfo);
                    }
                    else if (attr is FieldAttribute fAttr)
                    {
                        var fieldInfo = new FieldInfo(fAttr, new PropertyAccessor(p));
                        AddField(fieldInfo);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Error initializing FieldInfoCollection for " + type.Name, e);
            }
        }

        public void AddField(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException("field");

            if (field.Property.CanRead == false
                || field.Property.CanWrite == false)
            {
                throw new FieldAccesabilityException(field.Name, !field.Property.CanRead, !field.Property.CanWrite);
            }

            if (field.IsKeyField)
            {
                if (PrimaryKeyField != null) { throw new InvalidOperationException("Primary Key field already set"); }
                PrimaryKeyField = field;
            }
            else
            {
                _fields.Add(field);
            }
        }

        public IEnumerable<FieldInfo> GetFields()
        {
            if (PrimaryKeyField != null)
            {
                yield return PrimaryKeyField;
            }

            foreach (var fi in _fields)
            {
                yield return fi;
            }
        }

        //public IEnumerable<FieldInfo> GetPersistedFields(bool includeKeyField, PersistanceFlags filter)
        //{
        //    if (includeKeyField && this.PrimaryKeyField != null)
        //    {
        //        yield return this.PrimaryKeyField;
        //    }

        //    foreach (var fi in _fields)
        //    {
        //        if ((fi.PersistanceFlags & filter) == filter)
        //        {
        //            yield return fi;
        //        }
        //    }
        //}

        public IEnumerator<FieldInfo> GetEnumerator()
        {
            return GetFields().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetFields().GetEnumerator();
        }
    }
}