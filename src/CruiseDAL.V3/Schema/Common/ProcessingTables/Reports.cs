namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_REPORTS =
            "CREATE TABLE Reports( " +
                "ReportID TEXT NOT NULL, " +
                "Selected BOOLEAN Default 0, " +
                "Title TEXT, " +
                "UNIQUE (ReportID)" +
            ");";
    }
}