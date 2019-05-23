namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITRESOLUTION =
@"CREATE TABLE TreeAuditResolution (
    TreeAuditResolution_CN INTEGER PRIMARY KEY AUTOINCREMENT, 
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,
    Initials TEXT,

    UNIQUE (TreeID, TreeAuditRuleID),

    FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE
);";

        public const string CREATE_INDEX_TreeAuditResolution_TreeAuditRuleID =
            @"CREATE INDEX 'TreeAuditResolution_TreeAuditRuleID' ON 'TreeAuditResolution'('TreeAuditRuleID');";

        public const string CREATE_INDEX_TreeAuditResolution_TreeID =
            @"CREATE INDEX 'TreeAuditResolution_TreeID' ON 'TreeAuditResolution'('TreeID');";
    }
}