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
}