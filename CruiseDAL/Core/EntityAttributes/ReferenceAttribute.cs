using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL.EntityAttributes
{
    public class ReferenceAttribute
    {
        public Type EntityType { get; set; }
        public String ReferenceSourceName { get; set; }
        public String ForeignKeyFieldName { get; set; }

    }
}
