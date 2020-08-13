using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_Stem =
@"CREATE TABLE Stem (
        Stem_CN INTEGER PRIMARY KEY AUTOINCREMENT,
        StemID TEXT NOT NULL COLLATE NOCASE,
        CruiseID TEXT NOT NULL COLLATE NOCASE,
        TreeID TEXT NOT NULL COLLATE NOCASE,
        Diameter REAL Default 0.0,
        DiameterType TEXT,
        CreatedBy TEXT DEFAULT '',
        CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
        ModifiedBy TEXT ,
        ModifiedDate DateTime ,
        RowVersion INTEGER DEFAULT 0,
        
        CHECK(StemID LIKE '________-____-____-____-____________'),        

        UNIQUE (StemID), 
        FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE 
);";

        public const string CREATE_TRIGGER_Stem_ON_UPDATE =
@"CREATE TRIGGER Stem_On_Update 
AFTER UPDATE OF 
    TreeID, 
    Diameter, 
    DiameterType 
ON Stem 
FOR EACH ROW 
BEGIN 
    UPDATE Stem SET ModifiedDate = datetime('now', 'localtime') WHERE Stem_CN = old.Stem_CN; 
    UPDATE Stem SET RowVersion = RowVersion + 1 WHERE Stem_CN = old.Stem_CN;
END;";
    }
}
