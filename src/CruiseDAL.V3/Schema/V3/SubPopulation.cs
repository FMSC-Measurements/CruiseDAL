﻿namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SUBPOPULATION =
            "CREATE TABLE Subpopulation (" +
                "Subpopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +

                "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead), " +

                "CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL)" +

                //"FOREIGN KEY (StratumCode) REFERENCES Stratum (Code), " +
                "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE ON UPDATE CASCADE," +
                "FOREIGN KEY (Species) REFERENCES SpeciesCode (Species) ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_Subpopulation_Species =
            @"CREATE INDEX Subpopulation_Species ON Subpopulation (Species COLLATE NOCASE);";

        public const string CREATE_INDEX_Subpopulation_StratumCode_SampleGroupCode =
            @"CREATE INDEX Subpopulation_StratumCode_SampleGroupCode ON Subpopulation (StratumCode COLLATE NOCASE, SampleGroupCode COLLATE NOCASE);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SUBPOPULATION_FROM_SAMPLEGROUPTREEDEFAULTVALUE =
            "INSERT INTO {0}.Subpopulation ( " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead " +
                ") " +
                "SELECT " +
                    "sg.StratumCode, " +
                    "sg.SampleGroupCode, " +
                    "tdv.Species, " +
                    "tdv.LiveDead " +
                "FROM {1}.SampleGroupTreeDefaultValue as sgtdv " +
                "JOIN {0}.SampleGroup_V3 AS sg USING (SampleGroup_CN) " +
                "JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_SUBPOPULATION_FROM_SAMPLEGROUPTREEDEFAULTVALUE =
    //        "INSERT INTO SubPopulation " +
    //        "SELECT " +
    //        "null AS Subpopulation_CN, " +
    //        "sg.StratumCode, " +
    //        "sg.SampleGroupCode, " +
    //        "tdv.Species, " +
    //        "tdv.LiveDead " +
    //        "FROM SampleGroupTreeDefaultValue as sgtdv " +
    //        "JOIN SampleGroup_V3 AS sg USING (SampleGroup_CN) " +
    //        "JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN);";
    //}
}