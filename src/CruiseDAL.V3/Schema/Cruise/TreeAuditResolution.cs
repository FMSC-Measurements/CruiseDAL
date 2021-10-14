using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeAuditResolutionTableDefinition : ITableDefinition
    {
        public string TableName => "TreeAuditResolution";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    TreeAuditResolution_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,    -- description indicating the reason of the resolution. optional
    Initials TEXT NOT NULL, -- initials of the cruiser that resolved the error.

    UNIQUE (TreeID, TreeAuditRuleID),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeAuditResolution_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,
    Initials TEXT NOT NULL
);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_TreeAuditResolution_TreeAuditRuleID ON TreeAuditResolution ('TreeAuditRuleID');

CREATE INDEX NIX_TreeAuditResolution_TreeID ON TreeAuditResolution ('TreeID');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeAuditResolution_OnDelete };

        public const string CREATE_TRIGGER_TreeAuditResolution_OnDelete =
@"CREATE TRIGGER TreeAuditResolution_OnDelete
BEFORE DELETE ON TreeAuditResolution
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO TreeAuditResolution_Tombstone (
        CruiseID,
        TreeID,
        TreeAuditRuleID,
        Resolution,
        Initials
    ) VALUES (
        OLD.CruiseID,
        OLD.TreeID,
        OLD.TreeAuditRuleID,
        OLD.Resolution,
        OLD.Initials
    );
END;";
    }
}