namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEESTIMATE =
            "CREATE VIEW TreeEstimate_V2 AS " +
            "SELECT " +
            "0 AS TreeEstimate_CN, " +
            "0 AS CountTree_CN, " +
            "0 AS KPI, " +
            "null AS CreatedBy, " +
            "null AS CreatedDate, " +
            "null AS ModifiedBy, " +
            "null AS ModifiedDate " +
            "WHERE 0" +
            ";";
    }
}