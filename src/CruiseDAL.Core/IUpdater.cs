using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public interface IUpdater
    {
        void Update(CruiseDatastore datastore);
    }
}
