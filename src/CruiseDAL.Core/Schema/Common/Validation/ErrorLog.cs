namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_ERRORLOG =
            "CREATE TABLE ErrorLog ( " +
                "TableName TEXT NOT NULL,  " +
                "CN_Number INTEGER NOT NULL, " +
                "ColumnName TEXT NOT NULL, " +
                "Level TEXT NOT NULL, " +
                "Message TEXT, " +
                "Program TEXT, " +
                "Suppress BOOLEAN Default 0, " +
                "UNIQUE(TableName, CN_Number, ColumnName, Level)" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_ERRORLOG_FORMAT_STR =
            "INSERT INTO {0}.ErrorLog ( " +
                    "TableName, " +
                    "CN_Number, " +
                    "ColumnName, " +
                    "Level, " +
                    "Message, " +
                    "Program, " +
                    "Suppress " +
                ") " +
                "SELECT " +
                    "TableName, " +
                    "CN_Number, " +
                    "ColumnName, " +
                    "Level, " +
                    "Message, " +
                    "Program, " +
                    "Suppress " +
                "FROM {1}.ErrorLog;";
    }
}