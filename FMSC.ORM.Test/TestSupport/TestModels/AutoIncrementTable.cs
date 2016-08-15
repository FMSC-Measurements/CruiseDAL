using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.TestSupport.TestModels
{
    [EntitySource(SourceName = "AutoIncrementTable")]
    public class AutoIncrementTable
    {
        [PrimaryKeyField(Name = "ID")]
        public long? ID { get; set; }

        [Field(Name = "Data")]
        public string Data { get; set; }
    }
}