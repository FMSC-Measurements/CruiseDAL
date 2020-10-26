using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SaleTableDefinition : ITableDefinition
    {
        public string TableName => "Sale";

        public string CreateTable =>
@"CREATE TABLE Sale(
    SaleID TEXT NOT NULL,
    Sale_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleNumber TEXT NOT NULL,
    Name TEXT,
    Purpose TEXT,
    Region TEXT DEFAULT '',
    Forest TEXT DEFAULT '',
    District TEXT DEFAULT '',
    MeasurementYear TEXT,
    CalendarYear INTEGER Default 0,
    LogGradingEnabled BOOLEAN Default 0,
    Remarks TEXT,
    DefaultUOM TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (SaleID LIKE '________-____-____-____-____________'),
    UNIQUE(SaleID),
    UNIQUE(SaleNumber)
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