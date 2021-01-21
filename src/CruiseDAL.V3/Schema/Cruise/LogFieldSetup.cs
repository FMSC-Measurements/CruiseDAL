using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class LogFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "LogFieldSetup";

        public string CreateTable =>
@"CREATE TABLE LogFieldSetup (
    StratumCode TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL,
    FieldOrder INTEGER Default 0,
    Heading TEXT,
    Width REAL Default 0.0,

    UNIQUE (CruiseID, StratumCode, Field),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES LogField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE LogFieldSetup_Tombstone (
    StratumCode TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL,
    FieldOrder INTEGER,
    Heading TEXT,
    Width REAL,
    Deleted_TS DATETIME
);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_LogFieldSetup_Field ON LogFieldSetup ('Field' COLLATE NOCASE);

CREATE INDEX NIX_LogFieldSetup_StratumCode_CruiseID ON LogFieldSetup ('StratumCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_LogFieldSetup_OnDelete,
        };

        public const string CREATE_TRIGGER_LogFieldSetup_OnDelete =
@"CREATE TRIGGER LogFieldSetup_OnDelete 
BEFORE DELETE ON LogFieldSetup
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO LogFieldSetup_Tombstone (
        StratumCode,
        CruiseID,
        Field,
        FieldOrder,
        Heading,
        Width,
        Deleted_TS
    ) VALUES (
        OLD.StratumCode,
        OLD.CruiseID,
        OLD.Field,
        OLD.FieldOrder,
        OLD.Heading,
        OLD.Width,
        CURRENT_TIMESTAMP
    );
END;";
    }
}