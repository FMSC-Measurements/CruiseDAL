namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] CUTTINGUNIT_STRATUM = new string[]{
            CREATE_TABLE_CUTTINGUNIT_STRATUM };

        public const string CREATE_TABLE_CUTTINGUNIT_STRATUM =
            "CREATE TABLE CuttingUnit_Stratum (" +
                "CuttingUnit_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumArea REAL, " + // can be null of user hasn't subdevided area

                "UNIQUE (CuttingUnitCode, StratumCode), " +

                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_CUTTINGUNIT_STRATUM_FROM_CUTTINGUNITSTRATUM =
                "INSERT INTO {0}.CuttingUnit_Stratum ( " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "StratumArea " +
                ") " +
                "SELECT " +
                    "cu.Code, " +
                    "st.Code, " +
                    "cust.StratumArea " +
                "FROM {1}.CuttingUnitStratum AS cust " +
                "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TABLE_CUTTINGUNIT_STRATUM_FROM_CUTTINGUNITSTRATUM =
    //        "INSERT INTO CuttingUnit_Stratum ( " +
    //            "CuttingUnitCode, " +
    //            "StratumCode, " +
    //            "StratumArea" +
    //        ")" +
    //        "SELECT " +
    //            "cu.Code, " +
    //            "st.Code, " +
    //            "cust.StratumArea " +
    //        "FROM CuttingUnitStratum AS cust " +
    //        "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //        "JOIN Stratum AS st USING (Stratum_CN) " +
    //        ";";
    //}
}