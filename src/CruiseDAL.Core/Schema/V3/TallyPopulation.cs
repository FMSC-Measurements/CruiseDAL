namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        //public const string CREATE_TABLE_TALLYPOPULATION =
        //    "CREATE TABLE TallyPopulation ( " +
        //        "TallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
        //        "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
        //        "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
        //        "Species TEXT DEFAULT '' COLLATE NOCASE, " +
        //        "LiveDead TEXT DEFAULT 'default' COLLATE NOCASE, " +
        //        "Description TEXT, " +
        //        "HotKey COLLATE NOCASE, " +
        //        "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code), " +
        //        "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode)  ON DELETE CASCADE ON UPDATE CASCADE, " +
        //        "FOREIGN KEY (Species) REFERENCES Species (Species), " +
        //        "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) " +
        //    ");";

        public const string CREATE_VIEW_TALLYPOPULATION =
            "CREATE VIEW TallyPopulation AS " +
            "SELECT " +
                "sp.StratumCode, " +
                "sp.SampleGroupCode, " +
                "sp.Species, " +
                "sp.LiveDead, " +
                "ifnull(td.Description, '') AS Description, " +
                "ifnull(thk.HotKey, '') AS HotKey " +
            "FROM SubPopulation AS sp " +
            "JOIN SampleGroup_V3 AS sg USING (StratumCode, SampleGroupCode) " +
            "LEFT JOIN TallyHotKey AS thk USING (StratumCode, SampleGroupCode, Species, LiveDead) " +
            "LEFT JOIN TallyDescription AS td USING (StratumCode, SampleGroupCode, Species, LiveDead) " +
            "WHERE sg.TallyBySubPop != 0 " +
            "UNION ALL " +
            "SELECT " +
                "sg.StratumCode, " +
                "sg.SampleGroupCode, " +
                "null AS Species, " +
                "null AS LiveDead, " +
                "ifnull(td.Description,'') AS Description, " +
                "ifnull(thk.HotKey, '') AS HotKey " +
            "FROM SampleGroup_V3 AS sg " +
            "LEFT JOIN TallyHotKey AS thk ON " +
                    "thk.StratumCode = sg.StratumCode " +
                    "AND thk.SampleGroupCode = sg.SampleGroupCode " +
                    "AND ifnull(thk.Species, '') = '' " +
                    "AND ifnull(thk.LiveDead, '') = '' " +
            "LEFT JOIN TallyDescription AS td ON " +
                    "td.StratumCode = sg.StratumCode " +
                    "AND td.SampleGroupCode = sg.SampleGroupCode " +
                    "AND ifnull(td.Species, '') = '' " +
                    "AND ifnull(td.LiveDead, '') = '' " +
            "WHERE sg.TallyBySubPop == 0" +
            ";";
    }

    public partial class Migrations
    {
        //public const string MIGRATE_TALLYPOPULATION_FROM_COUNTTREE_FORMAT_STR =
        //    "INSERT INTO {0}.TallyPopulation ( " +
        //            "StratumCode, " +
        //            "SampleGroupCode, " +
        //            "Species, " +
        //            "LiveDead, " +
        //            "Description, " +
        //            "HotKey " +
        //        ") " +
        //        "SELECT " +
        //            "st.Code AS StratumCode, " +
        //            "sg.Code AS SampleGroupCode, " +
        //            "tdv.Species AS Species, " +
        //            "tdv.LiveDead AS LiveDead, " +
        //            "tal.Description, " +
        //            "tal.HotKey " +
        //        "FROM {1}.CountTree " +
        //        "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
        //        "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
        //        "JOIN {1}.Stratum as st USING (Stratum_CN) " +
        //        "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
        //        "JOIN {1}.Tally tal USING (Tally_CN) " +
        //        "GROUP BY " +
        //            "cu.Code, " +
        //            "st.Code, " +
        //            "sg.Code, " +
        //            "ifnull(tdv.Species, ''), " +
        //            "ifnull(tdv.LiveDead, '');";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TALLYPOPULATION_FROM_COUNTTREE =
    //        "INSERT INTO TallyPopulation " +
    //        "(StratumCode, SampleGroupCode, Species, LiveDead, Description, HotKey) " +
    //            "SELECT " +
    //                "st.Code AS StratumCode, " +
    //                "sg.Code AS SampleGroupCode, " +
    //                "tdv.Species AS Species, " +
    //                "tdv.LiveDead AS LiveDead, " +
    //                "tal.Description, " +
    //                "tal.HotKey " +
    //            "FROM CountTree " +
    //            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //            "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
    //            "JOIN Stratum as st USING (Stratum_CN) " +
    //            "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
    //            "JOIN Tally tal USING (Tally_CN) " +
    //            "GROUP BY cu.Code, st.Code, sg.Code, ifnull(tdv.Species, ''), ifnull(tdv.LiveDead, '');";
    //}
}