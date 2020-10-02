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

        public string CreateTombstoneTable => null;// TODO

        public string CreateIndexes =>
@"CREATE INDEX 'LogFieldSetup_Field' ON 'LogFieldSetup'('Field' COLLATE NOCASE);

CREATE INDEX 'LogFieldSetup_StratumCode_CruiseID' ON 'LogFieldSetup'('StratumCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}