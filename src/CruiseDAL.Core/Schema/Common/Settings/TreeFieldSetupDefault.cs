namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDSETUPDEFAULT =
            "CREATE TABLE TreeFieldSetupDefault( " +
                "TreeFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Method TEXT NOT NULL COLLATE NOCASE, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                //"FieldName TEXT, " +
                "FieldOrder INTEGER Default 0, " +
                "ColumnType TEXT, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "Format TEXT, " +
                "Behavior TEXT, " +
                "UNIQUE (Method, Field)" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEFIELDSETUPDEFAULT_FORMAT_STR =
            "INSERT INTO {0}.TreeFieldSetupDefault ( " +
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
                    "TreeFieldSetupDefault_CN, " +
                    "Method, " +
                    "Field, " +
                    "FieldOrder, " +
                    "ColumnType, " +
                    "Heading, " +
                    "Width, " +
                    "Format, " +
                    "Behavior " +
                    "FROM {1}.TreeFieldSetupDefault;";
    }
}