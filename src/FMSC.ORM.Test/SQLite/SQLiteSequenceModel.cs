using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.SQLite
{
    [EntitySource(SourceName = "sqlite_sequence")]
    public class SQLiteSequenceModel
    {
        [Field(Name = "name")]
        public string Name { get; set; }

        [Field(Name = "seq")]
        public int Seq { get; set; }
    }
}