namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYCLASS =
            "CREATE TABLE FixCNTTallyClass ( " +
                "FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +

                "UNIQUE (CruiseID, StratumCode), " +

                "FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (Field) REFERENCES TreeField (Field) " +
            ");";

        public const string CREATE_INDEX_FixCNTTallyClass_Field =
            @"CREATE INDEX FixCNTTallyClass_Field ON FixCNTTallyClass (Field);";

        public const string CREATE_INDEX_FixCNTTallyClass_StratumCode_CruiseID =
            @"CREATE INDEX FixCNTTallyClass_StratumCode_CruiseID ON FixCNTTallyClass (StratumCode, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYCLASS_FORMAT_STR =
            "INSERT INTO {0}.FixCNTTallyClass ( " +
                "FixCNTTallyClass_CN, " +
                "CruiseID, " +
                "StratumCode, " +
                "Field " +
            ") " +
            "SELECT " +
                "FixCNTTallyClass_CN, " +
                "'{3}', " +
                "st.Code AS StratumCode, " +
                "FieldName " +
            "FROM {1}.FixCNTTallyClass " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}