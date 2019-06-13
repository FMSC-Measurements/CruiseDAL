using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class FieldInfo
    {
        public string FieldName { get; set; }

        public bool IsPK { get; set; }

        //public bool IsAutoIncr { get; set; }

        public bool NotNull { get; set; }

        public Type RuntimeTimeType { get; set; }
    }
}
