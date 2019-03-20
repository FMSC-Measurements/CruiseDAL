namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEFIELDSETUP_V3 =
            "CREATE TABLE TreeFieldSetup_V3 ( " +
                "StratumCode TEXT NOT NULL, " +
                "Field TEXT NOT NULL, " +
                "FieldOrder INTEGER Default 0, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "UNIQUE(StratumCode, Field), " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEFIELDSETUP_V3_FROM_TREEFIELDSETUP_FORMAT_STR =
            "INSERT INTO {0}.TreeFieldSetup_V3 ( " +
                    "StratumCode, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                ") " +
                "SELECT " +
                    "st.Code AS StratumCode, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                "FROM {1}.TreeFieldSetup " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TREEFIELDSETUP_V3_FROM_TREEFIELDSETUP =
    //        "INSERT INTO TreeFieldSetup_V3 " +
    //        "SELECT " +
    //            "st.Code AS StratumCode, " +
    //            "Field, " +
    //            "FieldOrder, " +
    //            "Heading, " +
    //            "Width " +
    //        "FROM TreeFieldSetup " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}