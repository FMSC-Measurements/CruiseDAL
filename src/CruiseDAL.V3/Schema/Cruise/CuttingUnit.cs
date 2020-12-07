using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CuttingUnitTableDefinition : ITableDefinition
    {
        public string TableName => "CuttingUnit";

        public string CreateTable =>
@"CREATE TABLE CuttingUnit(
    CuttingUnit_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CuttingUnitID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Area REAL DEFAULT 0.0,
    Description TEXT,
    LoggingMethod TEXT COLLATE NOCASE,
    PaymentUnit TEXT,
    Rx TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME ,

    UNIQUE(CuttingUnitID),
    UNIQUE(CuttingUnitCode, CruiseID),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (LoggingMethod) REFERENCES LoggingMethods (LoggingMethod),

    CHECK (CuttingUnitID LIKE '________-____-____-____-____________'),
    CHECK (length(CuttingUnitCode) > 0)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE CuttingUnit_Tombstone (
    CuttingUnitID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Area REAL,
    Description TEXT,
    LoggingMethod TEXT,
    PaymentUnit TEXT,
    Rx TEXT,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME,

    UNIQUE(CuttingUnitID)
);

CREATE INDEX CuttingUnit_Tombstone_CruiseID_CuttingUnitCode ON CuttingUnit_Tombstone 
(CruiseID, CuttingUnitCode);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE,
            CREATE_TRIGGER_CuttingUnit_OnDelete,
        };

        public const string CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE =
@"CREATE TRIGGER CuttingUnit_OnUpdate
AFTER UPDATE OF
    CuttingUnitCode,
    Area,
    Description,
    LoggingMethod,
    PaymentUnit,
    Rx
ON CuttingUnit
FOR EACH ROW
BEGIN
    UPDATE CuttingUnit SET ModifiedDate = CURRENT_TIMESTAMP WHERE CuttingUnit_CN = old.CuttingUnit_CN;
END; ";

        public const string CREATE_TRIGGER_CuttingUnit_OnDelete =
@"CREATE TRIGGER CuttingUnit_OnDelete
BEFORE DELETE ON CuttingUnit
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO CuttingUnit_Tombstone (
        CuttingUnitCode,
        CruiseID,
        Area,
        LoggingMethod,
        PaymentUnit,
        Rx,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.CuttingUnitCode,
        OLD.CruiseID,
        OLD.Area,
        OLD.LoggingMethod,
        OLD.PaymentUnit,
        OLD.Rx,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}