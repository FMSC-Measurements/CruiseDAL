namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] LOGGRADEAUDITRULE_V3 = new string[]
        {
            CREATE_TABLE_LOGGRADEAUDITRULE_V3,
            CREATE_TABLE_LOGGRADEAUDITRULE_V3,
        };

        public const string CREATE_TABLE_LOGGRADEAUDITRULE_V3 =
            "CREATE TABLE LogGradeAuditRule_V3 ( " +
                "LogGradeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Species TEXT COLLATE NOCASE, " +
                "DefectMax REAL Default 0.0, " +
                "Grade TEXT NOT NULL COLLATE NOCASE CHECK (length(Grade) > 0), " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");";

        public const string CREATE_INDEX_LOGGRADEAUDITRULE_V3 =
            "CREATE UNIQUE INDEX LogGradeAuditRule_V3_Species_DefectMax_Grade " +
            "ON LogGradeAuditRule_V3 " +
            "( ifnull(Species, ''), Grade );";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGGRADEAUDITRULE_V3 =
            "INSERT INTO {0}.LogGradeAuditRule_V3 ( " +
                "Species, " +
                "DefectMax, " +
                "Grade" +
            ") " +
            "WITH RECURSIVE splitGrades(Species, DefectMax, Grade, ValidGrades) AS (" +
                "SELECT Species, DefectMax, '', replace(ValidGrades, ' ', '') FROM {1}.LogGradeAuditRule " + // select values from original table removing all white space
                "UNION ALL " +
                "SELECT " +
                        "Species, DefectMax, " +
                        "substr(ValidGrades, 0, instr(ValidGrades, ','))," + // grab value upto the next comma
                        "substr(ValidGrades, instr(ValidGrades, ',')+1) " + // send rest of the original text after our comma to next itteration
                "   FROM splitGrades " +
                    "WHERE length(ValidGrades) > 0" + // end loop when length of remaining text is 0
            ") " +
            "SELECT " +
                "sg.Species, " +
                "sg.DefectMax, " +
                "sg.Grade " +
            "FROM splitGrades AS sg " +
            "WHERE length(sg.Grade) > 0;";
    }
}