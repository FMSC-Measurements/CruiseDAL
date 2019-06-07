namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_CUTTINGUNIT =
            "CREATE TABLE CuttingUnit( " +
                "CuttingUnit_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Code TEXT NOT NULL COLLATE NOCASE, " +
                "Area REAL DEFAULT 0.0, " +
                "TallyHistory TEXT, " + // although tally history is no longer used it is being kept for backwards compatibility
                "Description TEXT, " +
                "LoggingMethod TEXT, " +
                "PaymentUnit TEXT, " +
                "Rx TEXT, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(Code)" +
            ");";

        public const string CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE =
            "CREATE TRIGGER CuttingUnit_OnUpdate " +
            "AFTER UPDATE OF " +
            "Code, " +
            "Area, " +
            "Description, " +
            "LoggingMethod, " +
            "PaymentUnit, " +
            "Rx " +
            "ON CuttingUnit " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE CuttingUnit SET ModifiedDate = datetime('now', 'localtime') WHERE CuttingUnit_CN = old.CuttingUnit_CN; " +
                "UPDATE CuttingUnit SET RowVersion = old.RowVersion + 1 WHERE CuttingUnit_CN = old.CuttingUnit_CN; " +
            "END; ";
    }

    public partial class Migrations
    {
        public const string MIGRATE_CUTTINGUNIT_FORMAT_STR =
            "INSERT INTO {0}.CuttingUnit ( " +
                    "CuttingUnit_CN, " +
                    "Code, " +
                    "Area, " +
                    "Description, " +
                    "LoggingMethod, " +
                    "PaymentUnit, " +
                    "Rx " +
                ") " +
                "SELECT " +
                    "CuttingUnit_CN, " +
                    "Code, " +
                    "Area, " +
                    "Description, " +
                    "LoggingMethod, " +
                    "PaymentUnit, " +
                    "Rx " +
                "FROM {1}.CuttingUnit; ";
    }
}