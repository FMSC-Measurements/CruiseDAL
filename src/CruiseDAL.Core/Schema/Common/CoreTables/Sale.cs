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
                "CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) , " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(SaleNumber) " +
            ");";
    }
}