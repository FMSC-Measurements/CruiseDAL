namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYCLASS_V3 =
            "CREATE TABLE FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +

                "UNIQUE (CruiseID, StratumCode), " +

                "FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (Field) REFERENCES TreeField (Field) " +
            ");";

        public const string CREATE_INDEX_FixCNTTallyClass_V3_Field =
            @"CREATE INDEX FixCNTTallyClass_V3_Field ON FixCNTTallyClass_V3 (Field);";

        public const string CREATE_INDEX_FixCNTTallyClass_V3_StratumCode_CruiseID =
            @"CREATE INDEX FixCNTTallyClass_V3_StratumCode_CruiseID ON FixCNTTallyClass_V3 (StratumCode, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYCLASS_V3_FORMAT_STR =
            "INSERT INTO {0}.FixCNTTallyClass_V3 ( " +
                "FixCNTTallyClass_CN, " +
                "CruiseID, " +
                "StratumCode, " +
                "Field " +
            ") " +
            "SELECT " +
                "FixCNTTallyClass_CN, " +
                "'{4}', " +
                "st.Code AS StratumCode, " +
                "FieldName " +
            "FROM {1}.FixCNTTallyClass " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}