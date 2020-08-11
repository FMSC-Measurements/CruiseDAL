namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_CUTTINGUNIT_STRATUM =
            "CREATE TABLE CuttingUnit_Stratum (" +
                "CuttingUnit_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumArea REAL, " + // can be null of user hasn't subdevided area

                "UNIQUE (CuttingUnitCode, StratumCode, CruiseID), " +

                "FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE, " +
                "FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_CUTTINGUNIT_STRATUM_StratumCode_CruiseID =
            @"CREATE INDEX CuttingUnit_Stratum_StratumCode_CruiseID ON CuttingUnit_Stratum (StratumCode, CruiseID);";

        public const string CREATE_INDEX_CuttingUnit_Stratum_CuttingUnitCode_CruiseID =
            @"CREATE INDEX CuttingUnit_Stratum_CuttingUnitCode_CruiseID ON CuttingUnit_Stratum (CuttingUnitCode, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_CUTTINGUNIT_STRATUM_FROM_CUTTINGUNITSTRATUM =
                "INSERT INTO {0}.CuttingUnit_Stratum ( " +
                    "CruiseID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "StratumArea " +
                ") " +
                "SELECT " +
                    "'{4}', " +
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