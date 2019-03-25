using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDVALUE =
@"CREATE TABLE TreeFieldValue (
    TreeID TEXT NOTNULL,
    Field TEXT NOTNULL COLLATE NOCASE,
    ValueInt INTEGER, 
    ValueReal REAL, 
    ValueBool BOOLEAN, 
    ValueText TEXT,
    CreatedDate DateTime DEFAULT(datetime('now', 'localtime')) , 
    FOREIGN KEY (Field) REFERENCES TreeField (Field),
    FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE 
);";
    }
}
