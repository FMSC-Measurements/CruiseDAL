using System;

using System.Collections.Generic;
using System.Text;
using FMSC.ORM.Core.EntityAttributes;

namespace CruiseDAL.TableModels
{
    public partial class ErrorLogDO
    {
        [PrimaryKeyField(FieldName="RowID")]
        public long? RowID {get; set;}
    }
}
