namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        // instead of following the convention used by other tables
        // the primary key for MessageLog is Message_CN
        // this is for compatibility with the older cruise schema
        public const string CREATE_TABLE_MESSAGELOG =
            "CREATE TABLE MessageLog( " +
                "Message_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Program TEXT COLLATE NOCASE, " +
                "Message TEXT, " +
                "Date TEXT DEFAULT (date('now', 'localtime')), " +
                "Time TEXT DEFAULT (time('now', 'localtime')), " +
                "Level TEXT COLLATE NOCASE DEFAULT 'N' " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_MESSAGELOG_FORMAT_STR =
            "INSERT INTO {0}.MessageLog ( " +
                    "Program, " +
                    "Date, " +
                    "Time, " +
                    "Level " +
                ") " +
                "SELECT " +
                    "Program, " +
                    "Date, " +
                    "Time, " +
                    "Level " +
                "FROM {1}.MessageLog;";
    }
}