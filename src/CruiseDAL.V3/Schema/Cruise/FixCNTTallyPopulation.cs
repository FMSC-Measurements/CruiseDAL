namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYPOPULATION =
            "CREATE TABLE FixCNTTallyPopulation ( " +
                "FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "SpeciesCode TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "IntervalSize INTEGER Default 0, " +
                "Min INTEGER Default 0, " +
                "Max INTEGER Default 0, " +

                "UNIQUE (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead), " +

                "FOREIGN KEY (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) REFERENCES SubPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_FixCNTTallyPopulation_SpeciesCode =
            @"CREATE INDEX FixCNTTallyPopulation_SpeciesCode ON FixCNTTallyPopulation (SpeciesCode);";

        public const string CREATE_INDEX_FixCNTTallyPopulation_StratumCode_CruiseID =
            @"CREATE INDEX FixCNTTallyPopulation_StratumCode_CruiseID ON FixCNTTallyPopulation (StratumCode, CruiseID);";

        public const string CREATE_INDEX_FixCNTTallyPopulation_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID =
            @"CREATE INDEX 'FixCNTTallyPopulation_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID' ON FixCNTTallyPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYPOPULATION =
            "INSERT INTO {0}.FixCNTTallyPopulation ( " +
                    "FixCNTTallyPopulation_CN, " +
                    "CruiseID, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "SpeciesCode, " +
                    "LiveDead, " +
                    "IntervalSize, " +
                    "Min, " +
                    "Max " +
                ") " +
                "SELECT " +
                    "fixTP.FixCNTTallyPopulation_CN, " +
                    "'{3}', " +
                    "st.Code AS StratumCode, " +
                    "sg.Code AS SampleGroupCode, " +
                    "tdv.Species, " +
                    "tdv.LiveDead, " +
                    "fixTP.IntervalSize, " +
                    "fixTP.Min, " +
                    "fixTP.Max " +
                "FROM {1}.FixCNTTallyPopulation fixTP " +
                "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
                "JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
            ";";
    }
}