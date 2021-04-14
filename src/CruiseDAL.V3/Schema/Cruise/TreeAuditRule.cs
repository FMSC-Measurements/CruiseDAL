using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeAuditRuleTableDefinition : ITableDefinition
    {
        public string TableName => "TreeAuditRule";

        public string CreateTable =>
@"CREATE TABLE TreeAuditRule (
    TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeAuditRuleID TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Min REAL,
    Max REAL,
    Description TEXT,
    UNIQUE (TreeAuditRuleID),

    CHECK ((Min IS NULL OR Max IS NULL) OR (Min < Max)),
    CHECK (TreeAuditRuleID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeAuditRule_Tombstone ( 
    TreeAuditRuleID TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Min REAL,
    Max REAL,
    Description TEXT
);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_TreeAuditRule_Field ON TreeAuditRule ('Field');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeAuditRule_OnDelete };


        public const string CREATE_TRIGGER_TreeAuditRule_OnDelete =
@"CREATE TRIGGER TreeAuditRule_OnDelete
BEFORE DELETE ON TreeAuditRule
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO TreeAuditRule_Tombstone (
        TreeAuditRuleID,
        CruiseID,
        Field,
        Min,
        Max,
        Description
    ) VALUES (
        OLD.TreeAuditRuleID,
        OLD.CruiseID,
        OLD.Field,
        OLD.Min,
        OLD.Max,
        OLD.Description 
    );
END;";

    }
}