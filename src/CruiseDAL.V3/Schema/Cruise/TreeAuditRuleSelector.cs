using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeAuditRuleSelectorTableDefinition : ITableDefinition
    {
        public string TableName => "TreeAuditRuleSelector";

        public string CreateTable =>
@"CREATE TABLE TreeAuditRuleSelector (
    TreeDefaultValue_TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    TreeAuditRuleID TEXT NOT NULL,

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeAuditRuleSelector_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    TreeAuditRuleID TEXT NOT NULL
);";

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX UIX_TreeAuditRuleSelector_SpeciesCode_LiveDead_PrimaryProduct_TreeAuditRuleID_CruiseID
ON TreeAuditRuleSelector (
    CruiseID,
    ifnull(SpeciesCode, ''),
    ifnull(LiveDead, ''),
    ifnull(PrimaryProduct, ''),
    TreeAuditRuleID
);

CREATE INDEX NIX_TreeAuditRuleSelector_SpeciesCode ON TreeAuditRuleSelector ('SpeciesCode');

CREATE INDEX NIX_TreeAuditRuleSelector_TreeAuditRuleID ON TreeAuditRuleSelector ('TreeAuditRuleID');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeAuditRuleSelector_OnDelete };

        public const string CREATE_TRIGGER_TreeAuditRuleSelector_OnDelete =
@"CREATE TRIGGER TreeAuditRuleSelector_OnDelete
BEFORE DELETE ON TreeAuditRuleSelector
BEGIN
    INSERT OR REPLACE INTO TreeAuditRuleSelector_Tombstone (
        CruiseID,
        SpeciesCode,
        LiveDead,
        PrimaryProduct,
        TreeAuditRuleID
    ) VALUES (
        OLD.CruiseID,
        OLD.SpeciesCode,
        OLD.LiveDead,
        OLD.PrimaryProduct,
        OLD.TreeAuditRuleID
    );
END;";
    }
}