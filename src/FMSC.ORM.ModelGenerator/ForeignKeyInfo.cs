using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class ForeignKeyInfo
    {
        [Field]
        public string Table { get; set; }
        [Field]
        public string FromFieldNames { get; set; }
        [Field]
        public string ToFieldNames { get; set; }
    }
}
