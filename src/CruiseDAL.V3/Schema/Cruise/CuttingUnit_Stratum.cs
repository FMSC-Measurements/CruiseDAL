using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CuttingUnit_StratumTableDefinition : ITableDefinition
    {
        public string TableName => "CuttingUnit_Stratum";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    CuttingUnit_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    StratumArea REAL, --can be null of user hasn't subdevided area
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CuttingUnitCode, StratumCode, CruiseID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE CuttingUnit_Stratum_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    StratumArea REAL,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_CuttingUnit_Stratum_Tombstone_CruiseID_CuttingUnitCode_StratumCode ON CuttingUnit_Stratum_Tombstone
(CuttingUnitCode, StratumCode, CruiseID);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_CuttingUnit_Stratum_StratumCode_CruiseID ON CuttingUnit_Stratum (StratumCode, CruiseID);

CREATE INDEX NIX_CuttingUnit_Stratum_CuttingUnitCode_CruiseID ON CuttingUnit_Stratum (CuttingUnitCode, CruiseID);
";

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_CuttingUnit_Stratum_OnUpdate,
            CREATE_TRIGGER_CuttingUnit_Stratum_OnDelete
        };

        public const string CREATE_TRIGGER_CuttingUnit_Stratum_OnUpdate =
@"CREATE TRIGGER CuttingUnit_Stratum_OnUpdate
AFTER UPDATE OF
    StratumArea
ON CuttingUnit_Stratum
FOR EACH ROW
BEGIN
    UPDATE CuttingUnit_Stratum SET Modified_TS = CURRENT_TIMESTAMP WHERE CuttingUnit_Stratum_CN = OLD.CuttingUnit_Stratum_CN;
END;";

        public const string CREATE_TRIGGER_CuttingUnit_Stratum_OnDelete =
@"CREATE TRIGGER CuttingUnit_Stratum_OnDelete
BEFORE DELETE ON CuttingUnit_Stratum
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO CuttingUnit_Stratum_Tombstone (
        CruiseID,
        CuttingUnitCode,
        StratumCode,
        StratumArea,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.StratumArea,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}