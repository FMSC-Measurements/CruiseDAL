using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core
{
    public interface IDatastoreBuilder
    {
        void CreateDatastore(Datastore datastore);
    }
}
