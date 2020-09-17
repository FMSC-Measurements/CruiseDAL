namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SUBPOPULATION =
@"CREATE TABLE Subpopulation (
    Subpopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    LiveDead TEXT NOT NULL COLLATE NOCASE,

    UNIQUE (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead),

    CHECK (LiveDead IN ('L', 'D')),

    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE
);";

        public const string CREATE_INDEX_Subpopulation_SpeciesCode_CruiseID =
            @"CREATE INDEX Subpopulation_SpeciesCode_CruiseID ON Subpopulation (SpeciesCode, CruiseID);";

        public const string CREATE_INDEX_Subpopulation_StratumCode_SampleGroupCode_CruiseID =
            @"CREATE INDEX Subpopulation_StratumCode_SampleGroupCode_CruiseID ON Subpopulation (StratumCode, SampleGroupCode,  CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SUBPOPULATION_FROM_SAMPLEGROUPTREEDEFAULTVALUE =
@"INSERT INTO {0}.Subpopulation (
        CruiseID,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead
    )
    SELECT DISTINCT 
        '{3}',
        sg.StratumCode,
        sg.SampleGroupCode,
        tdv.Species,
        tdv.LiveDead
    FROM {1}.SampleGroupTreeDefaultValue as sgtdv
    JOIN {0}.SampleGroup AS sg USING (SampleGroup_CN)
    JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN);";
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