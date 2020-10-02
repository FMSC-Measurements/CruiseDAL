using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CuttingUnitTableDefinition : ITableDefinition
    {
        public string TableName => "CuttingUnit";

        public string CreateTable =>
@"CREATE TABLE CuttingUnit(
    CuttingUnit_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Area REAL DEFAULT 0.0,
    Description TEXT,
    LoggingMethod TEXT,
    PaymentUnit TEXT,
    Rx TEXT,
    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DateTime ,
    RowVersion INTEGER DEFAULT 0,
    UNIQUE(Code, CruiseID),
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    CHECK (length(Code) > 0)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE CuttingUnit_Tombstone (
    Code TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Area REAL DEFAULT 0.0,
    Description TEXT,
    LoggingMethod TEXT,
    PaymentUnit TEXT,
    Rx TEXT,
    CreatedBy TEXT,
    CreatedDate DateTime,
    ModifiedBy TEXT,
    ModifiedDate DateTime ,
    RowVersion INTEGER DEFAULT 0,
    UNIQUE(Code, CruiseID),
    CHECK (length(Code) > 0)
);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE,
            CREATE_TRIGGER_CuttingUnit_OnDelete,
        };

        public const string CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE =
@"CREATE TRIGGER CuttingUnit_OnUpdate
AFTER UPDATE OF
    Code,
    Area,
    Description,
    LoggingMethod,
    PaymentUnit,
    Rx
ON CuttingUnit
FOR EACH ROW
BEGIN
    UPDATE CuttingUnit SET ModifiedDate = datetime('now', 'localtime') WHERE CuttingUnit_CN = old.CuttingUnit_CN;
    UPDATE CuttingUnit SET RowVersion = old.RowVersion + 1 WHERE CuttingUnit_CN = old.CuttingUnit_CN;
END; ";

        public const string CREATE_TRIGGER_CuttingUnit_OnDelete =
@"CREATE TRIGGER CuttingUnit_OnDelete
BEFORE DELETE ON CuttingUnit
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO CuttingUnit_Tombstone (
        Code,
        CruiseID,
        Area,
        LoggingMethod,
        PaymentUnit,
        Rx,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate
    ) VALUES (
        OLD.Code,
        OLD.CruiseID,
        OLD.Area,
        OLD.LoggingMethod,
        OLD.PaymentUnit,
        OLD.Rx,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate
    );
END;;";
    }
}