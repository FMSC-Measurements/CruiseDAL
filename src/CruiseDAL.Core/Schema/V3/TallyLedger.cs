namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYLEDGER =
        "CREATE TABLE TallyLedger ( " +
            "TallyLedger_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "TallyLedgerID TEXT NOT NULL, " +
            "TreeID TEXT, " +
            "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
            "PlotNumber INTEGER, " +
            "Species TEXT DEFAULT '' COLLATE NOCASE, " +
            "LiveDead TEXT DEFAULT '' COLLATE NOCASE, " +
            "TreeCount INTEGER NOT NULL, " +
            "KPI INTEGER Default 0, " +
            "STM TEXT DEFAULT 'N' COLLATE NOCASE, " +
            "ThreePRandomValue INTEGER Default 0, " +
            "Signature TEXT COLLATE NOCASE, " +
            "Reason TEXT, " +
            "Remarks TEXT, " +
            "EntryType TEXT COLLATE NOCASE, " +

            "CreatedBy TEXT DEFAULT 'none', " +
            "CreatedDate DATETIME DEFAULT (datetime('now', 'localtime'))," +
            "IsDeleted BOOLEAN DEFAULT 0," +

            "UNIQUE (TallyLedgerID)," +

            "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) " +
            //"FOREIGN KEY (CuttingUnitCode, StratumCode) REFERENCES CuttingUnit_Stratum (CuttingUnitCode, StratumCode), " +
            "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) , " +
            "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode), " +
            //"FOREIGN KEY (StratumCode, SampleGroupCode, Species, LiveDead) REFERENCES TallyPopulation (StratumCode, SampleGroupCode, Species, LiveDead), " +
            //"FOREIGN KEY (CuttingUnitCode, StratumCode, PlotNumber) REFERENCES Plot_Stratum (CuttingUnitCode, StratumCode, PlotNumber), " +
            "FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE" +
        ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYLEDGER_FROM_COUNTTREE_FORMAT_STR =
            "INSERT INTO {0}.TallyLedger ( " +
                    "TallyLedgerID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "EntryType " +
                ") " +
                "SELECT " +
                    "'initFromCountTree-' || cu.Code || ',' || st.Code || ',' || sg.Code || ',' || ifnull(tdv.Species, 'null') || ',' || ifnull(tdv.LiveDead, 'null') || ',' || ifnull(Component_CN, 'master'), " +
                    "cu.Code AS CuttingUnitCode, " +
                    "st.Code AS StratumCode, " +
                    "sg.Code AS SampleGroupCode, " +
                    "tdv.Species AS Species, " +
                    "tdv.LiveDead AS LiveDead, " +
                    "Sum(ct.TreeCount) AS TreeCount, " +
                    "Sum(ct.SumKPI) AS SumKPI, " +
                    "'utility' AS EntryType " +
                "FROM {1}.CountTree AS ct " +
                "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
                "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
                "GROUP BY " +
                    "cu.Code, " +
                    "st.Code, " +
                    "sg.Code, " +
                    "ifnull(tdv.Species, ''), " +
                    "ifnull(tdv.LiveDead, ''), " +
                    "ifnull(ct.Component_CN, 0)" +
                ";";

        public const string MIGRATE_TALLYLEDGER_FROM_TREE =
            "INSERT INTO {0}.TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "PlotNumber, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "EntryType " +
                ") " +
            "SELECT " +
                "'migrateFromTree-' || t.Tree_CN, " +
                "t3.TreeID, " +
                "cu.Code AS CuttingUnitCode, " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "tdv.Species AS Species, " +
                "tdv.LiveDead AS LiveDead, " +
                "t.TreeCount, " +
                "t.KPI AS KPI, " +
                "'utility' AS EntryType " +
            "FROM {1}.Tree AS t " +
            "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
            "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN {0}.Tree_V3 AS t3 USING (Tree_CN) " +
            "LEFT JOIN {0}.Plot_Stratum AS ps ON t.Plot_CN = ps.Plot_Stratum_CN " +
            "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
            "WHERE t.TreeCount > 0 OR t.KPI > 0 OR t.CountOrMeasure = 'M' " +
            ";";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TALLYLEDGER_FROM_COUNTTREE =
    //        "INSERT INTO TallyLedger ( " +
    //            "TallyLedgerID, " +
    //            "CuttingUnitCode, " +
    //            "StratumCode, " +
    //            "SampleGroupCode, " +
    //            "Species, " +
    //            "LiveDead, " +
    //            "TreeCount, " +
    //            "KPI, " +
    //            "EntryType" +
    //        ") " +
    //        "SELECT " +
    //            "'initFromCountTree-' || cu.Code || ',' || st.Code || ',' || sg.Code || ',' || ifnull(tdv.Species, 'null') || ',' || ifnull(tdv.LiveDead, 'null') || ',' || ifnull(Component_CN, 'master'), " +
    //            "cu.Code AS CuttingUnitCode, " +
    //            "st.Code AS StratumCode, " +
    //            "sg.Code AS SampleGroupCode, " +
    //            "tdv.Species AS Species, " +
    //            "tdv.LiveDead AS LiveDead, " +
    //            "Sum(ct.TreeCount) AS TreeCount, " +
    //            "Sum(ct.SumKPI) AS SumKPI, " +
    //            "'utility' AS EntryType " +
    //        "FROM CountTree AS ct " +
    //        "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //        "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
    //        "JOIN Stratum AS st USING (Stratum_CN) " +
    //        "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
    //        "GROUP BY " +
    //            "cu.Code, " +
    //            "st.Code, " +
    //            "sg.Code, " +
    //            "ifnull(tdv.Species, ''), " +
    //            "ifnull(tdv.LiveDead, ''), " +
    //            "ifnull(ct.Component_CN, 0)" +
    //        ";";

    //    public const string INITIALIZE_TALLYLEDGER_FROM_TREE =
    //        "WITH measureTrees AS (" +
    //            "SELECT " +
    //                "tv3.TreeID, " +
    //                "tv3.CuttingUnitCode, " +
    //                "tp.StratumCode, " +
    //                "tp.SampleGroupCode, " +
    //                "tp.Species, " +
    //                "tp.LiveDead, " +
    //                "t.TreeCount, " +
    //                "t.KPI, " +
    //                "t.STM  " +
    //            //"* " +
    //            "FROM Tree as t " +
    //            "JOIN Tree_V3 as tv3 USING (Tree_CN) " +
    //            "JOIN TallyPopulation AS tp ON " +
    //                "tp.StratumCode = tv3.StratumCode " +
    //                "AND tp.SampleGroupCode = tv3.SampleGroupCode " +
    //                "AND (tp.Species = tv3.Species OR tp.Species = '') " +
    //                "AND (tp.LiveDead = tv3.LiveDead OR tp.LiveDead = 'default') " +
    //            "WHERE t.CountOrMeasure = 'M' OR t.CountOrMeasure = 'm'" +
    //        ") " +

    //        "INSERT INTO TallyLedger ( " +
    //            "TallyLedgerID, " +
    //            "TreeID, " +
    //            "CuttingUnitCode, " +
    //            "StratumCode, " +
    //            "SampleGroupCode, " +
    //            "Species, " +
    //            "LiveDead, " +
    //            "TreeCount, " +
    //            "KPI, " +
    //            "STM" +
    //        ") " +
    //        "SELECT " +
    //            "'initFromTree' || TreeID AS TallyLedgerID, " +
    //            "* " +
    //        "FROM measureTrees;";
    //}
}