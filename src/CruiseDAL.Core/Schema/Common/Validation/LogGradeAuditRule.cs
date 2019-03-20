namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGGRADEAUDITRULE =
            "CREATE TABLE LogGradeAuditRule( " +
                "Species TEXT, " +
                "DefectMax REAL Default 0.0, " +
                "ValidGrades TEXT);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGGRADEAUDITRULE =
            "INSERT INTO {0}.LogGradeAuditRule ( " +
                "Species, " +
                "DefectMax, " +
                "ValidGrades" +
            ") " +
            "SELECT " +
                "Species, " +
                "DefectMax, " +
                "ValidGrades " +
            "FROM {1}.LogGradeAuditRule;";
    }
}