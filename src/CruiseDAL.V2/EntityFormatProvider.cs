using FMSC.ORM.Core;
using System;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityFormatProvider : IFormatProvider
    {
        private Datastore _dataStore;

        public EntityFormatProvider(Datastore dataStore)
        {
            _dataStore = dataStore;
        }

        public object GetFormat(Type formatType)
        {
            EntityDescription description = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(formatType);
            return new EntityFormatter(description);
        }
    }
}