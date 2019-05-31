namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEDEFAULTVALUE_TREEAUDITVALUE =
            "CREATE TABLE TreeDefaultValue_TreeAuditRule (" +
                "TreeDefaultValue_TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
                "TreeAuditRuleID TEXT NOT NULL, " +
                "FOREIGN KEY (Species, LiveDead, PrimaryProduct) REFERENCES TreeDefaultValue (Species, LiveDead, PrimaryProduct) ON DELETE CASCADE, " +
                "FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE, " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON UPDATE CASCADE" +
            ");";

        public const string CREATE_INDEX_TreeDefaultValue_TreeAuditRule_Species =
            @"CREATE INDEX 'TreeDefaultValue_TreeAuditRule_Species' ON 'TreeDefaultValue_TreeAuditRule'('Species');";

        public const string CREATE_INDEX_TreeDefaultValue_TreeAuditRule_TreeAuditRuleID =
            @"CREATE INDEX 'TreeDefaultValue_TreeAuditRule_TreeAuditRuleID' ON 'TreeDefaultValue_TreeAuditRule'('TreeAuditRuleID');";

        public const string CREATE_INDEX_TreeDefaultValue_TreeAuditRule_Species_LiveDead_PrimaryProduct =
@"CREATE INDEX 'TreeDefaultValue_TreeAuditRule_Species_LiveDead_PrimaryProduct'
ON 'TreeDefaultValue_TreeAuditRule'('Species', 'LiveDead', 'PrimaryProduct');";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEDEFAULTVALUE_TREEAUDITVALUE_FROM_TREEDEFAULTVALUETREEAUDITVALUE =
            "INSERT INTO {0}.TreeDefaultValue_TreeAuditRule ( " +
                    "TreeDefaultValue_TreeAuditRule_CN," +
                    "Species, " +
                    "LiveDead, " +
                    "PrimaryProduct, " +
                    "TreeAuditRuleID " +
                ") " +
                "SELECT " +
                    "tdvtav.RowID AS TreeDefaultValue_TreeAuditValue_CN," +
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