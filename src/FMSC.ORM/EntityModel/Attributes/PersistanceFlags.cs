using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    [Flags]
    public enum PersistanceFlags
    {
        Never = 0
        , OnInsert = 1
        , OnUpdate = 2
        , Always = OnInsert | OnUpdate
    }
}