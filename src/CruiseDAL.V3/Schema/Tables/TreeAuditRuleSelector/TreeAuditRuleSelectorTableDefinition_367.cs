using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Tables.TreeAuditRuleSelector
{
    public class TreeAuditRuleSelectorTableDefinition_367 : ITableDefinition
    {
        public string TableName => "TreeAuditRuleSelector";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    TreeAuditRuleSelector_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    TreeAuditRuleID TEXT NOT NULL,

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID, CruiseID) REFERENCES TreeAuditRule (TreeAuditRuleID, CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE,
    FOREIGN KEY (PrimaryProduct) REFERENCES LK_Product (Product)
);";
        }

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

        public IEnumerable<string> CreateTriggers => new[] {
            CREATE_TRIGGER_TreeAuditRuleSelector_OnDelete,
            CREATE_TRIGGER_TreeAuditRuleSelector_OnInsert_ClearTombstone,
        };

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

        public const string CREATE_TRIGGER_TreeAuditRuleSelector_OnInsert_ClearTombstone =
@"CREATE TRIGGER TreeAuditRuleSelector_OnInsert_ClearTombstone 
AFTER INSERT ON TreeAuditRuleSelector
BEGIN
    DELETE FROM TreeAuditRuleSelector_Tombstone 
        WHERE CruiseID = NEW.CruiseID
        AND ifnull(SpeciesCode, '') = ifnull(NEW.SpeciesCode, '')
        AND ifnull(LiveDead, '') = ifnull(NEW.LiveDead, '')
        AND ifnull(PrimaryProduct, '') = ifnull(NEW.PrimaryProduct, '');
END;
";
    }
}
