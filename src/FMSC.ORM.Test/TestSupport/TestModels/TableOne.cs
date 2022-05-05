using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.TestSupport.TestModels
{
    [Table]
    public class TableOne
    {
        [PrimaryKeyField]
        public int? TableOne_ID { get; set; }
        public string Data { get; set; }
    }
}
