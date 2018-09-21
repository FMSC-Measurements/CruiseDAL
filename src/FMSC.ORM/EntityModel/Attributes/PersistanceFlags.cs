using System;

namespace FMSC.ORM.EntityModel.Attributes
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