using FMSC.ORM.EntityModel.Attributes;
using System.Collections.Generic;

namespace FMSC.ORM.EntityModel.Support
{
    public interface IFieldInfoCollection : IEnumerable<FieldInfo>
    {
        FieldInfo PrimaryKeyField { get; }

        IEnumerable<FieldInfo> DataFields { get; }

        //IEnumerable<FieldInfo> GetPersistedFields(bool includeKeyField, PersistanceFlags filter);
    }
}