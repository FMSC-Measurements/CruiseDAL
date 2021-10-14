using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StemTableDefinition : ITableDefinition
    {
        public string TableName => "Stem";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Stem_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StemID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL COLLATE NOCASE,
    Diameter REAL Default 0.0,
    DiameterType TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK(StemID LIKE '________-____-____-____-____________'),

    UNIQUE (StemID),
    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Stem_Tombstone (
    StemID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL COLLATE NOCASE,
    Diameter REAL,
    DiameterType TEXT,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_Stem_Tombstone_TreeID ON Stem_Tombstone (TreeID);
CREATE INDEX NIX_Stem_Tombstone_CruiseID ON Stem_Tombstone (CruiseID);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_Stem_TreeID ON Stem (TreeID);
CREATE INDEX NIX_Stem_CruiseID ON Stem (CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_Stem_ON_UPDATE, CREATE_TRIGGER_Stem_OnDelete };

        public const string CREATE_TRIGGER_Stem_ON_UPDATE =
@"CREATE TRIGGER Stem_On_Update
AFTER UPDATE OF
    TreeID,
    Diameter,
    DiameterType
ON Stem
FOR EACH ROW
BEGIN
    UPDATE Stem SET Modified_TS = CURRENT_TIMESTAMP WHERE Stem_CN = old.Stem_CN;
END;";

        public const string CREATE_TRIGGER_Stem_OnDelete =
@"CREATE TRIGGER Stem_OnDelete
BEFORE DELETE ON Stem
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Stem_Tombstone (
        StemID,
        CruiseID,
        TreeID,
        Diameter,
        DiameterType,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.StemID,
        OLD.CruiseID,
        OLD.TreeID,
        OLD.Diameter,
        OLD.DiameterType,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}