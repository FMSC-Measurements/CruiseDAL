using FMSC.ORM.Core;
using System;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityFormatProvider : IFormatProvider
    {
        private DatastoreRedux _dataStore;

        public EntityFormatProvider(DatastoreRedux dataStore)
        {
            _dataStore = dataStore;
        }

        public object GetFormat(Type formatType)
        {
            EntityDescription description = DatastoreRedux.LookUpEntityByType(formatType);
            return new EntityFormatter(description);
        }
    }
}