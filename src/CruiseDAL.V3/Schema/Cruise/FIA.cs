using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIA =
@"CREATE TABLE FIA (
    FIA_cn INTEGER PRIMARY KEY AUTOINCREMENT,
    FIACode INTEGER NOT NULL,
    CommonName TEXT NOT NULL,

    UNIQUE (FIACode)
);";
            
    }
}
