namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEDEFAULTVALUETREEAUDITVALUE =
            "CREATE VIEW TreeDefaultValueTreeAuditValue_V2 AS " +
            "SELECT " +
                "tdv.TreeDefaultValue_CN, " +
                "tar.TreeAuditRule_CN AS TreeAuditValue_CN " +
            "FROM TreeDefaultValue_TreeAuditRule tdvtar " +
            "JOIN TreeDefaultValue AS tdv USING (Species, LiveDead, PrimaryProduct) " +
            "JOIN TreeAuditRule AS tar USING (TreeAuditRuleID)";
    }
}