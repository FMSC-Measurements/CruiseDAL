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
}