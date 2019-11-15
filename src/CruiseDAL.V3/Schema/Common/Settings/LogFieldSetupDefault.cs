namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGFIELDSETUPDEFAULT =
            "CREATE TABLE LogFieldSetupDefault ( " +
                "LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                "FieldOrder INTEGER Default 0, " +
                "ColumnType TEXT, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "Format TEXT, " +
                "Behavior TEXT, " +
                "UNIQUE(Field)," +
                "FOREIGN KEY (Field) REFERENCES LogField (Field) " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGFIELDSETUPDEFAULT_FORMAT_STR =
            "INSERT INTO {0}.LogFieldSetupDefault ( " +
                    "LogFieldSetupDefault_CN, " +
                    "Field, " +
                    "FieldOrder, " +
                    "ColumnType, " +
                    "Heading, " +
                    "Width, " +
                    "Format, " +
                    "Behavior" +
                ") " +
                "SELECT " +
                    "lfsd.LogFieldSetupDefault_CN, " +
                    "lfsd.Field, " +
                    "lfsd.FieldOrder, " +
                    "lfsd.ColumnType, " +
                    "lfsd.Heading, " +
                    "lfsd.Width, " +
                    "lfsd.Format, " +
                    "lfsd.Behavior " +
                "FROM {1}.LogFieldSetupDefault AS lfsd " +
                "JOIN {0}.TreeField AS tf USING (Field);";
    }
}