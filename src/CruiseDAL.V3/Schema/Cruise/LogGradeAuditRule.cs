using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class LogGradeAuditRuleTableDefinition : ITableDefinition
    {
        public string TableName => "LogGradeAuditRule";

        public string CreateTable =>
@"CREATE TABLE LogGradeAuditRule(
    LogGradeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    DefectMax REAL Default 0.0,
    Grade TEXT NOT NULL COLLATE NOCASE CHECK (length(Grade) > 0),

    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX LogGradeAuditRule_SpeciesCode_DefectMax_Grade_CruiseID 
ON LogGradeAuditRule 
(ifnull(SpeciesCode, ''), round(DefectMax, 2), Grade, CruiseID);

CREATE INDEX 'LogGradeAuditRule_SpeciesCode' ON 'LogGradeAuditRule' ('SpeciesCode');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGGRADEAUDITRULE =
            "INSERT INTO {0}.LogGradeAuditRule ( " +
                "CruiseID," +
                "SpeciesCode, " +
                "DefectMax, " +
                "Grade" +
            ") " +
            "WITH RECURSIVE splitGrades(SpeciesCode, DefectMax, Grade, ValidGrades) AS (" +
                "SELECT Species AS SpeciesCode, DefectMax, '', replace(ValidGrades, ' ', '') FROM {1}.LogGradeAuditRule " + // select values from original table removing all white space
                "UNION ALL " +
                "SELECT " +
                        "SpeciesCode, DefectMax, " +
                        "substr(ValidGrades, 0, ifnull(nullif(instr(ValidGrades, ','), 0), length(ValidGrades) + 1)), " + // grab value upto the next comma, if no comma return whole string
                        "substr(ValidGrades, ifnull(nullif(instr(ValidGrades, ','), 0), length(ValidGrades) + 1)+1) " + // send rest of the original text after our comma to next itteration
                "   FROM splitGrades " +
                    "WHERE length(ValidGrades) > 0" + // end loop when length of remaining text is 0
            ") " +
            "SELECT " +
                "'{3}', " +
                "nullif(sg.SpeciesCode, 'ANY') AS SpeciesCode, " + // in version 2 'ANY' was used to indicate that a rule applied to all species values
                "sg.DefectMax, " +
                "sg.Grade " +
            "FROM splitGrades AS sg " +
            "WHERE length(sg.Grade) > 0;";
    }
}