using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Test.TestSupport.TestModels
{
    [Table(nameof(RowIDAsPrimaryKey))]
    public class RowIDAsPrimaryKey
    {
        public const string CREATE_TABLE_COMMAND = "CREATE TABLE " + nameof(RowIDAsPrimaryKey) + " (StringField TEXT)";

        [PrimaryKeyField(Name = "RowID")]
        public long RowID { get; set; }

        [Field(Name = "StringField")]
        public string StringField { get; set; }
    }
}
