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
@"CREATE UNIQUE INDEX TreeAuditRuleSelector_SpeciesCode_LiveDead_PrimaryProduct_TreeAuditRuleID_CruiseID 
ON TreeAuditRuleSelector (
    CruiseID, 
    ifnull(SpeciesCode, ''), 
    ifnull(LiveDead, ''), 
    ifnull(PrimaryProduct, ''), 
    TreeAuditRuleID
);

CREATE INDEX 'TreeAuditRuleSelector_SpeciesCode' ON 'TreeAuditRuleSelector'('SpeciesCode');

CREATE INDEX 'TreeAuditRuleSelector_TreeAuditRuleID' ON 'TreeAuditRuleSelector'('TreeAuditRuleID');";

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
        CruiseID,
        SpeciesCode,
        LiveDead,
        PrimaryProduct,
        TreeAuditRuleID
    );
END;";

    }

    public partial class Migrations
    {
        public const string MIGRATE_TreeAuditRuleSelector_FROM_TREEDEFAULTVALUETREEAUDITVALUE =
            "INSERT INTO {0}.TreeAuditRuleSelector ( " +
                    "CruiseID, " +
                    "SpeciesCode, " +
                    "LiveDead, " +
                    "PrimaryProduct, " +
                    "TreeAuditRuleID " +
                ") " +
                "SELECT " +
                    "'{3}', " +
                    "tdv.Species, " +
                    "tdv.LiveDead, " +
                    "tdv.PrimaryProduct, " +
                    "tar.TreeAuditRuleID " +
                "FROM {1}.TreeDefaultValueTreeAuditValue AS tdvtav " +
                "JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
                "JOIN {0}.TreeAuditRule AS tar ON tdvtav.TreeAuditValue_CN = tar.TreeAuditRule_CN;";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TREEDEFAULTVALUE_TREEAUDITVALUE_FROM_TREEDEFAULTVALUETREEAUDITVALUE =
    //        "INSERT INTO TreeDefaultValue_TreeAuditValue " +
    //        "SELECT " +
    //            "tdvtav.RowID AS TreeDefaultValue_TreeAuditValue_CN," +
    //            "tdv.Species, " +
    //            "tdv.LiveDead, " +
    //            "tdv.PrimaryProduct, " +
    //            "tav.TreeAuditValueID " +
    //        "FROM TreeDefaultValueTreeAuditValue AS tdvtav " +
    //        "JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
    //        "JOIN TreeAuditValue AS tav USING (TreeAuditValue_CN);";
    //}
}