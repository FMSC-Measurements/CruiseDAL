namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDSETUPDEFAULT =
            "CREATE TABLE TreeFieldSetupDefault( " +
                "TreeFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Method TEXT NOT NULL COLLATE NOCASE, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                "FieldOrder INTEGER Default 0, " +
                "ColumnType TEXT, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "Format TEXT, " +
                "Behavior TEXT, " +
                "UNIQUE (Method, Field), " +
                "FOREIGN KEY (Field) REFERENCES TreeField (Field)" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEFIELDSETUPDEFAULT_FORMAT_STR =
            "INSERT OR IGNORE INTO {0}.TreeFieldSetupDefault ( " +
                    "TreeFieldSetupDefault_CN, " +
                    "Method, " +
                    "Field, " +
                    "FieldOrder, " +
                    "ColumnType, " +
                    "Heading, " +
                    "Width, " +
                    "Format, " +
                    "Behavior " +
                ") " +
                "SELECT " +
                    "tfsd.TreeFieldSetupDefault_CN, " +
                    "tfsd.Method, " +
                    "tfsd.Field, " +
                    "tfsd.FieldOrder, " +
                    "tfsd.ColumnType, " +
                    "tfsd.Heading, " +
                    "tfsd.Width, " +
                    "tfsd.Format, " +
                    "tfsd.Behavior " +
                    "FROM {1}.TreeFieldSetupDefault AS tfsd " +
                    "JOIN {0}.TreeField AS tf USING (Field);";
    }
}