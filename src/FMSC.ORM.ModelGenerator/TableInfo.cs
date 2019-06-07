using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class TableInfo
    {
        public string TableName { get; set; }

        public FieldInfo PrimaryKeyField { get; set; }

        public IEnumerable<FieldInfo> Fields { get; set; }
    }
}
