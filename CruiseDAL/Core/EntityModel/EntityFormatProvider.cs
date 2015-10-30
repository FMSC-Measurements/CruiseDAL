using System;

namespace CruiseDAL.Core.EntityModel
{
    public class EntityFormatProvider : IFormatProvider
    {
        private DatastoreBase _dataStore;

        public EntityFormatProvider(DatastoreBase dataStore)
        {
            _dataStore = dataStore;
        }

        public object GetFormat(Type formatType)
        {
            EntityDescription description = DatastoreBase.GetObjectDiscription(formatType);
            return new EntityFormatter(description);
        }
    }
}
