using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldValueTableDefinition : ITableDefinition
    {
        public string TableName => "TreeFieldValue";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    TreeFieldValue_OID INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    ValueInt INTEGER,
    ValueReal REAL,
    ValueBool BOOLEAN,
    ValueText TEXT,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    FOREIGN KEY (Field) REFERENCES TreeField (Field),
    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeFieldValue_Tombstone (
    TreeID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    ValueInt INTEGER,
    ValueReal REAL,
    ValueBool BOOLEAN,
    ValueText TEXT,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_TreeFieldValue_Tombstone_TreeID_Field ON TreeFieldValue_Tombstone
(TreeID, Field);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_TreeFieldValue_TreeID ON TreeFieldValue ('TreeID');

CREATE INDEX NIX_TreeFieldValue_Field ON TreeFieldValue ('Field');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeFieldValue_OnDelete };

        public const string CREATE_TRIGGER_TreeFieldValue_OnUpdate =
@"CREATE TRIGGER TreeFieldValue_OnUpdate
AFTER UPDATE OF
    ValueInt,
    ValueReal,
    ValueBool,
    ValueText
ON TreeFieldValue
FOR EACH ROW
BEGIN
    UPDATE TreeFieldValue SET Modified_TS = CURRENT_TIMESTAMP WHERE TreeFieldValue_OID = old.TreeFieldValue_OID;
END;";

        public const string CREATE_TRIGGER_TreeFieldValue_OnDelete =
@"CREATE TRIGGER TreeFieldValue_OnDelete
BEFORE DELETE ON TreeFieldValue
BEGIN
    INSERT OR REPLACE INTO TreeFieldValue_Tombstone (
        TreeID,
        Field,
        ValueInt,
        ValueReal,
        ValueBool,
        ValueText,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.TreeID,
        OLD.Field,
        OLD.ValueInt,
        OLD.ValueReal,
        OLD.ValueBool,
        OLD.ValueText,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}