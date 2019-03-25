namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGGRADEAUDITRULE_V3 =
            "CREATE TABLE LogGradeAuditRule_V3 ( " +
                "LogGradeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Species TEXT COLLATE NOCASE, " +
                "DefectMax REAL Default 0.0, " +
                "Grade TEXT NOT NULL COLLATE NOCASE CHECK (length(Grade) > 0), " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");" +
            "CREATE UNIQUE INDEX LogGradeAuditRule_V3_Species_DefectMax_Grade " +
            "( ifnull(Species, ''), Grade );";

    
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