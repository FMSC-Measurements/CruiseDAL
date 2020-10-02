using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CuttingUnit_StratumTableDefinition : ITableDefinition
    {
        public string TableName => "CuttingUnit_Stratum";

        public string CreateTable =>
@"CREATE TABLE CuttingUnit_Stratum (
    CuttingUnit_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    StratumArea REAL, --can be null of user hasn't subdevided area

    UNIQUE (CuttingUnitCode, StratumCode, CruiseID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE CuttingUnit_Stratum_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    StratumArea REAL,

    UNIQUE (CuttingUnitCode, StratumCode, CruiseID)
);";

        public string CreateIndexes =>
@"CREATE INDEX CuttingUnit_Stratum_StratumCode_CruiseID ON CuttingUnit_Stratum (StratumCode, CruiseID);

CREATE INDEX CuttingUnit_Stratum_CuttingUnitCode_CruiseID ON CuttingUnit_Stratum (CuttingUnitCode, CruiseID);
";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_CuttingUnit_Stratum_OnDelete };

        public const string CREATE_TRIGGER_CuttingUnit_Stratum_OnDelete =
@"CREATE TRIGGER CuttingUnit_Stratum_OnDelete
BEFORE DELETE ON CuttingUnit_Stratum
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO CuttingUnit_Stratum_Tombstone (
        CruiseID,
        CuttingUnitCode,
        StratumCode,
        StratumArea
    ) VALUES (
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.StratumArea
    );
END;";
    }
}