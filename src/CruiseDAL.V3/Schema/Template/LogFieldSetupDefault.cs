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
}