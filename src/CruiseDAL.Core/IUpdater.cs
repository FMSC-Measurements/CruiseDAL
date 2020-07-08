using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL
{
    public interface IUpdater
    {
        void Update(CruiseDatastore datastore);
    }
}
