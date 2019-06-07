using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.EntityModel
{
    public interface ISupportInitializeFromDatastore
    {
        void Initialize(Datastore datastore);
    }
}
