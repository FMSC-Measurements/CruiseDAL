using System;

namespace FMSC.ORM.Core.EntityModel
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
