using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class FixCNTTallyClassTableDefinition : ITableDefinition
    {
        public string TableName => "FixCNTTallyClass";

        public string CreateTable =>
@"CREATE TABLE FixCNTTallyClass (
    FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,

    UNIQUE (CruiseID, StratumCode),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX FixCNTTallyClass_Field ON FixCNTTallyClass (Field);

CREATE INDEX FixCNTTallyClass_StratumCode_CruiseID ON FixCNTTallyClass (StratumCode, CruiseID);";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}