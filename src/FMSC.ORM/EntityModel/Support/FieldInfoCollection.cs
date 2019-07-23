using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.EntityModel.Support
{
    public class FieldInfoCollection : IEnumerable<FieldInfo>
    {
        List<FieldInfo> _fields = new List<FieldInfo>();

        public FieldInfo PrimaryKeyField { get; set; }


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

        public IEnumerable<FieldInfo> GetPersistedFields(bool includeKeyField, PersistanceFlags filter)
        {
            foreach (var fi in _fields)
            {
                if ((fi.PersistanceFlags & filter) == filter)
                {
                    yield return fi;
                }
            }

            if (includeKeyField && this.PrimaryKeyField != null)
            {
                yield return this.PrimaryKeyField;
            }
        }

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
