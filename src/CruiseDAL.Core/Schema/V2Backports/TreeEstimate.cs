namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEESTIMATE =
            "CREATE VIEW TreeEstimate AS " +
            "SELECT " +
                "tl.TallyLedger_CN AS TreeEstimate_CN, " +
                "ct.CountTree_CN, " +
                "TallyLedgerID AS TreeEstimate_GUID, " +
                "tl.KPI, " +
                "tl.CreatedBy, " +
                "tl.CreatedDate, " +
                "null AS ModifiedBy, " +
                "null AS ModifiedDate " +
            "FROM TallyLedger AS tl " +
            "JOIN CountTree AS ct " +
            "WHERE tl.KPI > 0 AND IsDeleted = 0" +
            ";";
    }
}