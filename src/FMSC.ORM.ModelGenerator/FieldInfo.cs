using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class FieldInfo
    {
        [Field("name")]
        public string FieldName { get; set; }

        [Field("pk")]
        public bool IsPK { get; set; }

        [Field("IsFK")]
        public bool IsFK { get; set; }

        [Field("fkReferences")]
        public string References { get; set; }

        [Field("notnull")]
        public bool NotNull { get; set; }

        [Field("type")]
        public string DbType { get; set; }

    }
}
