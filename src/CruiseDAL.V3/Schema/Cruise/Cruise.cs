using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CruiseTableDefinition : ITableDefinition
    {
        public string TableName => "Cruise";

        public string CreateTable =>
@"CREATE TABLE Cruise (
    Cruise_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SaleID TEXT NOT NULL COLLATE NOCASE,
    CruiseNumber TEXT NOT NULL COLLATE NOCASE,
    Purpose TEXT,
    Remarks TEXT,
    DefaultUOM TEXT COLLATE NOCASE,
    MeasurementYear TEXT COLLATE NOCASE,
    LogGradingEnabled BOOLEAN Default 0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID),
    UNIQUE (CruiseNumber),

    CHECK (CruiseID LIKE '________-____-____-____-____________'),
    CHECK (LogGradingEnabled IN (0, 1)),

    FOREIGN KEY (SaleID) REFERENCES Sale (SaleID) ON DELETE CASCADE,
    FOREIGN KEY (DefaultUOM) REFERENCES LK_UOM (UOM),
    FOREIGN KEY (Purpose) REFERENCES LK_Purpose (Purpose)
);";

        public string InitializeTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_CRUISE_ONUPDATE };

        public string CreateTombstoneTable => null;

        public const string CREATE_TRIGGER_CRUISE_ONUPDATE =
@"CREATE TRIGGER OnUpdateCruise
AFTER UPDATE OF
    SaleID,
    Purpose,
    LogGradingEnabled,
    Remarks,
    DefaultUOM
ON Sale
BEGIN
    UPDATE Sale SET Modified_TS = CURRENT_TIMESTAMP WHERE Sale_CN = old.Sale_CN;
END;";
    }
}