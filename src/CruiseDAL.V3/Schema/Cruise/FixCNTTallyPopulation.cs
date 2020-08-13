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
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "IntervalSize INTEGER Default 0, " +
                "Min INTEGER Default 0, " +
                "Max INTEGER Default 0, " +

                "UNIQUE (CruiseID, StratumCode, SampleGroupCode, Species, LiveDead), " +

                "FOREIGN KEY (StratumCode, SampleGroupCode, Species, LiveDead, CruiseID) REFERENCES SubPopulation (StratumCode, SampleGroupCode, Species, LiveDead, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_FixCNTTallyPopulation_Species =
            @"CREATE INDEX FixCNTTallyPopulation_Species ON FixCNTTallyPopulation (Species);";

        public const string CREATE_INDEX_FixCNTTallyPopulation_StratumCode_CruiseID =
            @"CREATE INDEX FixCNTTallyPopulation_StratumCode_CruiseID ON FixCNTTallyPopulation (StratumCode, CruiseID);";

        public const string CREATE_INDEX_FixCNTTallyPopulation_StratumCode_SampleGroupCode_Species_LiveDead_CruiseID =
            @"CREATE INDEX 'FixCNTTallyPopulation_StratumCode_SampleGroupCode_Species_LiveDead_CruiseID' ON FixCNTTallyPopulation (StratumCode, SampleGroupCode, Species, LiveDead, CruiseID);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYPOPULATION =
            "INSERT INTO {0}.FixCNTTallyPopulation ( " +
                    "FixCNTTallyPopulation_CN, " +
                    "CruiseID, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "IntervalSize, " +
                    "Min, " +
                    "Max " +
                ") " +
                "SELECT " +
                    "fixTP.FixCNTTallyPopulation_CN, " +
                    "'{4}', " +
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