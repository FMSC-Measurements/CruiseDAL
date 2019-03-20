namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_GLOBALS =
            "CREATE TABLE Globals ( " +
                "Block TEXT DEFAULT 'Database' COLLATE NOCASE, " +
                "Key TEXT COLLATE NOCASE, " +
                "Value TEXT, " +
                "UNIQUE (Block, Key)" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_GLOBALS_FORMAT_STR =
            "INSERT OR IGNORE INTO {0}.Globals ( " + // dont overwrite existing global valus
                    "Block, " +
                    "Key, " +
                    "Value " +
                ") " +
                "SELECT " +
                    "Block, " +
                    "Key, " +
                    "Value " +
                "FROM {1}.Globals;";
    }

    //public partial class Updater
    //{
    //    public const string UPDATE_GLOBALS_TO_V3 =
    //        "CREATE TABLE new_Globals ( " +
    //            "Block TEXT DEFAULT 'Database' COLLATE NOCASE, " +
    //            "Key TEXT COLLATE NOCASE, " +
    //            "Value TEXT, " +
    //            "UNIQUE (Block, Key)" +
    //        ");" +
    //        "INSERT INTO new_Globals " +
    //        "SELECT * FROM Globals; " +
    //        "DROP TABLE Globals; " +
    //        "ALTER TABLE new_Globals RENAME TO Globals;";
    //}
}