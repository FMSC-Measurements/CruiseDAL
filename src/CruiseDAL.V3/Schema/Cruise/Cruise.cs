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
    CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DateTime,

    CHECK (CruiseID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (SaleID) REFERENCES Sale (SaleID) ON DELETE CASCADE,
    UNIQUE (CruiseID)
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
    UPDATE Sale SET ModifiedDate = datetime('now', 'localtime') WHERE Sale_CN = old.Sale_CN;
    UPDATE Sale SET RowVersion = old.RowVersion + 1 WHERE Sale_CN = old.Sale_CN;
END;";
    }
}