using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldValueTableDefinition : ITableDefinition
    {
        public string TableName => "TreeFieldValue";

        public string CreateTable =>
@"CREATE TABLE TreeFieldValue (
    TreeID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    ValueInt INTEGER,
    ValueReal REAL,
    ValueBool BOOLEAN,
    ValueText TEXT,
    CreatedDate DateTime DEFAULT(datetime('now', 'localtime')) ,
    FOREIGN KEY (Field) REFERENCES TreeField (Field),
    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeFieldValue_Tombstone (
    TreeID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    ValueInt INTEGER,
    ValueReal REAL,
    ValueBool BOOLEAN,
    ValueText TEXT,
    CreatedDate DateTime
);";

        public string CreateIndexes =>
@"CREATE INDEX 'TreeFieldValue_TreeID' ON 'TreeFieldValue'('TreeID');

CREATE INDEX 'TreeFieldValue_Field' ON 'TreeFieldValue'('Field');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeFieldValue_OnDelete };

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
        CreatedDate
    ) VALUES (
        OLD.TreeID,
        OLD.Field,
        OLD.ValueInt,
        OLD.ValueReal,
        OLD.ValueBool,
        OLD.ValueText,
        OLD.CreatedDate
    );
END;";

    }
}