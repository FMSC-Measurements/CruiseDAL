namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDSETUP =
            "CREATE TABLE TreeFieldSetup ( " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE," +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                "FieldOrder INTEGER Default 0, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "UNIQUE(StratumCode, Field, CruiseID), " +
                "FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE," +
                "FOREIGN KEY (Field) REFERENCES TreeField (Field) " +
            ");";

        public const string CREATE_INDEX_TreeFieldSetup_Field =
            @"CREATE INDEX TreeFieldSetup_Field ON TreeFieldSetup (Field);";

        public const string CREATE_INDEX_TreeFieldSetup_StratumCode_CruiseID =
            @"CREATE INDEX TreeFieldSetup_StratumCode_CruiseID ON TreeFieldSetup (StratumCode, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEFIELDSETUP_FROM_TREEFIELDSETUP_FORMAT_STR =
            "INSERT INTO {0}.TreeFieldSetup ( " +
                    "CruiseID," +
                    "StratumCode, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                ") " +
                "SELECT " +
                    "'{3}', " +
                    "st.Code AS StratumCode, " +
                    "tfs.Field, " +
                    "tfs.FieldOrder, " +
                    "tfs.Heading, " +
                    "tfs.Width " +
                "FROM {1}.TreeFieldSetup AS tfs " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN)" +
                "JOIN {0}.TreeField AS tf USING (Field);";
    }
}