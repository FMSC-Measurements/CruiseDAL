using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SaleTableDefinition : ITableDefinition
    {
        public string TableName => "Sale";

        public string CreateTable =>
@"CREATE TABLE Sale(
    Sale_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleID TEXT NOT NULL COLLATE NOCASE,
    SaleNumber TEXT NOT NULL,
    Name TEXT,
    Region  TEXT COLLATE NOCASE,
    Forest TEXT COLLATE NOCASE,
    District TEXT DEFAULT '',
    CalendarYear INTEGER Default 0,
    Remarks TEXT,
    DefaultUOM TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (SaleID LIKE '________-____-____-____-____________'),
    UNIQUE(SaleID),
    UNIQUE(SaleNumber),

    FOREIGN KEY (Region) REFERENCES LK_Region (Region)--,
    --FOREIGN KEY (Forest, Region) REFERENCES LK_Forest (Forest, Region)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_SALE_ONUPDATE };

        public const string CREATE_TRIGGER_SALE_ONUPDATE =
@"CREATE TRIGGER OnUpdateSale
AFTER UPDATE OF
    Sale_CN,
    SaleNumber,
    Name,
    Purpose,
    Region,
    Forest,
    District,
    MeasurementYear,
    CalendarYear,
    LogGradingEnabled,
    Remarks,
    DefaultUOM
ON Sale
BEGIN
    UPDATE Sale SET Modified_TS = CURRENT_TIMESTAMP WHERE Sale_CN = old.Sale_CN;
END;";
    }
}