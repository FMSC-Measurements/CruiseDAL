using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.EntityAttributes
{
    [Flags]
    public enum PersistanceFlags
    {
        Undefinded = 0
        , Never = 1
        , OnInsert = 2
        , OnUpdate = 4
        , Always = OnInsert | OnUpdate
    }
}
