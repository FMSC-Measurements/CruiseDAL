namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] LOGFIELDSETUP_V3 = new string[]
        {
            CREATE_TABLE_LOGFIELDSETUP_V3
        };

        public const string CREATE_TABLE_LOGFIELDSETUP_V3 =
            "CREATE TABLE LogFieldSetup_V3 (" +
                "StratumCode TEXT NOT NULL, " +
                "Field TEXT NOT NULL, " +
                "FieldOrder INTEGER Default 0, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +

                "UNIQUE (StratumCode, Field), " +

                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE," +
                "FOREIGN KEY (Field) REFERENCES LogField (Field) " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGFIELDSETUP_V3_FROM_LOGFIELDSETUP =
            "INSERT INTO {0}.LogFieldSetup_V3 ( " +
                    "StratumCode, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                ") " +
                "SELECT " +
                    "st.Code AS StratumCode, " +
                    "lfs.Field, " +
                    "lfs.FieldOrder, " +
                    "lfs.Heading, " +
                    "lfs.Width " +
                "FROM {1}.LogFieldSetup AS lfs " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN)" +
                "JOIN {0}.LogField USING (Field);"; // join with LogField so we only get valid fields
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_LOGFIELDSETUP_V3_FROM_LOGFIELDSETUP =
    //        "INSERT INTO LogFieldSetup_V3 " +
    //        "SELECT " +
    //        "st.Code AS StratumCode, " +
    //        "Field, " +
    //        "FieldOrder, " +
    //        "Heading, " +
    //        "Width " +
    //        "FROM LogFieldSetup " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}