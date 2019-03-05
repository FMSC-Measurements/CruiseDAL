namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SALE =
            "CREATE TABLE Sale( " +
                "Sale_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "SaleNumber TEXT NOT NULL, " +
                "Name TEXT, " +
                "Purpose TEXT, " +
                "Region TEXT DEFAULT '', " +
                "Forest TEXT DEFAULT '', " +
                "District TEXT DEFAULT '', " +
                "MeasurementYear TEXT, " +
                "CalendarYear INTEGER Default 0, " +
                "LogGradingEnabled BOOLEAN Default 0, " +
                "Remarks TEXT, " +
                "DefaultUOM TEXT, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(SaleNumber) " +
            ");";

        public const string CREATE_TRIGGER_SALE_ONUPDATE =
            "CREATE TRIGGER OnUpdateSale " +
            "AFTER UPDATE OF " +
                "Sale_CN, " +
                "SaleNumber, " +
                "Name, " +
                "Purpose, " +
                "Region, " +
                "Forest, " +
                "District, " +
                "MeasurementYear, " +
                "CalendarYear, " +
                "LogGradingEnabled, " +
                "Remarks, " +
                "DefaultUOM " +
            "ON Sale " +
            "BEGIN " +
                "UPDATE Sale SET ModifiedDate = datetime('now', 'localtime') WHERE Sale_CN = old.Sale_CN; " +
                "UPDATE Sale SET RowVersion = old.RowVersion + 1 WHERE Sale_CN = old.Sale_CN;" +
            "END;";
    }
}