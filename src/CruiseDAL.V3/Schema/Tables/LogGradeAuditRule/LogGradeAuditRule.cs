using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LogGradeAuditRuleTableDefinition : ITableDefinition
    {
        public string TableName => "LogGradeAuditRule";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    LogGradeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    Grade TEXT NOT NULL COLLATE NOCASE CHECK (length(Grade) > 0),
    DefectMax REAL Default 0.0,

    -- see indexes for unique constraints

    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE LogGradeAuditRule_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    Grade TEXT NOT NULL COLLATE NOCASE,
    DefectMax REAL,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_LogGradeAuditRule_Tombstone_CruiseID_SpeciesCode_Grade ON LogGradeAuditRule_Tombstone
(CruiseID, (ifnull(SpeciesCode, '')), Grade);";

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX UIX_LogGradeAuditRule_SpeciesCode_DefectMax_Grade_CruiseID
ON LogGradeAuditRule
(ifnull(SpeciesCode, ''), round(DefectMax, 2), Grade, CruiseID);

CREATE INDEX UIX_LogGradeAuditRule_CruiseID_SpeciesCode ON LogGradeAuditRule (CruiseID, SpeciesCode);";

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_LogGradeAuditRule_OnDelete,
        };

        public const string CREATE_TRIGGER_LogGradeAuditRule_OnDelete =
@"CREATE TRIGGER LogGradeAuditRule_OnDelete
BEFORE DELETE ON LogGradeAuditRule
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO LogGradeAuditRule_Tombstone (
        CruiseID,
        SpeciesCode,
        Grade,
        DefectMax,
        Deleted_TS
    ) VALUES (
        OLD.CruiseID,
        OLD.SpeciesCode,
        OLD.Grade,
        OLD.DefectMax,
        CURRENT_TIMESTAMP
    );
END;";
    }
}