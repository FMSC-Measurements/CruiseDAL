namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_MESSAGELOG =
            "CREATE TABLE MessageLog( " +
                "MessageLog_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Program TEXT COLLATE NOCASE, " +
                "Message TEXT, " +
                "Date TEXT DEFAULT (date('now', 'localtime')), " +
                "Time TEXT DEFAULT (time('now', 'localtime')), " +
                "Level TEXT COLLATE NOCASE DEFAULT 'N' " +
            ");";
    }

    public partial class Migrations
    {
        public static string MIGRATE_MESSAGELOG_FORMAT_STR =
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