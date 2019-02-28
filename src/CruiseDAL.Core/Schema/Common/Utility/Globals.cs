using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_GLOBALS =
            "CREATE TABLE Globals( " +
                "Block TEXT DEFAULT 'Database' COLLATE NOCASE, " +
                "Key TEXT COLLATE NOCASE, " +
                "Value TEXT, " +
                "UNIQUE (Block, Key));";
    }
}
