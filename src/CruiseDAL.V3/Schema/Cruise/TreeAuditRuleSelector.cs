namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TreeAuditRuleSelector =
            "CREATE TABLE TreeAuditRuleSelector (" +
                "TreeDefaultValue_TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE," +
                "SpeciesCode TEXT COLLATE NOCASE, " +
                "LiveDead TEXT COLLATE NOCASE, " +
                "PrimaryProduct TEXT COLLATE NOCASE, " +
                "TreeAuditRuleID TEXT NOT NULL, " +

                "CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL)," +

                "FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE," +
                "FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE, " +
                "FOREIGN KEY (SpeciesCode) REFERENCES Species (SpeciesCode) ON UPDATE CASCADE" +
            ");";

        public const string CREATE_UNIQUE_INDEX_TreeAuditRuleSelector_SpeciesCode_LiveDead_PrimaryProduct_TreeAuditRuleID_CruiseID =
    @"CREATE UNIQUE INDEX TreeAuditRuleSelector_SpeciesCode_LiveDead_PrimaryProduct_TreeAuditRuleID_CruiseID 
ON TreeAuditRuleSelector (
    CruiseID, 
    ifnull(SpeciesCode, ''), 
    ifnull(LiveDead, ''), 
    ifnull(PrimaryProduct, ''), 
    TreeAuditRuleID
);";

        public const string CREATE_INDEX_TreeAuditRuleSelector_SpeciesCode =
            @"CREATE INDEX 'TreeAuditRuleSelector_SpeciesCode' ON 'TreeAuditRuleSelector'('SpeciesCode');";

        public const string CREATE_INDEX_TreeAuditRuleSelector_TreeAuditRuleID =
            @"CREATE INDEX 'TreeAuditRuleSelector_TreeAuditRuleID' ON 'TreeAuditRuleSelector'('TreeAuditRuleID');";

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