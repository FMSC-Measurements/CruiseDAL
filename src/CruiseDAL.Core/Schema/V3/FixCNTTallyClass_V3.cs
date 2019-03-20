namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYCLASS_V3 =
            "CREATE TABLE FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumCode TEXT NOT NULL, " +
                "FieldName INTEGER Default 0, " +

                "UNIQUE (StratumCode) " +

                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYCLASS_V3_FORMAT_STR =
            "INSERT INTO {0}.FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN, " +
                "StratumCode, " +
                "FieldName " +
            ") " +
            "SELECT " +
                "FixCNTTallyClass_CN, " +
                "st.Code AS StratumCode, " +
                "FieldName " +
            "FROM {1}.FixCNTTallyClass " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}