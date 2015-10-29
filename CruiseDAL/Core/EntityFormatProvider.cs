using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core
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
