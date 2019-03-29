namespace CruiseDAL.Schema
{
    public partial class DDL
    {


        public const string CREATE_TABLE_FIXCNTTALLYCLASS_V3 =
            "CREATE TABLE FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumCode TEXT NOT NULL, " +
                "Field TEXT NOT NULL, " +

                "UNIQUE (StratumCode) " +

                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE, " +
                "FOREIGN KEY (Field) REFERENCES TreeField (Field) " +
            ");";

        public const string CREATE_INDEX_FixCNTTallyClass_V3_Field =
            @"CREATE INDEX 'FixCNTTallyClass_V3_Field' ON 'FixCNTTallyClass_V3'('Field' COLLATE NOCASE);";

        public const string CREATE_INDEX_FixCNTTallyClass_V3_StratumCode =
            @"CREATE INDEX 'FixCNTTallyClass_V3_StratumCode' ON 'FixCNTTallyClass_V3'('StratumCode' COLLATE NOCASE);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYCLASS_V3_FORMAT_STR =
            "INSERT INTO {0}.FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN, " +
                "StratumCode, " +
                "Field " +
            ") " +
            "SELECT " +
                "FixCNTTallyClass_CN, " +
                "st.Code AS StratumCode, " +
                "FieldName " +
            "FROM {1}.FixCNTTallyClass " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}