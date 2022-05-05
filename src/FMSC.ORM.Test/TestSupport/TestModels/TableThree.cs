using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.TestSupport.TestModels
{ 
    [Table]
    public class TableThree
    {
        [PrimaryKeyField]
        public int? TableThree_ID { get; set; }
        public int? TableOne_ID { get; set; }
        public string Data { get; set; }
    }
}
