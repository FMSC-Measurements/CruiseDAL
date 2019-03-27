namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITRESOLUTION =
@"CREATE TABLE TreeAuditResolution (
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,
    Initials TEXT,
    FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE
);";
    }
}