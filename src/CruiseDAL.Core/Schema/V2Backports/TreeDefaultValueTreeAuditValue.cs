namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEDEFAULTVALUETREEAUDITVALUE =
            "CREATE VIEW TreeDefaultValueTreeAuditValue AS " +
            "SELECT " +
                "tdv.TreeDefaultValue_CN, " +
                "tav.TreeAuditValue_CN " +
            "FROM TreeDefaultValue_TreeAuditValue tdvtav " +
            "JOIN TreeDefaultValue AS tdv USING (Species, LiveDead, PrimaryProduct) " +
            "JOIN TreeAuditValue AS tav USING (TreeAuditValueID)";
    }
}