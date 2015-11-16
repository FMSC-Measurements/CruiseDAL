using System;

using System.Collections.Generic;
using System.Text;
using FMSC.ORM.Core.EntityAttributes;

namespace CruiseDAL.DataObjects
{
    public partial class ErrorLogDO
    {
        [PrimaryKeyField(FieldName="RowID")]
        public long? MyRowID {get; set;}
    }
}
