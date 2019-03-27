namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] FIXCNTTALLYPOPULATION_V3 = new string[]
        {
            CREATE_TABLE_FIXCNTTALLYPOPULATION_V3
        };

        public const string CREATE_TABLE_FIXCNTTALLYPOPULATION_V3 =
            "CREATE TABLE FixCNTTallyPopulation_V3 ( " +
                "FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "IntervalSize INTEGER Default 0, " +
                "Min INTEGER Default 0, " +
                "Max INTEGER Default 0, " +

                "UNIQUE (StratumCode, SampleGroupCode), " +

                "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE," +
                "FOREIGN KEY (StratumCode) REFERENCES FixCNTTallyClass_V3 (StratumCode) ON DELETE CASCADE, " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON UPDATE CASCADE ON DELETE CASCADE" +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_FIXCNTTALLYPOPULATION_V3 =
            "INSERT INTO {0}.FixCNTTallyPopulation_V3 ( " +
                    "FixCNTTallyPopulation_CN, " +
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