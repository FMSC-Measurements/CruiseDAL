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
        //        "FOREIGN KEY (Species) REFERENCES SpeciesCode (Species), " +
        //        "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) " +
        //    ");";

        public const string CREATE_VIEW_TALLYPOPULATION =
            "CREATE VIEW TallyPopulation AS " +
            "SELECT " +
                "sp.CruiseID," +
                "sp.StratumCode, " +
                "sp.SampleGroupCode, " +
                "sp.SpeciesCode, " +
                "sp.LiveDead, " +
                "ifnull(td.Description, '') AS Description, " +
                "ifnull(thk.HotKey, '') AS HotKey " +
            "FROM SubPopulation AS sp " +
            "JOIN SampleGroup AS sg USING (StratumCode, SampleGroupCode, CruiseID) " +
            "LEFT JOIN TallyHotKey AS thk USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) " +
            "LEFT JOIN TallyDescription AS td USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) " +
            "WHERE sg.TallyBySubPop != 0 " +
            "UNION ALL " +
            "SELECT " +
                "sg.CruiseID," +
                "sg.StratumCode, " +
                "sg.SampleGroupCode, " +
                "null AS SpeciesCode, " +
                "null AS LiveDead, " +
                "ifnull(td.Description,'') AS Description, " +
                "ifnull(thk.HotKey, '') AS HotKey " +
            "FROM SampleGroup AS sg " +
            "LEFT JOIN TallyHotKey AS thk ON " +
                    "thk.CruiseID = sg.CruiseID " +
                    "AND thk.StratumCode = sg.StratumCode " +
                    "AND thk.SampleGroupCode = sg.SampleGroupCode " +
                    "AND ifnull(thk.SpeciesCode, '') = '' " +
                    "AND ifnull(thk.LiveDead, '') = '' " +
            "LEFT JOIN TallyDescription AS td ON " +
                    "thk.CruiseID = sg.CruiseID " +
                    "AND td.StratumCode = sg.StratumCode " +
                    "AND td.SampleGroupCode = sg.SampleGroupCode " +
                    "AND ifnull(td.SpeciesCode, '') = '' " +
                    "AND ifnull(td.LiveDead, '') = '' " +
            "WHERE sg.TallyBySubPop == 0" +
            ";";
    }

}