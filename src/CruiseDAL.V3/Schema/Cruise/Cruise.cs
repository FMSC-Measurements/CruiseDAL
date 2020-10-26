using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class CruiseTableDefinition : ITableDefinition
    {
        public string TableName => "Cruise";

        public string CreateTable =>
@"CREATE TABLE Cruise (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SaleID TEXT NOT NULL COLLATE NOCASE,
    Purpose TEXT,
    Remarks TEXT,
    DefaultUOM TEXT,
    LogGradingEnabled BOOLEAN Default 0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID),

    CHECK (CruiseID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (SaleID) REFERENCES Sale (SaleID) ON DELETE CASCADE
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